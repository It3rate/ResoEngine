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

    /// <summary>Initialize the page — create renderers, register hit targets.</summary>
    void Init(CoordinateSystem coords, HitTestEngine hitTest);

    /// <summary>Render the page onto the SkiaSharp canvas.</summary>
    void Render(SKCanvas canvas);

    /// <summary>Tear down — dispose renderers.</summary>
    void Destroy();
}
