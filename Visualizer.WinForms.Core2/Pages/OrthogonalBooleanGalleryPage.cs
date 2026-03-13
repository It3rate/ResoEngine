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

    private readonly SKPaint _redStripePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(214, 95, 95, 180),
        IsAntialias = true,
    };
    private readonly SKPaint _blueStripePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(95, 125, 214, 180),
        IsAntialias = true,
    };

    public string Title => "Orthogonal Boolean";

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

        canvas.DrawText("Drag A and B above. The tiles below show boolean truth over the four segment components.", 28, 32, _titlePaint);

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

        const float top = 300f;
        const float left = 28f;
        const float gapX = 14f;
        const float gapY = 14f;
        const int columns = 4;

        float tileWidth = (_coords.Width - left * 2f - gapX * (columns - 1)) / columns;
        float tileHeight = 136f;

        var aImag = CreateRange(0f, _axisA.Imaginary);
        var aReal = CreateRange(0f, _axisA.Real);
        var bImag = CreateRange(0f, _axisB.Imaginary);
        var bReal = CreateRange(0f, _axisB.Real);

        float worldMinX = MathF.Min(MathF.Min(aImag.Min, aReal.Min), 0f);
        float worldMaxX = MathF.Max(MathF.Max(aImag.Max, aReal.Max), 0f);
        float worldMinY = MathF.Min(MathF.Min(bImag.Min, bReal.Min), 0f);
        float worldMaxY = MathF.Max(MathF.Max(bImag.Max, bReal.Max), 0f);

        if (worldMaxX <= worldMinX) worldMaxX = worldMinX + 1f;
        if (worldMaxY <= worldMinY) worldMaxY = worldMinY + 1f;

        var defs = BooleanOperationCatalog.All.ToArray();
        for (int index = 0; index < defs.Length; index++)
        {
            int col = index % columns;
            int row = index / columns;
            var tile = new SKRect(
                left + col * (tileWidth + gapX),
                top + row * (tileHeight + gapY),
                left + col * (tileWidth + gapX) + tileWidth,
                top + row * (tileHeight + gapY) + tileHeight);

            DrawTile(canvas, tile, defs[index], worldMinX, worldMaxX, worldMinY, worldMaxY, aImag, aReal, bImag, bReal);
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
        AxisRange aImag,
        AxisRange aReal,
        AxisRange bImag,
        AxisRange bReal)
    {
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBackgroundPaint);
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBorderPaint);

        const float pad = 16f;
        const float labelSpace = 30f;
        var plot = new SKRect(tile.Left + pad, tile.Top + pad, tile.Right - pad, tile.Bottom - labelSpace);

        DrawAxes(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY);
        DrawBooleanRegions(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, definition, aImag, aReal, bImag, bReal);

        canvas.DrawText(definition.Name, tile.MidX, tile.Bottom - 10f, _labelPaint);
    }

    private void DrawBooleanRegions(
        SKCanvas canvas,
        SKRect plot,
        float worldMinX,
        float worldMaxX,
        float worldMinY,
        float worldMaxY,
        BooleanOperationDefinition definition,
        AxisRange aImag,
        AxisRange aReal,
        AxisRange bImag,
        AxisRange bReal)
    {
        var regions = new[]
        {
            new Region(aImag, bImag, definition.Evaluate(false, false)),
            new Region(aImag, bReal, definition.Evaluate(false, true)),
            new Region(aReal, bImag, definition.Evaluate(true, false)),
            new Region(aReal, bReal, definition.Evaluate(true, true)),
        };

        foreach (var region in regions.Where(region => !region.Active))
        {
            DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, region.XRange, region.YRange, false);
        }

        foreach (var region in regions.Where(region => region.Active))
        {
            DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, region.XRange, region.YRange, true);
        }
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
        DrawStripes(canvas, rect);

        if (!active)
        {
            canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, _crossPaint);
            canvas.DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top, _crossPaint);
        }
    }

    private void DrawStripes(SKCanvas canvas, SKRect rect)
    {
        canvas.Save();
        canvas.ClipRect(rect);

        const float stripe = 8f;
        for (float x = rect.Left - 1f; x <= rect.Right + 1f; x += stripe)
        {
            canvas.DrawLine(x, rect.Top, x, rect.Bottom, _redStripePaint);
        }

        for (float y = rect.Top - 1f; y <= rect.Bottom + 1f; y += stripe)
        {
            canvas.DrawLine(rect.Left, y, rect.Right, y, _blueStripePaint);
        }

        canvas.Restore();
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

    private static AxisRange CreateRange(float start, float end) => new(MathF.Min(start, end), MathF.Max(start, end));

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
        _redStripePaint.Dispose();
        _blueStripePaint.Dispose();
        _axisGuidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

    private readonly record struct AxisRange(float Min, float Max)
    {
        public bool HasSpan => Max > Min;
    }

    private readonly record struct Region(AxisRange XRange, AxisRange YRange, bool Active);
}
