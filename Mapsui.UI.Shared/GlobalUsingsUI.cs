#if __AVALONIA__
global using Avalonia;
global using Avalonia.Interactivity;
global using Avalonia.Skia;

global using BindableObject = Avalonia.AvaloniaObject;
global using BindingMode = Avalonia.Data.BindingMode;
global using Color = System.Drawing.Color;
global using KnownColor = System.Drawing.Color;
#elif __WINUI__
global using Microsoft.UI.Xaml;
global using SkiaSharp.Views.Windows;

global using BindableProperty = Microsoft.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
global using BindingMode = Microsoft.UI.Xaml.Data.BindingMode;
global using Color = System.Drawing.Color;
#elif __UWP__
global using Windows.UI.Xaml;
global using SkiaSharp.Views.UWP;

global using BindableProperty = Windows.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
global using BindingMode = Windows.UI.Xaml.Data.BindingMode;
global using Color = System.Drawing.Color;
#elif __WPF__
global using System.Windows;
global using SkiaSharp.Views.WPF;

global using BindableProperty = System.Windows.DependencyProperty;
global using Point = System.Windows.Point;
global using BindingMode = System.Windows.Data.BindingMode;
global using Color = System.Windows.Media.Color;
global using KnownColor = System.Windows.Media.Color;
global using FontAttributes = System.Windows.FontWeight;
#elif __MAUI__
global using Microsoft.Maui.Controls;
global using SkiaSharp.Views.Maui;

global using Point = Microsoft.Maui.Graphics.Point;
global using Color = Microsoft.Maui.Graphics.Color;
global using KnownColor = Mapsui.UI.Utils.KnownColor;
global using TextAlignment = Microsoft.Maui.TextAlignment;
global using Thickness = Microsoft.Maui.Thickness;
#elif __FORMS__
global using Xamarin.Forms;
global using SkiaSharp.Views.Forms;

global using Color = Xamarin.Forms.Color;
global using KnownColor = Xamarin.Forms.Color;
global using Point = Xamarin.Forms.Point;
#elif __ANDROID__
global using Mapsui.UI.Utils;
global using SkiaSharp.Views.Android;

global using Point = Android.Graphics.Point;
#elif __IOS__
global using Mapsui.UI.Utils;
global using SkiaSharp.Views.iOS;

global using Point = System.Drawing.Point;
#elif __ETO_FORMS__
global using Mapsui.UI.Utils;

global using Color = System.Drawing.Color;
global using KnownColor = System.Drawing.Color;
#endif