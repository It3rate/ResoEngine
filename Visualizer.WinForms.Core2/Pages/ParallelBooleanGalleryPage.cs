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
    private readonly AxisDisplayMapper _axisA = new(new Axis(new Proportion(3, 1), new Proportion(5, 1)), "A");
    private readonly AxisDisplayMapper _axisB = new(new Axis(new Proportion(1, 1), new Proportion(7, 1)), "B");

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;

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
        Color = new SKColor(180, 180, 180),
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
    private readonly SKPaint _axisGuidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(170, 170, 170),
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
    private readonly SKPaint _zeroBarPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(165, 165, 165),
        IsAntialias = true,
    };
    private readonly SKPaint _topRegionBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(198, 198, 198),
        IsAntialias = true,
    };

    private const float InputAY = 2.0f;
    private const float InputBY = 0.5f;

    public string Title => "Parallel Boolean";

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

        var geometry = new AreaDisplayGeometry(_axisA.Axis.Pin(_axisB.Axis));
        DrawTopComponentField(canvas, geometry);

        var zeroTop = _coords.MathToPixel(0, InputAY);
        var zeroBottom = _coords.MathToPixel(0, InputBY);
        canvas.DrawLine(zeroTop, zeroBottom, _zeroBarPaint);

        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

        var originPx = GetZeroCenterPixel();
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

        const float top = 246f;
        const float left = 28f;
        const float gapX = 14f;
        const float gapY = 12f;
        const int columns = 4;

        float tileWidth = (_coords.Width - left * 2f - gapX * (columns - 1)) / columns;
        float tileHeight = 98f;

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

            DrawTile(canvas, tile, defs[index], geometry);
        }
    }

    private void DrawTile(SKCanvas canvas, SKRect tile, BooleanOperationDefinition definition, AreaDisplayGeometry geometry)
    {
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBackgroundPaint);
        canvas.DrawRoundRect(tile, 12f, 12f, _tileBorderPaint);

        const float pad = 14f;
        const float labelSpace = 26f;
        var plot = new SKRect(tile.Left + pad, tile.Top + pad, tile.Right - pad, tile.Bottom - labelSpace);

        float aMin = MathF.Min(geometry.HorizontalImaginary, geometry.HorizontalReal);
        float aMax = MathF.Max(geometry.HorizontalImaginary, geometry.HorizontalReal);
        float bMin = MathF.Min(geometry.VerticalImaginary, geometry.VerticalReal);
        float bMax = MathF.Max(geometry.VerticalImaginary, geometry.VerticalReal);

        float frameMin = geometry.FrameMin;
        float frameMax = geometry.FrameMax;
        if (frameMax <= frameMin)
        {
            frameMax = frameMin + 1f;
        }

        float bandGap = 4f;
        float bandHeight = (plot.Height - bandGap * 2f) / 3f;
        var aBand = new SKRect(plot.Left, plot.Top, plot.Right, plot.Top + bandHeight);
        var bBand = new SKRect(plot.Left, aBand.Bottom + bandGap, plot.Right, aBand.Bottom + bandGap + bandHeight);
        var resultBand = new SKRect(plot.Left, bBand.Bottom + bandGap, plot.Right, bBand.Bottom + bandGap + bandHeight);

        DrawAxisGuide(canvas, aBand, frameMin, frameMax);
        DrawAxisGuide(canvas, bBand, frameMin, frameMax);
        DrawAxisGuide(canvas, resultBand, frameMin, frameMax);

        const float epsilon = 0.0001f;
        var boundaries = geometry.ParallelBoundaries;

        for (int i = 0; i < boundaries.Count - 1; i++)
        {
            float start = boundaries[i];
            float end = boundaries[i + 1];
            if (end - start <= epsilon)
            {
                continue;
            }

            float mid = (start + end) * 0.5f;
            bool inAImag = geometry.InHorizontalImag(mid);
            bool inAReal = geometry.InHorizontalReal(mid);
            bool inBImag = geometry.InVerticalImag(mid);
            bool inBReal = geometry.InVerticalReal(mid);
            bool inA = geometry.InHorizontal(mid);
            bool inB = geometry.InVertical(mid);

            var rect = new SKRect(
                MapX(start, plot, frameMin, frameMax),
                0f,
                MapX(end, plot, frameMin, frameMax),
                0f);

            DrawStyledSegment(canvas, aBand, rect.Left, rect.Right, inAImag || inAReal, inAImag, inAReal, false, false);
            DrawStyledSegment(canvas, bBand, rect.Left, rect.Right, inBImag || inBReal, inBImag, false, inBReal, false);

            bool active = definition.Evaluate(inA, inB);
            bool showYellow = active && (inAImag || inBImag);
            bool showRed = active && inAReal && inB;
            bool showBlue = active && inBReal && inA;
            DrawStyledSegment(canvas, resultBand, rect.Left, rect.Right, active, showYellow, showRed, showBlue, true);
        }

        canvas.DrawText(definition.Name, tile.MidX, tile.Bottom - 10f, _labelPaint);
    }

    private void DrawTopComponentField(SKCanvas canvas, AreaDisplayGeometry geometry)
    {
        if (_coords == null)
        {
            return;
        }

        float frameMin = geometry.FrameMin;
        float frameMax = geometry.FrameMax;
        if (frameMax <= frameMin)
        {
            frameMax = frameMin + 1f;
        }

        const float epsilon = 0.0001f;
        var boundaries = geometry.ParallelBoundaries;

        float topMath = MathF.Max(InputAY, InputBY) + 0.45f;
        float bottomMath = MathF.Min(InputAY, InputBY) - 0.45f;

        for (int i = 0; i < boundaries.Count - 1; i++)
        {
            float start = boundaries[i];
            float end = boundaries[i + 1];
            if (end - start <= epsilon)
            {
                continue;
            }

            float mid = (start + end) * 0.5f;
            bool inAImag = geometry.InHorizontalImag(mid);
            bool inAReal = geometry.InHorizontalReal(mid);
            bool inBImag = geometry.InVerticalImag(mid);
            bool inBReal = geometry.InVerticalReal(mid);

            var leftTop = _coords.MathToPixel(start, topMath);
            var rightBottom = _coords.MathToPixel(end, bottomMath);
            var rect = new SKRect(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);

            canvas.DrawRect(rect, _falsePaint);
            if (inAImag || inBImag)
            {
                canvas.DrawRect(rect, _yellowPaint);
            }

            if (inAReal || inBReal)
            {
                canvas.Save();
                canvas.ClipRect(rect);
                const float stripe = 8f;
                if (inAReal)
                {
                    for (float y = rect.Top - 1f; y <= rect.Bottom + 1f; y += stripe)
                    {
                        canvas.DrawLine(rect.Left, y, rect.Right, y, _redStripePaint);
                    }
                }

                if (inBReal)
                {
                    for (float x = rect.Left - 1f; x <= rect.Right + 1f; x += stripe)
                    {
                        canvas.DrawLine(x, rect.Top, x, rect.Bottom, _blueStripePaint);
                    }
                }

                canvas.Restore();
            }

            canvas.DrawRect(rect, _topRegionBorderPaint);
        }
    }

    private void DrawStyledSegment(
        SKCanvas canvas,
        SKRect band,
        float left,
        float right,
        bool active,
        bool showYellow,
        bool showRed,
        bool showBlue,
        bool drawCrossWhenFalse)
    {
        var rect = new SKRect(left, band.Top, right, band.Bottom);
        canvas.DrawRect(rect, _falsePaint);
        if (active)
        {
            canvas.DrawRect(rect, _activeWashPaint);
        }

        if (showYellow)
        {
            canvas.DrawRect(rect, _yellowPaint);
        }

        if (showRed || showBlue)
        {
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

        canvas.DrawRect(rect, active ? _activeRegionBorderPaint : _inactiveRegionBorderPaint);

        if (drawCrossWhenFalse && !active)
        {
            canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, _crossPaint);
            canvas.DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top, _crossPaint);
        }
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

        var originPx = GetZeroCenterPixel();
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_axisA, _axisB];
    public SKPoint? GetOriginPixel() => _coords == null ? null : GetZeroCenterPixel();

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
        _labelPaint.Dispose();
        _tileBorderPaint.Dispose();
        _tileBackgroundPaint.Dispose();
        _yellowPaint.Dispose();
        _activeWashPaint.Dispose();
        _falsePaint.Dispose();
        _crossPaint.Dispose();
        _axisGuidePaint.Dispose();
        _redStripePaint.Dispose();
        _blueStripePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
        _inactiveRegionBorderPaint.Dispose();
        _activeRegionBorderPaint.Dispose();
        _zeroBarPaint.Dispose();
        _topRegionBorderPaint.Dispose();
    }

    private SKPoint GetZeroCenterPixel()
    {
        return _coords!.MathToPixel(0, (InputAY + InputBY) * 0.5f);
    }

}
