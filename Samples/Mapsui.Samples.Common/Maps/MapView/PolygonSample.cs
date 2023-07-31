using System;
using Mapsui.Nts.EventArgs;
using Mapsui.Nts.Extensions;
using Mapsui.Nts.Objects;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Maps;
using Mapsui.Samples.Common.Maps.Demo;
using Mapsui.Styles;
using Mapsui.UI;

namespace Mapsui.Samples.Common.Maps.MapView;

public class PolygonSample : IMapViewSample
{
    static readonly Random random = new Random(1);

    public string Name => "Add Polygon Sample";

    public string Category => "MapView";

    public bool UpdateLocation => true;

    public bool OnClick(object? sender, EventArgs args)
    {
        var mapView = sender as IMapControl;
        var e = args as MapClickedEventArgs;

        if (e == null)
            return false;

        var center = new Position(e.Point);
        var diffX = random.Next(0, 1000) / 100.0;
        var diffY = random.Next(0, 1000) / 100.0;

        var polygon = new Polygon
        {
            StrokeColor = new Color(
                Convert.ToInt32(random.Next(0, 255) / 255.0f),
                Convert.ToInt32(random.Next(0, 255) / 255.0f),
                Convert.ToInt32(random.Next(0, 255) / 255.0f)),
            FillColor = new Color(
                Convert.ToInt32(random.Next(0, 255) / 255.0f),
                Convert.ToInt32(random.Next(0, 255) / 255.0f),
                Convert.ToInt32(random.Next(0, 255) / 255.0f))
        };

        polygon.Positions.Add(new Position(center.Latitude - diffY, center.Longitude - diffX));
        polygon.Positions.Add(new Position(center.Latitude + diffY, center.Longitude - diffX));
        polygon.Positions.Add(new Position(center.Latitude + diffY, center.Longitude + diffX));
        polygon.Positions.Add(new Position(center.Latitude - diffY, center.Longitude + diffX));

        // Be carefull: holes should have other direction of Positions.
        // If Positions is clockwise, than Holes should all be counter clockwise and the other way round.
        polygon.Holes.Add(new Position[] {
            new Position(center.Latitude - diffY * 0.3, center.Longitude - diffX * 0.3),
            new Position(center.Latitude + diffY * 0.3, center.Longitude + diffX * 0.3),
            new Position(center.Latitude + diffY * 0.3, center.Longitude - diffX * 0.3),
        });

        polygon.IsClickable = true;
        polygon.Clicked += (s, a) =>
        {
            if (s is Polygon p)
            {
                p.FillColor = new Color(
                    Convert.ToInt32(random.Next(0, 255) / 255.0f),
                    Convert.ToInt32(random.Next(0, 255) / 255.0f),
                    Convert.ToInt32(random.Next(0, 255) / 255.0f));
                a.Handled = true;
            }
        };

        mapView?.Map.Drawables().Add(polygon);

        return true;
    }

    public void Setup(IMapControl mapControl)
    {
        mapControl.Map = OsmSample.CreateMap();
    }
}
