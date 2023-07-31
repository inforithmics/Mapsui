#if NET6_0_OR_GREATER

using System;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Samples.Common.Maps.Demo;
using Mapsui.UI;

namespace Mapsui.Samples.Common.Maps.MapView;

public sealed class AnimatedMyLocationSample : IMapViewSample, IDisposable
{
    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
    private MPoint _newLocation = new();
    private IMapControl? _mapControl;
    private MyLocationLayer? _myLocationLayer;

    public string Name => "Animated MyLocation Sample";

    public string Category => "MapView";

    public void Setup(IMapControl mapControl)
    {
        // 54°54′24″N 25°19′12″E Center of Europe
        _newLocation = SphericalMercator.FromLonLat(54.5424, 25.1912).ToMPoint();

        _mapControl = mapControl;
        var map = OsmSample.CreateMap();
        map.Home = n => n.CenterOnAndZoomTo(_newLocation, n.Resolutions[14]);
        mapControl.Map = map;

        _myLocationLayer?.Dispose();
        _myLocationLayer = _mapControl.Map.MyLocationLayer()!;
        _myLocationLayer.IsMoving = true;
        _myLocationLayer.Enabled = true;
        _myLocationLayer.IsCentered = true;

        _myLocationLayer.UpdateMyLocation(_newLocation);
        _mapControl.Map.Navigator.CenterOn(_newLocation);

        Catch.TaskRun(RunTimerAsync);
    }

    public bool UpdateLocation => false;
    public bool OnClick(object? sender, EventArgs args)
    {
        return true; 
    }

    private async Task RunTimerAsync()
    {
        while(true)
        {
            await _timer.WaitForNextTickAsync();

            _newLocation = new (_newLocation.X + 0.00005, _newLocation.Y + 0.00005);                                

            _myLocationLayer!.UpdateMyLocation(_newLocation, true);
            _myLocationLayer!.UpdateMyDirection(_myLocationLayer!.Direction + 10, 0, true);
            _myLocationLayer!.UpdateMyViewDirection(_myLocationLayer!.ViewingDirection + 10, 0, true);
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _myLocationLayer?.Dispose();
    }
}
#endif
