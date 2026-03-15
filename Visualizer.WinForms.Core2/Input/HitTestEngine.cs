using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Rendering;

namespace ResoEngine.Visualizer.Input;

/// <summary>
/// Aggregates hit-testing across all segment renderers.
/// Tests in reverse registration order (last registered = on top = tested first).
/// </summary>
public class HitTestEngine
{
    private readonly List<(SegmentRenderer Renderer, ISegmentValue Segment)> _targets = [];

    public void Register(SegmentRenderer renderer, ISegmentValue segment) =>
        _targets.Add((renderer, segment));

    public void Clear() => _targets.Clear();

    /// <summary>Hit-test a viewBox pixel point against all registered segments.</summary>
    public DragTarget? HitTest(SKPoint pixelPoint)
    {
        // Iterate in reverse so top-most segments are tested first
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            var (renderer, segment) = _targets[i];
            var zone = renderer.HitTest(pixelPoint, segment);
            if (zone.HasValue)
                return new DragTarget(segment, zone.Value, renderer.GetSegmentOrientation(), renderer.Scale);
        }
        return null;
    }
}
