﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BruTile;
using BruTile.Cache;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Tiling.Extensions;
using Mapsui.Utilities;

namespace Mapsui.Tiling.Fetcher;

public class TileFetchDispatcher : IFetchDispatcher, INotifyPropertyChanged
{
    private FetchInfo? _fetchInfo;
    private readonly object _lockRoot = new();
    private bool _busy;
    private bool _viewportIsModified;
    private readonly ITileCache<IFeature?> _tileCache;
    private readonly IDataFetchStrategy _dataFetchStrategy;
    private readonly ConcurrentQueue<TileInfo> _tilesToFetch = [];
    private readonly ConcurrentHashSet<TileIndex> _tilesInProgress = [];
    private readonly ITileSchema? _tileSchema;
    private readonly FetchMachine _fetchMachine;
    private readonly Func<TileInfo, Task<IFeature?>> _fetchTileAsFeature;
    private readonly int _fetchThreadCount = 4;

    public TileFetchDispatcher(
        ITileCache<IFeature?> tileCache,
        ITileSchema? tileSchema,
        Func<TileInfo, Task<IFeature?>> fetchTileAsFeature,
        IDataFetchStrategy? dataFetchStrategy = null)
    {
        _tileCache = tileCache;
        _tileSchema = tileSchema;
        _fetchTileAsFeature = fetchTileAsFeature;
        _dataFetchStrategy = dataFetchStrategy ?? new MinimalDataFetchStrategy();
        _fetchMachine = new FetchMachine(this);
    }

    public event DataChangedEventHandler? DataChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    public int NumberTilesNeeded { get; private set; }

    public static int MaxTilesInOneRequest { get; set; } = 128;

    public void SetViewport(FetchInfo fetchInfo)
    {
        lock (_lockRoot)
        {
            _fetchInfo = fetchInfo;
            Busy = true;
            _viewportIsModified = true;
        }
    }

    public void FetchNextTiles()
    {
        lock (_lockRoot)
        {
            UpdateIfViewportIsModified();
            if (_tilesToFetch.TryDequeue(out var tileInfo))
            {
                var tilesToQueue = GetNumberOfTilesToQueue(tilesToFetch);

            Busy = _tilesInProgress.Count > 0 || !_tilesToFetch.IsEmpty;
            // else the queue is empty, we are done.
            method = null;
            return false;
        }
    }

    private async Task FetchOnThreadAsync(TileInfo tileInfo)
    {
        try
        {
            var feature = await _fetchTileAsFeature(tileInfo).ConfigureAwait(false);
            FetchCompleted(tileInfo, feature, null);
        }
        catch (Exception ex)
        {
            // The exception is returned to the caller and should be logged there.
            FetchCompleted(tileInfo, null, ex);
        }
    }

    private void UpdateIfViewportIsModified()
    {
        if (_viewportIsModified)
        {
            UpdateTilesToFetchForViewportChange();
            _viewportIsModified = false;
        }
    }

    private void FetchCompleted(TileInfo tileInfo, IFeature? feature, Exception? exception)
    {
        lock (_lockRoot)
        {
            if (exception == null)
                _tileCache.Add(tileInfo.Index, feature);
            _tilesInProgress.TryRemove(tileInfo.Index);

            Busy = _tilesInProgress.Count > 0 || !_tilesToFetch.IsEmpty;

            DataChanged?.Invoke(this, new DataChangedEventArgs(exception, false, tileInfo));
        }
    }

    public bool Busy
    {
        get => _busy;
        private set
        {
            if (_busy == value) return; // prevent notify              
            _busy = value;
            OnPropertyChanged(nameof(Busy));
        }
    }

    public void StopFetching()
    {
        _fetchMachine.Stop();
    }

    public void StartFetching()
    {
        _fetchMachine.Start();
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UpdateTilesToFetchForViewportChange()
    {
        // Use local fields to avoid changes caused by other threads during this calculation.
        var localFetchInfo = _fetchInfo;
        var localTileSchema = _tileSchema;

        if (localFetchInfo is null || localTileSchema is null)
            return;

        var levelId = BruTile.Utilities.GetNearestLevel(localTileSchema.Resolutions, localFetchInfo.Resolution);
        var tilesToCoverViewport = _dataFetchStrategy.Get(localTileSchema, localFetchInfo.Extent.ToExtent(), levelId);
        NumberTilesNeeded = tilesToCoverViewport.Count;
        var tilesToFetch = tilesToCoverViewport.Where(t => _tileCache.Find(t.Index) == null && !_tilesInProgress.Contains(t.Index));
        if (tilesToFetch.Count() > MaxTilesInOneRequest)
        {
            tilesToFetch = tilesToFetch.Take(MaxTilesInOneRequest).ToList();
            Logger.Log(LogLevel.Warning,
                $"The number tiles requested is '{tilesToFetch.Count()}' which exceeds the maximum " +
                $"of '{MaxTilesInOneRequest}'. The number of tiles will be limited to the maximum. Note, " +
                $"that this may indicate a bug or configuration error");
        }

        _tilesToFetch.Clear();
        _tilesToFetch.AddRange(tilesToFetch);
        if (!_tilesToFetch.IsEmpty) Busy = true;
    }
}
