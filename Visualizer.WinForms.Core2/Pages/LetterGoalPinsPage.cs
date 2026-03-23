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

        float boxWidth = MathF.Min(520f, width - 180f);
        float boxHeight = MathF.Min(760f, height - 160f);
        SKRect letterBox = new(
            (width - boxWidth) * 0.5f,
            86f + MathF.Max(0f, (height - 86f - boxHeight - 80f) * 0.5f),
            (width + boxWidth) * 0.5f,
            86f + MathF.Max(0f, (height - 86f - boxHeight - 80f) * 0.5f) + boxHeight);

        canvas.DrawRoundRect(letterBox, 28f, 28f, _letterBoxFillPaint);
        canvas.DrawRoundRect(letterBox, 28f, 28f, _letterBoxStrokePaint);

        foreach (LetterGoalPin pin in _prototype.Pins)
        {
            SKPoint point = MapPin(letterBox, pin);
            canvas.DrawCircle(point, 18f, _pinFillPaint);
            canvas.DrawCircle(point, 18f, _pinStrokePaint);

            bool placeLeft = ToFloat(_prototype.ActiveCalibration.Resolve(pin.Horizontal).Representative) >= 0.66f;
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

    private SKPoint MapPin(SKRect letterBox, LetterGoalPin pin) =>
        new(
            letterBox.Left + (letterBox.Width * ToFloat(_prototype.ActiveCalibration.Resolve(pin.Horizontal).Representative)),
            letterBox.Top + (letterBox.Height * ToFloat(_prototype.ActiveCalibration.Resolve(pin.Vertical).Representative)));

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
