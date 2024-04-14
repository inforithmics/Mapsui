﻿using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Samples.Common.DataBuilders;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets.InfoWidgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Styles;

public class AtlasSample : ISample
{
    private const string AtlasLayerName = "Atlas Layer";
    private static Uri _atlasBitmapPath = typeof(AtlasSample).LoadBitmapPath("Images.osm-liberty.png");
    private static readonly Random Random = new(1);

    public string Name => "Atlas";

    public string Category => "Styles";

    public Task<Map> CreateMapAsync()
    {
        var map = new Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreateAtlasLayer(map.Extent));

        map.Widgets.Add(new MapInfoWidget(map));

        return Task.FromResult(map);
    }

    private static ILayer CreateAtlasLayer(MRect? envelope)
    {
        return new MemoryLayer
        {
            Name = AtlasLayerName,
            Features = CreateAtlasFeatures(RandomPointsBuilder.GenerateRandomPoints(envelope, 1000)),
            Style = null,
            IsMapInfoLayer = true
        };
    }

    private static IEnumerable<IFeature> CreateAtlasFeatures(IEnumerable<MPoint> randomPoints)
    {
        var counter = 0;

        return randomPoints.Select(p =>
        {
            var feature = new PointFeature(p) { ["Label"] = counter.ToString() };

            var x = 0 + Random.Next(0, 12) * 21;
            var y = 64 + Random.Next(0, 6) * 21;
            var bitmap = new Sprite(_atlasBitmapPath, x, y, 21, 21, 1);
            feature.Styles.Add(new SymbolStyle { Bitmap = bitmap });
            counter++;
            return feature;
        }).ToList();
    }
}
