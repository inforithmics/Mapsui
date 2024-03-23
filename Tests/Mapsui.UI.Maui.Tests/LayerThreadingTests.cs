﻿using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Mapsui.UI.Maui;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
                await GetFeaturesAsync(provider, CancellationToken.None);
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

        ClassicAssert.IsTrue(_exceptions.Count == 0); // no Exceptions should have occurred
    }

    private async Task GetFeaturesAsync(ObservableCollectionProvider<Callout> provider, CancellationToken cancellationToken)
    {
        for (int i = 0; i < 5000; i++)
        {
            try
            {
                await provider.GetFeaturesAsync(new FetchInfo(new MSection(new MRect(0, 0, 0, 0), 1)), cancellationToken);
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
                observableCollection.Add(new Callout(new Pin()));
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }
    }
}
