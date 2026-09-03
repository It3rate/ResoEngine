using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.VisualTree;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Pages;
using SkiaSharp;

namespace ResoEngine.Visualizer.Controls;

public sealed class SkiaCanvas : Control
{
    private const float LogicalWidth = 1400f;
    private const float LogicalHeight = 1000f;
    private IVisualizerPage page;

    public SkiaCanvas(IVisualizerPage page)
    {
        this.page = page;
        Coords = new CoordinateSystem(LogicalWidth, LogicalHeight, LogicalWidth / 2f, LogicalHeight / 2f, 30f);
        ClipToBounds = true;
    }

    public CoordinateSystem Coords { get; }

    public CanvasClientSize ClientSize => new(LogicalWidth, LogicalHeight);

    public void SetPage(IVisualizerPage nextPage)
    {
        page = nextPage;
        InvalidateVisual();
    }

    public void InvalidateCanvas() => InvalidateVisual();

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.Custom(new VisualizerDrawOperation(new Rect(Bounds.Size), page));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        InvalidateVisual();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Pointer.Capture(this);
        if (page.OnPointerDown(ToLogical(e.GetPosition(this))))
        {
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        page.OnPointerMove(ToLogical(e.GetPosition(this)));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        page.OnPointerUp(ToLogical(e.GetPosition(this)));
        e.Pointer.Capture(null);
    }

    private SKPoint ToLogical(Point point)
    {
        var width = Math.Max(1d, Bounds.Width);
        var height = Math.Max(1d, Bounds.Height);
        return new SKPoint(
            (float)(point.X * LogicalWidth / width),
            (float)(point.Y * LogicalHeight / height));
    }

    public readonly record struct CanvasClientSize(float Width, float Height);

    private sealed class VisualizerDrawOperation(Rect bounds, IVisualizerPage page) : ICustomDrawOperation
    {
        public Rect Bounds { get; } = bounds;

        public void Dispose()
        {
        }

        public bool HitTest(Point point) => Bounds.Contains(point);

        public bool Equals(ICustomDrawOperation? other) => false;

        public void Render(ImmediateDrawingContext context)
        {
            if (context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) is not ISkiaSharpApiLeaseFeature leaseFeature)
            {
                return;
            }

            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            canvas.Save();
            canvas.Clear(new SKColor(247, 246, 241));
            canvas.ClipRect(new SKRect(0f, 0f, (float)Bounds.Width, (float)Bounds.Height));
            canvas.Scale((float)Bounds.Width / LogicalWidth, (float)Bounds.Height / LogicalHeight);
            page.Render(canvas);
            canvas.Restore();
        }
    }
}
