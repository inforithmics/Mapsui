﻿using Mapsui.Extensions;
using SkiaSharp;

namespace Mapsui.Rendering.Skia.Extensions;

public static class ViewportExtensions
{
    public static SKMatrix ToSKMatrix(this ViewportState viewport)
    {
        var mapCenterX = (float)viewport.Width * 0.5f;
        var mapCenterY = (float)viewport.Height * 0.5f;
        var invertedResolution = 1f / (float)viewport.Resolution;

        var matrix = SKMatrix.CreateScale(invertedResolution, invertedResolution, mapCenterX, mapCenterY);
        matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(1, -1, 0, -mapCenterY)); // As a consequence images will be up side down :(
        if (viewport.IsRotated()) matrix = SKMatrix.Concat(matrix, SKMatrix.CreateRotationDegrees((float)-viewport.Rotation));
        matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation((float)-viewport.CenterX, (float)-viewport.CenterY));
        return matrix;
    }

    public static SKMatrix ToSKMatrix(this ViewportState viewport, SKMatrix priorMatrix)
    {      
        var userRotation = SKMatrix.CreateRotationDegrees((float)viewport.Rotation);
        var zoomScale = SKMatrix.CreateScale((float)(1.0 / viewport.Resolution), (float)(1.0 / viewport.Resolution));
        var moveToCenter = SKMatrix.CreateTranslation((float)-viewport.CenterX, (float)-viewport.CenterY);
             
        var matrix = SKMatrix.Concat(userRotation, zoomScale);
        matrix = SKMatrix.Concat(moveToCenter, matrix);
        matrix = SKMatrix.Concat(priorMatrix, matrix);

        return matrix;
    }

    /// <summary> Converts the Extent of the Viewport to a SKRect </summary>
    /// <param name="viewport">viewport</param>
    /// <returns>SkRect</returns>
    public static SKRect ToSkiaRect(this ViewportState viewport)
    {
        return viewport.WorldToScreen(viewport.ToExtent()).ToSkia();
    }
}
