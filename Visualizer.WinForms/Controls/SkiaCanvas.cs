using SkiaSharp;
using SkiaSharp.Views.Desktop;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Controls;

/// <summary>
/// Custom control wrapping SKControl for SkiaSharp rendering.
/// Provides math↔pixel coordinate system and input event forwarding.
/// </summary>
public class SkiaCanvas : UserControl
{
    private readonly SKControl _skControl;

    public CoordinateSystem Coords { get; }

    /// <summary>Fired each frame — pages register their render method here.</summary>
    public event Action<SKCanvas>? OnRender;

    /// <summary>Input events in viewBox coordinates (560×500 logical space).</summary>
    public event Action<SKPoint>? OnPointerDown;
    public event Action<SKPoint>? OnPointerMove;
    public event Action<SKPoint>? OnPointerUp;

    public SkiaCanvas()
    {
        Coords = new CoordinateSystem();

        _skControl = new SKControl { Dock = DockStyle.Fill };
        _skControl.PaintSurface += OnPaintSurface;
        _skControl.MouseDown += (_, e) => OnPointerDown?.Invoke(ControlToViewBox(e.Location));
        _skControl.MouseMove += (_, e) => OnPointerMove?.Invoke(ControlToViewBox(e.Location));
        _skControl.MouseUp += (_, e) => OnPointerUp?.Invoke(ControlToViewBox(e.Location));

        Controls.Add(_skControl);
    }

    /// <summary>Request a repaint.</summary>
    public void InvalidateCanvas() => _skControl.Invalidate();

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        // Scale from physical pixels to logical viewBox coordinates
        float scaleX = e.Info.Width / Coords.Width;
        float scaleY = e.Info.Height / Coords.Height;
        canvas.Scale(scaleX, scaleY);

        OnRender?.Invoke(canvas);
    }

    /// <summary>Convert control pixel position to viewBox coordinates.</summary>
    private SKPoint ControlToViewBox(Point mousePos)
    {
        float scaleX = _skControl.Width / Coords.Width;
        float scaleY = _skControl.Height / Coords.Height;
        return new SKPoint(mousePos.X / scaleX, mousePos.Y / scaleY);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _skControl.PaintSurface -= OnPaintSurface;
            _skControl.Dispose();
        }
        base.Dispose(disposing);
    }
}
