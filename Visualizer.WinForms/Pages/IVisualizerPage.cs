using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Interface for visualizer pages. Each page owns its segments, renderers,
/// and registers hit-test targets.
/// </summary>
public interface IVisualizerPage : IDisposable
{
    string Title { get; }
    void Init(CoordinateSystem coords, HitTestEngine hitTest);
    void Render(SKCanvas canvas);
    void Destroy();

    /// <summary>Check if a pixel point hits the origin dot. Used for center drag.</summary>
    bool IsOriginHit(SKPoint pixelPoint) => false;

    /// <summary>Get all segments on this page (for origin drag: move all at once).</summary>
    IReadOnlyList<DirectedSegment>? GetDraggableSegments() => null;

    /// <summary>Get the origin's pixel position (for cursor changes).</summary>
    SKPoint? GetOriginPixel() => null;
}
