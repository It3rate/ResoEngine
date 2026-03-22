using Applied.Geometry.LetterFormation;
using Applied.Geometry.Utils;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;
using System.Drawing;
using System.Windows.Forms;

namespace ResoEngine.Visualizer.Pages;

public sealed class LetterFormationDynamicsPage : IVisualizerPage
{
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
        StrokeWidth = 1.4f,
        Color = new SKColor(218, 218, 218),
        IsAntialias = true,
    };

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(195, 195, 195),
        PathEffect = SKPathEffect.CreateDash([6f, 6f], 0f),
        IsAntialias = true,
    };

    private readonly SKPaint _letterboxPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(210, 210, 210),
        IsAntialias = true,
    };

    private readonly SKPaint _siteFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _siteStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(92, 92, 92),
        IsAntialias = true,
    };

    private readonly SKPaint _siteLabelPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _carrierLabelPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _infoTitlePaint = new()
    {
        Color = new SKColor(54, 54, 54),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _infoTextPaint = new()
    {
        Color = new SKColor(82, 82, 82),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _proposalPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(78, 156, 255, 170),
        IsAntialias = true,
    };

    private readonly SKPaint _proposalHeadPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(78, 156, 255, 170),
        IsAntialias = true,
    };

    private readonly SKPaint _statusAccentPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(238, 246, 255),
        IsAntialias = true,
    };

    private readonly Dictionary<string, SKColor> _carrierColors = new(StringComparer.Ordinal)
    {
        ["LeftLeg"] = SKColor.Parse("#C77000"),
        ["RightLeg"] = SKColor.Parse("#8415D0"),
        ["Crossbar"] = SKColor.Parse("#12824A"),
    };

    private SkiaCanvas? _canvasHost;
    private Panel? _controlsPanel;
    private CheckBox? _animateCheck;
    private CheckBox? _showMovesCheck;
    private Button? _stepButton;
    private Button? _resetButton;
    private System.Windows.Forms.Timer? _timer;
    private LetterFormationState? _state;
    private IReadOnlyList<LetterFormationProposal> _proposals = [];
    private int _resetCount;

    public string Title => "Letter Formation Dynamics";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureControls();
        EnsureTimer();
        ResetState();
    }

    public void Render(SKCanvas canvas)
    {
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);

        float width = _canvasHost?.ClientSize.Width ?? 1400f;
        float height = _canvasHost?.ClientSize.Height ?? 1000f;

        canvas.DrawText("Letter Formation Dynamics", 34f, 44f, _headingPaint);
        float subtitleY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "This is a fixed-topology capital A with only local desires. Sites and carriers keep proposing small moves based on their own tensions, then the system advances one immutable snapshot at a time until the remaining pulls get quiet.",
            34f,
            ref subtitleY,
            760f,
            _bodyPaint);

        if (_state is null)
        {
            return;
        }

        SKRect graphCard = new(34f, 136f, width - 350f, height - 36f);
        SKRect statusCard = new(width - 292f, 150f, width - 34f, 292f);
        SKRect tensionCard = new(width - 292f, 310f, width - 34f, 632f);
        SKRect proposalCard = new(width - 292f, 650f, width - 34f, height - 36f);

        DrawCard(canvas, graphCard);
        DrawCard(canvas, statusCard);
        DrawCard(canvas, tensionCard);
        DrawCard(canvas, proposalCard);

        DrawGraph(canvas, graphCard);
        DrawStatus(canvas, statusCard);
        DrawTopTensions(canvas, tensionCard);
        DrawTopProposals(canvas, proposalCard);
    }

    public void Destroy()
    {
        if (_controlsPanel is not null)
        {
            _canvasHost?.Controls.Remove(_controlsPanel);
            _controlsPanel.Dispose();
            _controlsPanel = null;
        }

        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardStrokePaint.Dispose();
        _guidePaint.Dispose();
        _letterboxPaint.Dispose();
        _siteFillPaint.Dispose();
        _siteStrokePaint.Dispose();
        _siteLabelPaint.Dispose();
        _carrierLabelPaint.Dispose();
        _infoTitlePaint.Dispose();
        _infoTextPaint.Dispose();
        _proposalPaint.Dispose();
        _proposalHeadPaint.Dispose();
        _statusAccentPaint.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(232, 126),
            BackColor = Color.FromArgb(248, 248, 248),
        };

        var title = new Label
        {
            Text = "Formation",
            Location = new Point(14, 10),
            Size = new Size(120, 18),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(62, 62, 62),
        };

        _animateCheck = new CheckBox
        {
            Text = "Animate",
            Checked = true,
            Location = new Point(14, 38),
            Size = new Size(92, 22),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
        };

        _showMovesCheck = new CheckBox
        {
            Text = "Show moves",
            Checked = true,
            Location = new Point(112, 38),
            Size = new Size(102, 22),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
        };
        _showMovesCheck.CheckedChanged += (_, _) =>
        {
            RefreshProposals();
            _canvasHost?.InvalidateCanvas();
        };

        _stepButton = new Button
        {
            Text = "Step",
            Location = new Point(14, 78),
            Size = new Size(92, 30),
            FlatStyle = FlatStyle.Flat,
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            BackColor = Color.White,
        };
        _stepButton.FlatAppearance.BorderColor = Color.FromArgb(214, 214, 214);
        _stepButton.Click += (_, _) => AdvanceOneStep();

        _resetButton = new Button
        {
            Text = "Reset",
            Location = new Point(112, 78),
            Size = new Size(92, 30),
            FlatStyle = FlatStyle.Flat,
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            BackColor = Color.White,
        };
        _resetButton.FlatAppearance.BorderColor = Color.FromArgb(214, 214, 214);
        _resetButton.Click += (_, _) => ResetState();

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(_animateCheck);
        _controlsPanel.Controls.Add(_showMovesCheck);
        _controlsPanel.Controls.Add(_stepButton);
        _controlsPanel.Controls.Add(_resetButton);
        _canvasHost.Controls.Add(_controlsPanel);
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);
    }

    private void EnsureTimer()
    {
        if (_timer is not null || _canvasHost is null)
        {
            return;
        }

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 90,
            Enabled = true,
        };
        _timer.Tick += (_, _) =>
        {
            if (_animateCheck?.Checked == true)
            {
                AdvanceOneStep();
            }
        };
        _timer.Start();
    }

    private void ResetState()
    {
        Random random = new(Environment.TickCount ^ (++_resetCount * 7919));
        LetterFormationEnvironment environment = LetterFormationEnvironment.CreateLetterBox(
            widthTicks: 10,
            heightTicks: 14,
            randomMotionWeight: new Core2.Elements.Proportion(1, 2));
        _state = LetterFormationTensionEvaluator.Evaluate(
            LetterFormationPresetFactory.CreateCapitalASeed(random, environment));
        RefreshProposals();
        _canvasHost?.InvalidateCanvas();
    }

    private void AdvanceOneStep()
    {
        if (_state is null)
        {
            return;
        }

        _state = LetterFormationStepper.Step(_state);
        RefreshProposals();

        if (SumTension(_state.Tensions) < 0.06d && _animateCheck is not null)
        {
            _animateCheck.Checked = false;
        }

        _canvasHost?.InvalidateCanvas();
    }

    private void RefreshProposals()
    {
        if (_state is null)
        {
            _proposals = [];
            return;
        }

        _proposals = _showMovesCheck?.Checked == true
            ? LetterFormationStepper.GenerateProposals(_state)
            : [];
    }

    private void DrawCard(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 22f, 22f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 22f, 22f, _cardStrokePaint);
    }

    private void DrawGraph(SKCanvas canvas, SKRect cardRect)
    {
        if (_state is null)
        {
            return;
        }

        canvas.DrawText("Evolving Letter", cardRect.Left + 22f, cardRect.Top + 30f, _infoTitlePaint);

        SKRect plotRect = new(cardRect.Left + 30f, cardRect.Top + 54f, cardRect.Right - 30f, cardRect.Bottom - 28f);
        SKRect letterboxRect = FitLetterbox(plotRect, _state.Environment);

        canvas.DrawRoundRect(letterboxRect, 18f, 18f, _statusAccentPaint);
        canvas.DrawRoundRect(letterboxRect, 18f, 18f, _letterboxPaint);

        float centerX = MapX(_state.Environment.CenterlineX, letterboxRect);
        float midY = MapY(_state.Environment.MidlineY, letterboxRect);
        canvas.DrawLine(centerX, letterboxRect.Top, centerX, letterboxRect.Bottom, _guidePaint);
        canvas.DrawLine(letterboxRect.Left, midY, letterboxRect.Right, midY, _guidePaint);

        if (_showMovesCheck?.Checked == true)
        {
            foreach ((string siteId, PlanarOffset offset) in AggregateSiteProposals(_proposals))
            {
                LetterFormationSiteState site = _state.GetSite(siteId);
                SKPoint start = MapPoint(site.Position, letterboxRect);
                SKPoint end = new(
                    start.X + (float)(ToDouble(offset.Horizontal) * 18d),
                    start.Y + (float)(ToDouble(offset.Vertical) * 18d));
                DrawArrow(canvas, start, end, _proposalPaint, _proposalHeadPaint);
            }
        }

        foreach (LetterFormationCarrierState carrier in _state.Carriers)
        {
            SKColor color = _carrierColors.TryGetValue(carrier.Id, out SKColor named) ? named : new SKColor(96, 96, 96);
            using SKPaint carrierPaint = new()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 6f,
                Color = color,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
            };

            SKPoint start = MapPoint(_state.GetStartPoint(carrier.Id), letterboxRect);
            SKPoint end = MapPoint(_state.GetEndPoint(carrier.Id), letterboxRect);
            canvas.DrawLine(start, end, carrierPaint);

            SKPoint mid = new((start.X + end.X) * 0.5f, (start.Y + end.Y) * 0.5f);
            using SKPaint labelPaint = _carrierLabelPaint.Clone();
            labelPaint.Color = color;
            canvas.DrawText(carrier.Id, mid.X + 8f, mid.Y - 8f, labelPaint);
        }

        foreach (LetterFormationSiteState site in _state.Sites)
        {
            SKPoint point = MapPoint(site.Position, letterboxRect);
            canvas.DrawCircle(point, 12f, _siteFillPaint);
            canvas.DrawCircle(point, 12f, _siteStrokePaint);
            canvas.DrawText(site.Id, point.X + 14f, point.Y - 10f, _siteLabelPaint);
        }
    }

    private void DrawStatus(SKCanvas canvas, SKRect cardRect)
    {
        if (_state is null)
        {
            return;
        }

        canvas.DrawText("Status", cardRect.Left + 18f, cardRect.Top + 28f, _infoTitlePaint);
        float y = cardRect.Top + 54f;
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"step => {_state.StepIndex}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"sites => {_state.Sites.Count}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"carriers => {_state.Carriers.Count}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"tensions => {_state.Tensions.Count}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"proposals => {_proposals.Count}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"total => {SumTension(_state.Tensions):0.###}");
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"mode => {(_animateCheck?.Checked == true ? "running" : "paused")}");
    }

    private void DrawTopTensions(SKCanvas canvas, SKRect cardRect)
    {
        if (_state is null)
        {
            return;
        }

        canvas.DrawText("Top Tensions", cardRect.Left + 18f, cardRect.Top + 28f, _infoTitlePaint);
        float y = cardRect.Top + 54f;
        foreach (LetterFormationTension tension in _state.Tensions
                     .OrderByDescending(tension => ToDouble(tension.Magnitude))
                     .Take(9))
        {
            DrawInfoLine(
                canvas,
                cardRect.Left + 18f,
                ref y,
                $"{tension.ComponentId}: {tension.Source} => {Format(tension.Magnitude)}",
                cardRect.Width - 34f);
        }
    }

    private void DrawTopProposals(SKCanvas canvas, SKRect cardRect)
    {
        canvas.DrawText("Top Moves", cardRect.Left + 18f, cardRect.Top + 28f, _infoTitlePaint);
        float y = cardRect.Top + 54f;

        if (_showMovesCheck?.Checked != true)
        {
            DrawInfoLine(canvas, cardRect.Left + 18f, ref y, "move display => off", cardRect.Width - 34f);
            return;
        }

        foreach (LetterFormationProposal proposal in _proposals
                     .OrderByDescending(proposal => ToDouble(proposal.Strength))
                     .Take(8))
        {
            DrawInfoLine(
                canvas,
                cardRect.Left + 18f,
                ref y,
                $"{proposal.SiteId}: {proposal.Source} => {Format(proposal.Offset)}",
                cardRect.Width - 34f);
        }
    }

    private void DrawInfoLine(SKCanvas canvas, float x, ref float y, string text, float width = 210f)
    {
        foreach (string line in PageChrome.WrapText(text, width, _infoTextPaint))
        {
            canvas.DrawText(line, x, y, _infoTextPaint);
            y += _infoTextPaint.TextSize + 5f;
        }
    }

    private static SKRect FitLetterbox(SKRect plotRect, LetterFormationEnvironment environment)
    {
        double aspect = ToDouble(environment.Width) / Math.Max(0.0001d, ToDouble(environment.Height));
        float plotAspect = plotRect.Width / plotRect.Height;
        if (plotAspect > aspect)
        {
            float width = (float)(plotRect.Height * aspect);
            float left = plotRect.MidX - width * 0.5f;
            return new SKRect(left, plotRect.Top, left + width, plotRect.Bottom);
        }

        float height = (float)(plotRect.Width / aspect);
        float top = plotRect.MidY - height * 0.5f;
        return new SKRect(plotRect.Left, top, plotRect.Right, top + height);
    }

    private SKPoint MapPoint(PlanarPoint point, SKRect rect) =>
        new(MapX(point.Horizontal, rect), MapY(point.Vertical, rect));

    private float MapX(Core2.Elements.Proportion value, SKRect rect)
    {
        if (_state is null)
        {
            return rect.Left;
        }

        double ratio = ToDouble(value - _state.Environment.Left) /
                       Math.Max(0.0001d, ToDouble(_state.Environment.Width));
        return rect.Left + (float)(ratio * rect.Width);
    }

    private float MapY(Core2.Elements.Proportion value, SKRect rect)
    {
        if (_state is null)
        {
            return rect.Top;
        }

        double ratio = ToDouble(value - _state.Environment.Top) /
                       Math.Max(0.0001d, ToDouble(_state.Environment.Height));
        return rect.Top + (float)(ratio * rect.Height);
    }

    private static void DrawArrow(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint linePaint, SKPaint headPaint)
    {
        canvas.DrawLine(start, end, linePaint);

        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt((dx * dx) + (dy * dy));
        if (length < 2f)
        {
            return;
        }

        float ux = dx / length;
        float uy = dy / length;
        float head = 8f;
        SKPoint left = new(end.X - ux * head - uy * 4f, end.Y - uy * head + ux * 4f);
        SKPoint right = new(end.X - ux * head + uy * 4f, end.Y - uy * head - ux * 4f);
        using SKPath path = new();
        path.MoveTo(end);
        path.LineTo(left);
        path.LineTo(right);
        path.Close();
        canvas.DrawPath(path, headPaint);
    }

    private static IReadOnlyList<(string SiteId, PlanarOffset Offset)> AggregateSiteProposals(IReadOnlyList<LetterFormationProposal> proposals) =>
        proposals
            .GroupBy(proposal => proposal.SiteId, StringComparer.Ordinal)
            .Select(group =>
            {
                double totalWeight = 0d;
                double horizontal = 0d;
                double vertical = 0d;
                foreach (LetterFormationProposal proposal in group)
                {
                    double weight = Math.Max(0.0001d, ToDouble(proposal.Strength));
                    totalWeight += weight;
                    horizontal += ToDouble(proposal.Offset.Horizontal) * weight;
                    vertical += ToDouble(proposal.Offset.Vertical) * weight;
                }

                if (totalWeight <= 0d)
                {
                    return (group.Key, PlanarOffset.Zero);
                }

                return (group.Key, ToOffset(horizontal / totalWeight, vertical / totalWeight));
            })
            .ToArray();

    private static string Format(Core2.Elements.Proportion value) => $"{value.Numerator}/{value.Denominator}";

    private static string Format(PlanarOffset offset) =>
        $"{ToDouble(offset.Horizontal):+0.###;-0.###;0},{ToDouble(offset.Vertical):+0.###;-0.###;0}";

    private static double SumTension(IEnumerable<LetterFormationTension> tensions) =>
        tensions.Sum(tension => ToDouble(tension.Magnitude));

    private static double ToDouble(Core2.Elements.Proportion value) =>
        (double)value.Numerator / value.Denominator;

    private static PlanarOffset ToOffset(double horizontal, double vertical) =>
        new(
            new Core2.Elements.Proportion((long)Math.Round(horizontal * 1000d), 1000),
            new Core2.Elements.Proportion((long)Math.Round(vertical * 1000d), 1000));
}
