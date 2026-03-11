using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Page 1: Two directed segments joined orthogonally.
/// Segment A (red, horizontal) and Segment B (blue, vertical).
/// Origin dot drawn on top of everything. Dragging origin moves all segments.
/// </summary>
public class OrthogonalAxesPage : IVisualizerPage
{
    public string Title => "Orthogonal Axes";

    private readonly DirectedSegment _segA = new(-3, 5, "A");
    private readonly DirectedSegment _segB = new(-2, 5, "B");
    private readonly List<DirectedSegment> _allSegments;

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private GridRenderer? _gridRenderer;

    // Origin dot paints
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

    public OrthogonalAxesPage()
    {
        _allSegments = [_segA, _segB];
    }

    public void Init(CoordinateSystem coords, HitTestEngine hitTest)
    {
        _coords = coords;
        _gridRenderer = new GridRenderer(coords);

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Red, crossPosition: 0);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Vertical,
            SegmentColors.Blue, crossPosition: 0);

        hitTest.Register(_rendererA, _segA);
        hitTest.Register(_rendererB, _segB);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null) return;

        // 1. Grid (behind everything)
        _gridRenderer?.Render(canvas, _segA, _segB, SegmentColors.Red, SegmentColors.Blue);

        // 2. Segments
        _rendererA?.Render(canvas, _segA);
        _rendererB?.Render(canvas, _segB);

        // 3. Origin dot (ON TOP of everything)
        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null) return false;
        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<DirectedSegment>? GetDraggableSegments() => _allSegments;

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0, 0);

    public void Destroy()
    {
        _rendererA?.Dispose(); _rendererA = null;
        _rendererB?.Dispose(); _rendererB = null;
        _gridRenderer?.Dispose(); _gridRenderer = null;
        _coords = null;
    }

    public void Dispose()
    {
        Destroy();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
