﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tests.Fetcher.Providers;
using Mapsui.Tiling.Extensions;
using Mapsui.Tiling.Fetcher;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Mapsui.Tests.Fetcher;

[TestFixture]
public class FetchMachineTests
{
    // Note, The Thread.Sleep(1) in the while loop is necessary to avoid
    // a hang in some rare cases.

    [Test]
    public void TileFetcherShouldRequestAllTilesJustOnes()
    {
        // Arrange
        var tileProvider = new CountingTileSource();
        var tileSchema = new GlobalSphericalMercator();
        var tileSource = new TileSource(tileProvider, tileSchema);
        using var cache = new MemoryCache<IFeature?>();
        var fetchDispatcher = new TileFetchDispatcher(cache, tileSource.Schema, async tileInfo => await TileToFeatureAsync(tileSource, tileInfo));
        var level = 3;
        var expectedTiles = 64;

        var fetchInfo = new FetchInfo(new MSection(tileSchema.Extent.ToMRect(), tileSchema.Resolutions[level].UnitsPerPixel));

        // Act
        // Get all tiles of level 3
        fetchDispatcher.RefreshData(fetchInfo);
        // Assert
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        ClassicAssert.AreEqual(expectedTiles, tileProvider.CountByTile.Keys.Count);
        ClassicAssert.AreEqual(expectedTiles, tileProvider.CountByTile.Values.Sum());
        ClassicAssert.AreEqual(expectedTiles, tileProvider.TotalCount);
    }

    [Test]
    public void TilesFetchedShouldNotBeFetchAgain()
    {
        // Arrange
        var tileProvider = new CountingTileProvider();
        var tileSchema = new GlobalSphericalMercator();
        var tileSource = new TileSource(tileProvider, tileSchema);
        using var cache = new MemoryCache<IFeature?>();
        var fetchDispatcher = new TileFetchDispatcher(cache, tileSource.Schema, async tileInfo => await TileToFeatureAsync(tileSource, tileInfo));
        var level = 3;
        var expectedTiles = 64;
        var fetchInfo = new FetchInfo(new MSection(tileSchema.Extent.ToMRect(), tileSchema.Resolutions[level].UnitsPerPixel));

        // Act
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }
        var countAfterFirstTry = tileProvider.CountByTile.Keys.Count;
        // do it again
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        // Assert
        ClassicAssert.AreEqual(countAfterFirstTry, tileProvider.CountByTile.Values.Sum());
        ClassicAssert.AreEqual(expectedTiles, tileProvider.CountByTile.Keys.Count);
        ClassicAssert.AreEqual(expectedTiles, tileProvider.CountByTile.Values.Sum());
        ClassicAssert.AreEqual(expectedTiles, tileProvider.TotalCount);
    }


    [Test]
    public void TileRequestThatReturnsNullShouldNotBeRequestedAgain()
    {
        // Arrange
        var tileProvider = new NullTileProvider();
        var tileSchema = new GlobalSphericalMercator();
        var tileSource = new TileSource(tileProvider, tileSchema);
        using var cache = new MemoryCache<IFeature?>();
        var fetchDispatcher = new TileFetchDispatcher(cache, tileSource.Schema, async tileInfo => await TileToFeatureAsync(tileSource, tileInfo));
        var level = 3;
        var tilesInLevel = 64;
        var fetchInfo = new FetchInfo(new MSection(tileSchema.Extent.ToMRect(), tileSchema.Resolutions[level].UnitsPerPixel));
        // Act
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }
        // do it again
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        // Assert
        ClassicAssert.AreEqual(tilesInLevel, tileProvider.TotalCount);
    }

    [Test]
    public void TileFetcherWithFailingFetchesShouldTryAgain()
    {
        // Arrange
        var tileProvider = new FailingTileProvider();
        var tileSchema = new GlobalSphericalMercator();
        var tileSource = new TileSource(tileProvider, tileSchema);
        using var cache = new MemoryCache<IFeature?>();
        var fetchDispatcher = new TileFetchDispatcher(cache, tileSource.Schema, async tileInfo => await TileToFeatureAsync(tileSource, tileInfo));
        var level = 3;
        var tilesInLevel = 64;
        var fetchInfo = new FetchInfo(new MSection(tileSchema.Extent.ToMRect(), tileSchema.Resolutions[level].UnitsPerPixel));

        // Act
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        // Act again
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        // Assert
        ClassicAssert.AreEqual(tilesInLevel * 2, tileProvider.TotalCount); // tried all tiles twice
    }

    [Test]
    public void TileFetcherWithSometimesFailingFetchesShouldTryAgain()
    {
        // Arrange
        var tileProvider = new SometimesFailingTileProvider();
        var tileSchema = new GlobalSphericalMercator();
        var tileSource = new TileSource(tileProvider, tileSchema);
        using var cache = new MemoryCache<IFeature?>();
        var fetchDispatcher = new TileFetchDispatcher(cache, tileSource.Schema, async tileInfo => await TileToFeatureAsync(tileSource, tileInfo));
        var level = 3;
        var tilesInLevel = 64;
        var fetchInfo = new FetchInfo(new MSection(tileSchema.Extent.ToMRect(), tileSchema.Resolutions[level].UnitsPerPixel));

        // Act
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        var tileCountAfterFirstBatch = tileProvider.TotalCount;

        // Act again
        fetchDispatcher.RefreshData(fetchInfo);
        while (fetchDispatcher.Busy) { Thread.Sleep(1); }

        // Assert
        ClassicAssert.GreaterOrEqual(tileProvider.TotalCount, tileCountAfterFirstBatch);
        ClassicAssert.GreaterOrEqual(tileProvider.CountByTile.Values.Sum(), tilesInLevel);

    }

    private async Task<RasterFeature?> TileToFeatureAsync(ITileSource tileSource, TileInfo tileInfo, CancellationToken cancellationToken)
    {
        // A tile layer can return a null value. This indicates the tile is not
        // present in the source, permanently. If this is the case no further 
        // requests should be done. To avoid further fetches a feature should
        // be returned with the Geometry set to null. If a null Feature is returned
        // this equates to having no tile at all and there should be no other attempts
        // to fetch the tile. 
        // 
        // Note, the fact that we have to define this complex method on the outside
        // indicates a design flaw.
        var tile = await tileSource.GetTileAsync(tileInfo);
        if (tile == null)
            return new RasterFeature((MRaster?)null);

        return new RasterFeature(new MRaster(tile, tileInfo.Extent.ToMRect()));
    }
}
