using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Page 1: Two directed segments joined orthogonally.
/// Segment A (red, horizontal) and Segment B (blue, vertical).
/// Grid lines show the OR combination of their real regions.
/// Endpoints are draggable; grid updates live.
/// </summary>
public class OrthogonalAxesPage : IVisualizerPage
{
    public string Title => "Orthogonal Axes";

    private readonly DirectedSegment _segA = new(-3, 5, "A");
    private readonly DirectedSegment _segB = new(-2, 5, "B");

    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private GridRenderer? _gridRenderer;

    public void Init(CoordinateSystem coords, HitTestEngine hitTest)
    {
        _gridRenderer = new GridRenderer(coords);

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Red, crossPosition: 0);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Vertical,
            SegmentColors.Blue, crossPosition: 0);

        // Register hit targets (B on top of A, tested first)
        hitTest.Register(_rendererA, _segA);
        hitTest.Register(_rendererB, _segB);
    }

    public void Render(SKCanvas canvas)
    {
        // Draw order: grid behind, then A, then B on top
        _gridRenderer?.Render(canvas, _segA, _segB, SegmentColors.Red, SegmentColors.Blue);
        _rendererA?.Render(canvas, _segA);
        _rendererB?.Render(canvas, _segB);
    }

    public void Destroy()
    {
        _rendererA?.Dispose(); _rendererA = null;
        _rendererB?.Dispose(); _rendererB = null;
        _gridRenderer?.Dispose(); _gridRenderer = null;
    }

    public void Dispose() => Destroy();
}
