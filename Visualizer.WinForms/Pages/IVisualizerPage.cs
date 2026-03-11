using SkiaSharp;
using ResoEngine.Visualizer.Controls;
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
    void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas);
    void Render(SKCanvas canvas);
    void Destroy();

    /// <summary>Check if a pixel point hits the origin dot. Used for center drag.</summary>
    bool IsOriginHit(SKPoint pixelPoint) => false;

    /// <summary>Get all segments on this page (for origin drag: move all at once).</summary>
    IReadOnlyList<DirectedSegment>? GetDraggableSegments() => null;

    /// <summary>Get the origin's pixel position (for cursor changes).</summary>
    SKPoint? GetOriginPixel() => null;

    /// <summary>
    /// Handle a pointer-down at a viewBox pixel point.
    /// Return true if the event was consumed (suppresses origin drag and segment drag).
    /// </summary>
    bool OnPointerDown(SKPoint pixelPoint) => false;
}
