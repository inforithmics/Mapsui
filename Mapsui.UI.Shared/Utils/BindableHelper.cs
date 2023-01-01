using System;

namespace Mapsui.UI.Utils
{
    public static class BindableHelper
    {
        public static BindableProperty Create(
            string name, 
            Type returnType, 
            Type declaringType, 
            object defaultValue = null, 
            BindingMode defaultBindingMode = BindingMode.OneWay)
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
#elif __WPF__ || __UWP__ || __WINUI__
            return DependencyProperty.Register(name, returnType, declaringType, new PropertyMetadata(declaringType));
#elif __MAUI__ || __FORMS__
            return BindableProperty.Create(name, returnType, declaringType, defaultValue, defaultBindingMode);
#else
            // Dummy Bindable Property is used as Dictionary Key
            return new BindableProperty();
#endif
        }
    }
}