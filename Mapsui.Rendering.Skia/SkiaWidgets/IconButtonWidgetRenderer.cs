﻿using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidget;
using SkiaSharp;

namespace Mapsui.Rendering.Skia.SkiaWidgets;

public class IconButtonWidgetRenderer : ISkiaWidgetRenderer
{
    public void Draw(SKCanvas canvas, Viewport viewport, IWidget widget, float layerOpacity)
    {
        var button = (IconButtonWidget)widget;

        if (button.Picture == null && string.IsNullOrEmpty(button.SvgImage))
            return;

        button.Picture ??= button.SvgImage?.LoadSvgPicture();

        var picture = button.Picture as SKPicture;

        if (picture == null)
            return;

        // Calc Envelope by Width/Height or, if not set, by size of content
        button.UpdateEnvelope(
            button.Width != 0 ? button.Width : picture.CullRect.Width + button.PaddingX * 2, 
            button.Height != 0 ? button.Height : picture.CullRect.Height + button.PaddingY * 2, 
            viewport.Width, 
            viewport.Height);

        if (button.Envelope == null)
            return;

        using var backPaint = new SKPaint { Color = button.BackColor.ToSkia(layerOpacity), IsAntialias = true };
        canvas.DrawRoundRect(button.Envelope.ToSkia(), (float)button.CornerRadius, (float)button.CornerRadius, backPaint);

        // Get the scale for picture in each direction
        var scaleX = (button.Envelope.Width - button.PaddingX * 2) / picture.CullRect.Width;
        var scaleY = (button.Envelope.Height - button.PaddingY * 2) / picture.CullRect.Height;

        // Rotate picture
        var matrix = SKMatrix.CreateRotationDegrees((float)button.Rotation, picture.CullRect.Width / 2f, picture.CullRect.Height / 2f);

        // Create a scale matrix
        matrix = matrix.PostConcat(SKMatrix.CreateScale((float)scaleX, (float)scaleY));

        // Translate picture to right place
        matrix = matrix.PostConcat(SKMatrix.CreateTranslation((float)(button.Envelope.MinX + button.PaddingX), (float)(button.Envelope.MinY + button.PaddingY)));

        using var skPaint = new SKPaint { IsAntialias = true };
        canvas.DrawPicture(picture, ref matrix, skPaint);
    }
}
