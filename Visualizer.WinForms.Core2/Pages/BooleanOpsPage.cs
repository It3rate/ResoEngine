using Core2.Elements;
using Core2.Support;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class BooleanOpsPage : IVisualizerPage
{
    private const float InputAY = 6.4f;
    private const float InputBY = 4.1f;
    private const float AndY = 0.9f;
    private const float OrY = -0.5f;
    private const float NandY = -1.9f;
    private const float NorY = -3.3f;
    private const float NotAY = -4.9f;
    private const float NotBY = -6.3f;
    private const float XorY = -7.7f;
    private const float XnorY = -9.1f;

    public string Title => "Boolean Operations (Core2)";

    private readonly AxisDisplayMapper _axisA = new(
        new Axis(new Proportion(3, 1), new Proportion(5, 1)),
        string.Empty);
    private readonly AxisDisplayMapper _axisB = new(
        new Axis(new Proportion(1, 1), new Proportion(3, 1)),
        string.Empty);

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(45, 45, 45),
        TextSize = 23f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(92, 92, 92),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _graphFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(249, 249, 250),
        IsAntialias = true,
    };

    private readonly SKPaint _graphBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(206, 206, 206),
        IsAntialias = true,
    };

    private readonly SKPaint _rulerLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(176, 176, 176),
        IsAntialias = true,
    };

    private readonly SKPaint _zeroAxisPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.3f,
        Color = new SKColor(176, 176, 176),
        IsAntialias = true,
    };

    private readonly SKPaint _topTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(24, 38, 94),
        IsAntialias = true,
    };

    private readonly SKPaint _bottomTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(80, 30, 112),
        IsAntialias = true,
    };

    private readonly SKPaint _tickTextPaint = new()
    {
        Color = new SKColor(132, 132, 132),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _badgeTextPaint = new()
    {
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _rowGuidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        PathEffect = SKPathEffect.CreateDash([6f, 4f], 0f),
        IsAntialias = true,
    };

    private readonly SKPaint _rowLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 4f,
        Color = SKColors.Black,
        StrokeCap = SKStrokeCap.Butt,
        IsAntialias = true,
    };

    private readonly SKPaint _rowDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.Black,
        IsAntialias = true,
    };

    private readonly SKPaint _rowArrowPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.Black,
        IsAntialias = true,
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
        StrokeWidth = VisualStyle.StrokeWidth,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };

    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };

    private static readonly BooleanRow[] Rows =
    [
        new("AND", AndY, SegmentColors.Green, AxisBooleanOperation.And),
        new("OR", OrY, SegmentColors.Orange, AxisBooleanOperation.Or),
        new("NAND", NandY, SegmentColors.Green, AxisBooleanOperation.Nand),
        new("NOR", NorY, SegmentColors.Orange, AxisBooleanOperation.Nor),
        new("NOT A", NotAY, SegmentColors.Red, AxisBooleanOperation.NotA),
        new("NOT B", NotBY, SegmentColors.Blue, AxisBooleanOperation.NotB),
        new("XOR", XorY, SegmentColors.Purple, AxisBooleanOperation.Xor),
        new("XNOR", XnorY, SegmentColors.Purple, AxisBooleanOperation.Xnor),
    ];

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        coords.OriginX = coords.Width * 0.5f;
        coords.OriginY = 380f;

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: InputAY);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Blue, crossPosition: InputBY);

        hitTest.Register(_rendererA, _axisA);
        hitTest.Register(_rendererB, _axisB);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawText("Boolean Operations On Directed Segments", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "The two interactive segments on the top define a shared frame. The lower rows show which partitions of that framed line stay true for each boolean rule.",
            34f,
            ref subtitleY,
            560f,
            _bodyPaint);

        GetGraphBounds(out var minValue, out var maxValue, out var topRect, out var resultsRect);
        DrawGraphFrames(canvas, minValue, maxValue, topRect, resultsRect);

        _rendererA?.Render(canvas, _axisA);
        DrawRowBadge(canvas, topRect, "A", InputAY, SegmentColors.Red);
        _rendererB?.Render(canvas, _axisB);
        DrawRowBadge(canvas, topRect, "B", InputBY, SegmentColors.Blue);

        DrawBooleanRows(canvas, minValue, maxValue, resultsRect);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawGraphFrames(SKCanvas canvas, decimal minValue, decimal maxValue, SKRect topRect, SKRect resultsRect)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawRoundRect(topRect, 16f, 16f, _graphFillPaint);
        canvas.DrawRoundRect(topRect, 16f, 16f, _graphBorderPaint);
        canvas.DrawRoundRect(resultsRect, 16f, 16f, _graphFillPaint);
        canvas.DrawRoundRect(resultsRect, 16f, 16f, _graphBorderPaint);

        float rulerTop = _coords.MathToPixel(0f, InputAY - 0.35f).Y;
        float rulerBottom = _coords.MathToPixel(0f, InputBY + 0.35f).Y;
        float rulerAxisY = _coords.MathToPixel(0f, (InputAY + InputBY) * 0.5f).Y;
        float rulerInsetLeft = topRect.Left + 164f;
        float rulerInsetRight = topRect.Right - 22f;

        canvas.Save();
        canvas.ClipRect(new SKRect(rulerInsetLeft, topRect.Top + 6f, rulerInsetRight, topRect.Bottom - 6f));
        PageChrome.DrawRuler(
            canvas,
            _coords,
            minValue,
            maxValue,
            rulerAxisY,
            rulerTop,
            rulerBottom,
            _rulerLinePaint,
            _zeroAxisPaint,
            _topTickPaint,
            _bottomTickPaint,
            _tickTextPaint,
            _tickTextPaint);
        canvas.Restore();

        float resultsTop = _coords.MathToPixel(0f, AndY + 0.7f).Y;
        float resultsBottom = _coords.MathToPixel(0f, XnorY - 0.7f).Y;
        float zeroX = ValueToPixelX(0m, minValue, maxValue, resultsRect);
        canvas.DrawLine(zeroX, resultsTop, zeroX, resultsBottom, _zeroAxisPaint);
    }

    private void DrawBooleanRows(SKCanvas canvas, decimal minValue, decimal maxValue, SKRect resultsRect)
    {
        foreach (var row in Rows)
        {
            DrawRowBadge(canvas, resultsRect, row.Label, row.Y, row.Colors);
            DrawBooleanRow(canvas, row, minValue, maxValue, resultsRect);
        }
    }

    private void DrawBooleanRow(
        SKCanvas canvas,
        BooleanRow row,
        decimal minValue,
        decimal maxValue,
        SKRect resultsRect)
    {
        if (_coords == null)
        {
            return;
        }

        decimal frameLeft = Math.Min(Math.Min(_axisA.Axis.Start.Value, _axisA.Axis.End.Value), Math.Min(_axisB.Axis.Start.Value, _axisB.Axis.End.Value));
        decimal frameRight = Math.Max(Math.Max(_axisA.Axis.Start.Value, _axisA.Axis.End.Value), Math.Max(_axisB.Axis.Start.Value, _axisB.Axis.End.Value));
        if (frameRight <= frameLeft)
        {
            return;
        }

        float y = _coords.MathToPixel(0f, row.Y).Y;
        float lineLeft = ValueToPixelX(frameLeft, minValue, maxValue, resultsRect);
        float lineRight = ValueToPixelX(frameRight, minValue, maxValue, resultsRect);
        canvas.DrawLine(lineLeft, y, lineRight, y, _rowGuidePaint);

        foreach (var piece in AxisBooleanProjection.Project(_axisA.Axis, _axisB.Axis, row.Operation))
        {
            DrawIntervalPiece(canvas, row.Colors, row.Y, piece.Segment.Start.Value, piece.Segment.End.Value, minValue, maxValue, resultsRect);
        }
    }

    private void DrawIntervalPiece(
        SKCanvas canvas,
        SegmentColorSet colors,
        float y,
        decimal start,
        decimal end,
        decimal minValue,
        decimal maxValue,
        SKRect resultsRect)
    {
        if (_coords == null)
        {
            return;
        }

        float startX = ValueToPixelX(start, minValue, maxValue, resultsRect);
        float endX = ValueToPixelX(end, minValue, maxValue, resultsRect);
        float pixelY = _coords.MathToPixel(0f, y).Y;

        _rowLinePaint.Color = colors.Solid;
        _rowDotPaint.Color = colors.Solid;
        _rowArrowPaint.Color = colors.Solid;

        float span = endX - startX;
        if (Math.Abs(span) < 1f)
        {
            canvas.DrawLine(startX, pixelY - 10f, startX, pixelY + 10f, _rowLinePaint);
            canvas.DrawCircle(startX, pixelY, VisualStyle.DotRadius, _rowDotPaint);
            return;
        }

        float direction = Math.Sign(span);
        float arrowSize = VisualStyle.ArrowSize;
        float lineEndX = endX - direction * (arrowSize * 1.3f);

        canvas.DrawLine(startX, pixelY, lineEndX, pixelY, _rowLinePaint);
        canvas.DrawCircle(startX, pixelY, VisualStyle.DotRadius, _rowDotPaint);

        using var arrowPath = new SKPath();
        arrowPath.MoveTo(endX, pixelY);
        arrowPath.LineTo(endX - direction * arrowSize * 1.5f, pixelY - arrowSize);
        arrowPath.LineTo(endX - direction * arrowSize * 1.5f, pixelY + arrowSize);
        arrowPath.Close();
        canvas.DrawPath(arrowPath, _rowArrowPaint);
    }

    private void DrawRowBadge(SKCanvas canvas, SKRect boxRect, string label, float y, SegmentColorSet colors)
    {
        if (_coords == null)
        {
            return;
        }

        var point = _coords.MathToPixel(0f, y);
        var bounds = new SKRect();
        _badgeTextPaint.MeasureText(label, ref bounds);
        float width = Math.Max(94f, bounds.Width + 28f);
        var rect = new SKRect(boxRect.Left + 16f, point.Y - 16f, boxRect.Left + 16f + width, point.Y + 16f);

        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(colors.Grid.Red, colors.Grid.Green, colors.Grid.Blue, 68),
            IsAntialias = true,
        };
        using var border = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = colors.Solid,
            IsAntialias = true,
        };

        _badgeTextPaint.Color = colors.Solid;
        canvas.DrawRoundRect(rect, 12f, 12f, fill);
        canvas.DrawRoundRect(rect, 12f, 12f, border);
        canvas.DrawText(label, rect.MidX, rect.MidY + 5f, _badgeTextPaint);
    }

    private void GetGraphBounds(out decimal minValue, out decimal maxValue, out SKRect topRect, out SKRect resultsRect)
    {
        if (_coords == null)
        {
            minValue = 0m;
            maxValue = 1m;
            topRect = SKRect.Empty;
            resultsRect = SKRect.Empty;
            return;
        }

        minValue = Math.Min(Math.Min(_axisA.Axis.Start.Value, _axisA.Axis.End.Value), Math.Min(_axisB.Axis.Start.Value, _axisB.Axis.End.Value));
        maxValue = Math.Max(Math.Max(_axisA.Axis.Start.Value, _axisA.Axis.End.Value), Math.Max(_axisB.Axis.Start.Value, _axisB.Axis.End.Value));

        minValue = decimal.Floor(minValue) - 1m;
        maxValue = decimal.Ceiling(maxValue) + 1m;
        if (maxValue - minValue < 24m)
        {
            minValue = -12m;
            maxValue = 12m;
        }

        float topTop = _coords.MathToPixel(0f, InputAY + 1.0f).Y;
        float topBottom = _coords.MathToPixel(0f, InputBY - 1.0f).Y;
        float resultsTop = _coords.MathToPixel(0f, AndY + 0.6f).Y;
        float resultsBottom = _coords.MathToPixel(0f, XnorY - 1.1f).Y;
        topRect = new SKRect(_coords.Width * 0.05f, topTop - 16f, _coords.Width * 0.95f, topBottom + 18f);
        resultsRect = new SKRect(_coords.Width * 0.05f, resultsTop - 12f, _coords.Width * 0.95f, resultsBottom + 22f);
    }

    private float ValueToPixelX(decimal value, decimal minValue, decimal maxValue, SKRect rect)
    {
        if (maxValue == minValue)
        {
            return rect.MidX;
        }

        float t = (float)((value - minValue) / (maxValue - minValue));
        return rect.Left + t * rect.Width;
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_axisA, _axisB];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0, 0);

    public void Destroy()
    {
        _rendererA?.Dispose(); _rendererA = null;
        _rendererB?.Dispose(); _rendererB = null;
        _coords = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _graphFillPaint.Dispose();
        _graphBorderPaint.Dispose();
        _rulerLinePaint.Dispose();
        _zeroAxisPaint.Dispose();
        _topTickPaint.Dispose();
        _bottomTickPaint.Dispose();
        _tickTextPaint.Dispose();
        _badgeTextPaint.Dispose();
        _rowGuidePaint.Dispose();
        _rowLinePaint.Dispose();
        _rowDotPaint.Dispose();
        _rowArrowPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

    private sealed record BooleanRow(string Label, float Y, SegmentColorSet Colors, AxisBooleanOperation Operation);
}
