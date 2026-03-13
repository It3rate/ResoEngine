using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class BooleanOpsPage : IVisualizerPage
{
    public string Title => "Boolean Operations (Core2)";

    private readonly AxisDisplayMapper _axisA = new(
        new Axis(new Proportion(3, 1), new Proportion(5, 1)),
        "A");
    private readonly AxisDisplayMapper _axisB = new(
        new Axis(new Proportion(1, 1), new Proportion(3, 1)),
        "B");

    private Axis _andAxis = Axis.Zero;
    private Axis _orAxis = Axis.Zero;
    private Axis _notAAxis = Axis.Zero;
    private Axis _notBAxis = Axis.Zero;
    private Axis _xorAxis = Axis.Zero;

    private readonly AxisDisplayMapper _andDisplay = new(Axis.Zero);
    private readonly AxisDisplayMapper _orDisplay = new(Axis.Zero);
    private readonly AxisDisplayMapper _notADisplay = new(Axis.Zero);
    private readonly AxisDisplayMapper _notBDisplay = new(Axis.Zero);
    private readonly AxisDisplayMapper _xorDisplay = new(Axis.Zero);

    private CoordinateSystem? _coords;

    private const float InputAY = 2.0f;
    private const float InputBY = 0.5f;
    private const float AndY = -2.0f;
    private const float OrY = -3.5f;
    private const float NotAY = -5.0f;
    private const float NotBY = -6.5f;
    private const float XorY = -8.0f;

    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private SegmentRenderer? _andRenderer;
    private SegmentRenderer? _orRenderer;
    private SegmentRenderer? _notARenderer;
    private SegmentRenderer? _notBRenderer;
    private SegmentRenderer? _xorRenderer;

    private readonly SKPaint _separatorPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(180, 180, 180),
        IsAntialias = true,
    };

    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = VisualStyle.FontSize * 0.85f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Right,
        IsAntialias = true,
    };

    private readonly SKPaint _originFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true
    };
    private readonly SKPaint _originStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = VisualStyle.StrokeWidth,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true
    };
    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true
    };

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;

        coords.OriginX = coords.Width / 2;
        coords.OriginY = 130;

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: InputAY);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Blue, crossPosition: InputBY);
        hitTest.Register(_rendererA, _axisA);
        hitTest.Register(_rendererB, _axisB);

        _andRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Green, crossPosition: AndY);
        _orRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Orange, crossPosition: OrY);
        _notARenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: NotAY);
        _notBRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Blue, crossPosition: NotBY);
        _xorRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Purple, crossPosition: XorY);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        SyncInputsFromDisplay();
        ComputeResults();

        float sepY = (InputBY + AndY) / 2f;
        var sepLeft = _coords.MathToPixel(-14, sepY);
        var sepRight = _coords.MathToPixel(14, sepY);
        canvas.DrawLine(sepLeft, sepRight, _separatorPaint);

        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

        _andRenderer?.Render(canvas, _andDisplay);
        _orRenderer?.Render(canvas, _orDisplay);
        _notARenderer?.Render(canvas, _notADisplay);
        _notBRenderer?.Render(canvas, _notBDisplay);
        _xorRenderer?.Render(canvas, _xorDisplay);

        float labelX = _coords.Width - 20;
        DrawLabel(canvas, "A", InputAY, SegmentColors.Red.Label, labelX);
        DrawLabel(canvas, "B", InputBY, SegmentColors.Blue.Label, labelX);
        DrawLabel(canvas, "AND", AndY, SegmentColors.Green.Label, labelX);
        DrawLabel(canvas, "OR", OrY, SegmentColors.Orange.Label, labelX);
        DrawLabel(canvas, "NOT A", NotAY, SegmentColors.Red.Label, labelX);
        DrawLabel(canvas, "NOT B", NotBY, SegmentColors.Blue.Label, labelX);
        DrawLabel(canvas, "XOR", XorY, SegmentColors.Purple.Label, labelX);

        var originPx = _coords.MathToPixel(0, 0);
        var axisTop = _coords.MathToPixel(0, InputAY + 1.5f);
        var axisBot = _coords.MathToPixel(0, XorY - 1.5f);
        using var axisPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 0.5f,
            Color = new SKColor(200, 200, 200),
            IsAntialias = true,
        };
        canvas.DrawLine(axisTop, axisBot, axisPaint);

        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void SyncInputsFromDisplay()
    {
    }

    private void DrawLabel(SKCanvas canvas, string text, float mathY, SKColor color, float pixelX)
    {
        var pos = _coords!.MathToPixel(0, mathY);
        _labelPaint.Color = color;
        canvas.DrawText(text, pixelX, pos.Y + 6, _labelPaint);
    }

    private void ComputeResults()
    {
        _andAxis = _axisA.Axis.Intersect(_axisB.Axis);
        _orAxis = _axisA.Axis.Union(_axisB.Axis);
        _notAAxis = _axisA.Axis.BooleanNot();
        _notBAxis = _axisB.Axis.BooleanNot();
        _xorAxis = _axisA.Axis.Xor(_axisB.Axis);

        _andDisplay.SetAxis(_andAxis);
        _orDisplay.SetAxis(_orAxis);
        _notADisplay.SetAxis(_notAAxis);
        _notBDisplay.SetAxis(_notBAxis);
        _xorDisplay.SetAxis(_xorAxis);
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_axisA, _axisB];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0, 0);

    public void Destroy()
    {
        _rendererA?.Dispose(); _rendererA = null;
        _rendererB?.Dispose(); _rendererB = null;
        _andRenderer?.Dispose(); _andRenderer = null;
        _orRenderer?.Dispose(); _orRenderer = null;
        _notARenderer?.Dispose(); _notARenderer = null;
        _notBRenderer?.Dispose(); _notBRenderer = null;
        _xorRenderer?.Dispose(); _xorRenderer = null;
        _coords = null;
    }

    public void Dispose()
    {
        Destroy();
        _separatorPaint.Dispose();
        _labelPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
