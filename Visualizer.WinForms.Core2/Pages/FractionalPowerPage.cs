using Core2.Elements;
using Core2.Repetition;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class FractionalPowerPage : IVisualizerPage
{
    private const float InputY = 4.2f;
    private static readonly float[] CandidateRows = [1.5f, -0.9f, -3.3f];

    private readonly AxisDisplayMapper _input = new(
        Axis.FromCoordinates((Scalar)(-0.5m), (Scalar)1m, Scalar.One, Scalar.One),
        string.Empty);

    private readonly AxisDisplayMapper[] _candidateDisplays =
    [
        new AxisDisplayMapper(Axis.Zero, string.Empty),
        new AxisDisplayMapper(Axis.Zero, string.Empty),
        new AxisDisplayMapper(Axis.Zero, string.Empty),
    ];

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(45, 45, 45),
        TextSize = 23f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(90, 90, 90),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _sectionPaint = new()
    {
        Color = new SKColor(72, 72, 72),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _resultBadgePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(248, 248, 248),
        IsAntialias = true,
    };

    private readonly SKPaint _resultBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(196, 196, 196),
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

    private readonly SKPaint _rulerLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.25f,
        Color = new SKColor(172, 172, 172),
        IsAntialias = true,
    };

    private readonly SKPaint _zeroAxisPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.5f,
        Color = new SKColor(138, 138, 138),
        IsAntialias = true,
    };

    private readonly SKPaint _topTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.25f,
        Color = new SKColor(24, 38, 94),
        IsAntialias = true,
    };

    private readonly SKPaint _bottomTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.25f,
        Color = new SKColor(80, 30, 112),
        IsAntialias = true,
    };

    private readonly SKPaint _topTickTextPaint = new()
    {
        Color = new SKColor(24, 38, 94),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _bottomTickTextPaint = new()
    {
        Color = new SKColor(80, 30, 112),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
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
    private SegmentRenderer? _inputRenderer;
    private SegmentRenderer[]? _candidateRenderers;

    private Panel? _controlsPanel;
    private ComboBox? _exponentCombo;
    private ComboBox? _branchCombo;
    private CheckBox? _animateCheck;
    private System.Windows.Forms.Timer? _timer;

    private float _phase;
    private Axis? _branchReference;

    public string Title => "Fractional Power";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width * 0.73f;
        coords.OriginY = 552f;

        _inputRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: InputY);
        _candidateRenderers =
        [
            new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Green, crossPosition: CandidateRows[0]),
            new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Orange, crossPosition: CandidateRows[1]),
            new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Purple, crossPosition: CandidateRows[2]),
        ];

        hitTest.Register(_inputRenderer, _input);
        EnsureControls();
        EnsureTimer();
        _canvasHost.InvalidateCanvas();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawText("Fractional Power / Branch", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        DrawWrappedText(
            canvas,
            "Drag z or animate an orbit. The candidates below come from inverse continuation, with one selected as the principal branch.",
            34f,
            ref subtitleY,
            390f,
            _bodyPaint);

        var exponent = SelectedExponent;
        var branchRule = SelectedBranchRule;
        Axis? reference = branchRule == InverseContinuationRule.NearestToReference ? _branchReference : null;
        var result = _input.Axis.TryPow(exponent, branchRule, reference);
        UpdateCandidateDisplays(result);

        DrawResultBadge(canvas, exponent, branchRule, result);
        DrawGraphFrame(canvas, result);
        _inputRenderer?.Render(canvas, _input);
        DrawRowBadge(canvas, "Input z", InputY, SegmentColors.Red);

        if (_candidateRenderers != null)
        {
            for (int i = 0; i < _candidateRenderers.Length; i++)
            {
                _candidateRenderers[i].Render(canvas, _candidateDisplays[i]);
                DrawRowBadge(
                    canvas,
                    i switch
                    {
                        0 => "Principal",
                        1 => "Alt 1",
                        _ => "Alt 2",
                    },
                    CandidateRows[i],
                    i switch
                    {
                        0 => SegmentColors.Green,
                        1 => SegmentColors.Orange,
                        _ => SegmentColors.Purple,
                    });
            }
        }

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void UpdateCandidateDisplays(PowerResult<Axis> result)
    {
        List<Axis> ordered = [];
        if (result.Succeeded && result.PrincipalCandidate is not null)
        {
            ordered.Add(result.PrincipalCandidate);
            ordered.AddRange(result.Candidates.Where(candidate => candidate != result.PrincipalCandidate));
            _branchReference = result.PrincipalCandidate;
        }

        for (int i = 0; i < _candidateDisplays.Length; i++)
        {
            if (i < ordered.Count)
            {
                _candidateDisplays[i].SetAxis(ordered[i]);
                _candidateDisplays[i].Label = string.Empty;
            }
            else
            {
                _candidateDisplays[i].SetAxis(Axis.Zero);
                _candidateDisplays[i].Label = string.Empty;
            }
        }
    }

    private void DrawResultBadge(SKCanvas canvas, Proportion exponent, InverseContinuationRule branchRule, PowerResult<Axis> result)
    {
        if (_coords == null)
        {
            return;
        }

        string branchText = branchRule switch
        {
            InverseContinuationRule.Principal => "principal",
            InverseContinuationRule.PreferPositiveDominant => "prefer +dominant",
            InverseContinuationRule.NearestToReference => "nearest to previous",
            _ => branchRule.ToString(),
        };

        string summary = result.Succeeded && result.PrincipalCandidate is not null
            ? $"z^{exponent}  ->  {FormatAxis(result.PrincipalCandidate)}   ({result.Candidates.Count} candidate{(result.Candidates.Count == 1 ? string.Empty : "s")}, {branchText})"
            : $"z^{exponent}  ->  no candidate ({branchText})";

        var bounds = new SKRect();
        _sectionPaint.MeasureText(summary, ref bounds);
        var rect = new SKRect(36f, 104f, 36f + bounds.Width + 28f, 136f);
        canvas.DrawRoundRect(rect, 12f, 12f, _resultBadgePaint);
        canvas.DrawRoundRect(rect, 12f, 12f, _resultBorderPaint);
        canvas.DrawText(summary, rect.Left + 14f, rect.MidY + 5f, _sectionPaint);
    }

    private void DrawGraphFrame(SKCanvas canvas, PowerResult<Axis> result)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(result, out var minValue, out var maxValue, out var outerRect, out var innerLeft, out var innerRight);

        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphFillPaint);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphBorderPaint);

        float top = _coords.MathToPixel(0f, InputY + 0.9f).Y;
        float bottom = _coords.MathToPixel(0f, CandidateRows[^1] - 0.9f).Y;
        float axisY = _coords.MathToPixel(0f, 0f).Y;
        float zeroX = _coords.MathToPixel(0f, 0f).X;

        canvas.DrawLine(zeroX, top, zeroX, bottom, _zeroAxisPaint);
        canvas.DrawLine(innerLeft, axisY, innerRight, axisY, _rulerLinePaint);

        int start = (int)decimal.Floor(minValue);
        int end = (int)decimal.Ceiling(maxValue);
        for (int tick = start; tick <= end; tick++)
        {
            float x = _coords.MathToPixel(tick, 0f).X;
            canvas.DrawLine(x, axisY - 8f, x, axisY, _topTickPaint);
            canvas.DrawLine(x, axisY, x, axisY + 8f, _bottomTickPaint);

            if (tick % 2 == 0)
            {
                canvas.DrawText(FormatRealTick(tick), x, axisY - 12f, _topTickTextPaint);
                canvas.DrawText(FormatImaginaryTick(tick), x, axisY + 22f, _bottomTickTextPaint);
            }
        }
    }

    private void DrawRowBadge(SKCanvas canvas, string text, float y, SegmentColorSet colors)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(
            _input.Axis.TryPow(SelectedExponent, SelectedBranchRule, SelectedBranchRule == InverseContinuationRule.NearestToReference ? _branchReference : null),
            out _,
            out _,
            out var outerRect,
            out _,
            out _);

        var center = _coords.MathToPixel(0f, y);
        var bounds = new SKRect();
        _sectionPaint.MeasureText(text, ref bounds);

        float width = Math.Max(84f, bounds.Width + 26f);
        var rect = new SKRect(
            outerRect.Left + 14f,
            center.Y - 16f,
            outerRect.Left + 14f + width,
            center.Y + 16f);

        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(colors.Grid.Red, colors.Grid.Green, colors.Grid.Blue, 70),
            IsAntialias = true,
        };
        using var border = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = colors.Solid,
            IsAntialias = true,
        };
        using var textPaint = new SKPaint
        {
            Color = colors.Solid,
            TextSize = 15f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 12f, 12f, fill);
        canvas.DrawRoundRect(rect, 12f, 12f, border);
        canvas.DrawText(text, rect.MidX, rect.MidY + 5f, textPaint);
    }

    private void GetGraphBounds(
        PowerResult<Axis> result,
        out decimal minValue,
        out decimal maxValue,
        out SKRect outerRect,
        out float innerLeft,
        out float innerRight)
    {
        if (_coords == null)
        {
            minValue = 0m;
            maxValue = 1m;
            outerRect = SKRect.Empty;
            innerLeft = 0f;
            innerRight = 0f;
            return;
        }

        var axes = new List<Axis> { _input.Axis };
        axes.AddRange(result.Candidates.Take(_candidateDisplays.Length));

        minValue = 0m;
        maxValue = 0m;
        foreach (var axis in axes)
        {
            minValue = Math.Min(minValue, Math.Min(axis.Start.Value, axis.End.Value));
            maxValue = Math.Max(maxValue, Math.Max(axis.Start.Value, axis.End.Value));
        }

        minValue = decimal.Floor(minValue) - 1m;
        maxValue = decimal.Ceiling(maxValue) + 1m;

        innerLeft = _coords.MathToPixel((float)minValue, 0f).X;
        innerRight = _coords.MathToPixel((float)maxValue, 0f).X;
        float top = _coords.MathToPixel(0f, InputY + 0.95f).Y;
        float bottom = _coords.MathToPixel(0f, CandidateRows[^1] - 0.95f).Y;
        outerRect = new SKRect(innerLeft - 104f, top - 18f, innerRight + 28f, bottom + 20f);
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
            Size = new Size(242, 126),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        PositionControlsPanel();

        var exponentLabel = new Label
        {
            Text = "Exponent",
            AutoSize = true,
            Location = new Point(10, 10),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _exponentCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(10, 30),
            Width = 90,
        };
        _exponentCombo.Items.AddRange(["1/2", "1/3", "2/3", "3/2"]);
        _exponentCombo.SelectedIndex = 0;
        _exponentCombo.SelectedIndexChanged += (_, _) =>
        {
            _branchReference = null;
            _canvasHost?.InvalidateCanvas();
        };

        var branchLabel = new Label
        {
            Text = "Branch Rule",
            AutoSize = true,
            Location = new Point(110, 10),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _branchCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(110, 30),
            Width = 100,
        };
        _branchCombo.Items.AddRange(["Principal", "Prefer +Dominant", "Nearest"]);
        _branchCombo.SelectedIndex = 0;
        _branchCombo.SelectedIndexChanged += (_, _) =>
        {
            _branchReference = null;
            _canvasHost?.InvalidateCanvas();
        };

        _animateCheck = new CheckBox
        {
            Text = "Animate Orbit",
            AutoSize = true,
            Location = new Point(10, 72),
            Font = new Font("Arial", 9f, FontStyle.Regular),
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
            Text = "Nearest follows the previous principal while the orbit moves.",
            AutoSize = false,
            Location = new Point(10, 92),
            Size = new Size(220, 28),
            Font = new Font("Arial", 8f, FontStyle.Regular),
            ForeColor = Color.FromArgb(105, 105, 105),
        };

        _controlsPanel.Controls.Add(exponentLabel);
        _controlsPanel.Controls.Add(_exponentCombo);
        _controlsPanel.Controls.Add(branchLabel);
        _controlsPanel.Controls.Add(_branchCombo);
        _controlsPanel.Controls.Add(_animateCheck);
        _controlsPanel.Controls.Add(note);
        _canvasHost.Controls.Add(_controlsPanel);
        _controlsPanel.BringToFront();
        _canvasHost.Resize += CanvasHostOnResize;
    }

    private void PositionControlsPanel()
    {
        if (_canvasHost == null || _controlsPanel == null)
        {
            return;
        }

        _controlsPanel.Location = new Point(
            Math.Max(12, _canvasHost.ClientSize.Width - _controlsPanel.Width - 18),
            18);
    }

    private void CanvasHostOnResize(object? sender, EventArgs e) => PositionControlsPanel();

    private void EnsureTimer()
    {
        if (_timer != null || _canvasHost == null)
        {
            return;
        }

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 30,
            Enabled = false,
        };
        _timer.Tick += (_, _) =>
        {
            _phase += 0.04f;
            decimal start = (decimal)(-Math.Sin(_phase));
            decimal end = (decimal)Math.Cos(_phase);
            _input.SetAxis(Axis.FromCoordinates((Scalar)start, (Scalar)end, Scalar.One, Scalar.One));
            _canvasHost.InvalidateCanvas();
        };
    }

    private Proportion SelectedExponent =>
        _exponentCombo?.SelectedItem?.ToString() switch
        {
            "1/2" => new Proportion(1, 2),
            "1/3" => new Proportion(1, 3),
            "2/3" => new Proportion(2, 3),
            "3/2" => new Proportion(3, 2),
            _ => new Proportion(1, 2),
        };

    private InverseContinuationRule SelectedBranchRule =>
        _branchCombo?.SelectedItem?.ToString() switch
        {
            "Prefer +Dominant" => InverseContinuationRule.PreferPositiveDominant,
            "Nearest" => InverseContinuationRule.NearestToReference,
            _ => InverseContinuationRule.Principal,
        };

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        return SKPoint.Distance(pixelPoint, _coords.MathToPixel(0f, 0f)) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_input];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0f, 0f);

    public void Destroy()
    {
        _inputRenderer?.Dispose();
        _inputRenderer = null;

        if (_candidateRenderers != null)
        {
            foreach (var renderer in _candidateRenderers)
            {
                renderer.Dispose();
            }

            _candidateRenderers = null;
        }

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
        _branchReference = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _sectionPaint.Dispose();
        _resultBadgePaint.Dispose();
        _resultBorderPaint.Dispose();
        _graphFillPaint.Dispose();
        _graphBorderPaint.Dispose();
        _rulerLinePaint.Dispose();
        _zeroAxisPaint.Dispose();
        _topTickPaint.Dispose();
        _bottomTickPaint.Dispose();
        _topTickTextPaint.Dispose();
        _bottomTickTextPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

    private void DrawWrappedText(SKCanvas canvas, string text, float x, ref float y, float width, SKPaint paint)
    {
        foreach (var line in WrapText(text, width, paint))
        {
            canvas.DrawText(line, x, y, paint);
            y += paint.TextSize + 5f;
        }
    }

    private static IReadOnlyList<string> WrapText(string text, float width, SKPaint paint)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = string.Empty;

        foreach (var word in words)
        {
            var candidate = string.IsNullOrEmpty(current) ? word : $"{current} {word}";
            if (paint.MeasureText(candidate) <= width)
            {
                current = candidate;
            }
            else
            {
                if (!string.IsNullOrEmpty(current))
                {
                    lines.Add(current);
                }

                current = word;
            }
        }

        if (!string.IsNullOrEmpty(current))
        {
            lines.Add(current);
        }

        return lines;
    }

    private static string FormatRealTick(int value) =>
        value > 0 ? $"+{value}" : value.ToString();

    private static string FormatImaginaryTick(int value)
    {
        int display = -value;
        string sign = display > 0 ? "+" : string.Empty;
        return $"{sign}{display}i";
    }

    private static string FormatAxis(Axis axis)
    {
        decimal recessive = axis.Recessive.Fold();
        decimal dominant = axis.Dominant.Fold();

        if (recessive == 0m)
        {
            return dominant.ToString("0.0");
        }

        if (dominant == 0m)
        {
            return $"{recessive:0.0}i";
        }

        string sign = dominant >= 0m ? "+" : "-";
        return $"{recessive:0.0}i {sign} {Math.Abs(dominant):0.0}";
    }
}
