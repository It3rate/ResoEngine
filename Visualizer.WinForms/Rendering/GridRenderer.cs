using SkiaSharp;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Rendering;

/// <summary>
/// Renders the orthogonal grid for two directed segments:
/// - Horizontal lines in V-segment's real region (blue tint)
/// - Vertical lines in H-segment's real region (red tint)
/// - Yellow fill in the double-negative (imaginary) quadrant
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
        float xMin = hSeg.Imaginary;
        float xMax = hSeg.Real;
        float yMin = vSeg.Imaginary;
        float yMax = vSeg.Real;

        // --- Yellow fill: quadrant where both are imaginary (x < 0 AND y < 0) ---
        if (xMin < 0 && yMin < 0)
        {
            var origin = _coords.MathToPixel(0, 0);
            var imagCorner = _coords.MathToPixel(xMin, yMin);

            float fillX = imagCorner.X;
            float fillY = origin.Y;
            float fillW = origin.X - imagCorner.X;
            float fillH = imagCorner.Y - origin.Y;

            if (fillW > 0 && fillH > 0)
                canvas.DrawRect(fillX, fillY, fillW, fillH, _fillPaint);
        }

        // --- Horizontal grid lines (V-segment's color, in V's real region) ---
        using (var hPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.GridStrokeWidth,
            Color = vColors.Grid,
            IsAntialias = true,
        })
        {
            int startY = Math.Max((int)MathF.Ceiling(0), (int)MathF.Ceiling(yMin));
            int endY = (int)MathF.Floor(yMax);
            for (int my = startY; my <= endY; my++)
            {
                if (my == 0) continue;
                var left = _coords.MathToPixel(xMin, my);
                var right = _coords.MathToPixel(xMax, my);
                canvas.DrawLine(left, right, hPaint);
            }
        }

        // --- Vertical grid lines (H-segment's color, in H's real region) ---
        using (var vPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.GridStrokeWidth,
            Color = hColors.Grid,
            IsAntialias = true,
        })
        {
            int startX = Math.Max((int)MathF.Ceiling(0), (int)MathF.Ceiling(xMin));
            int endX = (int)MathF.Floor(xMax);
            for (int mx = startX; mx <= endX; mx++)
            {
                if (mx == 0) continue;
                var top = _coords.MathToPixel(mx, yMax);
                var bot = _coords.MathToPixel(mx, yMin);
                canvas.DrawLine(top, bot, vPaint);
            }
        }
    }

    public void Dispose()
    {
        _fillPaint.Dispose();
        GC.SuppressFinalize(this);
    }
}
