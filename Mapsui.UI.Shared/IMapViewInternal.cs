using Mapsui.UI.Objects;

namespace Mapsui.UI
{
    internal interface IMapViewInternal : IMapView
    {
        void RemoveCallout(Callout callout);
        void AddCallout(Callout callout);
        bool IsCalloutVisible(Callout callout);
    }
}