using SkiaSharp;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Page 2: Two parallel horizontal input segments (A, B) at the top,
/// with boolean operation results (AND, OR, NOT A, NOT B, XOR) displayed below.
/// Dragging A or B immediately updates all computed results.
/// </summary>
public class BooleanOpsPage : IVisualizerPage
{
    public string Title => "Boolean Operations";

    // --- Input segments (draggable) ---
    private readonly DirectedSegment _segA = new(-3, 5, "A");
    private readonly DirectedSegment _segB = new(-1, 3, "B");

    // --- Result segments (computed each frame, not draggable) ---
    private readonly DirectedSegment _andResult = new(0, 0, "");
    private readonly DirectedSegment _orResult = new(0, 0, "");
    private readonly DirectedSegment _notAResult = new(0, 0, "");
    private readonly DirectedSegment _notBResult = new(0, 0, "");
    private readonly DirectedSegment _xorResult = new(0, 0, "");

    private CoordinateSystem? _coords;

    // --- Y positions (math coords) for each row ---
    private const float InputAY = 2.0f;
    private const float InputBY = 0.5f;
    private const float AndY = -2.0f;
    private const float OrY = -3.5f;
    private const float NotAY = -5.0f;
    private const float NotBY = -6.5f;
    private const float XorY = -8.0f;

    // --- Renderers ---
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private SegmentRenderer? _andRenderer;
    private SegmentRenderer? _orRenderer;
    private SegmentRenderer? _notARenderer;
    private SegmentRenderer? _notBRenderer;
    private SegmentRenderer? _xorRenderer;

    // --- Separator line paint ---
    private readonly SKPaint _separatorPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(180, 180, 180),
        IsAntialias = true,
    };

    // --- Row label paint ---
    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = VisualStyle.FontSize * 0.85f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Right,
        IsAntialias = true,
    };

    // --- Origin dot paints ---
    private readonly SKPaint _originFillPaint = new()
    {
        Style = SKPaintStyle.Fill, Color = SKColors.White, IsAntialias = true
    };
    private readonly SKPaint _originStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke, StrokeWidth = VisualStyle.StrokeWidth,
        Color = new SKColor(80, 80, 80), IsAntialias = true
    };
    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill, Color = new SKColor(80, 80, 80), IsAntialias = true
    };

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;

        // Position origin near the top so inputs are at top, results flow downward
        coords.OriginX = coords.Width / 2;
        coords.OriginY = 130;

        // Input renderers (draggable — registered with hit test)
        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Red, crossPosition: InputAY);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Blue, crossPosition: InputBY);

        hitTest.Register(_rendererA, _segA);
        hitTest.Register(_rendererB, _segB);

        // Result renderers (NOT registered — can't be dragged)
        _andRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Green, crossPosition: AndY);
        _orRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Orange, crossPosition: OrY);
        _notARenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Red, crossPosition: NotAY);
        _notBRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Blue, crossPosition: NotBY);
        _xorRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Purple, crossPosition: XorY);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null) return;

        // Recompute boolean results from current A and B values
        ComputeResults();

        // --- Draw separator line between inputs and results ---
        float sepY = (InputBY + AndY) / 2f;
        var sepLeft = _coords.MathToPixel(-14, sepY);
        var sepRight = _coords.MathToPixel(14, sepY);
        canvas.DrawLine(sepLeft, sepRight, _separatorPaint);

        // --- Render input segments ---
        _rendererA?.Render(canvas, _segA);
        _rendererB?.Render(canvas, _segB);

        // --- Render result segments ---
        _andRenderer?.Render(canvas, _andResult);
        _orRenderer?.Render(canvas, _orResult);
        _notARenderer?.Render(canvas, _notAResult);
        _notBRenderer?.Render(canvas, _notBResult);
        _xorRenderer?.Render(canvas, _xorResult);

        // --- Draw row labels on the right edge ---
        float labelX = _coords.Width - 20;
        DrawLabel(canvas, "A", InputAY, SegmentColors.Red.Label, labelX);
        DrawLabel(canvas, "B", InputBY, SegmentColors.Blue.Label, labelX);
        DrawLabel(canvas, "AND", AndY, SegmentColors.Green.Label, labelX);
        DrawLabel(canvas, "OR", OrY, SegmentColors.Orange.Label, labelX);
        DrawLabel(canvas, "NOT A", NotAY, SegmentColors.Red.Label, labelX);
        DrawLabel(canvas, "NOT B", NotBY, SegmentColors.Blue.Label, labelX);
        DrawLabel(canvas, "XOR", XorY, SegmentColors.Purple.Label, labelX);

        // --- Origin dot ---
        var originPx = _coords.MathToPixel(0, 0);
        // Draw a thin vertical axis line through all rows as a reference
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

        // Small origin marker at (0,0) math
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawLabel(SKCanvas canvas, string text, float mathY, SKColor color, float pixelX)
    {
        var pos = _coords!.MathToPixel(0, mathY);
        _labelPaint.Color = color;
        canvas.DrawText(text, pixelX, pos.Y + 6, _labelPaint);
    }

    /// <summary>
    /// Compute boolean operation results from segments A and B.
    ///
    /// Interval interpretation:
    ///   AND  = intersection: (max(A.imag, B.imag), min(A.real, B.real))
    ///   OR   = union:        (min(A.imag, B.imag), max(A.real, B.real))
    ///   NOT  = negate:       (-real, -imag)   (swap and negate)
    ///   XOR  = symmetric difference: OR(AND(A, NOT_B), AND(NOT_A, B))
    /// </summary>
    private void ComputeResults()
    {
        float aI = _segA.Imaginary, aR = _segA.Real;
        float bI = _segB.Imaginary, bR = _segB.Real;

        // AND: intersection
        float andI = MathF.Max(aI, bI);
        float andR = MathF.Min(aR, bR);
        if (andI >= andR) { andI = 0; andR = 0; }
        _andResult.Imaginary = andI;
        _andResult.Real = andR;

        // OR: union
        _orResult.Imaginary = MathF.Min(aI, bI);
        _orResult.Real = MathF.Max(aR, bR);

        // NOT A: negate (swap and negate)
        _notAResult.Imaginary = -aR;
        _notAResult.Real = -aI;

        // NOT B: negate
        _notBResult.Imaginary = -bR;
        _notBResult.Real = -bI;

        // XOR: OR(AND(A, NOT_B), AND(NOT_A, B))
        float notBI = -bR, notBR = -bI;
        float notAI = -aR, notAR = -aI;

        // AND(A, NOT_B)
        float x1I = MathF.Max(aI, notBI);
        float x1R = MathF.Min(aR, notBR);
        if (x1I >= x1R) { x1I = 0; x1R = 0; }

        // AND(NOT_A, B)
        float x2I = MathF.Max(notAI, bI);
        float x2R = MathF.Min(notAR, bR);
        if (x2I >= x2R) { x2I = 0; x2R = 0; }

        // OR of the two parts
        if (x1I == 0 && x1R == 0)
        {
            _xorResult.Imaginary = x2I;
            _xorResult.Real = x2R;
        }
        else if (x2I == 0 && x2R == 0)
        {
            _xorResult.Imaginary = x1I;
            _xorResult.Real = x1R;
        }
        else
        {
            _xorResult.Imaginary = MathF.Min(x1I, x2I);
            _xorResult.Real = MathF.Max(x1R, x2R);
        }
    }

    // --- Origin hit for panning support ---
    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null) return false;
        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_segA, _segB];
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



