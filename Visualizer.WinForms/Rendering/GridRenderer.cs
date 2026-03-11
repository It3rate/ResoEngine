using OpenTK.Graphics;
using ResoEngine.Visualizer.Core;
using SkiaSharp;
using static System.Windows.Forms.AxHost;

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

    float _xMin;
    float _xMax;
    float _yMin;
    float _yMax;
    public void Render(SKCanvas canvas, DirectedSegment hSeg, DirectedSegment vSeg,
                       SegmentColorSet hColors, SegmentColorSet vColors)
    {
        _xMin = hSeg.Imaginary;
        _xMax = hSeg.Real;
        _yMin = vSeg.Imaginary;
        _yMax = vSeg.Real;

        var origin = _coords.MathToPixel(0, 0);
        var imagCorner = _coords.MathToPixel(_xMin, _yMin);

        float fillX = imagCorner.X;
        float fillY = origin.Y;
        float fillW = origin.X - imagCorner.X;
        float fillH = imagCorner.Y - origin.Y;

        canvas.DrawRect(fillX, fillY, fillW, fillH, _fillPaint);

        // --- Horizontal grid lines (V-segment's color, in V's real region) ---
        using (var bluePaint = GetPen(vColors.Grid))
        {
            var minVal = MathF.Min(_yMin, _yMax);
            var maxVal = MathF.Max(_yMin, _yMax);
            int start = (int)Math.Ceiling(minVal);
            int end = (int)Math.Floor(maxVal);
            for (int my = start; my <= end; my++)
            {
                DrawVLine(my, hSeg, bluePaint, canvas);
            }
        }

        // --- Vertical grid lines (H-segment's color, in H's real region) ---
        using (var redPaint = GetPen(hColors.Grid))
        {
            var minVal = MathF.Min(_xMin, _xMax);
            var maxVal = MathF.Max(_xMin, _xMax);
            int start = (int)Math.Ceiling(minVal);
            int end = (int)Math.Floor(maxVal);
            for (int mx = start; mx <= end; mx++)
            {
                DrawHLine(mx, vSeg, redPaint, canvas);
            }
        }
    }


    private void DrawVLine(float my, DirectedSegment seg, SKPaint bluePaint, SKCanvas canvas)
    {
        if (my == 0) return;
        var hMin = my > 0 ? MathF.Min(_xMin, 0) : MathF.Max(_xMin, 0);
        var start = _coords.MathToPixel(hMin, my);
        var end = _coords.MathToPixel(_xMax, my);
        canvas.DrawLine(start, end, bluePaint);
    }
    private void DrawHLine(float mx, DirectedSegment seg, SKPaint redPaint, SKCanvas canvas)
    {
        if (mx == 0) return;
        var vMin = mx > 0 ? MathF.Min(_yMin, 0) : MathF.Max(_yMin, 0);
        var start = _coords.MathToPixel(mx, vMin);
        var end = _coords.MathToPixel(mx, _yMax);
        canvas.DrawLine(start, end, redPaint);
    }
    private SKPaint GetPen(SKColor color)
    {
        return new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = VisualStyle.GridStrokeWidth,
            Color = color,
            IsAntialias = true,
        };
    }
    public void Dispose()
    {
        _fillPaint.Dispose();
        GC.SuppressFinalize(this);
    }
}
