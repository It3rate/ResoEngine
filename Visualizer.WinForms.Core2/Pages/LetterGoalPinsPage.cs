using Applied.Geometry.LetterFormation;
using Core2.Elements;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed class LetterGoalPinsPage : IVisualizerPage
{
    private readonly LetterGoalPrototype _prototype = LetterGoalPrototypeCatalog.CapitalA;

    public string Title => "Letter A Goal Pins";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
    }

    public void Render(SKCanvas canvas)
    {
        float width = canvas.LocalClipBounds.Width;
        float height = canvas.LocalClipBounds.Height;

        canvas.DrawText("Letter A Goal Pins", 38f, 48f, _titlePaint);

        float boxWidth = MathF.Min(620f, width - 180f);
        float boxHeight = MathF.Min(820f, height - 170f);
        float boxTop = 86f + MathF.Max(0f, (height - 86f - boxHeight - 80f) * 0.5f);
        SKRect letterBox = new(
            (width - boxWidth) * 0.5f,
            boxTop,
            (width + boxWidth) * 0.5f,
            boxTop + boxHeight);

        canvas.DrawRoundRect(letterBox, 28f, 28f, _letterBoxFillPaint);
        canvas.DrawRoundRect(letterBox, 28f, 28f, _letterBoxStrokePaint);

        SKRect frameRect = CreateFrameRect(letterBox);
        DrawFrame(canvas, frameRect);

        foreach (LetterGoalPin pin in _prototype.Pins)
        {
            SKPoint point = MapPin(letterBox, pin);
            canvas.DrawCircle(point, 18f, _pinFillPaint);
            canvas.DrawCircle(point, 18f, _pinStrokePaint);

            bool placeLeft = point.X >= frameRect.MidX;
            float labelX = placeLeft ? point.X - 16f : point.X + 16f;
            float labelY = point.Y - 12f;
            float labelWidth = _pinLabelPaint.MeasureText(pin.Id);
            canvas.DrawText(
                pin.Id,
                placeLeft ? labelX - labelWidth : labelX,
                labelY,
                _pinLabelPaint);
        }
    }

    private void DrawFrame(SKCanvas canvas, SKRect frameRect)
    {
        foreach (LetterBoxFrameAxis axis in _prototype.Frame.Axes)
        {
            (Applied.Geometry.Utils.PlanarPoint from, Applied.Geometry.Utils.PlanarPoint to) = _prototype.Frame.ResolveEndpoints(axis);
            canvas.DrawLine(MapPoint(frameRect, from), MapPoint(frameRect, to), _frameAxisPaint);
        }
    }

    private static SKRect CreateFrameRect(SKRect container)
    {
        const float fillRatio = 0.8f;
        const float letterAspect = 0.68f;
        float availableWidth = container.Width * fillRatio;
        float availableHeight = container.Height * fillRatio;
        float frameHeight = MathF.Min(availableHeight, availableWidth / letterAspect);
        float frameWidth = frameHeight * letterAspect;
        float left = container.MidX - (frameWidth * 0.5f);
        float top = container.MidY - (frameHeight * 0.5f);
        return new SKRect(left, top, left + frameWidth, top + frameHeight);
    }

    private SKPoint MapPin(SKRect frameRect, LetterGoalPin pin) =>
        MapPoint(frameRect, _prototype.Frame.ResolvePoint(pin.Placement.AxisId, pin.Placement.PositionOnAxis));

    private static SKPoint MapPoint(SKRect frameRect, Applied.Geometry.Utils.PlanarPoint point) =>
        new(
            frameRect.Left + (frameRect.Width * ToFloat(point.Horizontal)),
            frameRect.Top + (frameRect.Height * ToFloat(point.Vertical)));

    private static float ToFloat(Proportion value) => (float)(double)value.Fold();





    private readonly SKPaint _titlePaint = new()
    {
        Color = new SKColor(38, 38, 38),
        TextSize = 25f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _letterBoxFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(239, 247, 255),
        IsAntialias = true,
    };

    private readonly SKPaint _letterBoxStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(205, 214, 224),
        IsAntialias = true,
    };

    private readonly SKPaint _frameAxisPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.5f,
        Color = new SKColor(150, 150, 150),
        PathEffect = SKPathEffect.CreateDash([6f, 6f], 0f),
        IsAntialias = true,
    };

    private readonly SKPaint _pinFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _pinStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(92, 92, 92),
        IsAntialias = true,
    };

    private readonly SKPaint _pinLabelPaint = new()
    {
        Color = new SKColor(56, 56, 56),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    public void Destroy()
    {
    }

    public void Dispose()
    {
    }
}
