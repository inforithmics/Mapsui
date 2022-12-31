using System;
#if __AVALONIA__
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
#endif

#if __ANDROID__ || __IOS__ || __AVALONIA__ || __WPF__
namespace Mapsui.UI.Utils
{
#if __AVALONIA__
publi class BindableProperty : AttachedProperty<object>
#elif  __WPF__
public class BindableProperty : DependencyProperty
#else
public class BindableProperty    
#endif    
    {
        public static BindableProperty Create(
            string name, 
            Type returnType, 
            Type declaringType, 
            object defaultValue, 
            BindingMode defaultBindingMode = BindingMode.TwoWay)
        {
#if __AVALONIA__
            // Copied from this Method
            // AttachedProperty<object>.RegisterAttached<object, Interactive, object>(name, defaultValue, defaultBindingMode);
            _ = name ?? throw new ArgumentNullException(nameof(name));

            var metadata = new StyledPropertyMetadata<object>(
                defaultValue,
                defaultBindingMode: defaultBindingMode);

            var result = new BindableProperty(name, declaringType, metadata);
            var registry = AvaloniaPropertyRegistry.Instance;
            registry.Register(declaringType, result);
            registry.RegisterAttached(typeof(Interactive), result);
            return result;
#elif __WPF__
            return DependencyProperty.Create(name, returnType, declaringType, defaultValue, defaultBindingMode);
#else            
            return new BindableProperty();
#endif
        }
#if __AVALONIA__
        public BindableProperty(string name, Type ownerType, StyledPropertyMetadata<object> metadata, bool inherits = false, Func<object, bool>? validate = null) : base(name, ownerType, metadata, inherits, validate) { }
#endif
    }
#endif
}