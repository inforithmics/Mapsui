using System;
using System.Drawing;
using System.Linq;
using Mapsui.Nts.EventArgs;
using Mapsui.Nts.Extensions;
using Mapsui.Nts.Objects;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Maps;
using Mapsui.Samples.Common.Maps.Demo;
using Mapsui.UI;
using KnownColor = Mapsui.Styles.Color;

namespace Mapsui.Samples.Common.Maps.MapView;

public class PolylineSample : IMapViewSample
{
    public string Name => "Add Polyline Sample";

    public string Category => "MapView";

    public bool UpdateLocation => true;

    public bool OnClick(object? sender, EventArgs args)
    {
        var mapView = sender as IMapControl;
        var e = args as MapClickedEventArgs;

        if (mapView == null)
            return false;

        if (e == null)
            return false;

        var drawables = mapView.Map.Drawables();
        lock (drawables)
        {
            Drawable f;
            if (drawables.Count == 0)
            {
                f = new Polyline { StrokeWidth = 4, StrokeColor = KnownColor.Red, IsClickable = true };
                drawables.Add(f);
            }
            else
            {
                f = drawables.First();
            }

            if (f is Polyline polyline)
            {
                polyline.Positions.Add(e.Point);
            }
        }

        return true;
    }

    public void Setup(IMapControl mapControl)
    {
        mapControl.Map = OsmSample.CreateMap();

        ////((IMapControl)mapControl).UseDoubleTap = false;
    }
}
