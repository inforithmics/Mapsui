﻿#if __AVALONIA__
global using Avalonia;
global using Avalonia.Data;
global using Avalonia.Interactivity;

global using BindableObject = Avalonia.AvaloniaObject;
#elif __WINUI__
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Data;

global using BindableProperty = Microsoft.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
#elif __UWP__
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Data;

global using BindableProperty = Windows.UI.Xaml.DependencyProperty;
global using Point = Windows.Foundation.Point;
#elif __WPF__
global using System.Windows;
global using System.Windows.Data;

global using BindableProperty = System.Windows.DependencyProperty;
#elif __MAUI__
global using Microsoft.Maui.Controls;

global using Point = Microsoft.Maui.Graphics.Point;
global using Color = Microsoft.Maui.Graphics.Color;
global using KnownColor = Mapsui.UI.Maui.KnownColor;
#elif __FORMS__
global using Xamarin.Forms;

global using Color = Xamarin.Forms.Color;
global using KnownColor = Xamarin.Forms.Color;
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