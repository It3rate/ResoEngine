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
        string.Empty);

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
        coords.OriginY = 370f;

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
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "The same out-of-range value can wrap, reflect, or stay unresolved as tension. Drag the frame or animate the probe.",
            34f,
            ref subtitleY,
            430f,
            _bodyPaint);

        DrawGraphFrame(canvas);
        _frameRenderer?.Render(canvas, _frame);
        DrawRowBadge(canvas, "Frame", FrameY, SegmentColors.Red);
        DrawProbeMarker(canvas);
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
        const float top = 424f;
        const float gap = 18f;
        float cardWidth = (_coords.Width - left * 2f - gap * 2f) / 3f;
        const float cardHeight = 188f;

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

        float midY = rect.Top + 70f;
        float left = rect.Left + 24f;
        float right = rect.Right - 24f;
        canvas.DrawLine(left, midY, right, midY, _guidePaint);

        float frameLeft = Map(frameStart, min, max, left, right);
        float frameRight = Map(frameEnd, min, max, left, right);
        float direction = frameRight >= frameLeft ? 1f : -1f;
        float arrowLen = 12f;
        float lineEndX = frameRight - direction * arrowLen;
        canvas.DrawLine(frameLeft, midY, lineEndX, midY, _framePaint);
        canvas.DrawCircle(frameLeft, midY, 5f, _framePaint);
        using (var arrowPath = new SKPath())
        {
            arrowPath.MoveTo(frameRight, midY);
            arrowPath.LineTo(frameRight - direction * 12f, midY - 6f);
            arrowPath.LineTo(frameRight - direction * 12f, midY + 6f);
            arrowPath.Close();
            canvas.DrawPath(arrowPath, _framePaint);
        }

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

        canvas.DrawText(resultText, rect.MidX, rect.Top + 118f, _cardTextPaint);

        if (result.HasTension)
        {
            canvas.DrawText("boundary exceeded; kept as tension", rect.MidX, rect.Top + 138f, _cardTextPaint);
        }
        else if (law == BoundaryContinuationLaw.PeriodicWrap)
        {
            canvas.DrawText("periodic continuation", rect.MidX, rect.Top + 138f, _cardTextPaint);
        }
        else
        {
            canvas.DrawText("reflective continuation", rect.MidX, rect.Top + 138f, _cardTextPaint);
        }

        canvas.DrawText($"frame [{frameStart:0.0}, {frameEnd:0.0}]", rect.MidX, rect.Bottom - 14f, _cardTextPaint);
    }

    private void DrawRowBadge(SKCanvas canvas, string text, float y, SegmentColorSet colors)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out _, out _, out var outerRect);
        var center = _coords.MathToPixel(0f, y);
        var bounds = new SKRect();
        _cardTitlePaint.MeasureText(text, ref bounds);
        float width = Math.Max(82f, bounds.Width + 24f);
        var rect = new SKRect(outerRect.Left + 14f, center.Y - 16f, outerRect.Left + 14f + width, center.Y + 16f);

        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(colors.Grid.Red, colors.Grid.Green, colors.Grid.Blue, 68),
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

    private void GetGraphBounds(out decimal minValue, out decimal maxValue, out SKRect outerRect)
    {
        if (_coords == null)
        {
            minValue = 0m;
            maxValue = 1m;
            outerRect = SKRect.Empty;
            return;
        }

        minValue = Math.Min(Math.Min(_frame.Axis.Start.Value, _frame.Axis.End.Value), _probeValue);
        maxValue = Math.Max(Math.Max(_frame.Axis.Start.Value, _frame.Axis.End.Value), _probeValue);
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
            Size = new Size(206, 98),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

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
