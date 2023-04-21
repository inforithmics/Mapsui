﻿using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Mapsui.UI.Forms;
using NUnit.Framework;

#if __MAUI__
using Mapsui.UI.Maui;
#elif __UWP__
using Mapsui.UI.Uwp;
#elif __ANDROID__ && !HAS_UNO_WINUI
using Mapsui.UI.Android;
#elif __IOS__ && !HAS_UNO_WINUI && !__FORMS__
using Mapsui.UI.iOS;
#elif __WINUI__
using Mapsui.UI.WinUI;
#elif __FORMS__
using Mapsui.UI.Forms;
#elif __AVALONIA__
using Mapsui.UI.Avalonia;
#elif __ETO_FORMS__
using Mapsui.UI.Eto;
#elif __BLAZOR__
using Mapsui.UI.Blazor;
#elif __WPF__
using Mapsui.UI.Wpf;
#else
using Mapsui.UI;
#endif

namespace Mapsui.Tests.Layers;

[TestFixture]
public class LayerThreadingTests
{
    private readonly ConcurrentBag<Exception> _exceptions = new();

    [Test]
    public async Task TestObservableCollectionProviderAsync()
    {
        _exceptions.Clear();

        var observableCollection = new ObservableCollection<Callout>();
        var provider = new ObservableCollectionProvider<Callout>(observableCollection);
        using var layer = new Layer("test");
        layer.DataSource = provider;


        var task1 = Task.Run(() =>
        {
            try
            {
                FillCollection(observableCollection);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        });

        var task2 = Task.Run(async () =>
        {
            try
            {
                await GetFeaturesAsync(provider);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        });

        var task3 = Task.Run(() =>
        {
            try
            {
                GetFeatures(layer);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        });

        // wait for tasks to finish;
        await task2;
        await task3;
        await task1;

        Assert.IsTrue(_exceptions.Count == 0); // no Exceptions should have occurred
    }

    private async Task GetFeaturesAsync(ObservableCollectionProvider<Callout> provider)
    {
        for (int i = 0; i < 5000; i++)
        {
            try
            {
                await provider.GetFeaturesAsync(new FetchInfo(new MSection(new MRect(0, 0, 0, 0), 1)));
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }
    }

    private void GetFeatures(Layer layer)
    {
        for (int i = 0; i < 5000; i++)
        {
            try
            {
                layer.GetFeatures(new MRect(0, 0, 0, 0), 1);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }
    }

    private void FillCollection(ObservableCollection<Callout> observableCollection)
    {
        for (int i = 0; i < 5000; i++)
        {
            try
            {
#pragma warning disable IDISP004
                observableCollection.Add(new Callout(new Pin()));
#pragma warning restore IDISP004
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }
    }
}
