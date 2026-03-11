using SkiaSharp;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Page 1: Two directed segments joined orthogonally.
/// Segment A (red, horizontal) and Segment B (blue, vertical).
/// Origin dot drawn on top of everything. Dragging origin moves all segments.
/// </summary>
public class OrthogonalAxesPage : IVisualizerPage
{
    public string Title => "Orthogonal Axes";

    private readonly DirectedSegment _segA = new(-3, 5, "A");
    private readonly DirectedSegment _segB = new(-2, 5, "B");
    private readonly List<DirectedSegment> _allSegments;

    private CoordinateSystem? _coords;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private GridRenderer? _gridRenderer;

    // Formula paint (3-line multiplication display at bottom)
    private readonly SKPaint _formulaPaint = new()
    {
        Color = new SKColor(50, 50, 50),
        TextSize = 17f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    // Origin dot paints
    private readonly SKPaint _originFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };
    private readonly SKPaint _originStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = VisualStyle.StrokeWidth,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };
    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };

    public OrthogonalAxesPage()
    {
        _allSegments = [_segA, _segB];
    }

    public void Init(CoordinateSystem coords, HitTestEngine hitTest)
    {
        _coords = coords;

        // Reset origin to center (other pages may have moved it)
        coords.OriginX = coords.Width / 2;
        coords.OriginY = coords.Height / 2;

        _gridRenderer = new GridRenderer(coords);

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal,
            SegmentColors.Red, crossPosition: 0);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Vertical,
            SegmentColors.Blue, crossPosition: 0);

        hitTest.Register(_rendererA, _segA);
        hitTest.Register(_rendererB, _segB);
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null) return;

        // 1. Grid (behind everything)
        _gridRenderer?.Render(canvas, _segA, _segB, SegmentColors.Red, SegmentColors.Blue);

        // 2. Segments
        _rendererA?.Render(canvas, _segA);
        _rendererB?.Render(canvas, _segB);

        // 3. Origin dot (ON TOP of everything)
        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);

        // 4. Multiplication formula at bottom of canvas
        DrawFormula(canvas);
    }

    private void DrawFormula(SKCanvas canvas)
    {
        if (_coords == null) return;

        // Display values: imaginary is positive-left (negate stored value)
        float ai = -_segA.Imaginary;
        float ar = _segA.Real;
        float bi = -_segB.Imaginary;
        float br = _segB.Real;

        // FOIL the two factors (ai + ar)(bi + br):
        //   ai·bi → real contribution = -(ai*bi)  [i²=-1]
        //   ai·br → imaginary cross-term 1
        //   ar·bi → imaginary cross-term 2
        //   ar·br → real contribution
        float cross1   = ai * br;           // ai × br → cross1·i
        float cross2   = ar * bi;           // ar × bi → cross2·i
        float realProd = ar * br;           // ar × br → real
        float imagSq   = ai * bi;           // ai × bi → −imagSq real (i²=−1)
        float resultImag = cross1 + cross2;
        float resultReal = realProd - imagSq;

        // Line 1: (ai + ar)(bi + br)
        string line1 = $"({N(ai)}i {Pm(ar)} {A(ar)})({N(bi)}i {Pm(br)} {A(br)})";

        // Line 2: = (cross1·i ± cross2·i)(realProd ∓ imagSq)
        //   Second group: realProd + (−imagSq), so sign is opposite to imagSq
        string line2 = $"= ({N(cross1)}i {Pm(cross2)} {A(cross2)}i)" +
                       $"({N(realProd)} {PmNeg(imagSq)} {A(imagSq)})";

        // Line 3: = (resultImag·i ± resultReal)
        string line3 = $"= ({N(resultImag)}i {Pm(resultReal)} {A(resultReal)})";

        // Draw at fixed bottom of viewbox, unaffected by panning
        float cx = _coords.Width / 2;
        float lineH = 27f;
        float baseY = _coords.Height - 74;

        canvas.DrawText(line1, cx, baseY,           _formulaPaint);
        canvas.DrawText(line2, cx, baseY + lineH,   _formulaPaint);
        canvas.DrawText(line3, cx, baseY + lineH*2, _formulaPaint);
    }

    // --- Number formatting helpers ---

    /// Format float: integer if whole, 1 decimal otherwise. Includes sign if negative.
    private static string N(float v)
    {
        float r = MathF.Round(v * 10) / 10;
        return MathF.Abs(r - MathF.Round(r)) < 0.05f
            ? ((int)MathF.Round(r)).ToString()
            : r.ToString("F1");
    }

    /// Absolute value, formatted with N.
    private static string A(float v) => N(MathF.Abs(v));

    /// "+" or "−" based on sign of v (for the operator between terms).
    private static string Pm(float v) => v >= 0 ? "+" : "-";

    /// Negated: "−" when v > 0, "+" when v ≤ 0 (used for the i² real contribution).
    private static string PmNeg(float v) => v > 0 ? "-" : "+";

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null) return false;
        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<DirectedSegment>? GetDraggableSegments() => _allSegments;

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0, 0);

    public void Destroy()
    {
        _rendererA?.Dispose(); _rendererA = null;
        _rendererB?.Dispose(); _rendererB = null;
        _gridRenderer?.Dispose(); _gridRenderer = null;
        _coords = null;
    }

    public void Dispose()
    {
        Destroy();
        _formulaPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
