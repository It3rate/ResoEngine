using Core2.Elements;
using Core2.Repetition;
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

    private readonly AxisDisplayMapper _frame = new(
        Axis.FromCoordinates((Scalar)(-4m), (Scalar)8m, Scalar.One, Scalar.One),
        "Frame");

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

    private readonly SKPaint _probePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };

    private readonly SKPaint _probeStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(80, 80, 80),
        IsAntialias = true,
    };

    private readonly SKPaint _resultPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SegmentColors.Green.Solid,
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
    private Panel? _controlsPanel;
    private NumericUpDown? _probeInput;
    private CheckBox? _animateCheck;
    private System.Windows.Forms.Timer? _timer;
    private decimal _probeValue = 11m;

    public string Title => "Boundary Repetition";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width * 0.5f;
        coords.OriginY = 295f;

        _frameRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: FrameY);
        hitTest.Register(_frameRenderer, _frame);

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
        canvas.DrawText("The same out-of-range value can wrap, reflect, or stay unresolved as tension. Drag the frame or animate the probe.", 34f, 68f, _bodyPaint);

        _frameRenderer?.Render(canvas, _frame);
        DrawProbeMarker(canvas);
        DrawCards(canvas);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawProbeMarker(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        var top = _coords.MathToPixel((float)_probeValue, FrameY + 0.75f);
        var bottom = _coords.MathToPixel((float)_probeValue, FrameY - 0.75f);
        canvas.DrawLine(top, bottom, _probeStrokePaint);
        canvas.DrawCircle(new SKPoint(top.X, (top.Y + bottom.Y) * 0.5f), 6f, _probePaint);
        canvas.DrawText($"Probe {_probeValue:0.0}", top.X + 12f, top.Y - 6f, _cardTextPaint);
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
            ("Tension", BoundaryContinuationLaw.TensionPreserving),
        };

        const float left = 42f;
        const float top = 320f;
        const float gap = 18f;
        float cardWidth = (_coords.Width - left * 2f - gap * 2f) / 3f;
        const float cardHeight = 230f;

        for (int i = 0; i < cards.Length; i++)
        {
            var rect = new SKRect(
                left + i * (cardWidth + gap),
                top,
                left + i * (cardWidth + gap) + cardWidth,
                top + cardHeight);

            DrawContinuationCard(canvas, rect, cards[i].Item1, cards[i].Item2);
        }
    }

    private void DrawContinuationCard(SKCanvas canvas, SKRect rect, string title, BoundaryContinuationLaw law)
    {
        var result = _frame.Axis.Continue((Scalar)_probeValue, law);
        decimal frameStart = _frame.Axis.Start.Value;
        decimal frameEnd = _frame.Axis.End.Value;
        decimal min = Math.Min(Math.Min(frameStart, frameEnd), Math.Min(_probeValue, result.Value)) - 1m;
        decimal max = Math.Max(Math.Max(frameStart, frameEnd), Math.Max(_probeValue, result.Value)) + 1m;

        canvas.DrawRoundRect(rect, 14f, 14f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _cardBorderPaint);
        canvas.DrawText(title, rect.MidX, rect.Top + 24f, _cardTitlePaint);

        float midY = rect.Top + 92f;
        float left = rect.Left + 24f;
        float right = rect.Right - 24f;
        canvas.DrawLine(left, midY, right, midY, _guidePaint);

        float frameLeft = Map(frameStart, min, max, left, right);
        float frameRight = Map(frameEnd, min, max, left, right);
        canvas.DrawLine(frameLeft, midY, frameRight, midY, _framePaint);

        float zeroX = Map(0m, min, max, left, right);
        canvas.DrawLine(zeroX, midY - 18f, zeroX, midY + 18f, _baselinePaint);

        float probeX = Map(_probeValue, min, max, left, right);
        using var originalFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255),
            IsAntialias = true,
        };
        canvas.DrawCircle(probeX, midY, 7f, originalFill);
        canvas.DrawCircle(probeX, midY, 7f, _probeStrokePaint);

        float resultX = Map(result.Value, min, max, left, right);
        var resultPaint = result.HasTension ? _tensionPaint : _resultPaint;
        canvas.DrawCircle(resultX, midY, 7f, resultPaint);

        string resultText = result.HasTension
            ? $"preserved -> {result.Value:0.0}"
            : $"mapped -> {result.Value:0.0}";

        canvas.DrawText(resultText, rect.MidX, rect.Top + 146f, _cardTextPaint);

        if (result.HasTension)
        {
            canvas.DrawText("boundary exceeded; kept as tension", rect.MidX, rect.Top + 171f, _cardTextPaint);
        }
        else if (law == BoundaryContinuationLaw.PeriodicWrap)
        {
            canvas.DrawText("periodic continuation", rect.MidX, rect.Top + 171f, _cardTextPaint);
        }
        else
        {
            canvas.DrawText("reflective continuation", rect.MidX, rect.Top + 171f, _cardTextPaint);
        }

        canvas.DrawText($"frame [{frameStart:0.0}, {frameEnd:0.0}]", rect.MidX, rect.Bottom - 18f, _cardTextPaint);
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
            Size = new Size(206, 98),
            Location = new Point(650, 24),
        };

        var probeLabel = new Label
        {
            Text = "Probe Value",
            AutoSize = true,
            Location = new Point(12, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _probeInput = new NumericUpDown
        {
            DecimalPlaces = 1,
            Increment = 0.5m,
            Minimum = -20m,
            Maximum = 20m,
            Value = _probeValue,
            Width = 86,
            Location = new Point(12, 34),
        };
        _probeInput.ValueChanged += (_, _) =>
        {
            _probeValue = _probeInput.Value;
            _canvasHost?.InvalidateCanvas();
        };

        _animateCheck = new CheckBox
        {
            Text = "Animate Sweep",
            AutoSize = true,
            Location = new Point(110, 35),
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
            Text = "The cards below show the same probe under different continuation laws.",
            AutoSize = false,
            Location = new Point(12, 62),
            Size = new Size(182, 24),
            Font = new Font("Arial", 8f, FontStyle.Regular),
            ForeColor = Color.FromArgb(105, 105, 105),
        };

        _controlsPanel.Controls.Add(probeLabel);
        _controlsPanel.Controls.Add(_probeInput);
        _controlsPanel.Controls.Add(_animateCheck);
        _controlsPanel.Controls.Add(note);
        _canvasHost.Controls.Add(_controlsPanel);
        _controlsPanel.BringToFront();
    }

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
            _probeValue += 0.2m;
            if (_probeValue > 14m)
            {
                _probeValue = -8m;
            }

            if (_probeInput != null)
            {
                _probeInput.Value = decimal.Clamp(_probeValue, _probeInput.Minimum, _probeInput.Maximum);
            }

            _canvasHost.InvalidateCanvas();
        };
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

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_frame];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0f, 0f);

    public void Destroy()
    {
        _frameRenderer?.Dispose();
        _frameRenderer = null;

        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        if (_controlsPanel != null && _canvasHost != null)
        {
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
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _cardTitlePaint.Dispose();
        _cardTextPaint.Dispose();
        _baselinePaint.Dispose();
        _framePaint.Dispose();
        _probePaint.Dispose();
        _probeStrokePaint.Dispose();
        _resultPaint.Dispose();
        _tensionPaint.Dispose();
        _guidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
