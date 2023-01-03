using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if __ANDROID__ || __IOS__ || __ETO_FORMS__ || __WINUI__ || __UWP__ || __WPF__ || __ETO_FORMS__
namespace Mapsui.UI.Utils
{
#if __UWP__
    public class BindableObject : Windows.UI.Xaml.DependencyObject,INotifyPropertyChanged
#elif __WPF__
    public class BindableObject : System.Windows.DependencyObject, INotifyPropertyChanged
#elif __WINUI__
    public class BindableObject : Microsoft.UI.Xaml.DependencyObject, INotifyPropertyChanged
#elif __ETO_FORMS__
    public class BindableObject : Eto.Forms.BindableWidget, INotifyPropertyChanged
#else
    public class BindableObject : INotifyPropertyChanged
#endif    
    {
#if  __ANDROID__ || __IOS__ || __ETO_FORMS__
        private readonly Dictionary<object, object> _properties = new();
#endif
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
#if  __ANDROID__ || __IOS__ || __ETO_FORMS__        
        public object GetValue(object property)
        {
            _properties.TryGetValue(property, out var result);
            return result;
        }

        public void SetValue(object property, object value, [CallerMemberName] string? propertyName = null)
        {
            if (_properties[property] != value)
            {
                _properties[property] = value;
                OnPropertyChanged(propertyName);
            }
        }
#endif
    }
}
#endif