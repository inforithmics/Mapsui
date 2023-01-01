using Mapsui.UI.Objects;

namespace Mapsui.UI
{
#if  __ANDROID__ || __IOS__ || __ETO_FORMS__
    internal interface IMapViewInternal : IPropertiesInternal
#else    
    internal interface IMapViewInternal
#endif
    {
        void RemoveCallout(ICallout callout);
        void AddCallout(ICallout callout);
        bool IsCalloutVisible(ICallout callout);
    }
}