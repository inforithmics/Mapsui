using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Mapsui.Fetcher
{
    internal interface IFetchDispatcher // Todo: Make internal
    {
        bool TryTake([NotNullWhen(true)] out Func<Task>? method);
        event PropertyChangedEventHandler PropertyChanged;
    }
}