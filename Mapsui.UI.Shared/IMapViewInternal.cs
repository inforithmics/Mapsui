using Mapsui.UI.Objects;

namespace Mapsui.UI
{
    public interface IMapViewInternal
    {
        void RemoveCallout(ICallout callout);
        void AddCallout(ICallout callout);
        bool IsCalloutVisible(ICallout callout);
    }
}