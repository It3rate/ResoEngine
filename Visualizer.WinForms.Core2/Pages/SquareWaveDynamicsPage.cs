using Applied.Geometry.Frieze;
using Applied.Geometry.Utils;
using Core2.Dynamic;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class SquareWaveDynamicsPage : IVisualizerPage
{
    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(45, 45, 45),
        TextSize = 16f,
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
        Color = new SKColor(21, 24, 28),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(56, 63, 72),
        IsAntialias = true,
    };

    private readonly SKPaint _pathPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 4f,
        Color = new SKColor(61, 244, 223),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _pathGlowPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 9f,
        Color = new SKColor(61, 244, 223, 64),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _cardTitlePaint = new()
    {
        Color = new SKColor(244, 244, 244),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBodyPaint = new()
    {
        Color = new SKColor(186, 193, 201),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(58, 66, 76),
        PathEffect = SKPathEffect.CreateDash([6f, 6f], 0f),
        IsAntialias = true,
    };

    private SkiaCanvas? _canvasHost;
    private Panel? _controlsPanel;
    private NumericUpDown? _stepInput;
    private DynamicTrace<StripPathState, StripEnvironment, Orientation2D>? _trace;
    private int _cachedSteps = -1;

    public string Title => "Square Wave Dynamics";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureControls();
        RefreshTrace();
    }

    public void Render(SKCanvas canvas)
    {
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);

        canvas.DrawText("Square Wave Dynamics", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Three simple strands run in parallel: steady rightward drift, a vertical pulse, and a backtrack pulse that cancels drift on rise and fall steps.\n" +
            "The dynamic resolver commits the combined result as a path, preserving the whole process as executable Core 2 structure rather than a closed-form answer.",
            34f,
            ref subtitleY,
            640f,
            _bodyPaint);

        RefreshTrace();
        if (_trace is null || _trace.SelectedContext is null)
        {
            return;
        }

        var card = new SKRoundRect(new SKRect(34f, 220f, 860f, 550f), 24f, 24f);
        canvas.DrawRoundRect(card, _cardFillPaint);
        canvas.DrawRoundRect(card, _cardBorderPaint);
        canvas.DrawText("Resolved Path", 60f, 254f, _cardTitlePaint);

        DrawStripGuides(canvas, card.Rect);
        DrawResolvedPath(canvas, card.Rect, _trace.SelectedContext.State);
        DrawSummary(canvas, card.Rect, _trace);
        DrawStrandLegend(canvas, new SKRect(34f, 584f, 860f, 860f));
    }

    public void Destroy()
    {
        if (_controlsPanel is not null)
        {
            _canvasHost?.Controls.Remove(_controlsPanel);
            _controlsPanel.Dispose();
            _controlsPanel = null;
        }
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _pathPaint.Dispose();
        _pathGlowPaint.Dispose();
        _cardTitlePaint.Dispose();
        _cardBodyPaint.Dispose();
        _guidePaint.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(218, 86),
            BackColor = Color.FromArgb(248, 248, 248),
        };

        var title = new Label
        {
            Text = "Square Wave",
            Location = new Point(14, 10),
            Size = new Size(190, 18),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(62, 62, 62),
        };

        var stepLabel = new Label
        {
            Text = "Macro Steps",
            Location = new Point(14, 40),
            Size = new Size(96, 18),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(92, 92, 92),
        };

        _stepInput = new NumericUpDown
        {
            Minimum = 6,
            Maximum = 60,
            Value = 24,
            Increment = 6,
            DecimalPlaces = 0,
            Location = new Point(116, 37),
            Size = new Size(74, 24),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Regular),
        };
        _stepInput.ValueChanged += (_, _) =>
        {
            _cachedSteps = -1;
            _canvasHost?.InvalidateCanvas();
        };

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(stepLabel);
        _controlsPanel.Controls.Add(_stepInput);
        _canvasHost.Controls.Add(_controlsPanel);
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);
    }

    private void RefreshTrace()
    {
        int steps = (int)(_stepInput?.Value ?? 24m);
        if (_trace is not null && _cachedSteps == steps)
        {
            return;
        }

        _trace = SquareWaveDynamics.Run(steps);
        _cachedSteps = steps;
    }

    private void DrawStripGuides(SKCanvas canvas, SKRect rect)
    {
        float left = rect.Left + 36f;
        float right = rect.Right - 36f;
        float topLane = rect.Top + 118f;
        float bottomLane = rect.Bottom - 110f;

        canvas.DrawLine(left, topLane, right, topLane, _guidePaint);
        canvas.DrawLine(left, bottomLane, right, bottomLane, _guidePaint);
    }

    private void DrawResolvedPath(SKCanvas canvas, SKRect rect, StripPathState state)
    {
        if (state.Segments.Count == 0)
        {
            return;
        }

        int minX = state.Segments.Min(segment => Math.Min(segment.Start.X, segment.End.X));
        int maxX = state.Segments.Max(segment => Math.Max(segment.Start.X, segment.End.X));
        int minY = state.Segments.Min(segment => Math.Min(segment.Start.Y, segment.End.Y));
        int maxY = state.Segments.Max(segment => Math.Max(segment.Start.Y, segment.End.Y));

        float innerLeft = rect.Left + 54f;
        float innerRight = rect.Right - 54f;
        float innerTop = rect.Top + 110f;
        float innerBottom = rect.Bottom - 122f;

        float width = Math.Max(1, maxX - minX);
        float height = Math.Max(1, maxY - minY);
        float scale = Math.Min((innerRight - innerLeft) / width, (innerBottom - innerTop) / Math.Max(1f, height));
        float xPad = ((innerRight - innerLeft) - width * scale) * 0.5f;
        float yPad = ((innerBottom - innerTop) - Math.Max(1f, height) * scale) * 0.5f;

        SKPoint Map(StripPoint point)
        {
            float x = innerLeft + xPad + (point.X - minX) * scale;
            float y = innerBottom - yPad - (point.Y - minY) * scale;
            return new SKPoint(x, y);
        }

        using var path = new SKPath();
        bool first = true;
        foreach (var segment in state.Segments)
        {
            var start = Map(segment.Start);
            var end = Map(segment.End);
            if (first)
            {
                path.MoveTo(start);
                first = false;
            }
            else if (path.LastPoint != start)
            {
                path.MoveTo(start);
            }

            path.LineTo(end);
        }

        canvas.DrawPath(path, _pathGlowPaint);
        canvas.DrawPath(path, _pathPaint);
    }

    private void DrawSummary(
        SKCanvas canvas,
        SKRect rect,
        DynamicTrace<StripPathState, StripEnvironment, Orientation2D> trace)
    {
        string summary =
            $"steps {_cachedSteps}   ·   segments {trace.SelectedContext!.State.Segments.Count}   ·   " +
            $"cursor ({trace.SelectedContext.State.Cursor.X}, {trace.SelectedContext.State.Cursor.Y})   ·   " +
            $"graph nodes {trace.Graph.Nodes.Count}";

        canvas.DrawText(summary, rect.Left + 60f, rect.Bottom - 48f, _cardBodyPaint);
    }

    private void DrawStrandLegend(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawText("Active Strands", rect.Left, rect.Top + 18f, _headingPaint);

        float y = rect.Top + 48f;
        DrawLegendRow(canvas, y, "Advance", "+1 horizontal every step");
        y += 34f;
        DrawLegendRow(canvas, y, "Vertical", "up, pause, pause, down, pause, pause");
        y += 34f;
        DrawLegendRow(canvas, y, "Backtrack", "-1 horizontal on rise/fall pulses to create vertical edges");
    }

    private void DrawLegendRow(SKCanvas canvas, float y, string title, string detail)
    {
        canvas.DrawText(title, 34f, y, _headingPaint);
        canvas.DrawText(detail, 158f, y, _bodyPaint);
    }
}
