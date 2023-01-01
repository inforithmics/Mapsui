using Mapsui.UI.Objects;

namespace Mapsui.UI
{
#if  __ANDROID__ || __IOS__ || __ETO_FORMS__
    internal interface IMapViewInternal : IPropertiesInternal
#else    
    internal interface IMapViewInternal : IMapView
#endif
    {
        void RemoveCallout(Callout callout);
        void AddCallout(Callout callout);
        bool IsCalloutVisible(Callout callout);
    }
}