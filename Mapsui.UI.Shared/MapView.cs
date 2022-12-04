using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
    public partial class MapView : MapControl
    {
    }
}
