using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;

namespace ResoEngine.Visualizer.Rendering;

public enum SegmentOrientation { Horizontal, Vertical }

/// <summary>
/// Renders a single directed segment using SkiaSharp.
/// Draws: dashed imaginary line, filled dot, hollow origin circle,
/// solid real line, arrow, and value labels.
/// Also performs geometric hit-testing for drag targets.
/// </summary>
public class SegmentRenderer : IDisposable
{
    private readonly CoordinateSystem _coords;
    private readonly SegmentOrientation _orientation;
    private readonly SegmentColorSet _colors;
    private readonly float _crossPosition;

    // Pre-allocated paints
    private readonly SKPaint _dashPaint;
    private readonly SKPaint _solidPaint;
    private readonly SKPaint _arrowPaint;
    private readonly SKPaint _dotPaint;
    private readonly SKPaint _circleFillPaint;
    private readonly SKPaint _circleStrokePaint;
    private readonly SKPaint _labelPaint;

    public SegmentRenderer(CoordinateSystem coords, SegmentOrientation orientation,
                           SegmentColorSet colors, float crossPosition = 0f)
    {
        _coords = coords;
        _orientation = orientation;
        _colors = colors;
        _crossPosition = crossPosition;

        _dashPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.StrokeWidth,
            Color = colors.Solid,
            PathEffect = SKPathEffect.CreateDash(VisualStyle.DashPattern, 0),
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Butt,
        };

