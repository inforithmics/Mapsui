#if __AVALONIA__
global using Avalonia;
global using Avalonia.Interactivity;

global using BindableObject = Avalonia.AvaloniaObject;
global using BindingMode = Avalonia.Data.BindingMode;
#elif __WINUI__
global using Microsoft.UI.Xaml;

global using BindableProperty = Microsoft.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
global using BindingMode = Microsoft.UI.Xaml.Data.BindingMode;
#elif __UWP__
global using Windows.UI.Xaml;

global using BindableProperty = Windows.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
global using BindingMode = Windows.UI.Xaml.Data.BindingMode;
#elif __WPF__
global using System.Windows;

global using BindableProperty = System.Windows.DependencyProperty;
global using Point = System.Windows.Point;
global using BindingMode = System.Windows.Data.BindingMode;
#elif __MAUI__
global using Microsoft.Maui.Controls;

global using Point = Microsoft.Maui.Graphics.Point;
global using Color = Microsoft.Maui.Graphics.Color;
global using KnownColor = Mapsui.UI.Utils.KnownColor;
global using TextAlignment = Microsoft.Maui.TextAlignment;
global using Thickness = Microsoft.Maui.Thickness;
#elif __FORMS__
global using Xamarin.Forms;

global using Color = Xamarin.Forms.Color;
global using KnownColor = Xamarin.Forms.Color;
global using Point = Xamarin.Forms.Point;
#elif __ANDROID__
global using Mapsui.UI.Utils;

global using Point = Android.Graphics.Point;
#elif __IOS__
global using Mapsui.UI.Utils;

global using Point = System.Drawing.Point;
#elif __ETO_FORMS__
global using Mapsui.UI.Utils;

global using Color = System.Drawing.Color;
global using KnownColor = System.Drawing.Color;
#endif