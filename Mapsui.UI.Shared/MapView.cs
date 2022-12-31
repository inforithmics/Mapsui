using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Mapsui.UI.Objects;
using Mapsui.UI.Utils;

#if __MAUI__
using Microsoft.Maui.Controls;
namespace Mapsui.UI.Maui
#elif __UWP__
using Windows.UI.Xaml.Data;
namespace Mapsui.UI.Uwp
#elif __ANDROID__ && !HAS_UNO_WINUI
namespace Mapsui.UI.Android
#elif __IOS__ && !HAS_UNO_WINUI
namespace Mapsui.UI.iOS
#elif __WINUI__
using Microsoft.UI.Xaml.Data;
namespace Mapsui.UI.WinUI
#elif __FORMS__
using Xamarin.Forms;
namespace Mapsui.UI.Forms
#elif __AVALONIA__
namespace Mapsui.UI.Avalonia
#elif __ETO_FORMS__
namespace Mapsui.UI.Eto
#else
using System.Windows.Data;
namespace Mapsui.UI.Wpf
#endif
{
    public partial class MapView : MapControl, IMapView, IMapViewInternal
    {
#if __ANDROID__ || __IOS__
        private Dictionary<object, object> properties = new Dictionary<object, object>();
#endif
        
        public static readonly BindableProperty UniqueCalloutProperty = BindableProperty.Create(nameof(UniqueCallout), typeof(bool), typeof(MapView), false, defaultBindingMode: BindingMode.TwoWay);

        private readonly ObservableRangeCollection<ICallout> _callouts = new ObservableRangeCollection<ICallout>();
        
        /// <summary>
        /// Single or multiple callouts possible
        /// </summary>
        public bool UniqueCallout
        {
            get => (bool)GetValue(UniqueCalloutProperty);
            set => SetValue(UniqueCalloutProperty, value);
        }
        
#if __ANDROID__ || __IOS__
        private object GetValue(object property)
        {
            properties.TryGetValue(property, out var result);
            return result;
        }

        private void SetValue(object property, object value)
        {
            properties[property] = value;
        }
#endif        
        
        /// <summary>
        /// Hide all visible callouts
        /// </summary>
        public void HideCallouts()
        {
            _callouts.Clear();
        }
        
        void IMapViewInternal.AddCallout(ICallout callout)
        {
            if (!_callouts.Contains(callout))
            {
                if (UniqueCallout)
                    HideCallouts();

                _callouts.Add(callout);

                Refresh();
            }
        }

        void IMapViewInternal.RemoveCallout(ICallout? callout)
        {
            if (callout != null && _callouts.Contains(callout))
            {
                _callouts.Remove(callout);

                Refresh();
            }
        }

        bool IMapViewInternal.IsCalloutVisible(ICallout callout)
        {
            return _callouts.Contains(callout);
        }
    }
}
