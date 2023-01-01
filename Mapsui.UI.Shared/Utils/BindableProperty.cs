#if __ANDROID__ || __IOS__ || __AVALONIA__ || __ETO_FORMS__
namespace Mapsui.UI.Utils
{
#if __AVALONIA__
public class BindableProperty : AttachedProperty<object>
#else
public class BindableProperty    
#endif    
    {
#if __AVALONIA__
        public BindableProperty(string name, Type ownerType, StyledPropertyMetadata<object> metadata, bool inherits = false, Func<object, bool>? validate = null) : base(name, ownerType, metadata, inherits, validate) { }
#endif
    }
#endif
}