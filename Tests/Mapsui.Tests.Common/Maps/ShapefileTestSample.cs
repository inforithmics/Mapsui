﻿using System.IO;
using System.Threading.Tasks;
using Mapsui.Layers;
using Mapsui.Nts.Providers.Shapefile;
using Mapsui.Samples.Common;
using Mapsui.Styles;
using Mapsui.Tests.Common.Utilities;

namespace Mapsui.Tests.Common.Maps;

public class ShapefileTestSample : ISample
{
    static ShapefileTestSample()
    {
        TestShapeFilesDeployer.CopyEmbeddedResourceToFile("test_file.shp");
    }

    public string Name => "Shapefile Zoom";
    public string Category => "Tests";

    public Task<Map> CreateMapAsync()
    {
        return Task.FromResult(CreateMap());
    }

    public static Map CreateMap()
    {
        var map = new Map();

        var shapeFilePath = Path.Combine(TestShapeFilesDeployer.ShapeFilesLocation, "test.shp");
        var shpSource = new ShapeFile(shapeFilePath);

        // Apply basic styles
        var layer = new Layer
        {
            DataSource = shpSource,
            Style = new VectorStyle
            {
                Fill = new Brush(Color.FromArgb(128, 0, 255, 0)),
                Line = new Pen(Color.FromString("#0969da"), 4)
                {
                    PenStrokeCap = PenStrokeCap.Round, StrokeJoin = StrokeJoin.Round
                }
            }
        };

        // Add the new layer
        map.Layers.Add(layer);

        return map;
    }
}
