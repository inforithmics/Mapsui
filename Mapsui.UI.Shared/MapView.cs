using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Avalonia;
using Avalonia.Data;
using Mapsui.UI.Objects;

#if __MAUI__
using Microsoft.Maui.Controls;
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
