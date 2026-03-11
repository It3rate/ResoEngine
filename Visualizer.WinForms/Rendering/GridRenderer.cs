using ResoEngine.Visualizer.Core;
using SkiaSharp;

namespace ResoEngine.Visualizer.Rendering;

/// <summary>
/// Renders the orthogonal grid for two directed segments.
///
/// Each segment has three axis points: 0 (origin), imaginary, real.
/// This creates two sections:
///   Dashed section:  0 → imaginary
///   Solid section:   imaginary → real
///
/// Grid rules (two regions per axis, three rendering zones):
///   Stripe region = [real, clamp(0, solidMin, solidMax)]
///                  (from real toward 0, clipped to the solid section)
///   Imaginary region = [0, imaginary]  (the dashed section)
///   Yellow fill = where both axes' imaginary regions overlap
///   Grid lines = from each axis' stripe region, extending across the other axis
/// </summary>
public class GridRenderer : IDisposable
{
    private readonly CoordinateSystem _coords;
    private readonly SKPaint _fillPaint;

    public GridRenderer(CoordinateSystem coords)
    {
        _coords = coords;
        _fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = VisualStyle.FillColor,
            IsAntialias = false,
        };
    }

    public void Render(SKCanvas canvas, DirectedSegment hSeg, DirectedSegment vSeg,
                       SegmentColorSet hColors, SegmentColorSet vColors)
    {
        float ext = 0;// VisualStyle.GridExtension;

        // Imaginary region (dashed section): [0, imaginary]
        float hImagMin = MathF.Min(0, hSeg.Imaginary);
        float hImagMax = MathF.Max(0, hSeg.Imaginary);
        float vImagMin = MathF.Min(0, vSeg.Imaginary);
        float vImagMax = MathF.Max(0, vSeg.Imaginary);

        // Stripe region: from real toward 0, clipped to the solid section [imag..real].
        // Formula: [real, clamp(0, solidMin, solidMax)]
        float hRealMin, hRealMax, vRealMin, vRealMax;
        {
            float hSolidMin = MathF.Min(hSeg.Imaginary, hSeg.Real);
            float hSolidMax = MathF.Max(hSeg.Imaginary, hSeg.Real);
            float hZero = Math.Clamp(0f, hSolidMin, hSolidMax);
            hRealMin = MathF.Min(hSeg.Real, hZero);
            hRealMax = MathF.Max(hSeg.Real, hZero);

            float vSolidMin = MathF.Min(vSeg.Imaginary, vSeg.Real);
            float vSolidMax = MathF.Max(vSeg.Imaginary, vSeg.Real);
            float vZero = Math.Clamp(0f, vSolidMin, vSolidMax);
            vRealMin = MathF.Min(vSeg.Real, vZero);
            vRealMax = MathF.Max(vSeg.Real, vZero);
        }

        // --- Yellow fill where both imaginary regions overlap (drawn first as background) ---
        float yFillXMin = hImagMin;
        float yFillXMax = hImagMax;
        float yFillYMin = vImagMin;
        float yFillYMax = vImagMax;

        if (yFillXMax > yFillXMin && yFillYMax > yFillYMin)
        {
            var topLeft = _coords.MathToPixel(yFillXMin, yFillYMax);
            var botRight = _coords.MathToPixel(yFillXMax, yFillYMin);
            canvas.DrawRect(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y, _fillPaint);
        }

        // --- Vertical grid lines from H-segment's real region ---
        // At integer X in [hRealMin, hRealMax], extending through full V range + ext.
        float vLineYMin = MathF.Min(MathF.Min(0, vSeg.Imaginary), vSeg.Real) - ext;
        float vLineYMax = MathF.Max(MathF.Max(0, vSeg.Imaginary), vSeg.Real) + ext;

        using (var hPaint = MakeGridPaint(hColors.Solid))
        using (var hHalfPaint = MakeHalfTickPaint(hColors.Solid))
        {
            // Integer grid lines
            int startX = (int)MathF.Ceiling(hRealMin);
            int endX = (int)MathF.Floor(hRealMax);
            for (int mx = startX; mx <= endX; mx++)
            {
                if (mx == 0) continue;
                var top = _coords.MathToPixel(mx, vLineYMax);
                var bot = _coords.MathToPixel(mx, vLineYMin);
                canvas.DrawLine(top, bot, hPaint);
            }
            // Half-tick grid lines (0.5 increments)
            int halfStartX = (int)MathF.Ceiling(hRealMin * 2);
            int halfEndX = (int)MathF.Floor(hRealMax * 2);
            for (int hx = halfStartX; hx <= halfEndX; hx++)
            {
                if (hx % 2 == 0) continue; // skip integers, already drawn
                float mx = hx * 0.5f;
                var top = _coords.MathToPixel(mx, vLineYMax);
                var bot = _coords.MathToPixel(mx, vLineYMin);
                canvas.DrawLine(top, bot, hHalfPaint);
            }
        }

        // --- Horizontal grid lines from V-segment's real region ---
        // At integer Y in [vRealMin, vRealMax], extending through full H range + ext.
        float hLineXMin = MathF.Min(MathF.Min(0, hSeg.Imaginary), hSeg.Real) - ext;
        float hLineXMax = MathF.Max(MathF.Max(0, hSeg.Imaginary), hSeg.Real) + ext;

        using (var vPaint = MakeGridPaint(vColors.Solid))
        using (var vHalfPaint = MakeHalfTickPaint(vColors.Solid))
        {
            // Integer grid lines
            int startY = (int)MathF.Ceiling(vRealMin);
            int endY = (int)MathF.Floor(vRealMax);
            for (int my = startY; my <= endY; my++)
            {
                if (my == 0) continue;
                var left = _coords.MathToPixel(hLineXMin, my);
                var right = _coords.MathToPixel(hLineXMax, my);
                canvas.DrawLine(left, right, vPaint);
            }
            // Half-tick grid lines (0.5 increments)
            int halfStartY = (int)MathF.Ceiling(vRealMin * 2);
            int halfEndY = (int)MathF.Floor(vRealMax * 2);
            for (int hy = halfStartY; hy <= halfEndY; hy++)
            {
                if (hy % 2 == 0) continue; // skip integers, already drawn
                float my = hy * 0.5f;
                var left = _coords.MathToPixel(hLineXMin, my);
                var right = _coords.MathToPixel(hLineXMax, my);
                canvas.DrawLine(left, right, vHalfPaint);
            }
        }

    }

    private static SKPaint MakeGridPaint(SKColor color) => new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = VisualStyle.GridStrokeWidth,
        Color = color,
        IsAntialias = true,
    };

    private static SKPaint MakeHalfTickPaint(SKColor color) => new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = VisualStyle.HalfTickStrokeWidth,
        Color = color.WithAlpha(80),
        IsAntialias = true,
    };

    public void Dispose()
    {
        _fillPaint.Dispose();
        GC.SuppressFinalize(this);
    }
}