        _solidPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.StrokeWidth,
            Color = colors.Solid,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Butt,
        };

        _arrowPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = colors.Solid,
            IsAntialias = true,
        };

        _dotPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = colors.Solid,
            IsAntialias = true,
        };

        _circleFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White,
            IsAntialias = true,
        };

        _circleStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.StrokeWidth,
            Color = colors.Solid,
            IsAntialias = true,
        };

        _labelPaint = new SKPaint
        {
            Color = colors.Label,
            TextSize = VisualStyle.FontSize,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };
    }

    /// <summary>Render the segment onto the canvas.</summary>
    public void Render(SKCanvas canvas, DirectedSegment segment)
    {
        bool isH = _orientation == SegmentOrientation.Horizontal;
        float cp = _crossPosition;
        float imaginary = segment.Imaginary;
        float real = segment.Real;

        // Compute pixel positions
        var originPx = _coords.MathToPixel(isH ? 0 : cp, isH ? cp : 0);
        var imagPx = _coords.MathToPixel(isH ? imaginary : cp, isH ? cp : imaginary);
        var realPx = _coords.MathToPixel(isH ? real : cp, isH ? cp : real);

        // --- Dashed line (imaginary end → origin) ---
        canvas.DrawLine(imagPx, originPx, _dashPaint);

        // --- Solid line (origin → real end, stopping short for arrow) ---
        float arrowLen = VisualStyle.ArrowSize + 4;
        SKPoint solidEnd;
        if (isH)
        {
            float dx = real >= 0 ? -arrowLen : arrowLen;
            solidEnd = new SKPoint(realPx.X + dx, realPx.Y);
        }
        else
        {
            float dy = real >= 0 ? arrowLen : -arrowLen;
            solidEnd = new SKPoint(realPx.X, realPx.Y + dy);
        }
        canvas.DrawLine(originPx, solidEnd, _solidPaint);

        // --- Arrow polygon ---
        float s = VisualStyle.ArrowSize;
        using var arrowPath = new SKPath();
        if (isH)
        {
            float dir = real >= 0 ? 1 : -1;
            float tipX = realPx.X + dir * 4;
            arrowPath.MoveTo(tipX, realPx.Y);
            arrowPath.LineTo(tipX - dir * s * 1.5f, realPx.Y - s);
            arrowPath.LineTo(tipX - dir * s * 1.5f, realPx.Y + s);
            arrowPath.Close();
        }
        else
        {
            float dir = real >= 0 ? -1 : 1; // pixel Y is inverted
            float tipY = realPx.Y + dir * 4;
            arrowPath.MoveTo(realPx.X, tipY);
            arrowPath.LineTo(realPx.X - s, tipY - dir * s * 1.5f);
            arrowPath.LineTo(realPx.X + s, tipY - dir * s * 1.5f);
            arrowPath.Close();
        }
        canvas.DrawPath(arrowPath, _arrowPaint);

        // --- Filled dot at imaginary end ---
        canvas.DrawCircle(imagPx, VisualStyle.DotRadius, _dotPaint);

        // --- Hollow origin circle ---
        canvas.DrawCircle(originPx, VisualStyle.CircleRadius, _circleFillPaint);
        canvas.DrawCircle(originPx, VisualStyle.CircleRadius, _circleStrokePaint);

        // --- Labels ---
        // Imaginary value label (at imaginary endpoint)
        string imagLabel = FormatValue(imaginary, isImaginary: true);
        if (isH)
            canvas.DrawText(imagLabel, imagPx.X, imagPx.Y + 30, _labelPaint);
        else
        {
            using var leftAlign = _labelPaint.Clone();
            leftAlign.TextAlign = SKTextAlign.Right;
            canvas.DrawText(imagLabel, imagPx.X - 30, imagPx.Y + 7, leftAlign);
        }

        // Real value label
        string realLabel = FormatValue(real, isImaginary: false);
        if (isH)
            canvas.DrawText(realLabel, realPx.X, realPx.Y + 30, _labelPaint);
        else
        {
            using var leftAlign = _labelPaint.Clone();
            leftAlign.TextAlign = SKTextAlign.Right;
            canvas.DrawText(realLabel, realPx.X - 30, realPx.Y + 7, leftAlign);
        }

        // Axis label (e.g., "A" or "B")
        if (!string.IsNullOrEmpty(segment.Label))
        {
            if (isH)
                canvas.DrawText(segment.Label, realPx.X + 30, realPx.Y + 7, _labelPaint);
            else
                canvas.DrawText(segment.Label, realPx.X, realPx.Y - 20, _labelPaint);
        }
    }

    /// <summary>Hit-test a pixel point against this segment's drag zones.</summary>
    public DragZone? HitTest(SKPoint point, DirectedSegment segment)
    {
        bool isH = _orientation == SegmentOrientation.Horizontal;
        float cp = _crossPosition;

        var imagPx = _coords.MathToPixel(isH ? segment.Imaginary : cp, isH ? cp : segment.Imaginary);
        var realPx = _coords.MathToPixel(isH ? segment.Real : cp, isH ? cp : segment.Real);

        float pad = VisualStyle.HitPadding;

        // Dot (imaginary end) — circle hit
        if (SKPoint.Distance(point, imagPx) <= pad)
            return DragZone.Dot;

        // Arrow (real end) — circle hit
        if (SKPoint.Distance(point, realPx) <= pad)
            return DragZone.Arrow;

        // Bar (line segment) — distance from point to line
        if (DistanceToLineSegment(point, imagPx, realPx) <= pad)
            return DragZone.Bar;

        return null;
    }

    public SegmentOrientation GetSegmentOrientation() => _orientation;

    private static float DistanceToLineSegment(SKPoint p, SKPoint a, SKPoint b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float lenSq = dx * dx + dy * dy;
        if (lenSq < 0.001f) return SKPoint.Distance(p, a);

        float t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lenSq, 0, 1);
        var proj = new SKPoint(a.X + t * dx, a.Y + t * dy);
        return SKPoint.Distance(p, proj);
    }

    private static string FormatValue(float val, bool isImaginary)
    {
        float rounded = MathF.Round(val * 10) / 10;
        string sign = rounded >= 0 ? "+" : "";
        return isImaginary ? $"{sign}{rounded}i" : $"{sign}{rounded}";
    }

    public void Dispose()
    {
        _dashPaint.Dispose();
        _solidPaint.Dispose();
        _arrowPaint.Dispose();
        _dotPaint.Dispose();
        _circleFillPaint.Dispose();
        _circleStrokePaint.Dispose();
        _labelPaint.Dispose();
        GC.SuppressFinalize(this);
    }
}
