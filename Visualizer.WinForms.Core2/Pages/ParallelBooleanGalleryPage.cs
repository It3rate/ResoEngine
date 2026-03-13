using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class ParallelBooleanGalleryPage : IVisualizerPage
{
    private readonly int _startIndex;
    private readonly AxisDisplayMapper _axisA = new(new Axis(new Proportion(3, 1), new Proportion(5, 1)), "A");
    private readonly AxisDisplayMapper _axisB = new(new Axis(new Proportion(1, 1), new Proportion(7, 1)), "B");

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;

    private readonly SKPaint _titlePaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 18f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _tileBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(180, 180, 180),
        IsAntialias = true,
    };
    private readonly SKPaint _tileBackgroundPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(248, 248, 248),
        IsAntialias = true,
    };
    private readonly SKPaint _truePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(177, 225, 157),
        IsAntialias = true,
    };
    private readonly SKPaint _falsePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(231, 231, 231),
        IsAntialias = true,
    };
    private readonly SKPaint _crossPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(120, 120, 120, 200),
        IsAntialias = true,
    };
    private readonly SKPaint _axisGuidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(170, 170, 170),
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

    private const float InputAY = 2.0f;
    private const float InputBY = 0.5f;

    public ParallelBooleanGalleryPage(int startIndex)
    {
        _startIndex = startIndex;
    }

    public string Title => _startIndex == 0 ? "Parallel Boolean I" : "Parallel Boolean II";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        coords.OriginX = coords.Width / 2;
        coords.OriginY = 170;

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

        canvas.DrawText("Drag A and B above. The tiles below show boolean truth along the framed line, including gaps and overlaps.", 28, 32, _titlePaint);

        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

        var sepLeft = _coords.MathToPixel(-15, -0.5f);
        var sepRight = _coords.MathToPixel(15, -0.5f);
        canvas.DrawLine(sepLeft, sepRight, _axisGuidePaint);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);

        DrawTiles(canvas);
    }

    private void DrawTiles(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        const float top = 300f;
        const float left = 28f;
        const float gapX = 18f;
        const float gapY = 22f;
        const int columns = 4;

        float tileWidth = (_coords.Width - left * 2f - gapX * (columns - 1)) / columns;
        float tileHeight = 190f;

        var defs = BooleanOperationCatalog.All.Skip(_startIndex).Take(8).ToArray();
        for (int index = 0; index < defs.Length; index++)
        {
            int col = index % columns;
            int row = index / columns;
            var tile = new SKRect(
                left + col * (tileWidth + gapX),
                top + row * (tileHeight + gapY),
                left + col * (tileWidth + gapX) + tileWidth,
                top + row * (tileHeight + gapY) + tileHeight);

            DrawTile(canvas, tile, defs[index]);
        }
    }

    private void DrawTile(SKCanvas canvas, SKRect tile, BooleanOperationDefinition definition)
    {
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBackgroundPaint);
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBorderPaint);

        const float pad = 16f;
        const float labelSpace = 34f;
        var plot = new SKRect(tile.Left + pad, tile.Top + pad, tile.Right - pad, tile.Bottom - labelSpace);

        float aMin = MathF.Min(_axisA.Imaginary, _axisA.Real);
        float aMax = MathF.Max(_axisA.Imaginary, _axisA.Real);
        float bMin = MathF.Min(_axisB.Imaginary, _axisB.Real);
        float bMax = MathF.Max(_axisB.Imaginary, _axisB.Real);

        float min = MathF.Min(0f, MathF.Min(aMin, bMin));
        float max = MathF.Max(0f, MathF.Max(aMax, bMax));
        float margin = MathF.Max(1f, (max - min) * 0.18f);
        float frameMin = min - margin;
        float frameMax = max + margin;

        DrawAxisGuide(canvas, plot, frameMin, frameMax);

        var boundaries = new List<float> { frameMin, aMin, aMax, bMin, bMax, frameMax };
        boundaries.Sort();
        const float epsilon = 0.0001f;

        for (int i = 0; i < boundaries.Count - 1; i++)
        {
            float start = boundaries[i];
            float end = boundaries[i + 1];
            if (end - start <= epsilon)
            {
                continue;
            }

            float mid = (start + end) * 0.5f;
            bool a = mid >= aMin && mid <= aMax;
            bool b = mid >= bMin && mid <= bMax;
            bool active = definition.Evaluate(a, b);

            var rect = new SKRect(
                MapX(start, plot, frameMin, frameMax),
                plot.Top,
                MapX(end, plot, frameMin, frameMax),
                plot.Bottom);

            canvas.DrawRect(rect, active ? _truePaint : _falsePaint);
            canvas.DrawRect(rect, _tileBorderPaint);

            if (!active)
            {
                canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, _crossPaint);
                canvas.DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top, _crossPaint);
            }
        }

        canvas.DrawText(definition.Name, tile.MidX, tile.Bottom - 10f, _labelPaint);
    }

    private void DrawAxisGuide(SKCanvas canvas, SKRect plot, float frameMin, float frameMax)
    {
        float zero = MapX(0f, plot, frameMin, frameMax);
        canvas.DrawLine(zero, plot.Top, zero, plot.Bottom, _axisGuidePaint);
    }

    private static float MapX(float value, SKRect plot, float frameMin, float frameMax) =>
        plot.Left + ((value - frameMin) / (frameMax - frameMin)) * plot.Width;

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
        _rendererA?.Dispose();
        _rendererA = null;
        _rendererB?.Dispose();
        _rendererB = null;
        _coords = null;
    }

    public void Dispose()
    {
        Destroy();
        _titlePaint.Dispose();
        _labelPaint.Dispose();
        _tileBorderPaint.Dispose();
        _tileBackgroundPaint.Dispose();
        _truePaint.Dispose();
        _falsePaint.Dispose();
        _crossPaint.Dispose();
        _axisGuidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
