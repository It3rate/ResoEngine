using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Rendering;
using System.Diagnostics;

namespace ResoEngine.Visualizer.Input;

public class DragController
{
    private readonly CoordinateSystem _coords;
    private readonly HitTestEngine _hitTest;
    private readonly float _snapIncrement;

    private DragTarget? _active;
    private SKPoint _lastPixelPos;

    public event Action? Changed;

    public DragController(CoordinateSystem coords, HitTestEngine hitTest,
                          float snapIncrement = 0.5f)
    {
        _coords = coords;
        _hitTest = hitTest;
        _snapIncrement = snapIncrement;
    }

    public Cursor GetCursor(SKPoint pixelPos)
    {
        var target = _hitTest.HitTest(pixelPos);
        if (target == null) return Cursors.Default;
        if (_active != null) return Cursors.Hand; // dragging

        return target.Axis == SegmentOrientation.Horizontal
            ? Cursors.SizeWE
            : Cursors.SizeNS;
    }

    float _lastSegPosReal;
    float _lastSegPosImaginary;
    public bool BeginDrag(SKPoint pixelPos)
    {
        _active = _hitTest.HitTest(pixelPos);
        if(_active != null)
        {
            _lastSegPosReal = _active.Segment.Real;
            _lastSegPosImaginary = _active.Segment.Imaginary;
            _lastPixelPos = pixelPos;
        }
        return _active != null;
    }

    public bool UpdateDrag(SKPoint pixelPos)
    {
        if (_active == null) return false;

        float dx = pixelPos.X - _lastPixelPos.X;
        float dy = pixelPos.Y - _lastPixelPos.Y;

        if (_active.Axis == SegmentOrientation.Horizontal) dy = 0;
        if (_active.Axis == SegmentOrientation.Vertical) dx = 0;

        float mx = dx / _coords.Scale;
        float my = -dy / _coords.Scale;
        float delta = _active.Axis == SegmentOrientation.Horizontal ? mx : my;

        var seg = _active.Segment;
        switch (_active.Zone)
        {
            case DragZone.Dot:
                seg.Imaginary = Snap(_lastSegPosImaginary + delta);
                break;
            case DragZone.Arrow:
                seg.Real = Snap(_lastSegPosReal + delta);
                break;
            case DragZone.Bar:
                seg.Imaginary = Snap(_lastSegPosImaginary + delta);
                seg.Real = Snap(_lastSegPosReal + delta);
                break;
        }

        Changed?.Invoke();
        return true;
    }

    public void EndDrag()
    {
        _active = null;
        _lastSegPosReal = 0;
        _lastSegPosImaginary = 0;
    }

    public bool IsDragging => _active != null;

    private float Snap(float val) =>
        MathF.Round(val / _snapIncrement) * _snapIncrement;
}
