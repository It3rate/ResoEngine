using Core2.Elements;
using Core2.Resolution;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed class ResolutionModesPage : IVisualizerPage
{
    private const int MinSupport = 1;
    private const int MaxSupport = 12;
    private const int MinNumerator = 0;
    private const float HandleRadius = 11f;

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(38, 38, 38),
        TextSize = 25f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(95, 95, 95),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _cardFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(252, 252, 252),
        IsAntialias = true,
    };

    private readonly SKPaint _cardStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.3f,
        Color = new SKColor(218, 218, 218),
        IsAntialias = true,
    };

    private readonly SKPaint _sectionPaint = new()
    {
        Color = new SKColor(58, 58, 58),
        TextSize = 18f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(64, 64, 64),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _captionPaint = new()
    {
        Color = new SKColor(120, 120, 120),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _monoPaint = new()
    {
        Color = new SKColor(58, 58, 58),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _tickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(160, 160, 160),
        IsAntialias = true,
    };

    private readonly SKPaint _trackPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 5f,
        Color = new SKColor(214, 214, 214),
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _supportTextPaint = new()
    {
        Color = new SKColor(112, 112, 112),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _resultTrackPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 6f,
        Color = new SKColor(220, 220, 220),
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _handleFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _handleStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(110, 110, 110),
        IsAntialias = true,
    };

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(204, 204, 204),
        PathEffect = SKPathEffect.CreateDash([4f, 4f], 0f),
        IsAntialias = true,
    };

    private readonly SKColor _aColor = new(46, 125, 210);
    private readonly SKColor _bColor = new(214, 87, 60);
    private readonly SKColor _inheritColor = new(87, 155, 57);
    private readonly SKColor _reexpressColor = new(48, 155, 139);
    private readonly SKColor _commonFrameColor = new(83, 95, 205);
    private readonly SKColor _aggregateColor = new(222, 145, 45);
    private readonly SKColor _composeColor = new(150, 76, 186);

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private readonly List<HandleLayout> _handles = [];
    private RowLayout? _rowA;
    private RowLayout? _rowB;
    private DragTargetKind _dragTarget;

    private int _aNumerator = 5;
    private int _aSupport = 10;
    private int _bNumerator = 1;
    private int _bSupport = 2;

    public string Title => "Resolution Modes";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
    }

    public void Render(SKCanvas canvas)
    {
        _handles.Clear();
        _rowA = null;
        _rowB = null;

        float width = _coords?.Width ?? 1280f;
        float textY = 44f;

        canvas.DrawText("Resolution Modes", 32f, textY, _headingPaint);
        textY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "Drag the realized-value and support handles for A and B. The top rows show the raw tick counts directly. The cards below separate support-preserving scale, exact re-expression, exact common-frame alignment, pooled aggregation, and independent composition.",
            32f,
            ref textY,
            width - 64f,
            _bodyPaint);

        float inputTop = textY + 16f;
        var inputRect = new SKRect(32f, inputTop, width - 32f, inputTop + 250f);
        DrawCard(canvas, inputRect);
        canvas.DrawText("Inputs", inputRect.Left + 18f, inputRect.Top + 28f, _sectionPaint);

        float trackLeft = inputRect.Left + 126f;
        float trackRight = inputRect.Right - 42f;
        float tickSpacing = (trackRight - trackLeft) / MaxSupport;
        _rowA = new RowLayout(trackLeft, inputRect.Top + 92f, tickSpacing);
        _rowB = new RowLayout(trackLeft, inputRect.Top + 176f, tickSpacing);

        DrawInputRow(canvas, "A", CurrentA(), _rowA.Value, _aColor, DragTargetKind.AValue, DragTargetKind.ASupport);
        DrawInputRow(canvas, "B", CurrentB(), _rowB.Value, _bColor, DragTargetKind.BValue, DragTargetKind.BSupport);

        float resultsTop = inputRect.Bottom + 24f;
        float gap = 18f;
        float cardWidth = (width - 64f - gap * 2f) / 3f;
        float cardHeight = 176f;

        var aggregateRect = new SKRect(32f, resultsTop, 32f + cardWidth, resultsTop + cardHeight);
        var composeRect = new SKRect(aggregateRect.Right + gap, resultsTop, aggregateRect.Right + gap + cardWidth, resultsTop + cardHeight);
        var inheritRect = new SKRect(composeRect.Right + gap, resultsTop, composeRect.Right + gap + cardWidth, resultsTop + cardHeight);

        float secondRowTop = resultsTop + cardHeight + gap;
        var reexpressRect = new SKRect(32f, secondRowTop, 32f + cardWidth, secondRowTop + cardHeight);
        var commonRect = new SKRect(reexpressRect.Right + gap, secondRowTop, reexpressRect.Right + gap + cardWidth, secondRowTop + cardHeight);
        var fuzzyRect = new SKRect(commonRect.Right + gap, secondRowTop, commonRect.Right + gap + cardWidth, secondRowTop + cardHeight);

        var a = CurrentA();
        var b = CurrentB();
        var inheritScale = b.Fold();
        decimal inheritValueTicks = a.Numerator * (decimal)inheritScale;
        decimal inheritSupportTicks = a.Denominator;
        var refinedA = PrimitiveProportionResolution.RefineToSupport(a, a.Denominator * 2);
        long commonSupport = PrimitiveProportionResolution.GetCommonSupport(a, b);
        var commonFrame = PrimitiveProportionResolution.CommonFrameAdd(a, b);
        var aggregate = PrimitiveProportionResolution.Aggregate(a, b);
        var compose = a * b;
        var fuzzy = BuildFuzzyReadout(a, b);
        decimal maxSupport = new decimal[]
        {
            SupportMagnitude(a),
            SupportMagnitude(b),
            Math.Abs(inheritSupportTicks),
            SupportMagnitude(refinedA),
            SupportMagnitude(commonFrame),
            SupportMagnitude(aggregate),
            SupportMagnitude(compose),
            Math.Abs(fuzzy.SupportTicks),
        }.Max();

        DrawResultCard(
            canvas,
            aggregateRect,
            "Aggregate",
            "pool(A, B)",
            PrimitiveSupportLaw.Aggregate,
            FromProportion(aggregate, "supports are pooled"),
            _aggregateColor,
            maxSupport);

        DrawResultCard(
            canvas,
            composeRect,
            "Compose",
            "A * B",
            PrimitiveSupportLaw.Compose,
            FromProportion(compose, "supports form a product"),
            _composeColor,
            maxSupport);

        DrawResultCard(
            canvas,
            inheritRect,
            "Inherit",
            $"A * ({FormatScalarAsPseudoProportion(inheritScale)})",
            PrimitiveSupportLaw.Inherit,
            new DisplayReadout(
                $"{inheritValueTicks:0.###}/{inheritSupportTicks:0}",
                inheritValueTicks,
                inheritSupportTicks,
                FormatFold(inheritValueTicks, inheritSupportTicks),
                "support preserved from A"),
            _inheritColor,
            maxSupport);

        DrawResultCard(
            canvas,
            reexpressRect,
            "Re-express",
            "A -> refine(A, 2x support)",
            PrimitiveSupportLaw.Refine,
            FromProportion(refinedA, "same value, finer exact support"),
            _reexpressColor,
            maxSupport);

        DrawResultCard(
            canvas,
            commonRect,
            "Common Frame",
            $"align(A, B) on {commonSupport}; add",
            PrimitiveSupportLaw.CommonFrame,
            FromProportion(commonFrame, "exact temp alignment frame"),
            _commonFrameColor,
            maxSupport);

        DrawResultCard(
            canvas,
            fuzzyRect,
            "Fuzzy",
            "A + B with uncertainty bands",
            PrimitiveSupportLaw.CommonFrame,
            fuzzy with { Note = $"{fuzzy.Note} | NegFromUncert" },//{ResultSupportPolicy.NegotiateFromUncertainty}" },
            new SKColor(185, 92, 126),
            maxSupport);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        foreach (var handle in _handles)
        {
            if (Distance(handle.Center, pixelPoint) <= HandleRadius + 4f)
            {
                _dragTarget = handle.Target;
                return true;
            }
        }

        return false;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_dragTarget == DragTargetKind.None)
        {
            return;
        }

        switch (_dragTarget)
        {
            case DragTargetKind.AValue when _rowA is not null:
                _aNumerator = ClampTick(pixelPoint.X, _rowA.Value, MinNumerator, _aSupport);
                break;

            case DragTargetKind.ASupport when _rowA is not null:
                _aSupport = ClampTick(pixelPoint.X, _rowA.Value, MinSupport, MaxSupport);
                _aNumerator = Math.Min(_aNumerator, _aSupport);
                break;

            case DragTargetKind.BValue when _rowB is not null:
                _bNumerator = ClampTick(pixelPoint.X, _rowB.Value, MinNumerator, _bSupport);
                break;

            case DragTargetKind.BSupport when _rowB is not null:
                _bSupport = ClampTick(pixelPoint.X, _rowB.Value, MinSupport, MaxSupport);
                _bNumerator = Math.Min(_bNumerator, _bSupport);
                break;
        }

        _canvasHost?.InvalidateCanvas();
    }

    public void OnPointerUp(SKPoint pixelPoint) => _dragTarget = DragTargetKind.None;

    public bool IsOriginHit(SKPoint pixelPoint) => false;

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => null;

    public SKPoint? GetOriginPixel() => null;

    public void Destroy()
    {
    }

    public void Dispose()
    {
    }

    private void DrawInputRow(
        SKCanvas canvas,
        string label,
        Proportion value,
        RowLayout row,
        SKColor color,
        DragTargetKind valueTarget,
        DragTargetKind supportTarget)
    {
        canvas.DrawText(label, row.TrackLeft - 88f, row.AxisY + 6f, _sectionPaint);
        canvas.DrawText($"{value.Numerator}/{value.Denominator}", row.TrackLeft - 60f, row.AxisY - 22f, _monoPaint);
        canvas.DrawText($"fold {value.Fold()}", row.TrackLeft - 60f, row.AxisY + 24f, _captionPaint);

        for (int tick = 0; tick <= MaxSupport; tick++)
        {
            float x = row.TrackLeft + tick * row.TickSpacing;
            canvas.DrawLine(x, row.AxisY - 10f, x, row.AxisY + 10f, _tickPaint);
            canvas.DrawText(tick.ToString(), x - 4f, row.AxisY + 30f, _captionPaint);
        }

        canvas.DrawLine(row.TrackLeft, row.AxisY, row.TrackLeft + MaxSupport * row.TickSpacing, row.AxisY, _guidePaint);

        float supportX = row.TrackLeft + value.Denominator * row.TickSpacing;
        float valueX = row.TrackLeft + value.Numerator * row.TickSpacing;
        using var supportPaint = NewStrokePaint(new SKColor(color.Red, color.Green, color.Blue, 70), 6f);
        using var valuePaint = NewStrokePaint(color, 7f);
        canvas.DrawLine(row.TrackLeft, row.AxisY, supportX, row.AxisY, supportPaint);
        canvas.DrawLine(row.TrackLeft, row.AxisY, valueX, row.AxisY, valuePaint);

        DrawHandle(canvas, new SKPoint(valueX, row.AxisY), color);
        DrawHandle(canvas, new SKPoint(supportX, row.AxisY), new SKColor(92, 92, 92));
        _handles.Add(new HandleLayout(new SKPoint(valueX, row.AxisY), valueTarget));
        _handles.Add(new HandleLayout(new SKPoint(supportX, row.AxisY), supportTarget));

        canvas.DrawText($"value ticks: {value.Numerator}", supportX + 18f, row.AxisY - 4f, _labelPaint);
        canvas.DrawText($"support ticks: {value.Denominator}", supportX + 18f, row.AxisY + 16f, _supportTextPaint);
    }

    private void DrawResultCard(
        SKCanvas canvas,
        SKRect rect,
        string title,
        string formula,
        PrimitiveSupportLaw law,
        DisplayReadout value,
        SKColor color,
        decimal maxSupport)
    {
        DrawCard(canvas, rect);
        canvas.DrawText(title, rect.Left + 16f, rect.Top + 24f, _sectionPaint);
        canvas.DrawText(formula, rect.Left + 16f, rect.Top + 46f, _monoPaint);
        canvas.DrawText($"{law} | {value.Note}", rect.Left + 16f, rect.Top + 66f, _captionPaint);
        canvas.DrawText(value.RatioText, rect.Left + 16f, rect.Top + 92f, _monoPaint);
        canvas.DrawText($"fold: {value.FoldText}", rect.Left + 16f, rect.Top + 112f, _captionPaint);
        canvas.DrawText($"support ticks: {value.SupportTicks:0.###}", rect.Left + 16f, rect.Top + 130f, _captionPaint);

        DrawResultBar(canvas, rect, value, color, maxSupport, rect.Top + 152f);
    }

    private void DrawResultBar(SKCanvas canvas, SKRect rect, DisplayReadout value, SKColor color, decimal maxSupport, float y)
    {
        float left = rect.Left + 16f;
        float width = rect.Width - 32f;
        float supportRatio = maxSupport <= 0m ? 0f : Math.Clamp((float)(Math.Abs(value.SupportTicks) / maxSupport), 0f, 1f);
        float supportWidth = width * supportRatio;
        float valueWidth = value.SupportTicks == 0m
            ? 0f
            : supportWidth * Math.Clamp((float)(value.ValueTicks / Math.Abs(value.SupportTicks)), 0f, 1f);

        using var valuePaint = NewStrokePaint(color, 6f);
        canvas.DrawLine(left, y, left + supportWidth, y, _resultTrackPaint);
        canvas.DrawLine(left, y, left + valueWidth, y, valuePaint);
    }

    private void DrawHandle(SKCanvas canvas, SKPoint center, SKColor accent)
    {
        canvas.DrawCircle(center, HandleRadius, _handleFillPaint);
        using var accentStroke = NewStrokePaint(accent, 2.4f);
        canvas.DrawCircle(center, HandleRadius, accentStroke);
    }

    private static SKPaint NewStrokePaint(SKColor color, float strokeWidth) =>
        new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeWidth,
            StrokeCap = SKStrokeCap.Round,
            Color = color,
            IsAntialias = true,
        };

    private void DrawCard(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _cardStrokePaint);
    }

    private Proportion CurrentA() => new(_aNumerator, _aSupport);

    private Proportion CurrentB() => new(_bNumerator, _bSupport);

    private static int ClampTick(float pointerX, RowLayout row, int min, int max)
    {
        float raw = (pointerX - row.TrackLeft) / row.TickSpacing;
        return Math.Clamp((int)MathF.Round(raw), min, max);
    }

    private static float Distance(SKPoint left, SKPoint right)
    {
        float dx = left.X - right.X;
        float dy = left.Y - right.Y;
        return MathF.Sqrt((dx * dx) + (dy * dy));
    }

    private static long SupportMagnitude(Proportion value) => Math.Abs(value.Denominator);

    private static DisplayReadout FromProportion(Proportion value, string note) =>
        new(
            $"{value.Numerator}/{value.Denominator}",
            value.Numerator,
            value.Denominator,
            value.Fold().ToString(),
            note);

    private static DisplayReadout BuildFuzzyReadout(Proportion left, Proportion right)
    {
        decimal leftCenter = (decimal)left.Fold();
        decimal rightCenter = (decimal)right.Fold();
        decimal leftHalfWidth = left.Denominator == 0 ? 0m : 1m / (2m * Math.Abs(left.Denominator));
        decimal rightHalfWidth = right.Denominator == 0 ? 0m : 1m / (2m * Math.Abs(right.Denominator));

        decimal center = leftCenter + rightCenter;
        decimal halfWidth = leftHalfWidth + rightHalfWidth;
        decimal lower = center - halfWidth;
        decimal upper = center + halfWidth;

        decimal estimatedSupport = halfWidth <= 0m
            ? Math.Max(Math.Abs(left.Denominator), Math.Abs(right.Denominator))
            : Math.Clamp(Math.Round(1m / (2m * halfWidth), 2), 1m, 999m);

        decimal estimatedNumerator = Math.Round(center * estimatedSupport, 3);

        return new DisplayReadout(
            $"~ {estimatedNumerator:0.###}/{estimatedSupport:0.##}",
            estimatedNumerator,
            estimatedSupport,
            $"{center:0.###} in [{lower:0.###}, {upper:0.###}]",
            "uncertainty");// -weighted support");
    }

    private static string FormatScalarAsPseudoProportion(Scalar value) =>
        $"{((decimal)value):0.###}/1";

    private static string FormatFold(decimal valueTicks, decimal supportTicks)
    {
        if (supportTicks == 0m)
        {
            return "tension";
        }

        return new Scalar(valueTicks / supportTicks).ToString();
    }

    private readonly record struct RowLayout(float TrackLeft, float AxisY, float TickSpacing);

    private readonly record struct HandleLayout(SKPoint Center, DragTargetKind Target);

    private readonly record struct DisplayReadout(
        string RatioText,
        decimal ValueTicks,
        decimal SupportTicks,
        string FoldText,
        string Note);

    private enum DragTargetKind
    {
        None,
        AValue,
        ASupport,
        BValue,
        BSupport,
    }
}
