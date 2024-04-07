﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mapsui.Styles;

namespace Mapsui.Rendering.Skia.Cache;

public sealed class SymbolCache : ISymbolCache
{
    private readonly IDictionary<int, BitmapInfo> _cache = new ConcurrentDictionary<int, BitmapInfo>();
    private readonly IBitmapRegistry _bitmapRegistry;

    public SymbolCache(IBitmapRegistry bitmapRegistry)
    {
        _bitmapRegistry = bitmapRegistry;
    }

    public SymbolCache() : this(BitmapRegistry.Instance)
    {
    }

    public IBitmapInfo GetOrCreate(int bitmapId)
    {
        if (_cache.TryGetValue(bitmapId, out var result))
        {
            if (!BitmapHelper.InvalidBitmapInfo(result))
            {
                return result;
            }
        }

        var bitmapStream = _bitmapRegistry.Get(bitmapId);
        bool ownsBitmap = bitmapStream is not IDisposable;
        var loadBitmap = BitmapHelper.LoadBitmap(bitmapStream, ownsBitmap) ?? throw new ArgumentNullException(nameof(bitmapId));
        return _cache[bitmapId] = loadBitmap;
    }

    public Size? GetSize(int bitmapId)
    {
        var bitmap = (BitmapInfo?)GetOrCreate(bitmapId);
        if (bitmap == null)
            return null;

        return new Size(bitmap.Width, bitmap.Height);
    }

    public void Dispose()
    {
        foreach (var value in _cache.Values)
        {
            value.Dispose();
        }

        _cache.Clear();
    }
}
