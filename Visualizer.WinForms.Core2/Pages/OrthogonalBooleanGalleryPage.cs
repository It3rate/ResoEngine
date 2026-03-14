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

    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _tileBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(210, 210, 210),
        IsAntialias = true,
    };
    private readonly SKPaint _tileBackgroundPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(244, 244, 244),
        IsAntialias = true,
    };
    private readonly SKPaint _yellowPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = VisualStyle.FillColor,
        IsAntialias = true,
    };
    private readonly SKPaint _activeWashPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(176, 219, 170, 85),
        IsAntialias = true,
    };
    private readonly SKPaint _falsePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(244, 244, 244),
        IsAntialias = true,
    };
    private readonly SKPaint _crossPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(188, 188, 188, 190),
        IsAntialias = true,
    };
    private readonly SKPaint _inactiveRegionBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(206, 206, 206),
        IsAntialias = true,
    };
    private readonly SKPaint _activeRegionBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2.2f,
        Color = new SKColor(123, 168, 108),
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

    public string Title => "Orthogonal Boolean";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        coords.OriginX = 240;
        coords.OriginY = 284;

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

        canvas.DrawText("Orthogonal Boolean Gallery", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Drag the two orthogonal segments above. Each tile below shows one boolean rule over the four quadrant regions created by pinning the axes together.",
            34f,
            ref subtitleY,
            560f,
            _bodyPaint);

        var geometry = new AreaDisplayGeometry(_axisA.Axis.Pin(_axisB.Axis));
        _gridRenderer?.Render(canvas, _axisA, _axisB, SegmentColors.Red, SegmentColors.Blue);
        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);

        DrawTiles(canvas, geometry);
    }

    private void DrawTiles(SKCanvas canvas, AreaDisplayGeometry geometry)
    {
        if (_coords == null)
        {
            return;
        }

        const float top = 322f;
        const float rightPad = 24f;
        const float gapX = 10f;
        const float gapY = 10f;
        const int columns = 4;

        float availableWidth = MathF.Max(440f, _coords.Width - 580f);
        float tileSize = MathF.Min(122f, (availableWidth - gapX * (columns - 1)) / columns);
        float tileWidth = tileSize;
        float tileHeight = tileSize;
        float totalWidth = tileWidth * columns + gapX * (columns - 1);
        float left = _coords.Width - rightPad - totalWidth;

        float worldMinX = geometry.WorldMinX;
        float worldMaxX = geometry.WorldMaxX;
        float worldMinY = geometry.WorldMinY;
        float worldMaxY = geometry.WorldMaxY;

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

            DrawTile(canvas, tile, defs[index], worldMinX, worldMaxX, worldMinY, worldMaxY, geometry);
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
        AreaDisplayGeometry geometry)
    {
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBackgroundPaint);
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBorderPaint);

        const float pad = 8f;
        const float labelSpace = 22f;
        var plot = new SKRect(tile.Left + pad, tile.Top + pad, tile.Right - pad, tile.Bottom - labelSpace);

        DrawAxes(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY);
        DrawBooleanRegions(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, definition, geometry);

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
        AreaDisplayGeometry geometry)
    {
        var xBoundaries = geometry.OrthogonalXBoundaries;
        var yBoundaries = geometry.OrthogonalYBoundaries;

        for (int xi = 0; xi < xBoundaries.Count - 1; xi++)
        {
            var xRange = new AreaDisplayGeometry.AxisRange(xBoundaries[xi], xBoundaries[xi + 1]);
            if (!xRange.HasSpan)
            {
                continue;
            }

            for (int yi = 0; yi < yBoundaries.Count - 1; yi++)
            {
                var yRange = new AreaDisplayGeometry.AxisRange(yBoundaries[yi], yBoundaries[yi + 1]);
                if (!yRange.HasSpan)
                {
                    continue;
                }

                float midX = (xRange.Min + xRange.Max) * 0.5f;
                float midY = (yRange.Min + yRange.Max) * 0.5f;
                bool inAImag = geometry.InHorizontalImag(midX);
                bool inAReal = geometry.InHorizontalReal(midX);
                bool inBImag = geometry.InVerticalImag(midY);
                bool inBReal = geometry.InVerticalReal(midY);

                bool showYellow = inAImag && inBImag && definition.Evaluate(false, false);
                bool showRed = inAReal && (inBImag || inBReal) &&
                    ((inBImag && definition.Evaluate(true, false)) || (inBReal && definition.Evaluate(true, true)));
                bool showBlue = inBReal && (inAImag || inAReal) &&
                    ((inAImag && definition.Evaluate(false, true)) || (inAReal && definition.Evaluate(true, true)));

                bool active = showYellow || showRed || showBlue;
                DrawRegion(canvas, plot, worldMinX, worldMaxX, worldMinY, worldMaxY, xRange, yRange, active, showYellow, showRed, showBlue);
            }
        }
    }

    private void DrawRegion(
        SKCanvas canvas,
        SKRect plot,
        float worldMinX,
        float worldMaxX,
        float worldMinY,
        float worldMaxY,
        AreaDisplayGeometry.AxisRange xRange,
        AreaDisplayGeometry.AxisRange yRange,
        bool active,
        bool showYellow,
        bool showRed,
        bool showBlue)
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

        canvas.DrawRect(rect, _falsePaint);
        if (active)
        {
            canvas.DrawRect(rect, _activeWashPaint);
        }

        if (showYellow)
        {
            canvas.DrawRect(rect, _yellowPaint);
        }

        DrawStripes(canvas, rect, showRed, showBlue);
        canvas.DrawRect(rect, active ? _activeRegionBorderPaint : _inactiveRegionBorderPaint);

        if (!active)
        {
            canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, _crossPaint);
            canvas.DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top, _crossPaint);
        }
    }

    private void DrawStripes(SKCanvas canvas, SKRect rect, bool showRed, bool showBlue)
    {
        if (!showRed && !showBlue)
        {
            return;
        }

        canvas.Save();
        canvas.ClipRect(rect);

        const float stripe = 8f;
        if (showRed)
        {
            for (float x = rect.Left - 1f; x <= rect.Right + 1f; x += stripe)
            {
                canvas.DrawLine(x, rect.Top, x, rect.Bottom, _redStripePaint);
            }
        }

        if (showBlue)
        {
            for (float y = rect.Top - 1f; y <= rect.Bottom + 1f; y += stripe)
            {
                canvas.DrawLine(rect.Left, y, rect.Right, y, _blueStripePaint);
            }
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
        _labelPaint.Dispose();
        _tileBorderPaint.Dispose();
        _tileBackgroundPaint.Dispose();
        _yellowPaint.Dispose();
        _activeWashPaint.Dispose();
        _falsePaint.Dispose();
        _crossPaint.Dispose();
        _inactiveRegionBorderPaint.Dispose();
        _activeRegionBorderPaint.Dispose();
        _redStripePaint.Dispose();
        _blueStripePaint.Dispose();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _axisGuidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

}
