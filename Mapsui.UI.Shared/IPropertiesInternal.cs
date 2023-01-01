using System.Collections.Generic;

namespace Mapsui.UI
{
    internal interface IPropertiesInternal
    {
#if  __ANDROID__ || __IOS__ || __ETO_FORMS__
        public Dictionary<object, object> Properties { get; }
#endif
    }
}