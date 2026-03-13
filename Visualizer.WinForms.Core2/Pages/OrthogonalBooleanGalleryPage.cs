using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class OrthogonalBooleanGalleryPage : IVisualizerPage
{
    private readonly int _startIndex;
    private readonly AxisDisplayMapper _axisA = new(new Axis(new Proportion(3, 1), new Proportion(5, 1)), "A");
    private readonly AxisDisplayMapper _axisB = new(new Axis(new Proportion(2, 1), new Proportion(5, 1)), "B");

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private GridRenderer? _gridRenderer;

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

    public OrthogonalBooleanGalleryPage(int startIndex)
    {
        _startIndex = startIndex;
    }

    public string Title => _startIndex == 0 ? "Orthogonal Boolean I" : "Orthogonal Boolean II";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        coords.OriginX = 240;
        coords.OriginY = 210;

        _gridRenderer = new GridRenderer(coords);
        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, 0);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Vertical, SegmentColors.Blue, 0);

        hitTest.Register(_rendererA, _axisA);
        hitTest.Register(_rendererB, _axisB);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawText("Drag A and B above. The tiles below show boolean truth regions over the four orthogonal minterms.", 28, 32, _titlePaint);

        _gridRenderer?.Render(canvas, _axisA, _axisB, SegmentColors.Red, SegmentColors.Blue);
        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

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

        const float top = 350f;
        const float left = 28f;
        const float gapX = 18f;
        const float gapY = 22f;
        const int columns = 4;

        float tileWidth = (_coords.Width - left * 2f - gapX * (columns - 1)) / columns;
        float tileHeight = 215f;

        var hImag = CreateZeroRange(_axisA.Imaginary);
        var hReal = CreateZeroRange(_axisA.Real);
        var vImag = CreateZeroRange(_axisB.Imaginary);
        var vReal = CreateZeroRange(_axisB.Real);

        float worldMinX = MathF.Min(hImag.Min, hReal.Min);
        float worldMaxX = MathF.Max(hImag.Max, hReal.Max);
        float worldMinY = MathF.Min(vImag.Min, vReal.Min);
        float worldMaxY = MathF.Max(vImag.Max, vReal.Max);

        if (worldMaxX <= worldMinX) worldMaxX = worldMinX + 1f;
        if (worldMaxY <= worldMinY) worldMaxY = worldMinY + 1f;

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

            DrawTile(canvas, tile, defs[index], worldMinX, worldMaxX, worldMinY, worldMaxY, hImag, hReal, vImag, vReal);
        }
    }

    private void DrawTile(
        SKCanvas canvas,
        SKRect tile,
        BooleanOperationDefinition definition,
        float worldMinX,
        float worldMaxX,
        float worldMinY,
        float worldMaxY,
        AxisRange hImag,
        AxisRange hReal,
        AxisRange vImag,
        AxisRange vReal)
    {
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBackgroundPaint);
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBorderPaint);

        const float pad = 16f;
        const float labelSpace = 34f;
        var plot = new SKRect(tile.Left + pad, tile.Top + pad, tile.Right - pad, tile.Bottom - labelSpace);

        DrawAxes(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY);

        DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, hImag, vImag, definition.Evaluate(false, false));
        DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, hImag, vReal, definition.Evaluate(false, true));
        DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, hReal, vImag, definition.Evaluate(true, false));
        DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, hReal, vReal, definition.Evaluate(true, true));

        canvas.DrawText(definition.Name, tile.MidX, tile.Bottom - 10f, _labelPaint);
    }

    private void DrawRegion(
        SKCanvas canvas,
        SKRect plot,
        float worldMinX,
        float worldMaxX,
        float worldMinY,
        float worldMaxY,
        AxisRange xRange,
        AxisRange yRange,
        bool active)
    {
        if (!xRange.HasSpan || !yRange.HasSpan)
        {
            return;
        }

        float left = MapX(xRange.Min, plot, worldMinX, worldMaxX);
        float right = MapX(xRange.Max, plot, worldMinX, worldMaxX);
        float top = MapY(yRange.Max, plot, worldMinY, worldMaxY);
        float bottom = MapY(yRange.Min, plot, worldMinY, worldMaxY);
        var rect = new SKRect(left, top, right, bottom);

        canvas.DrawRect(rect, active ? _truePaint : _falsePaint);
        canvas.DrawRect(rect, _tileBorderPaint);

        if (!active)
        {
            canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, _crossPaint);
            canvas.DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top, _crossPaint);
        }
    }

    private void DrawAxes(SKCanvas canvas, SKRect plot, float worldMinX, float worldMaxX, float worldMinY, float worldMaxY)
    {
        if (worldMinX <= 0f && worldMaxX >= 0f)
        {
            float x = MapX(0f, plot, worldMinX, worldMaxX);
            canvas.DrawLine(x, plot.Top, x, plot.Bottom, _axisGuidePaint);
        }

        if (worldMinY <= 0f && worldMaxY >= 0f)
        {
            float y = MapY(0f, plot, worldMinY, worldMaxY);
            canvas.DrawLine(plot.Left, y, plot.Right, y, _axisGuidePaint);
        }
    }

    private static float MapX(float value, SKRect plot, float worldMinX, float worldMaxX) =>
        plot.Left + ((value - worldMinX) / (worldMaxX - worldMinX)) * plot.Width;

    private static float MapY(float value, SKRect plot, float worldMinY, float worldMaxY) =>
        plot.Bottom - ((value - worldMinY) / (worldMaxY - worldMinY)) * plot.Height;

    private static AxisRange CreateZeroRange(float value) => new(MathF.Min(0f, value), MathF.Max(0f, value));

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
        _gridRenderer?.Dispose();
        _gridRenderer = null;
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

    private readonly record struct AxisRange(float Min, float Max)
    {
        public bool HasSpan => Max > Min;
    }
}
