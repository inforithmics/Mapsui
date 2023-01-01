#if __AVALONIA__
global using Avalonia;
global using Avalonia.Data;
global using Avalonia.Interactivity;
#elif __WINUI__
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Data;
global using BindableProperty = Microsoft.UI.Xaml.DependencyProperty;
#elif __UWP__
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Data;
global using BindableProperty = Windows.UI.Xaml.DependencyProperty;
#elif __WPF__
global using System.Windows;
global using System.Windows.Data;
global using BindableProperty = System.Windows.DependencyProperty;
#elif __MAUI__
global using Microsoft.Maui.Controls;
#elif __FORMS__
global using Xamarin.Forms;
#else
global using Mapsui.UI.Utils;
#endif