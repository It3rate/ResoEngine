using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;

namespace ResoEngine.Visualizer.Rendering;

public enum SegmentOrientation { Horizontal, Vertical }

/// <summary>
/// Renders a single directed segment using SkiaSharp.
///
/// Rendering rules:
///   - Origin (0) → imaginary endpoint = DASHED line
///   - Imaginary endpoint → real endpoint = SOLID/BOLD line
///   - Filled dot at imaginary endpoint (start of segment)
///   - Arrow at real endpoint pointing FROM imaginary TOWARD real
///   - Origin dot is NOT drawn here (drawn by page on top of everything)
/// </summary>
public class SegmentRenderer : IDisposable
{
    private readonly CoordinateSystem _coords;
    private readonly SegmentOrientation _orientation;
    private readonly SegmentColorSet _colors;
    private readonly float _crossPosition;

    private readonly SKPaint _dashPaint;
    private readonly SKPaint _solidPaint;
    private readonly SKPaint _arrowPaint;
    private readonly SKPaint _dotPaint;
    private readonly SKPaint _labelPaint;
    private readonly SKPaint _labelRightPaint;

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

        _labelPaint = new SKPaint
        {
            Color = colors.Label,
            TextSize = VisualStyle.FontSize,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };

        _labelRightPaint = new SKPaint
        {
            Color = colors.Label,
            TextSize = VisualStyle.FontSize,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Right,
            IsAntialias = true,
        };
    }

    /// <summary>Compute key pixel positions for a segment.</summary>
    private (SKPoint origin, SKPoint imag, SKPoint real) GetPixelPositions(ISegmentValue segment)
    {
        bool isH = _orientation == SegmentOrientation.Horizontal;
        float cp = _crossPosition;
        var origin = _coords.MathToPixel(isH ? 0 : cp, isH ? cp : 0);
        var imag = _coords.MathToPixel(isH ? segment.Imaginary : cp, isH ? cp : segment.Imaginary);
        var real = _coords.MathToPixel(isH ? segment.Real : cp, isH ? cp : segment.Real);
        return (origin, imag, real);
    }

    /// <summary>Render the segment onto the canvas.</summary>
    public void Render(SKCanvas canvas, ISegmentValue segment)
    {
        var (originPx, imagPx, realPx) = GetPixelPositions(segment);

        // Direction vector from imaginary toward real (in pixel space)
        float dirX = realPx.X - imagPx.X;
        float dirY = realPx.Y - imagPx.Y;
        float len = MathF.Sqrt(dirX * dirX + dirY * dirY);
        bool isH = _orientation == SegmentOrientation.Horizontal;
        if (len < 1f)
        {
            canvas.DrawLine(originPx, imagPx, _dashPaint);
            DrawNoDirectionMarker(canvas, imagPx);

            string imagLabelZero = FormatValue(segment.Imaginary, isImaginary: true);
            string realLabelZero = FormatValue(segment.Real, isImaginary: false);
            DrawEndpointLabels(canvas, imagPx, realPx, imagLabelZero, realLabelZero, true);

            if (!string.IsNullOrEmpty(segment.Label))
            {
                float lx = isH ? imagPx.X + 20f : imagPx.X + 14f;
                float ly = isH ? imagPx.Y - 10f : imagPx.Y - 10f;
                canvas.DrawText(segment.Label, lx, ly, _labelPaint);
            }

            return;
        }

        float ux = dirX / len;
        float uy = dirY / len;

        // --- 1. Dashed line: origin → imaginary endpoint ---
        canvas.DrawLine(originPx, imagPx, _dashPaint);

        // --- 2. Solid line: imaginary → real (stopping short for arrow) ---
        float arrowLen = VisualStyle.ArrowSize * 1.5f - 2f;
        var solidEnd = new SKPoint(realPx.X - ux * arrowLen, realPx.Y - uy * arrowLen);
        canvas.DrawLine(imagPx, solidEnd, _solidPaint);

        // --- 3. Arrow at real endpoint, pointing from imaginary toward real ---
        float s = VisualStyle.ArrowSize;
        float tipX = realPx.X + ux * 2;
        float tipY = realPx.Y + uy * 2;
        float perpX = -uy;
        float perpY = ux;
        float baseX = tipX - ux * s * 1.5f;
        float baseY = tipY - uy * s * 1.5f;

        using var arrowPath = new SKPath();
        arrowPath.MoveTo(tipX, tipY);
        arrowPath.LineTo(baseX + perpX * s, baseY + perpY * s);
        arrowPath.LineTo(baseX - perpX * s, baseY - perpY * s);
        arrowPath.Close();
        canvas.DrawPath(arrowPath, _arrowPaint);

        // --- 4. Filled dot at imaginary endpoint (start of segment) ---
        canvas.DrawCircle(imagPx, VisualStyle.DotRadius, _dotPaint);

        // --- 5. Labels ---
        string imagLabel = FormatValue(segment.Imaginary, isImaginary: true);
        string realLabel = FormatValue(segment.Real, isImaginary: false);
        DrawEndpointLabels(canvas, imagPx, realPx, imagLabel, realLabel, false);

        if (!string.IsNullOrEmpty(segment.Label))
        {
            float lx = tipX + ux * 22 + perpX * 4;
            float ly = tipY + uy * 22 + perpY * 4 + 7;
            canvas.DrawText(segment.Label, lx, ly, _labelPaint);
        }
    }

    private void DrawNoDirectionMarker(SKCanvas canvas, SKPoint point)
    {
        canvas.DrawLine(point.X, point.Y - 10f, point.X, point.Y + 10f, _solidPaint);
        canvas.DrawCircle(point, VisualStyle.DotRadius, _dotPaint);
    }

    private void DrawEndpointLabels(
        SKCanvas canvas,
        SKPoint imagPx,
        SKPoint realPx,
        string imagLabel,
        string realLabel,
        bool zeroLength)
    {
        bool isH = _orientation == SegmentOrientation.Horizontal;
        if (isH)
        {
            float imagX = imagPx.X;
            float realX = realPx.X;
            float imagY = imagPx.Y + 28f;
            float realY = zeroLength ? realPx.Y + 46f : realPx.Y + 28f;

            if (!zeroLength)
            {
                float imagWidth = _labelPaint.MeasureText(imagLabel);
                float realWidth = _labelPaint.MeasureText(realLabel);
                float minGap = (imagWidth + realWidth) * 0.5f + 12f;
                float currentGap = MathF.Abs(realX - imagX);
                if (currentGap < minGap)
                {
                    float center = (imagX + realX) * 0.5f;
                    imagX = center - minGap * 0.5f;
                    realX = center + minGap * 0.5f;
                }
            }

            canvas.DrawText(imagLabel, imagX, imagY, _labelPaint);
            canvas.DrawText(realLabel, realX, realY, _labelPaint);
            return;
        }

        float imagXVertical = imagPx.X - 22f;
        float realXVertical = realPx.X - 22f;
        float imagYVertical = imagPx.Y + 7f;
        float realYVertical = zeroLength ? realPx.Y + 25f : realPx.Y + 7f;

        if (!zeroLength)
        {
            const float minVerticalGap = 18f;
            float currentGap = MathF.Abs(realYVertical - imagYVertical);
            if (currentGap < minVerticalGap)
            {
                float center = (imagYVertical + realYVertical) * 0.5f;
                imagYVertical = center - minVerticalGap * 0.5f;
                realYVertical = center + minVerticalGap * 0.5f;
            }
        }

        canvas.DrawText(imagLabel, imagXVertical, imagYVertical, _labelRightPaint);
        canvas.DrawText(realLabel, realXVertical, realYVertical, _labelRightPaint);
    }

    /// <summary>Hit-test a pixel point against this segment's drag zones.</summary>
    public DragZone? HitTest(SKPoint point, ISegmentValue segment)
    {
        var (_, imagPx, realPx) = GetPixelPositions(segment);
        float pad = VisualStyle.HitPadding;

        if (SKPoint.Distance(point, imagPx) <= pad)
            return DragZone.Dot;

        if (SKPoint.Distance(point, realPx) <= pad)
            return DragZone.Arrow;

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
        if (MathF.Abs(rounded) < 0.05f)
        {
            rounded = 0f;
        }

        if (isImaginary)
        {
            // Imaginary axis is positive-left, so negate for display
            float display = -rounded;
            if (MathF.Abs(display) < 0.05f)
            {
                display = 0f;
            }

            string sign = display >= 0 ? "+" : "";
            return $"{sign}{display}i";
        }
        else
        {
            string sign = rounded >= 0 ? "+" : "";
            return $"{sign}{rounded}";
        }
    }

    public void Dispose()
    {
        _dashPaint.Dispose();
        _solidPaint.Dispose();
        _arrowPaint.Dispose();
        _dotPaint.Dispose();
        _labelPaint.Dispose();
        _labelRightPaint.Dispose();
        GC.SuppressFinalize(this);
    }
}
