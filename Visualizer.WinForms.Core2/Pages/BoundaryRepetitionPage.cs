using Core2.Elements;
using Core2.Repetition;
using Core2.Symbolics.Expressions;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class BoundaryRepetitionPage : IVisualizerPage
{
    private const float FrameY = 4.2f;
    private const float ProbeY = 2.0f;

    private readonly AxisDisplayMapper _frame = new(
        Axis.FromCoordinates((Scalar)(-4m), (Scalar)8m, Scalar.One, Scalar.One),
        string.Empty);
    private readonly DirectedSegment _probeRange = new(9f, 13f);

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(45, 45, 45),
        TextSize = 23f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(92, 92, 92),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _graphFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(249, 249, 250),
        IsAntialias = true,
    };

    private readonly SKPaint _graphBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(206, 206, 206),
        IsAntialias = true,
    };

    private readonly SKPaint _topTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(24, 38, 94),
        IsAntialias = true,
    };

    private readonly SKPaint _bottomTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(80, 30, 112),
        IsAntialias = true,
    };

    private readonly SKPaint _topTickTextPaint = new()
    {
        Color = new SKColor(128, 128, 128),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _bottomTickTextPaint = new()
    {
        Color = new SKColor(128, 128, 128),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _cardFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(247, 247, 247),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(188, 188, 188),
        IsAntialias = true,
    };

    private readonly SKPaint _cardTitlePaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _cardTextPaint = new()
    {
        Color = new SKColor(82, 82, 82),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _symbolicTextPaint = new()
    {
        Color = new SKColor(64, 64, 64),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _baselinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(185, 185, 185),
        IsAntialias = true,
    };

    private readonly SKPaint _framePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 4f,
        Color = SegmentColors.Red.Solid,
        IsAntialias = true,
    };

    private readonly SKPaint _sweepLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 3f,
        Color = new SKColor(120, 120, 120),
        IsAntialias = true,
    };

    private readonly SKPaint _sweepStartPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SegmentColors.Purple.Solid,
        IsAntialias = true,
    };

    private readonly SKPaint _sweepEndPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SegmentColors.Green.Solid,
        IsAntialias = true,
    };

    private readonly SKPaint _mappedLinkPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(96, 96, 96),
        IsAntialias = true,
    };

    private readonly SKPaint _tensionPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(219, 125, 64),
        IsAntialias = true,
    };

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        PathEffect = SKPathEffect.CreateDash([6f, 4f], 0f),
        IsAntialias = true,
    };

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

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private SegmentRenderer? _frameRenderer;
    private SegmentRenderer? _probeRenderer;
    private Panel? _controlsPanel;
    private NumericUpDown? _probeStartInput;
    private NumericUpDown? _probeEndInput;
    private CheckBox? _animateCheck;
    private System.Windows.Forms.Timer? _timer;
    private decimal _probeValue = 11m;
    private float _animationPhase;
    private bool _syncingControls;

    public string Title => "Boundary Repetition";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width * 0.5f;
        coords.OriginY = 370f;

        _frameRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: FrameY);
        _probeRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Green, crossPosition: ProbeY);
        hitTest.Register(_frameRenderer, _frame);
        hitTest.Register(_probeRenderer, _probeRange);

        EnsureControls();
        EnsureTimer();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawText("Boundary Repetition", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "The same out-of-range value can wrap, reflect, clamp, or stay unresolved as tension.\n" +
            "This demonstrates the various effects on both the start and end points of a segment.",
            34f,
            ref subtitleY,
            430f,
            _bodyPaint);

        DrawGraphFrame(canvas);
        _frameRenderer?.Render(canvas, _frame);
        SyncProbeControlsFromState();
        if (_animateCheck?.Checked != true)
        {
            _probeValue = (decimal)_probeRange.Real;
        }
        DrawProbeSweep(canvas);
        DrawCards(canvas);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawGraphFrame(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out var minValue, out var maxValue, out var outerRect);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphFillPaint);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphBorderPaint);

        float top = _coords.MathToPixel(0f, FrameY + 0.9f).Y;
        float bottom = _coords.MathToPixel(0f, -0.9f).Y;
        float axisY = _coords.MathToPixel(0f, 0f).Y;
        PageChrome.DrawRuler(
            canvas,
            _coords,
            minValue,
            maxValue,
            axisY,
            top,
            bottom,
            _baselinePaint,
            _guidePaint,
            _topTickPaint,
            _bottomTickPaint,
            _topTickTextPaint,
            _bottomTickTextPaint);

        DrawSymbolicSummary(canvas, outerRect);
    }

    private void DrawProbeSweep(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        var start = _coords.MathToPixel(_probeRange.Imaginary, ProbeY);
        var end = _coords.MathToPixel(_probeRange.Real, ProbeY);
        float span = MathF.Abs(end.X - start.X);
        if (span < 1f)
        {
            canvas.DrawCircle(start, 6f, _sweepStartPaint);
            canvas.DrawLine(end.X, end.Y - 10f, end.X, end.Y + 10f, _sweepEndPaint);
            return;
        }

        float direction = end.X >= start.X ? 1f : -1f;
        float arrowLen = 12f;
        float lineEndX = end.X - direction * arrowLen;

        canvas.DrawLine(start.X, start.Y, lineEndX, start.Y, _sweepLinePaint);
        canvas.DrawCircle(start, 6f, _sweepStartPaint);

        using var path = new SKPath();
        path.MoveTo(end.X, end.Y);
        path.LineTo(end.X - direction * 12f, end.Y - 6f);
        path.LineTo(end.X - direction * 12f, end.Y + 6f);
        path.Close();
        canvas.DrawPath(path, _sweepEndPaint);
    }

    private void DrawCards(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        var cards = new[]
        {
            ("Wrap", BoundaryContinuationLaw.PeriodicWrap),
            ("Reflect", BoundaryContinuationLaw.ReflectiveBounce),
            ("Clamp", BoundaryContinuationLaw.Clamp),
            ("Tension", BoundaryContinuationLaw.TensionPreserving),
        };

        const float left = 42f;
        const float top = 448f;
        const float gap = 18f;
        float cardWidth = (_coords.Width - left * 2f - gap) / 2f;
        const float cardHeight = 146f;

        for (int i = 0; i < cards.Length; i++)
        {
            int row = i / 2;
            int col = i % 2;
            var rect = new SKRect(
                left + col * (cardWidth + gap),
                top + row * (cardHeight + gap),
                left + col * (cardWidth + gap) + cardWidth,
                top + row * (cardHeight + gap) + cardHeight);

            DrawContinuationCard(canvas, rect, cards[i].Item1, cards[i].Item2);
        }
    }

    private void DrawSymbolicSummary(SKCanvas canvas, SKRect outerRect)
    {
        float y = outerRect.Top + 24f;
        foreach (var line in BuildSymbolicLines())
        {
            PageChrome.DrawWrappedText(
                canvas,
                line,
                outerRect.Left + 18f,
                ref y,
                530f,
                _symbolicTextPaint);
        }
    }

    private IReadOnlyList<string> BuildSymbolicLines()
    {
        var frame = new ElementLiteralTerm(_frame.Axis);
        var startValue = CreateProbeValue(_probeRange.Imaginary);
        var endValue = CreateProbeValue(_probeRange.Real);

        return
        [
            $"frame => {SymbolicTermFormatter.Format(frame)}",
            $"start => {SymbolicTermFormatter.Format(startValue)}",
            $"end => {SymbolicTermFormatter.Format(endValue)}",
            BuildContinuationLine("wrap", frame, startValue, endValue, BoundaryContinuationLaw.PeriodicWrap),
            BuildContinuationLine("reflect", frame, startValue, endValue, BoundaryContinuationLaw.ReflectiveBounce),
            BuildContinuationLine("clamp", frame, startValue, endValue, BoundaryContinuationLaw.Clamp),
            BuildContinuationLine("tension", frame, startValue, endValue, BoundaryContinuationLaw.TensionPreserving),
        ];
    }

    private string BuildContinuationLine(
        string name,
        ElementLiteralTerm frame,
        ElementLiteralTerm startValue,
        ElementLiteralTerm endValue,
        BoundaryContinuationLaw law)
    {
        string start = EvaluateContinuation(frame, startValue, law, out bool startTension);
        string end = EvaluateContinuation(frame, endValue, law, out bool endTension);
        return $"{name} => start {start}{(startTension ? " [t]" : string.Empty)}, end {end}{(endTension ? " [t]" : string.Empty)}";
    }

    private string EvaluateContinuation(
        ElementLiteralTerm frame,
        ElementLiteralTerm value,
        BoundaryContinuationLaw law,
        out bool hasTension)
    {
        var proportion = (Proportion)value.Value;
        var result = _frame.Axis.Continue(proportion, law);
        hasTension = result.HasTension;

        var reduced = SymbolicReducer.Reduce(new ContinueTerm(frame, value, law));
        return reduced.Output is null ? "(none)" : SymbolicTermFormatter.Format(reduced.Output);
    }

    private static ElementLiteralTerm CreateProbeValue(float value)
    {
        decimal rounded = decimal.Round((decimal)value, 1, MidpointRounding.AwayFromZero);
        return new ElementLiteralTerm(((Scalar)rounded).AsProportion(10));
    }

    private void DrawContinuationCard(SKCanvas canvas, SKRect rect, string title, BoundaryContinuationLaw law)
    {
        decimal frameStart = _frame.Axis.Start.Value;
        decimal frameEnd = _frame.Axis.End.Value;
        decimal probeStart = (decimal)_probeRange.Imaginary;
        decimal probeEnd = (decimal)_probeRange.Real;
        decimal minFrame = Math.Min(frameStart, frameEnd);
        decimal maxFrame = Math.Max(frameStart, frameEnd);
        var startResult = _frame.Axis.Continue((Scalar)probeStart, law);
        var endResult = _frame.Axis.Continue((Scalar)probeEnd, law);

        canvas.DrawRoundRect(rect, 14f, 14f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _cardBorderPaint);
        canvas.DrawText(title, rect.MidX, rect.Top + 24f, _cardTitlePaint);

        float midY = rect.Top + 58f;
        float left = rect.Left + 24f;
        float right = rect.Right - 24f;
        canvas.DrawLine(left, midY, right, midY, _guidePaint);

        float frameLeft = left;
        float frameRight = right;
        float direction = frameRight >= frameLeft ? 1f : -1f;
        float arrowLen = 12f;
        float lineEndX = frameRight - direction * arrowLen;
        canvas.DrawLine(frameLeft, midY, lineEndX, midY, _framePaint);
        canvas.DrawCircle(frameLeft, midY, 5f, _framePaint);
        using (var framePath = new SKPath())
        {
            framePath.MoveTo(frameRight, midY);
            framePath.LineTo(frameRight - direction * 12f, midY - 6f);
            framePath.LineTo(frameRight - direction * 12f, midY + 6f);
            framePath.Close();
            canvas.DrawPath(framePath, _framePaint);
        }

        if (0m >= minFrame && 0m <= maxFrame)
        {
            float zeroRatio = (float)((0m - minFrame) / (maxFrame - minFrame));
            float zeroX = frameLeft + zeroRatio * (frameRight - frameLeft);
            canvas.DrawLine(zeroX, midY - 18f, zeroX, midY + 18f, _baselinePaint);
        }

        decimal displayedStart = startResult.HasTension ? decimal.Clamp(probeStart, minFrame, maxFrame) : startResult.Value.Fold().Value;
        decimal displayedEnd = endResult.HasTension ? decimal.Clamp(probeEnd, minFrame, maxFrame) : endResult.Value.Fold().Value;

        float startX = Map(displayedStart, minFrame, maxFrame, frameLeft, frameRight);
        float endX = Map(displayedEnd, minFrame, maxFrame, frameLeft, frameRight);

        using var startPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = startResult.HasTension ? _tensionPaint.Color : _sweepStartPaint.Color,
            IsAntialias = true,
        };
        using var endPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = endResult.HasTension ? _tensionPaint.Color : _sweepEndPaint.Color,
            IsAntialias = true,
        };

        float span = MathF.Abs(endX - startX);
        if (span < 1f)
        {
            canvas.DrawCircle(startX, midY, 7f, startPaint);
            canvas.DrawLine(endX, midY - 10f, endX, midY + 10f, endPaint);
        }
        else
        {
            canvas.DrawCircle(startX, midY, 7f, startPaint);
            float mappedDirection = endX >= startX ? 1f : -1f;
            float mappedArrowLen = 14f;
            float mappedLineEndX = endX - mappedDirection * mappedArrowLen;
            canvas.DrawLine(startX, midY, mappedLineEndX, midY, _mappedLinkPaint);
            using var mappedPath = new SKPath();
            mappedPath.MoveTo(endX, midY);
            mappedPath.LineTo(endX - mappedDirection * 14f, midY - 8f);
            mappedPath.LineTo(endX - mappedDirection * 14f, midY + 8f);
            mappedPath.Close();
            canvas.DrawPath(mappedPath, endPaint);
        }

        canvas.DrawText(
            $"start -> {startResult.Value.Fold().Value:0.0}    end -> {endResult.Value.Fold().Value:0.0}",
            rect.MidX,
            rect.Top + 98f,
            _cardTextPaint);

        if (law == BoundaryContinuationLaw.TensionPreserving)
        {
            canvas.DrawText("outside values stay as tension", rect.MidX, rect.Top + 118f, _cardTextPaint);
        }
        else if (law == BoundaryContinuationLaw.Clamp)
        {
            canvas.DrawText("values clamp to the nearest edge", rect.MidX, rect.Top + 118f, _cardTextPaint);
        }
        else if (law == BoundaryContinuationLaw.PeriodicWrap)
        {
            canvas.DrawText("values wrap through the frame period", rect.MidX, rect.Top + 118f, _cardTextPaint);
        }
        else
        {
            canvas.DrawText("values reflect from each boundary", rect.MidX, rect.Top + 118f, _cardTextPaint);
        }

        canvas.DrawText($"frame [{frameStart:0.0}, {frameEnd:0.0}]", rect.MidX, rect.Bottom - 12f, _cardTextPaint);
    }

    private void GetGraphBounds(out decimal minValue, out decimal maxValue, out SKRect outerRect)
    {
        if (_coords == null)
        {
            minValue = 0m;
            maxValue = 1m;
            outerRect = SKRect.Empty;
            return;
        }

        minValue = Math.Min(Math.Min(Math.Min(_frame.Axis.Start.Value, _frame.Axis.End.Value), (decimal)_probeRange.Imaginary), Math.Min((decimal)_probeRange.Real, _probeValue));
        maxValue = Math.Max(Math.Max(Math.Max(_frame.Axis.Start.Value, _frame.Axis.End.Value), (decimal)_probeRange.Imaginary), Math.Max((decimal)_probeRange.Real, _probeValue));
        minValue = decimal.Floor(minValue) - 1m;
        maxValue = decimal.Ceiling(maxValue) + 1m;

        decimal desiredSpan = 24m;
        decimal currentSpan = maxValue - minValue;
        if (currentSpan < desiredSpan)
        {
            minValue = -12m;
            maxValue = 12m;
        }

        float left = _coords.MathToPixel((float)minValue, 0f).X;
        float right = _coords.MathToPixel((float)maxValue, 0f).X;
        float top = _coords.MathToPixel(0f, FrameY + 0.95f).Y;
        float bottom = _coords.MathToPixel(0f, -1.0f).Y;
        outerRect = new SKRect(_coords.Width * 0.05f, top - 18f, _coords.Width * 0.95f, bottom + 22f);
    }

    private void EnsureControls()
    {
        if (_canvasHost == null || _controlsPanel != null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            BackColor = Color.FromArgb(248, 248, 248),
            Size = new Size(320, 114),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

        var startLabel = new Label
        {
            Text = "Start Probe",
            AutoSize = true,
            Location = new Point(12, 12),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
        };

        _probeStartInput = new NumericUpDown
        {
            DecimalPlaces = 1,
            Increment = 0.5m,
            Minimum = -20m,
            Maximum = 20m,
            Value = (decimal)_probeRange.Imaginary,
            Width = 94,
            Location = new Point(12, 34),
        };
        _probeStartInput.ValueChanged += (_, _) =>
        {
            if (_syncingControls)
            {
                return;
            }

            _probeRange.Imaginary = (float)_probeStartInput.Value;
            ClampProbeValueToRange();
            _canvasHost?.InvalidateCanvas();
        };

        var endLabel = new Label
        {
            Text = "End Probe",
            AutoSize = true,
            Location = new Point(126, 12),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
        };

        _probeEndInput = new NumericUpDown
        {
            DecimalPlaces = 1,
            Increment = 0.5m,
            Minimum = -20m,
            Maximum = 20m,
            Value = (decimal)_probeRange.Real,
            Width = 94,
            Location = new Point(126, 34),
        };
        _probeEndInput.ValueChanged += (_, _) =>
        {
            if (_syncingControls)
            {
                return;
            }

            _probeRange.Real = (float)_probeEndInput.Value;
            ClampProbeValueToRange();
            _canvasHost?.InvalidateCanvas();
        };

        _animateCheck = new CheckBox
        {
            Text = "Animate Sweep",
            AutoSize = true,
            Location = new Point(12, 64),
        };
        _animateCheck.CheckedChanged += (_, _) =>
        {
            if (_timer != null)
            {
                _timer.Enabled = _animateCheck.Checked;
            }
        };

        var note = new Label
        {
            Text = "The cards below show the current probe under different continuation laws.",
            AutoSize = false,
            Location = new Point(12, 84),
            Size = new Size(292, 24),
            Font = new Font(VisualStyle.UiFontFamily, 8f, FontStyle.Regular),
            ForeColor = Color.FromArgb(105, 105, 105),
        };

        _controlsPanel.Controls.Add(startLabel);
        _controlsPanel.Controls.Add(_probeStartInput);
        _controlsPanel.Controls.Add(endLabel);
        _controlsPanel.Controls.Add(_probeEndInput);
        _controlsPanel.Controls.Add(_animateCheck);
        _controlsPanel.Controls.Add(note);
        _canvasHost.Controls.Add(_controlsPanel);
        _controlsPanel.BringToFront();
        _canvasHost.Resize += CanvasHostOnResize;
    }

    private void CanvasHostOnResize(object? sender, EventArgs e) =>
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

    private void EnsureTimer()
    {
        if (_timer != null || _canvasHost == null)
        {
            return;
        }

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 40,
            Enabled = false,
        };

        _timer.Tick += (_, _) =>
        {
            _animationPhase += 0.035f;
            decimal start = (decimal)_probeRange.Imaginary;
            decimal end = (decimal)_probeRange.Real;
            decimal center = (start + end) * 0.5m;
            decimal halfSpan = (end - start) * 0.5m;
            _probeValue = center + halfSpan * (decimal)Math.Sin(_animationPhase);

            _canvasHost.InvalidateCanvas();
        };
    }

    private void SyncProbeControlsFromState()
    {
        if (_probeStartInput == null || _probeEndInput == null)
        {
            return;
        }

        _syncingControls = true;
        try
        {
            decimal start = decimal.Clamp((decimal)_probeRange.Imaginary, _probeStartInput.Minimum, _probeStartInput.Maximum);
            decimal end = decimal.Clamp((decimal)_probeRange.Real, _probeEndInput.Minimum, _probeEndInput.Maximum);

            if (_probeStartInput.Value != start)
            {
                _probeStartInput.Value = start;
            }

            if (_probeEndInput.Value != end)
            {
                _probeEndInput.Value = end;
            }
        }
        finally
        {
            _syncingControls = false;
        }
    }

    private void ClampProbeValueToRange()
    {
        decimal min = Math.Min((decimal)_probeRange.Imaginary, (decimal)_probeRange.Real);
        decimal max = Math.Max((decimal)_probeRange.Imaginary, (decimal)_probeRange.Real);
        _probeValue = decimal.Clamp(_probeValue, min, max);
    }

    private static float Map(decimal value, decimal min, decimal max, float left, float right)
    {
        if (max <= min)
        {
            return left;
        }

        return left + (float)((value - min) / (max - min)) * (right - left);
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        return SKPoint.Distance(pixelPoint, _coords.MathToPixel(0f, 0f)) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_frame, _probeRange];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0f, 0f);

    public void Destroy()
    {
        _frameRenderer?.Dispose();
        _frameRenderer = null;
        _probeRenderer?.Dispose();
        _probeRenderer = null;

        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        if (_controlsPanel != null && _canvasHost != null)
        {
            _canvasHost.Resize -= CanvasHostOnResize;
            _canvasHost.Controls.Remove(_controlsPanel);
            _controlsPanel.Dispose();
            _controlsPanel = null;
        }

        _coords = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _graphFillPaint.Dispose();
        _graphBorderPaint.Dispose();
        _topTickPaint.Dispose();
        _bottomTickPaint.Dispose();
        _topTickTextPaint.Dispose();
        _bottomTickTextPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _cardTitlePaint.Dispose();
        _cardTextPaint.Dispose();
        _baselinePaint.Dispose();
        _framePaint.Dispose();
        _sweepLinePaint.Dispose();
        _sweepStartPaint.Dispose();
        _sweepEndPaint.Dispose();
        _mappedLinkPaint.Dispose();
        _tensionPaint.Dispose();
        _guidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
        _symbolicTextPaint.Dispose();
    }
}
