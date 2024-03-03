﻿using NetTopologySuite.Geometries;

namespace Mapsui.Nts.Editing;

internal class ScaleInfo
{
    public GeometryFeature? Feature { get; set; }
    public Point? PreviousPosition { get; set; }
    public Point? Center { get; set; }

    public void Reset()
    {
        Feature = null;
        PreviousPosition = null;
        Center = null;
    }

}
