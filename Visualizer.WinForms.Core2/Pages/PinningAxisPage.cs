using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed partial class PinningAxisPage : IVisualizerPage
{
    private const long MinEncoding = -1;
    private const long MaxEncoding = 1;

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;

    private readonly List<FieldLayout> _fieldLayouts = [];
    private readonly List<ButtonLayout> _buttonLayouts = [];
    private string? _activeFieldKey;
    private bool _showCell = true;
    private bool _showGuides = true;

    private long _recessiveUnit = 1;
    private long _recessiveValue = 1;
    private long _dominantUnit = 1;
    private long _dominantValue = 1;

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(38, 38, 38),
        TextSize = 25f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(95, 95, 95),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _cardFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(252, 252, 252),
        IsAntialias = true,
    };
    private readonly SKPaint _cardStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(218, 218, 218),
        IsAntialias = true,
    };
    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _captionPaint = new()
    {
        Color = new SKColor(120, 120, 120),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _trackPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(195, 195, 195),
        IsAntialias = true,
        StrokeCap = SKStrokeCap.Round,
    };
    private readonly SKPaint _tickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(188, 188, 188),
        IsAntialias = true,
    };
    private readonly SKPaint _zeroTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(118, 118, 118),
        IsAntialias = true,
    };
    private readonly SKPaint _tickTextPaint = new()
    {
        Color = new SKColor(140, 140, 140),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _sceneFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(250, 250, 250),
        IsAntialias = true,
    };
    private readonly SKPaint _sceneStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(222, 222, 222),
        IsAntialias = true,
    };
    private readonly SKPaint _guideCarrierPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(185, 185, 185, 150),
        IsAntialias = true,
        PathEffect = SKPathEffect.CreateDash([6f, 6f], 0f),
    };
    private readonly SKPaint _guideTextPaint = new()
    {
        Color = new SKColor(150, 150, 150),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _cellFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 240, 184, 145),
        IsAntialias = true,
    };
    private readonly SKPaint _cellStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(208, 176, 62, 210),
        IsAntialias = true,
    };
    private readonly SKPaint _combinedPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 5f,
        Color = new SKColor(128, 128, 128),
        IsAntialias = true,
        StrokeCap = SKStrokeCap.Round,
    };
    private readonly SKPaint _originFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };
    private readonly SKPaint _originStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(72, 72, 72),
        IsAntialias = true,
    };
    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(72, 72, 72),
        IsAntialias = true,
    };
    private readonly SKPaint _badgeFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255, 242),
        IsAntialias = true,
    };
    private readonly SKPaint _badgeStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(214, 214, 214, 235),
        IsAntialias = true,
    };
    private readonly SKPaint _badgeTextPaint = new()
    {
        Color = new SKColor(72, 72, 72),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonTextPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _buttonFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(214, 214, 214),
        IsAntialias = true,
    };

    public string Title => "Pinning Axis Explorer";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
    }

    public void Render(SKCanvas canvas)
    {
        Axis descriptor = BuildDescriptor();
        var geometry = new PinAxisDisplayGeometry(descriptor);
        _fieldLayouts.Clear();
        _buttonLayouts.Clear();

        float width = _coords?.Width ?? 900f;
        float height = _coords?.Height ?? 800f;

        var inputRect = new SKRect(24f, 118f, 360f, height - 30f);
        var sceneRect = new SKRect(388f, 104f, width - 18f, height - 24f);

        canvas.DrawText("Pinning Axis Explorer", 32f, 44f, _headingPaint);
        float textY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "The four controls are arranged as a quadrant: top-right is uV, bottom-right is u, top-left is i, and bottom-left is iV. " +
            "Positive units keep a side on its native carrier, negative units rupture it into the next orthogonal carrier, and value sign chooses direction within that carrier.",
            32f,
            ref textY,
            840f,
            _bodyPaint);

        DrawInputCards(canvas, inputRect);
        DrawDerivedScene(canvas, sceneRect, descriptor, geometry);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        var button = HitButton(pixelPoint);
        if (button != null)
        {
            button.OnClick();
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var field = HitField(pixelPoint);
        if (field == null)
        {
            return false;
        }

        _activeFieldKey = field.Key;
        UpdateFieldValue(field, pixelPoint.X);
        _canvasHost?.InvalidateCanvas();
        return true;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost == null)
        {
            return;
        }

        if (_activeFieldKey != null)
        {
            var field = _fieldLayouts.FirstOrDefault(layout => layout.Key == _activeFieldKey);
            if (field != null)
            {
                UpdateFieldValue(field, pixelPoint.X);
                _canvasHost.Cursor = Cursors.SizeWE;
                _canvasHost.InvalidateCanvas();
                return;
            }
        }

        if (HitField(pixelPoint) != null)
        {
            _canvasHost.Cursor = Cursors.SizeWE;
            return;
        }

        _canvasHost.Cursor = HitButton(pixelPoint) != null ? Cursors.Hand : Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        _activeFieldKey = null;
        if (_canvasHost != null)
        {
            _canvasHost.Cursor = Cursors.Default;
        }
    }

    public void Destroy()
    {
        _fieldLayouts.Clear();
        _buttonLayouts.Clear();
        _activeFieldKey = null;
        _coords = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardStrokePaint.Dispose();
        _labelPaint.Dispose();
        _captionPaint.Dispose();
        _trackPaint.Dispose();
        _tickPaint.Dispose();
        _zeroTickPaint.Dispose();
        _tickTextPaint.Dispose();
        _sceneFillPaint.Dispose();
        _sceneStrokePaint.Dispose();
        _guideCarrierPaint.Dispose();
        _guideTextPaint.Dispose();
        _cellFillPaint.Dispose();
        _cellStrokePaint.Dispose();
        _combinedPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
        _badgeFillPaint.Dispose();
        _badgeStrokePaint.Dispose();
        _badgeTextPaint.Dispose();
        _buttonTextPaint.Dispose();
        _buttonFillPaint.Dispose();
        _buttonStrokePaint.Dispose();
    }

    private void DrawInputCards(SKCanvas canvas, SKRect inputRect)
    {
        canvas.DrawRoundRect(inputRect, 20f, 20f, _cardFillPaint);
        canvas.DrawRoundRect(inputRect, 20f, 20f, _cardStrokePaint);
        canvas.DrawText("Pin Descriptor", inputRect.Left + 20f, inputRect.Top + 28f, _labelPaint);

        float captionY = inputRect.Top + 50f;
        PageChrome.DrawWrappedText(
            canvas,
            "Right column is uV over u. Left column is i over iV, rotated into the recessive orientation.",
            inputRect.Left + 20f,
            ref captionY,
            inputRect.Width - 40f,
            _captionPaint);

        float gridTop = inputRect.Top + 108f;
        float cardGap = 16f;
        float fieldWidth = (inputRect.Width - 20f * 2f - cardGap) * 0.5f;
        float fieldHeight = 110f;
        float left = inputRect.Left + 20f;
        float right = left + fieldWidth + cardGap;

        DrawFieldCard(canvas, new SKRect(left, gridTop, left + fieldWidth, gridTop + fieldHeight), "i", "recessive unit", _recessiveUnit, SegmentColors.Red.Solid, "recessive-unit", MinEncoding, MaxEncoding);
        DrawFieldCard(canvas, new SKRect(right, gridTop, right + fieldWidth, gridTop + fieldHeight), "uV", "dominant value", _dominantValue, SegmentColors.Blue.Solid, "dominant-value", MinEncoding, MaxEncoding);
        DrawFieldCard(canvas, new SKRect(left, gridTop + fieldHeight + cardGap, left + fieldWidth, gridTop + fieldHeight * 2f + cardGap), "iV", "recessive value", _recessiveValue, SegmentColors.Red.Solid, "recessive-value", MinEncoding, MaxEncoding);
        DrawFieldCard(canvas, new SKRect(right, gridTop + fieldHeight + cardGap, right + fieldWidth, gridTop + fieldHeight * 2f + cardGap), "u", "dominant unit", _dominantUnit, SegmentColors.Blue.Solid, "dominant-unit", MinEncoding, MaxEncoding);

        float buttonsTop = gridTop + fieldHeight * 2f + cardGap + 22f;
        DrawActionButton(canvas, new SKRect(left, buttonsTop, left + fieldWidth, buttonsTop + 34f), "Directed", false, () => ApplyPreset(1, 1, 1, 1));
        DrawActionButton(canvas, new SKRect(right, buttonsTop, right + fieldWidth, buttonsTop + 34f), "Acceleration", false, () => ApplyPreset(1, -1, 1, 1));
        DrawActionButton(canvas, new SKRect(left, buttonsTop + 44f, left + fieldWidth, buttonsTop + 78f), "Bent", false, () => ApplyPreset(-1, 1, 1, 1));
        DrawActionButton(canvas, new SKRect(right, buttonsTop + 44f, right + fieldWidth, buttonsTop + 78f), "Noise Hold", false, () => ApplyPreset(0, 1, 1, 1));
        DrawActionButton(canvas, new SKRect(left, buttonsTop + 98f, left + fieldWidth, buttonsTop + 132f), "Show Cell", _showCell, () => _showCell = !_showCell);
        DrawActionButton(canvas, new SKRect(right, buttonsTop + 98f, right + fieldWidth, buttonsTop + 132f), "Show Guides", _showGuides, () => _showGuides = !_showGuides);
    }

    private void DrawFieldCard(
        SKCanvas canvas,
        SKRect rect,
        string label,
        string caption,
        long value,
        SKColor accent,
        string key,
        long minValue,
        long maxValue)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _cardStrokePaint);

        using var accentPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accent,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(new SKRect(rect.Left, rect.Top, rect.Left + 8f, rect.Bottom), 18f, 18f, accentPaint);
        using var accentTextPaint = new SKPaint
        {
            Color = accent,
            TextSize = 15f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };
        float labelY = rect.Top + 26f;
        canvas.DrawText(label, rect.Left + 18f, labelY, accentTextPaint);
        float labelWidth = accentTextPaint.MeasureText(label);
        canvas.DrawText(caption, rect.Left + 24f + labelWidth, labelY, _captionPaint);

        float trackLeft = rect.Left + 20f;
        float trackRight = rect.Right - 18f;
        float trackY = rect.Bottom - 30f;
        DrawField(canvas, key, string.Empty, string.Empty, value, accent, trackLeft, trackRight, trackY, rect.Top, minValue, maxValue);
    }

    private void DrawField(
        SKCanvas canvas,
        string key,
        string label,
        string kind,
        long value,
        SKColor accent,
        float left,
        float right,
        float y,
        float cardTop,
        long minValue,
        long maxValue)
    {
        float zeroX = ValueToX(0, left, right, minValue, maxValue);
        float handleX = ValueToX(value, left, right, minValue, maxValue);

        canvas.DrawLine(left, y, right, y, _trackPaint);

        for (long tick = minValue; tick <= maxValue; tick++)
        {
            float x = ValueToX(tick, left, right, minValue, maxValue);
            float tickTop = tick == 0 ? y - 11f : y - 7f;
            float tickBottom = tick == 0 ? y + 11f : y + 7f;
            canvas.DrawLine(x, tickTop, x, tickBottom, tick == 0 ? _zeroTickPaint : _tickPaint);

            if (tick != 0)
            {
                canvas.DrawText(tick.ToString(), x, y + 24f, _tickTextPaint);
            }
        }

        canvas.DrawLine(zeroX, y - 14f, zeroX, y + 14f, _zeroTickPaint);

        using var accentTextPaint = new SKPaint
        {
            Color = accent,
            TextSize = 14f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };
        using var valuePaint = new SKPaint
        {
            Color = new SKColor(72, 72, 72),
            TextSize = 13f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };
        using var handleFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accent,
            IsAntialias = true,
        };
        using var handleStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            Color = SKColors.White,
            IsAntialias = true,
        };

        if (!string.IsNullOrWhiteSpace(kind))
        {
            canvas.DrawText(label, left - 94f, y + 5f, accentTextPaint);
            canvas.DrawText(kind, left - 46f, y + 5f, _captionPaint);
        }
        else
        {
            canvas.DrawText(label, left, y - 18f, accentTextPaint);
        }

        float bubbleCenterX = (left + right) * 0.5f;
        var bubbleRect = new SKRect(bubbleCenterX - 24f, y - 50f, bubbleCenterX + 24f, y - 22f);
        canvas.DrawRoundRect(bubbleRect, 12f, 12f, _badgeFillPaint);
        canvas.DrawRoundRect(bubbleRect, 12f, 12f, _badgeStrokePaint);
        canvas.DrawText(value.ToString(), bubbleRect.MidX, bubbleRect.MidY + 5f, valuePaint);

        canvas.DrawCircle(handleX, y, 10f, handleFill);
        canvas.DrawCircle(handleX, y, 10f, handleStroke);

        var interactiveRect = new SKRect(left - 14f, y - 20f, right + 14f, y + 20f);
        var handleRect = new SKRect(handleX - 14f, y - 14f, handleX + 14f, y + 14f);
        _fieldLayouts.Add(new FieldLayout(key, interactiveRect, handleRect, left, right, y, minValue, maxValue));
    }

    private void DrawActionButton(SKCanvas canvas, SKRect rect, string text, bool isActive, Action onClick)
    {
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = isActive ? new SKColor(236, 244, 255) : _buttonFillPaint.Color,
            IsAntialias = true,
        };
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = isActive ? new SKColor(110, 154, 220) : _buttonStrokePaint.Color,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 14f, 14f, fillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, strokePaint);
        canvas.DrawText(text, rect.MidX, rect.MidY + 4f, _buttonTextPaint);

        _buttonLayouts.Add(new ButtonLayout(rect, onClick));
    }

    private void DrawDerivedScene(SKCanvas canvas, SKRect rect, Axis descriptor, PinAxisDisplayGeometry geometry)
    {
        canvas.DrawRoundRect(rect, 22f, 22f, _sceneFillPaint);
        canvas.DrawRoundRect(rect, 22f, 22f, _sceneStrokePaint);

        canvas.DrawText("Resolved Structure", rect.Left + 22f, rect.Top + 30f, _labelPaint);
        DrawBadge(canvas, new SKPoint(rect.Left + 22f, rect.Top + 58f), $"Relation: {geometry.Relation}");
        DrawBadge(canvas, new SKPoint(rect.Left + 22f, rect.Top + 92f), $"R: {DescribeSide(geometry.RecessiveRay)}");
        DrawBadge(canvas, new SKPoint(rect.Left + 220f, rect.Top + 92f), $"D: {DescribeSide(geometry.DominantRay)}");
        DrawBadge(canvas, new SKPoint(rect.Left + 22f, rect.Top + 126f), FormatDescriptor(descriptor));

        var viewport = new SKRect(rect.Left + 18f, rect.Top + 158f, rect.Right - 18f, rect.Bottom - 18f);
        DrawViewport(canvas, viewport, geometry);
    }

    private void DrawViewport(SKCanvas canvas, SKRect rect, PinAxisDisplayGeometry geometry)
    {
        float padding = 34f;
        var inner = new SKRect(rect.Left + padding, rect.Top + padding, rect.Right - padding, rect.Bottom - padding);
        var origin = new SKPoint(inner.MidX, inner.MidY + 8f);
        float scale = MathF.Min(inner.Width, inner.Height) / 2.8f;

        using var viewportFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 180),
            IsAntialias = true,
        };
        using var viewportStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = new SKColor(225, 225, 225),
            IsAntialias = true,
        };
        canvas.DrawRoundRect(rect, 18f, 18f, viewportFill);
        canvas.DrawRoundRect(rect, 18f, 18f, viewportStroke);

        string sceneTitle = DescribeSceneTitle(geometry.Descriptor);
        var titleBounds = new SKRect();
        _labelPaint.MeasureText(sceneTitle, ref titleBounds);
        canvas.DrawText(sceneTitle, rect.MidX - titleBounds.Width * 0.5f, rect.Top + 26f, _labelPaint);

        if (ShowGuides)
        {
            canvas.DrawLine(inner.Left, origin.Y, inner.Right, origin.Y, _guideCarrierPaint);
            canvas.DrawLine(origin.X, inner.Top, origin.X, inner.Bottom, _guideCarrierPaint);
        }

        DrawUnitDisplay(canvas, origin, scale, geometry);

        bool sameDirection = geometry.RecessiveRay.HasEndpoint &&
            geometry.DominantRay.HasEndpoint &&
            geometry.RecessiveRay.CarrierRank == geometry.DominantRay.CarrierRank &&
            geometry.RecessiveRay.DirectionSign == geometry.DominantRay.DirectionSign;

        DrawRay(canvas, origin, scale, geometry.RecessiveRay, SegmentColors.Red.Solid, "R", sameDirection ? new SKPoint(0f, -18f) : SKPoint.Empty);
        DrawRay(canvas, origin, scale, geometry.DominantRay, SegmentColors.Blue.Solid, "D", sameDirection ? new SKPoint(0f, 18f) : SKPoint.Empty);

        DrawNoiseIndicators(canvas, origin);

        canvas.DrawCircle(origin, VisualStyle.OriginDotRadius, _originFillPaint);
        canvas.DrawCircle(origin, VisualStyle.OriginDotRadius, _originStrokePaint);
        canvas.DrawCircle(origin, 3f, _originDotPaint);
    }

    private void DrawRay(SKCanvas canvas, SKPoint origin, float scale, PinDisplayRay ray, SKColor color, string shortLabel, SKPoint labelOffset)
    {
        using var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4f,
            Color = color,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
        };
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };
        using var labelPaint = new SKPaint
        {
            Color = color,
            TextSize = 13f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };

        if (ray.IsUnresolved)
        {
            canvas.DrawText($"{shortLabel}: unresolved", origin.X + 14f + labelOffset.X, origin.Y - 16f + labelOffset.Y, labelPaint);
            return;
        }

        if (!ray.HasEndpoint)
        {
            canvas.DrawText($"{shortLabel}: zero", origin.X + 14f + labelOffset.X, origin.Y - 16f + labelOffset.Y, labelPaint);
            return;
        }

        var end = ToPixel(ray.Endpoint, origin, scale);
        if (shortLabel == "R")
        {
            DrawLineWithCircleEnd(canvas, origin, end, linePaint, fillPaint);
        }
        else
        {
            DrawArrow(canvas, origin, end, linePaint, fillPaint);
        }

        if (ray.IsLifted)
        {
            DrawLiftMarker(canvas, origin, end, color);
        }

        var labelPoint = new SKPoint(
            end.X + (end.X >= origin.X ? 10f : -42f) + labelOffset.X,
            end.Y + (end.Y <= origin.Y ? -10f : 18f) + labelOffset.Y);
        canvas.DrawText($"{shortLabel} {DescribeDirection(ray)}", labelPoint.X, labelPoint.Y, labelPaint);
    }

    private void DrawUnitMarker(SKCanvas canvas, SKRect rect, SKColor color)
    {
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = _cellFillPaint.Color.WithAlpha(190),
            IsAntialias = true,
        };
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.6f,
            Color = color.WithAlpha(210),
            IsAntialias = true,
        };

        canvas.DrawRect(rect, fillPaint);
        canvas.DrawRect(rect, strokePaint);
    }

    private void DrawUnitDisplay(SKCanvas canvas, SKPoint origin, float scale, PinAxisDisplayGeometry geometry)
    {
        bool recessiveNoise = geometry.Resolution.RecessiveSide.ValueSign == 0 || geometry.Resolution.RecessiveSide.UnitSign == 0;
        bool dominantNoise = geometry.Resolution.DominantSide.ValueSign == 0 || geometry.Resolution.DominantSide.UnitSign == 0;
        SKPoint recessiveDisplayBasis = recessiveNoise ? SKPoint.Empty : geometry.RecessiveBasis;
        SKPoint dominantDisplayBasis = dominantNoise ? SKPoint.Empty : geometry.DominantBasis;
        SKPoint recessiveUnitBasis = GetUnitBasis(geometry.Resolution.RecessiveSide);
        SKPoint dominantUnitBasis = GetUnitBasis(geometry.Resolution.DominantSide);

        bool showUnitCell =
            ShowCell &&
            !recessiveNoise &&
            !dominantNoise &&
            recessiveDisplayBasis != SKPoint.Empty &&
            dominantDisplayBasis != SKPoint.Empty &&
            AreOrthogonal(recessiveDisplayBasis, dominantDisplayBasis);

        if (showUnitCell)
        {
            DrawUnitCell(canvas, origin, scale, recessiveDisplayBasis, dominantDisplayBasis);
            return;
        }

        if (!recessiveNoise && recessiveDisplayBasis != SKPoint.Empty && recessiveUnitBasis != SKPoint.Empty)
        {
            SKRect? recessiveRect = GetUnitMarkerRect(origin, scale, recessiveDisplayBasis, recessiveUnitBasis);
            if (recessiveRect.HasValue)
            {
                DrawUnitMarker(canvas, recessiveRect.Value, SegmentColors.Red.Solid);
            }
        }

        if (!dominantNoise && dominantDisplayBasis != SKPoint.Empty && dominantUnitBasis != SKPoint.Empty)
        {
            SKRect? dominantRect = GetUnitMarkerRect(origin, scale, dominantDisplayBasis, dominantUnitBasis);
            if (dominantRect.HasValue)
            {
                DrawUnitMarker(canvas, dominantRect.Value, SegmentColors.Blue.Solid);
            }
        }
    }

    private void DrawUnitCell(SKCanvas canvas, SKPoint origin, float scale, SKPoint recessiveBasis, SKPoint dominantBasis)
    {
        using var path = new SKPath();
        SKPoint[] corners =
        [
            origin,
            ToPixel(recessiveBasis, origin, scale),
            ToPixel(new SKPoint(recessiveBasis.X + dominantBasis.X, recessiveBasis.Y + dominantBasis.Y), origin, scale),
            ToPixel(dominantBasis, origin, scale),
        ];

        path.MoveTo(corners[0]);
        for (int index = 1; index < corners.Length; index++)
        {
            path.LineTo(corners[index]);
        }

        path.Close();
        canvas.DrawPath(path, _cellFillPaint);
        canvas.DrawPath(path, _cellStrokePaint);
    }

    private SKRect? GetUnitMarkerRect(SKPoint origin, float scale, SKPoint displayBasis, SKPoint unitBasis)
    {
        if (displayBasis == SKPoint.Empty || unitBasis == SKPoint.Empty)
        {
            return null;
        }

        float length = scale;
        float thickness = scale * 0.09f;
        float offset = thickness * 1.15f;
        var center = new SKPoint(
            origin.X + unitBasis.Y * offset,
            origin.Y + unitBasis.X * offset);
        var end = new SKPoint(center.X + displayBasis.X * length, center.Y - displayBasis.Y * length);
        float minX = MathF.Min(center.X, end.X);
        float maxX = MathF.Max(center.X, end.X);
        float minY = MathF.Min(center.Y, end.Y);
        float maxY = MathF.Max(center.Y, end.Y);

        return displayBasis.X != 0f
            ? new SKRect(minX, center.Y - thickness, maxX, center.Y + thickness)
            : new SKRect(center.X - thickness, minY, center.X + thickness, maxY);
    }

    private void DrawLiftMarker(SKCanvas canvas, SKPoint origin, SKPoint end, SKColor color)
    {
        using var markerPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = color.WithAlpha(180),
            IsAntialias = true,
        };

        float radius = 16f;
        var rect = new SKRect(origin.X - radius, origin.Y - radius, origin.X + radius, origin.Y + radius);
        float start = end.Y < origin.Y ? 180f : 0f;
        canvas.DrawArc(rect, start, 90f, false, markerPaint);
    }

    private void DrawNoiseHalo(SKCanvas canvas, SKPoint origin, SKColor color, int ringCount)
    {
        for (int index = 0; index < ringCount; index++)
        {
            DrawNoiseRing(canvas, origin, 22f + index * 12f, color);
        }
    }

    private void DrawNoiseRing(SKCanvas canvas, SKPoint origin, float radius, SKColor color)
    {
        using var noisePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.6f,
            Color = color.WithAlpha(180),
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash([2f, 5f], 0f),
        };

        canvas.DrawCircle(origin, radius, noisePaint);
    }

    private void DrawArrow(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint linePaint, SKPaint fillPaint)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length < 0.001f)
        {
            return;
        }

        float ux = dx / length;
        float uy = dy / length;
        float arrowLength = 13f;
        float arrowWidth = 8f;
        var lineEnd = new SKPoint(end.X - ux * arrowLength, end.Y - uy * arrowLength);

        canvas.DrawLine(start, lineEnd, linePaint);

        float perpX = -uy;
        float perpY = ux;
        float baseX = end.X - ux * arrowLength;
        float baseY = end.Y - uy * arrowLength;

        using var arrow = new SKPath();
        arrow.MoveTo(end);
        arrow.LineTo(baseX + perpX * arrowWidth, baseY + perpY * arrowWidth);
        arrow.LineTo(baseX - perpX * arrowWidth, baseY - perpY * arrowWidth);
        arrow.Close();
        canvas.DrawPath(arrow, fillPaint);
    }

    private void DrawLineWithCircleEnd(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint linePaint, SKPaint fillPaint)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length < 0.001f)
        {
            return;
        }

        float radius = 7f;
        float ux = dx / length;
        float uy = dy / length;
        var lineEnd = new SKPoint(end.X - ux * radius, end.Y - uy * radius);
        canvas.DrawLine(start, lineEnd, linePaint);
        canvas.DrawCircle(end, radius, fillPaint);
    }

    private void DrawBadge(SKCanvas canvas, SKPoint origin, string text)
    {
        var bounds = new SKRect();
        _badgeTextPaint.MeasureText(text, ref bounds);
        var rect = new SKRect(
            origin.X,
            origin.Y - 16f,
            origin.X + bounds.Width + 20f,
            origin.Y + 10f);

        canvas.DrawRoundRect(rect, 11f, 11f, _badgeFillPaint);
        canvas.DrawRoundRect(rect, 11f, 11f, _badgeStrokePaint);
        canvas.DrawText(text, rect.Left + 10f, rect.MidY + 5f, _badgeTextPaint);
    }

    private Axis BuildDescriptor() =>
        new(_recessiveValue, _recessiveUnit, _dominantValue, _dominantUnit);

    private void ApplyPreset(long recessiveUnit, long recessiveValue, long dominantUnit, long dominantValue)
    {
        _recessiveUnit = recessiveUnit;
        _recessiveValue = recessiveValue;
        _dominantUnit = dominantUnit;
        _dominantValue = dominantValue;
    }

    private bool ShowCell => _showCell;

    private bool ShowGuides => _showGuides;

    private FieldLayout? HitField(SKPoint pixelPoint) =>
        _fieldLayouts.FirstOrDefault(field =>
            field.HandleRect.Contains(pixelPoint.X, pixelPoint.Y) ||
            field.TrackRect.Contains(pixelPoint.X, pixelPoint.Y));

    private ButtonLayout? HitButton(SKPoint pixelPoint) =>
        _buttonLayouts.FirstOrDefault(button => button.Rect.Contains(pixelPoint.X, pixelPoint.Y));

    private void UpdateFieldValue(FieldLayout field, float x)
    {
        long snapped = XToValue(x, field.TrackLeft, field.TrackRight, field.MinValue, field.MaxValue);
        switch (field.Key)
        {
            case "recessive-unit":
                _recessiveUnit = snapped;
                break;
            case "recessive-value":
                _recessiveValue = snapped;
                break;
            case "dominant-unit":
                _dominantUnit = snapped;
                break;
            case "dominant-value":
                _dominantValue = snapped;
                break;
        }
    }

    private static float ValueToX(long value, float left, float right, long minValue, long maxValue)
    {
        float ratio = (value - minValue) / (float)(maxValue - minValue);
        return left + (right - left) * ratio;
    }

    private static long XToValue(float x, float left, float right, long minValue, long maxValue)
    {
        float ratio = Math.Clamp((x - left) / (right - left), 0f, 1f);
        long value = (long)MathF.Round(minValue + ratio * (maxValue - minValue));
        return Math.Clamp(value, minValue, maxValue);
    }

    private static SKPoint ToPixel(SKPoint logicalPoint, SKPoint origin, float scale) =>
        new(origin.X + logicalPoint.X * scale, origin.Y - logicalPoint.Y * scale);

    private string DescribeSceneTitle(Axis axis)
    {
        bool recessiveNoise = _recessiveValue == 0 || _recessiveUnit == 0;
        bool dominantNoise = _dominantValue == 0 || _dominantUnit == 0;
        if (recessiveNoise && dominantNoise)
        {
            return "Pure noise";
        }

        return axis.PinResolution.Behavior switch
        {
            PinBehaviorKind.DirectedSegment => "Directed segment",
            PinBehaviorKind.SequentialReinforcement => "Acceleration",
            PinBehaviorKind.OrthogonalStructure => "Bent segment",
            _ => "Noisy segment",
        };
    }

    private static string DescribeSide(PinDisplayRay ray)
    {
        if (ray.IsUnresolved)
        {
            return "noise or unresolved carrier";
        }

        if (!ray.CarrierRank.HasValue)
        {
            return "no carrier";
        }

        return $"{(ray.CarrierRank == 0 ? "carrier 0" : "carrier 1")}, {DescribeDirection(ray)}";
    }

    private static string DescribeDirection(PinDisplayRay ray)
    {
        if (!ray.CarrierRank.HasValue || ray.DirectionSign == 0)
        {
            return "still";
        }

        return ray.CarrierRank.Value switch
        {
            0 when ray.DirectionSign > 0 => "right",
            0 when ray.DirectionSign < 0 => "left",
            1 when ray.DirectionSign > 0 => "up",
            1 when ray.DirectionSign < 0 => "down",
            _ => "still",
        };
    }

    private static string FormatDescriptor(Axis axis) =>
        $"[{axis.Recessive.Dominant}/{axis.Recessive.Recessive}]i + [{axis.Dominant.Dominant}/{axis.Dominant.Recessive}]";

    private void DrawNoiseIndicators(SKCanvas canvas, SKPoint origin)
    {
        bool recessiveNoise = _recessiveValue == 0 || _recessiveUnit == 0;
        bool dominantNoise = _dominantValue == 0 || _dominantUnit == 0;

        if (!recessiveNoise && !dominantNoise)
        {
            return;
        }

        if (recessiveNoise && dominantNoise)
        {
            DrawAlternatingNoiseRings(canvas, origin);
            return;
        }

        DrawNoiseHalo(canvas, origin, recessiveNoise ? SegmentColors.Red.Solid : SegmentColors.Blue.Solid, 3);
    }

    private void DrawAlternatingNoiseRings(SKCanvas canvas, SKPoint origin)
    {
        SKColor[] colors =
        [
            SegmentColors.Blue.Solid,
            SegmentColors.Red.Solid,
            SegmentColors.Blue.Solid,
            SegmentColors.Red.Solid,
        ];

        for (int index = 0; index < colors.Length; index++)
        {
            DrawNoiseRing(canvas, origin, 22f + index * 12f, colors[index]);
        }
    }

    private static bool AreOrthogonal(SKPoint left, SKPoint right) =>
        Math.Abs(left.X * right.X + left.Y * right.Y) < 0.01f;

    private static SKPoint GetUnitBasis(PinResolvedSide side)
    {
        if (!side.HasCarrier)
        {
            return SKPoint.Empty;
        }

        int naturalDirection = side.Role == PinSideRole.Recessive ? -1 : 1;
        return side.CarrierRank switch
        {
            0 => new SKPoint(naturalDirection, 0f),
            1 => new SKPoint(0f, naturalDirection),
            _ => SKPoint.Empty,
        };
    }


    private sealed record FieldLayout(
        string Key,
        SKRect TrackRect,
        SKRect HandleRect,
        float TrackLeft,
        float TrackRight,
        float TrackY,
        long MinValue,
        long MaxValue);

    private sealed record ButtonLayout(SKRect Rect, Action OnClick);
}
