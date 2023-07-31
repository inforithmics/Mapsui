using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mapsui.Layers;
using Mapsui.Nts.Objects;
using Mapsui.UI;
using Mapsui.Utilities;

namespace Mapsui.Nts.Extensions;

public static class MapExtensions
{
    public static void  AssureMapViewLayers(this Map map)
    {
        if (!map.IsMapViewInitialized())
        {
            map.RemoveMapViewLayers();
            map.AddMapViewLayers();
        }
    }

    /// <summary>
    /// Add all layers that MapView uses
    /// </summary>
    private static void AddMapViewLayers(this Map map)
    {
        var myLocationLayer = new MyLocationLayer(map) { Enabled = true };
        var calloutLayer = new ObservableMemoryLayer<Callout>(f => f.Feature)
        {
            Name = MapConstants.CalloutLayerName, IsMapInfoLayer = true,
            ObservableCollection = new ObservableRangeCollection<Callout>(),
        };
        var pinLayer = new ObservableMemoryLayer<IPin>(f => f.Feature)
        {
            Name = MapConstants.PinLayerName, IsMapInfoLayer = true,
            ObservableCollection = new ObservableRangeCollection<IPin>(),
        };
        var drawableLayer = new ObservableMemoryLayer<Drawable>(f => f.Feature)
        {
            Name = MapConstants.DrawableLayerName, IsMapInfoLayer = true,
            ObservableCollection = new ObservableRangeCollection<Drawable>(),
        };

        // Add MapView layers
        map?.Layers.Add(drawableLayer, pinLayer, calloutLayer, myLocationLayer);
    }

    /// <summary>
    /// Remove all layers that MapView uses
    /// </summary>
    private static void RemoveMapViewLayers(this Map map)
    {
        var locationLayer = InternalLocationLayer(map);
        var calloutLayer = CalloutLayer(map);
        var pinLayer = PinLayer(map);
        var drawableLayer = DrawableLayer(map);

        var remove = new List<ILayer>();
        {
            if (locationLayer != null)
            {
                remove.Add(locationLayer);
            }

            if (calloutLayer != null)
            {
                remove.Add(calloutLayer);
            }

            if (pinLayer != null)
            {
                remove.Add(pinLayer);
            }

            if (drawableLayer != null)
            {
                remove.Add(drawableLayer);
            }
        }

        // Remove MapView layers
        map?.Layers.Remove(remove.ToArray());
    }

    public static ObservableMemoryLayer<Drawable>? DrawableLayer(this Map map)
    {
        return map?.Layers.FirstOrDefault(f => f is ObservableMemoryLayer<Drawable>) as ObservableMemoryLayer<Drawable>;
    }

    public static ObservableMemoryLayer<IPin>? PinLayer(this Map map)
    {
        return map?.Layers.FirstOrDefault(f => f is ObservableMemoryLayer<IPin>) as ObservableMemoryLayer<IPin>;
    }

    public static ObservableMemoryLayer<Callout>? CalloutLayer(this Map map)
    {
        return map?.Layers.FirstOrDefault(f => f is ObservableMemoryLayer<Callout>) as ObservableMemoryLayer<Callout>;
    }

    public static MyLocationLayer MyLocationLayer(this Map map)
    {
        map.AssureMapViewLayers();
        return InternalLocationLayer(map)!;
    }

    public static MyLocationLayer? InternalLocationLayer(this Map map)
    {
        return map?.Layers.FirstOrDefault(f => f is MyLocationLayer) as MyLocationLayer;
    }

    public static bool IsCalloutVisible(this Map map, Callout callout)
    {
        return map?.CalloutLayer()?.ObservableCollection?.Contains(callout) ?? false;
    }

    public static void RemoveCallout(this Map map, Callout callout)
    {
        map?.CalloutLayer()?.ObservableCollection?.Remove(callout);
    }

    public static void AddCallout(this Map map, Callout callout)
    {
        map?.CalloutLayer()?.ObservableCollection?.Add(callout);
    }

    public static ObservableRangeCollection<Drawable> Drawables(this Map map)
    {
        map.AssureMapViewLayers();
        return (ObservableRangeCollection<Drawable>)map.DrawableLayer()!.ObservableCollection!;
    }

    public static ObservableRangeCollection<IPin> Pins(this Map map)
    {
        map.AssureMapViewLayers();
        return (ObservableRangeCollection<IPin>)map.PinLayer()!.ObservableCollection!;
    }

    private static bool IsMapViewInitialized(this Map map)
    {
        return map.Layers.Any(f => f is MyLocationLayer) &&
               map.Layers.Any(f => f is ObservableMemoryLayer<Drawable>) &&
               map.Layers.Any(f => f is ObservableMemoryLayer<Callout>) &&
               map.Layers.Any(f => f is ObservableMemoryLayer<IPin>);

    }
}
