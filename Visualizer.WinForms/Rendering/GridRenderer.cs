using ResoEngine.Visualizer.Core;
using SkiaSharp;

namespace ResoEngine.Visualizer.Rendering;

/// <summary>
/// Renders the orthogonal grid for two directed segments.
///
/// Each segment contributes two independent regions relative to zero:
///   Imaginary region: [imaginary, 0]
///   Real region:      [real, 0]
///
/// Grid rules:
///   - Yellow fill always covers the overlap of the two imaginary regions.
///   - Red/blue extrusion lines always cover the overlap of each axis' real region
///     with the full extent of the opposite axis, even when the real value is negative.
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

    public void Render(SKCanvas canvas, ISegmentValue hSeg, ISegmentValue vSeg,
                       SegmentColorSet hColors, SegmentColorSet vColors)
    {
        float hImagMin = MathF.Min(0f, hSeg.Imaginary);
        float hImagMax = MathF.Max(0f, hSeg.Imaginary);
        float hRealMin = MathF.Min(0f, hSeg.Real);
        float hRealMax = MathF.Max(0f, hSeg.Real);

        float vImagMin = MathF.Min(0f, vSeg.Imaginary);
        float vImagMax = MathF.Max(0f, vSeg.Imaginary);
        float vRealMin = MathF.Min(0f, vSeg.Real);
        float vRealMax = MathF.Max(0f, vSeg.Real);

        float xMin = MathF.Min(hImagMin, hRealMin);
        float xMax = MathF.Max(hImagMax, hRealMax);
        float yMin = MathF.Min(vImagMin, vRealMin);
        float yMax = MathF.Max(vImagMax, vRealMax);

        if (hImagMax > hImagMin && vImagMax > vImagMin)
        {
            var topLeft = _coords.MathToPixel(hImagMin, vImagMax);
            var botRight = _coords.MathToPixel(hImagMax, vImagMin);
            canvas.DrawRect(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y, _fillPaint);
        }

        using (var hPaint = MakeGridPaint(hColors.Solid))
        using (var hHalfPaint = MakeHalfTickPaint(hColors.Solid))
        {
            int startX = (int)MathF.Ceiling(hRealMin);
            int endX = (int)MathF.Floor(hRealMax);
            for (int mx = startX; mx <= endX; mx++)
            {
                if (mx == 0) continue;
                var top = _coords.MathToPixel(mx, yMax);
                var bot = _coords.MathToPixel(mx, yMin);
                canvas.DrawLine(top, bot, hPaint);
            }

            int halfStartX = (int)MathF.Ceiling(hRealMin * 2f);
            int halfEndX = (int)MathF.Floor(hRealMax * 2f);
            for (int hx = halfStartX; hx <= halfEndX; hx++)
            {
                if (hx % 2 == 0 || hx == 0) continue;
                float mx = hx * 0.5f;
                var top = _coords.MathToPixel(mx, yMax);
                var bot = _coords.MathToPixel(mx, yMin);
                canvas.DrawLine(top, bot, hHalfPaint);
            }
        }

        using (var vPaint = MakeGridPaint(vColors.Solid))
        using (var vHalfPaint = MakeHalfTickPaint(vColors.Solid))
        {
            int startY = (int)MathF.Ceiling(vRealMin);
            int endY = (int)MathF.Floor(vRealMax);
            for (int my = startY; my <= endY; my++)
            {
                if (my == 0) continue;
                var left = _coords.MathToPixel(xMin, my);
                var right = _coords.MathToPixel(xMax, my);
                canvas.DrawLine(left, right, vPaint);
            }

            int halfStartY = (int)MathF.Ceiling(vRealMin * 2f);
            int halfEndY = (int)MathF.Floor(vRealMax * 2f);
            for (int hy = halfStartY; hy <= halfEndY; hy++)
            {
                if (hy % 2 == 0 || hy == 0) continue;
                float my = hy * 0.5f;
                var left = _coords.MathToPixel(xMin, my);
                var right = _coords.MathToPixel(xMax, my);
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
