using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mapsui.UI;
using Mapsui.UI.Objects;

namespace Mapsui.Nts.Objects;

public interface IPin : IFeatureProvider, IDisposable, INotifyPropertyChanged
{
    bool IsCalloutVisible();
    void HideCallout();
    bool RotateWithMap { get; set; }
    IMapControl MapView { get; set; }

    /// <summary>
    /// Position of pin, place where anchor is
    /// </summary>
    Position Position { get; set; }
}
