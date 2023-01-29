using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Mapsui.UI.Objects;
using Mapsui.UI.Utils;

#if __MAUI__
namespace Mapsui.UI.Maui
#elif __UWP__
namespace Mapsui.UI.Uwp
#elif __ANDROID__ && !HAS_UNO_WINUI
namespace Mapsui.UI.Android
#elif __IOS__ && !HAS_UNO_WINUI
namespace Mapsui.UI.iOS
#elif __WINUI__
namespace Mapsui.UI.WinUI
#elif __FORMS__
namespace Mapsui.UI.Forms
#elif __AVALONIA__
namespace Mapsui.UI.Avalonia
#elif __ETO_FORMS__
namespace Mapsui.UI.Eto
#else
namespace Mapsui.UI.Wpf
#endif
{
    public partial class MapView : MapControl, IMapView, IMapViewInternal
    {
#if __ANDROID__ || __IOS__ || __ETO_FORMS__
        private Dictionary<object,object> _properties { get; } = new();
#endif
        public static readonly BindableProperty UniqueCalloutProperty = BindableHelper.Create(nameof(UniqueCallout), typeof(bool), typeof(MapView), false, defaultBindingMode: BindingMode.TwoWay);

        private readonly ObservableRangeCollection<Callout> _callouts = new();
        
        /// <summary>
        /// Single or multiple callouts possible
        /// </summary>
        public bool UniqueCallout
        {
            get => (bool)GetValue(UniqueCalloutProperty);
            set => SetValue(UniqueCalloutProperty, value);
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
#else
        public void SetValue(BindableProperty property, object value, [CallerMemberName] string? propertyName = null)
        {
            if (this.GetValue(property) != value)
            {
                base.SetValue(property, value); 
                OnPropertyChanged(propertyName);
            }
        }
#endif        

        /// <summary>
        /// Hide all visible callouts
        /// </summary>
        public void HideCallouts()
        {
            _callouts.Clear();
        }
        
        void IMapViewInternal.AddCallout(Callout callout)
        {
            if (!_callouts.Contains(callout))
            {
                if (UniqueCallout)
                    HideCallouts();

                _callouts.Add(callout);

                Refresh();
            }
        }

        void IMapViewInternal.RemoveCallout(Callout? callout)
        {
            if (callout != null && _callouts.Contains(callout))
            {
                _callouts.Remove(callout);

                Refresh();
            }
        }

        bool IMapViewInternal.IsCalloutVisible(Callout callout)
        {
            return _callouts.Contains(callout);
        }
    }
}
