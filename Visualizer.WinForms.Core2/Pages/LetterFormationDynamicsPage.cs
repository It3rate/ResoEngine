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
    private readonly SKPaint _finePrintPaint = new()
    {
        Color = new SKColor(72, 72, 72, 170),
        TextSize = 10.5f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _strokeOrderPaint = new()
    {
        Color = new SKColor(54, 54, 54),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _strokeOrderBadgePaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255, 220),
        IsAntialias = true,
    };
    private readonly SKPaint _strokeOrderBadgeStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(205, 205, 205, 220),
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
    private readonly SKPaint _tensionHaloFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        IsAntialias = true,
    };
    private readonly SKPaint _tensionHaloStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2.2f,
        IsAntialias = true,
    };

    private readonly SKColor[] _fallbackCarrierColors =
    [
        SKColor.Parse("#C77000"),
        SKColor.Parse("#1D6FE8"),
        SKColor.Parse("#12824A"),
        SKColor.Parse("#8415D0"),
        SKColor.Parse("#D34836"),
        SKColor.Parse("#0F8CFF"),
        SKColor.Parse("#9B59B6"),
    ];

    private SkiaCanvas? _canvasHost;
    private Panel? _controlsPanel;
    private CheckBox? _animateCheck;
    private Button? _stepButton;
    private Button? _resetButton;
    private System.Windows.Forms.Timer? _timer;
    private LetterFormationState? _state;
    private IReadOnlyList<LetterFormationProposal> _proposals = [];
    private int _resetCount;
    private LetterFormationPresetKind _selectedPreset = LetterFormationPresetKind.LetterA;
    private readonly Dictionary<LetterFormationPresetKind, Button> _presetButtons = new();
    private readonly List<SiteHandleLayout> _siteHandles = [];
    private string[]? _draggedSiteIds;
    private SKRect _lastLetterboxRect;
    private const float SiteHandleRadius = 16f;
    private const float GraphSidebarWidth = 214f;

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
        _siteHandles.Clear();

        float width = _canvasHost?.ClientSize.Width ?? 1400f;
        float height = _canvasHost?.ClientSize.Height ?? 1000f;
        string presetName = LetterFormationPresetFactory.GetDisplayName(_selectedPreset);

        canvas.DrawText("Letter Formation Dynamics", 34f, 44f, _headingPaint);
        float subtitleY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            $"This is a fixed-topology {presetName} with only local desires. Sites and carriers start from random positions, keep proposing small moves from their own tensions, and assemble one immutable snapshot at a time until the remaining pulls get quiet.",
            34f,
            ref subtitleY,
            680f,
            _bodyPaint);

        if (_state is null)
        {
            return;
        }

        SKRect graphCard = new(14f, 116f, width - 490f, height - 320f);
        SKRect statusCard = new(width - 292f, 150f, width - 34f, 292f);
        SKRect tensionCard = new(width - 292f, 310f, width - 34f, 560f);
        SKRect proposalCard = new(width - 292f, 578f, width - 34f, height - 190f);

        DrawCard(canvas, graphCard);
        DrawCard(canvas, statusCard);
        DrawCard(canvas, tensionCard);
        DrawCard(canvas, proposalCard);

        PositionPresetButtons(graphCard);
        DrawGraph(canvas, graphCard);
        DrawStatus(canvas, statusCard);
        DrawTopTensions(canvas, tensionCard);
        DrawTopProposals(canvas, proposalCard);
    }

    public void Destroy()
    {
        foreach (Button button in _presetButtons.Values)
        {
            _canvasHost?.Controls.Remove(button);
            button.Dispose();
        }
        _presetButtons.Clear();

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
        _finePrintPaint.Dispose();
        _strokeOrderPaint.Dispose();
        _strokeOrderBadgePaint.Dispose();
        _strokeOrderBadgeStrokePaint.Dispose();
        _proposalPaint.Dispose();
        _proposalHeadPaint.Dispose();
        _statusAccentPaint.Dispose();
        _tensionHaloFillPaint.Dispose();
        _tensionHaloStrokePaint.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(232, 110),
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
            Size = new Size(112, 22),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
        };

        _stepButton = new Button
        {
            Text = "Step",
            Location = new Point(14, 66),
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
            Location = new Point(112, 66),
            Size = new Size(92, 30),
            FlatStyle = FlatStyle.Flat,
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            BackColor = Color.White,
        };
        _resetButton.FlatAppearance.BorderColor = Color.FromArgb(214, 214, 214);
        _resetButton.Click += (_, _) => ResetState();

        foreach (LetterFormationPresetKind preset in GetOrderedPresets())
        {
            var button = new Button
            {
                Text = LetterFormationPresetFactory.GetShortLabel(preset),
                Location = new Point(0, 0),
                Size = new Size(60, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
                BackColor = Color.White,
                Tag = preset,
                Visible = true,
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(214, 214, 214);
            button.Click += (_, _) =>
            {
                _selectedPreset = preset;
                UpdatePresetButtons();
                ResetState();
            };
            _presetButtons[preset] = button;
            _canvasHost.Controls.Add(button);
        }

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(_animateCheck);
        _controlsPanel.Controls.Add(_stepButton);
        _controlsPanel.Controls.Add(_resetButton);
        _canvasHost.Controls.Add(_controlsPanel);
        UpdatePresetButtons();
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
            LetterFormationPresetFactory.CreateSeed(_selectedPreset, random, environment));
        if (_animateCheck is not null)
        {
            _animateCheck.Checked = true;
        }
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
        _canvasHost?.InvalidateCanvas();
    }

    private void RefreshProposals()
    {
        if (_state is null)
        {
            _proposals = [];
            return;
        }

        _proposals = LetterFormationStepper.GenerateProposals(_state);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        SiteHandleLayout? handle = HitSiteHandle(pixelPoint);
        if (handle is null)
        {
            return false;
        }

        _draggedSiteIds = handle.SiteIds;
        _canvasHost?.InvalidateCanvas();
        return true;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost is null)
        {
            return;
        }

        if (_draggedSiteIds is not null)
        {
            DragSites(pixelPoint);
            _canvasHost.Cursor = Cursors.SizeAll;
            return;
        }

        _canvasHost.Cursor = HitSiteHandle(pixelPoint) is not null
            ? Cursors.SizeAll
            : Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        _draggedSiteIds = null;
        if (_canvasHost is not null)
        {
            _canvasHost.Cursor = Cursors.Default;
        }
    }

    private void UpdatePresetButtons()
    {
        foreach ((LetterFormationPresetKind preset, Button button) in _presetButtons)
        {
            bool selected = preset == _selectedPreset;
            button.BackColor = selected
                ? Color.FromArgb(239, 246, 255)
                : Color.White;
            button.FlatAppearance.BorderColor = selected
                ? Color.FromArgb(110, 158, 220)
                : Color.FromArgb(214, 214, 214);
        }
    }

    private static IReadOnlyList<LetterFormationPresetKind> GetOrderedPresets() =>
        Enum.GetValues<LetterFormationPresetKind>()
            .OrderBy(LetterFormationPresetFactory.GetShortLabel)
            .ToArray();

    private float ResolvePresetButtonBlockHeight()
    {
        if (_presetButtons.Count == 0)
        {
            return 0f;
        }

        const float buttonHeight = 30f;
        const float gap = 6f;
        int rows = (int)Math.Ceiling(_presetButtons.Count / 7d);
        return (rows * buttonHeight) + ((rows - 1) * gap);
    }

    private void PositionPresetButtons(SKRect graphCard)
    {
        if (_canvasHost is null || _presetButtons.Count == 0)
        {
            return;
        }

        const int buttonWidth = 48;
        const int buttonHeight = 30;
        const int gap = 6;
        int startX = (int)graphCard.Left + 22;
        int startY = (int)graphCard.Top + 110;

        int index = 0;
        foreach (LetterFormationPresetKind preset in GetOrderedPresets())
        {
            if (!_presetButtons.TryGetValue(preset, out Button? button))
            {
                continue;
            }

            int row = index / 7;
            int column = index % 7;
            button.Size = new Size(buttonWidth, buttonHeight);
            button.Location = new Point(
                startX + (column * (buttonWidth + gap)),
                startY + (row * (buttonHeight + gap)));
            button.Visible = true;
            button.BringToFront();
            index++;
        }
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

        float buttonBlockHeight = ResolvePresetButtonBlockHeight();
        float contentTop = cardRect.Top + 54f + buttonBlockHeight + 18f;
        SKRect tensionRect = new(
            cardRect.Left + 20f,
            contentTop,
            cardRect.Left + 20f + GraphSidebarWidth,
            cardRect.Bottom - 24f);
        SKRect plotRect = new(
            tensionRect.Right + 30f,
            cardRect.Top + 20,
            cardRect.Right - 6f,
            cardRect.Bottom - 20f);
        SKRect letterboxRect = FitLetterbox(plotRect, _state.Environment);
        _lastLetterboxRect = letterboxRect;

        canvas.DrawRoundRect(letterboxRect, 18f, 18f, _statusAccentPaint);
        canvas.DrawRoundRect(letterboxRect, 18f, 18f, _letterboxPaint);

        float centerX = MapX(_state.Environment.CenterlineX, letterboxRect);
        float midY = MapY(_state.Environment.MidlineY, letterboxRect);
        canvas.DrawLine(centerX, letterboxRect.Top, centerX, letterboxRect.Bottom, _guidePaint);
        canvas.DrawLine(letterboxRect.Left, midY, letterboxRect.Right, midY, _guidePaint);

        foreach (IGrouping<PlanarPoint, LetterFormationSiteState> siteGroup in _state.Sites.GroupBy(site => site.Position))
        {
            DrawSiteHalo(canvas, letterboxRect, siteGroup);
        }

        if (_proposals.Count > 0)
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

        for (int carrierIndex = 0; carrierIndex < _state.Carriers.Count; carrierIndex++)
        {
            LetterFormationCarrierState carrier = _state.Carriers[carrierIndex];
            SKColor color = ResolveCarrierColor(carrier, carrierIndex);
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
            SKPoint mid = DrawCarrier(canvas, carrier, start, end, carrierPaint, letterboxRect);
            DrawStrokeDirection(canvas, carrier, start, end, color, letterboxRect);
            using SKPaint labelPaint = _carrierLabelPaint.Clone();
            labelPaint.Color = color;
            canvas.DrawText(carrier.Id, mid.X + 8f, mid.Y - 8f, labelPaint);
        }

        foreach (var siteGroup in _state.Sites.GroupBy(site => site.Position))
        {
            PlanarPoint position = siteGroup.Key;
            SKPoint point = MapPoint(position, letterboxRect);
            canvas.DrawCircle(point, 12f, _siteFillPaint);
            canvas.DrawCircle(point, 12f, _siteStrokePaint);
            string label = string.Join(" / ", siteGroup.Select(site => site.Id));
            canvas.DrawText(label, point.X + 14f, point.Y - 10f, _siteLabelPaint);
            _siteHandles.Add(new SiteHandleLayout(
                new SKRect(point.X - 16f, point.Y - 16f, point.X + 16f, point.Y + 16f),
                siteGroup.Select(site => site.Id).ToArray()));
        }

        DrawRemainingTensionsOverlay(canvas, tensionRect);
    }

    private void DrawStatus(SKCanvas canvas, SKRect cardRect)
    {
        if (_state is null)
        {
            return;
        }

        canvas.DrawText("Status", cardRect.Left + 18f, cardRect.Top + 28f, _infoTitlePaint);
        float y = cardRect.Top + 54f;
        DrawInfoLine(canvas, cardRect.Left + 18f, ref y, $"preset => {LetterFormationPresetFactory.GetShortLabel(_selectedPreset)}");
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

    private void DrawRemainingTensionsOverlay(SKCanvas canvas, SKRect tensionRect)
    {
        if (_state is null)
        {
            return;
        }

        float x = tensionRect.Left;
        float y = tensionRect.Top;
        float width = tensionRect.Width;
        float bottom = tensionRect.Bottom;

        IReadOnlyList<LetterFormationTension> tensions = _state.Tensions
            .OrderByDescending(tension => ToDouble(tension.Magnitude))
            .ToArray();

        if (tensions.Count == 0)
        {
            canvas.DrawText("tensions: none", x, y, _finePrintPaint);
            return;
        }

        foreach (LetterFormationTension tension in tensions)
        {
            string text = $"{tension.ComponentId}: {tension.Source} {Format(tension.Magnitude)}";
            foreach (string line in PageChrome.WrapText(text, width, _finePrintPaint))
            {
                if (y > bottom)
                {
                    canvas.DrawText("...", x, y, _finePrintPaint);
                    return;
                }

                canvas.DrawText(line, x, y, _finePrintPaint);
                y += _finePrintPaint.TextSize + 3f;
            }
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

    private SKPoint DrawCarrier(
        SKCanvas canvas,
        LetterFormationCarrierState carrier,
        SKPoint start,
        SKPoint end,
        SKPaint carrierPaint,
        SKRect letterboxRect)
    {
        if (_selectedPreset == LetterFormationPresetKind.CapitalD &&
            string.Equals(carrier.Id, "Bowl", StringComparison.Ordinal))
        {
            return DrawCapitalDBowlCarrier(canvas, start, end, carrierPaint, letterboxRect);
        }

        canvas.DrawLine(start, end, carrierPaint);
        return new SKPoint((start.X + end.X) * 0.5f, (start.Y + end.Y) * 0.5f);
    }

    private void DrawStrokeDirection(
        SKCanvas canvas,
        LetterFormationCarrierState carrier,
        SKPoint start,
        SKPoint end,
        SKColor color,
        SKRect letterboxRect)
    {
        using SKPaint arrowLinePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2.8f,
            Color = new SKColor(color.Red, color.Green, color.Blue, 185),
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };
        using SKPaint arrowHeadPaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(color.Red, color.Green, color.Blue, 185),
            IsAntialias = true,
        };
        using SKPaint arrowOutlineLinePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5.4f,
            Color = new SKColor(255, 255, 255, 215),
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };
        using SKPaint arrowOutlineHeadPaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 215),
            IsAntialias = true,
        };

        if (_selectedPreset == LetterFormationPresetKind.CapitalD &&
            string.Equals(carrier.Id, "Bowl", StringComparison.Ordinal))
        {
            float startT = carrier.ReverseForStroke ? 0.62f : 0.38f;
            float endT = carrier.ReverseForStroke ? 0.46f : 0.54f;
            float curveFactor = ResolveCapitalDBowlCurveFactor();
            float rightmost = Math.Max(start.X, end.X);
            float outwardRoom = Math.Max(18f, letterboxRect.Right - rightmost - 26f);
            float controlX = rightmost + (outwardRoom * (0.45f + (0.4f * curveFactor)));
            SKPoint control1 = new(controlX, start.Y);
            SKPoint control2 = new(controlX, end.Y);
            SKPoint arrowStart = EvaluateCubic(start, control1, control2, end, startT);
            SKPoint arrowEnd = EvaluateCubic(start, control1, control2, end, endT);
            DrawArrow(canvas, arrowStart, arrowEnd, arrowOutlineLinePaint, arrowOutlineHeadPaint);
            DrawArrow(canvas, arrowStart, arrowEnd, arrowLinePaint, arrowHeadPaint);
            DrawStrokeOrderLabel(canvas, carrier, arrowEnd);
            return;
        }

        SKPoint directedStart = carrier.ReverseForStroke ? end : start;
        SKPoint directedEnd = carrier.ReverseForStroke ? start : end;
        SKPoint arrowStartPoint = Interpolate(directedStart, directedEnd, 0.24f);
        SKPoint arrowEndPoint = Interpolate(directedStart, directedEnd, 0.40f);
        SKPoint normal = ResolvePerpendicularOffset(directedStart, directedEnd, 11f);
        arrowStartPoint = new SKPoint(arrowStartPoint.X + normal.X, arrowStartPoint.Y + normal.Y);
        arrowEndPoint = new SKPoint(arrowEndPoint.X + normal.X, arrowEndPoint.Y + normal.Y);
        DrawArrow(canvas, arrowStartPoint, arrowEndPoint, arrowOutlineLinePaint, arrowOutlineHeadPaint);
        DrawArrow(canvas, arrowStartPoint, arrowEndPoint, arrowLinePaint, arrowHeadPaint);
        DrawStrokeOrderLabel(canvas, carrier, arrowEndPoint);
    }

    private void DrawStrokeOrderLabel(SKCanvas canvas, LetterFormationCarrierState carrier, SKPoint anchor)
    {
        if (carrier.StrokeOrder <= 0 || carrier.StrokeSegmentOrder != 0)
        {
            return;
        }

        string text = carrier.StrokeOrder.ToString();
        SKRect bounds = new();
        _strokeOrderPaint.MeasureText(text, ref bounds);
        float radius = MathF.Max(8f, MathF.Max(bounds.Width, bounds.Height) * 0.55f);
        SKPoint center = new(anchor.X + 12f, anchor.Y - 12f);
        canvas.DrawCircle(center, radius, _strokeOrderBadgePaint);
        canvas.DrawCircle(center, radius, _strokeOrderBadgeStrokePaint);
        canvas.DrawText(text, center.X - (bounds.MidX), center.Y - bounds.MidY, _strokeOrderPaint);
    }

    private SKPoint DrawCapitalDBowlCarrier(
        SKCanvas canvas,
        SKPoint start,
        SKPoint end,
        SKPaint carrierPaint,
        SKRect letterboxRect)
    {
        float curveFactor = ResolveCapitalDBowlCurveFactor();
        float rightmost = Math.Max(start.X, end.X);
        float outwardRoom = Math.Max(18f, letterboxRect.Right - rightmost - 26f);
        float controlX = rightmost + (outwardRoom * (0.45f + (0.4f * curveFactor)));
        SKPoint control1 = new(controlX, start.Y);
        SKPoint control2 = new(controlX, end.Y);

        using SKPath path = new();
        path.MoveTo(start);
        path.CubicTo(control1, control2, end);
        canvas.DrawPath(path, carrierPaint);

        return EvaluateCubic(start, control1, control2, end, 0.5f);
    }

    private float ResolveCapitalDBowlCurveFactor()
    {
        if (_state is null)
        {
            return 0.35f;
        }

        float top = ResolveJoinCloseness("StemTop", "BowlTop");
        float bottom = ResolveJoinCloseness("StemBottom", "BowlBottom");
        float joined = (top + bottom) * 0.5f;
        return 0.35f + (0.65f * joined);
    }

    private float ResolveJoinCloseness(string siteId, string otherSiteId)
    {
        if (_state is null)
        {
            return 0f;
        }

        LetterFormationSiteState site = _state.GetSite(siteId);
        JoinSiteDesire? desire = site.Desires
            .OfType<JoinSiteDesire>()
            .FirstOrDefault(join => string.Equals(join.OtherSiteId, otherSiteId, StringComparison.Ordinal));
        if (desire is null)
        {
            return 0f;
        }

        LetterFormationSiteState other = _state.GetSite(otherSiteId);
        double distance = Math.Sqrt(
            Math.Pow(ToDouble(other.Position.Horizontal - site.Position.Horizontal), 2d) +
            Math.Pow(ToDouble(other.Position.Vertical - site.Position.Vertical), 2d));
        double capture = ToDouble(desire.CaptureDistance) * 1.15d;
        if (capture <= 0d)
        {
            return 0f;
        }

        return (float)Math.Clamp(1d - (distance / capture), 0d, 1d);
    }

    private static SKPoint EvaluateCubic(SKPoint p0, SKPoint p1, SKPoint p2, SKPoint p3, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        return new SKPoint(
            (uuu * p0.X) + (3f * uu * t * p1.X) + (3f * u * tt * p2.X) + (ttt * p3.X),
            (uuu * p0.Y) + (3f * uu * t * p1.Y) + (3f * u * tt * p2.Y) + (ttt * p3.Y));
    }

    private void DrawSiteHalo(SKCanvas canvas, SKRect letterboxRect, IGrouping<PlanarPoint, LetterFormationSiteState> siteGroup)
    {
        if (_state is null)
        {
            return;
        }

        double signal = siteGroup.Max(site => ResolveSiteSignal(site.Id));
        SKColor color = LerpColor(
            new SKColor(78, 176, 92, 22),
            new SKColor(222, 76, 76, 76),
            (float)signal);
        _tensionHaloFillPaint.Color = color;
        _tensionHaloStrokePaint.Color = new SKColor(color.Red, color.Green, color.Blue, (byte)Math.Min(96, color.Alpha + 12));

        SKPoint point = MapPoint(siteGroup.Key, letterboxRect);
        float radius = 17f + (float)(signal * 11d);
        canvas.DrawCircle(point, radius, _tensionHaloFillPaint);
        canvas.DrawCircle(point, radius, _tensionHaloStrokePaint);
    }

    private double ResolveSiteSignal(string siteId)
    {
        if (_state is null)
        {
            return 0d;
        }

        double siteTension = _state.Tensions
            .Where(tension => string.Equals(tension.ComponentId, siteId, StringComparison.Ordinal))
            .Sum(tension => ToDouble(tension.Magnitude));

        double carrierTension = _state.Carriers
            .Where(carrier =>
                string.Equals(carrier.StartSiteId, siteId, StringComparison.Ordinal) ||
                string.Equals(carrier.EndSiteId, siteId, StringComparison.Ordinal))
            .SelectMany(carrier => _state.Tensions.Where(tension => string.Equals(tension.ComponentId, carrier.Id, StringComparison.Ordinal)))
            .Sum(tension => ToDouble(tension.Magnitude) * 0.45d);

        double total = siteTension + carrierTension;
        return Math.Clamp(total / 6d, 0d, 1d);
    }

    private SKColor ResolveCarrierColor(LetterFormationCarrierState carrier, int carrierIndex)
    {
        int colorIndex = carrier.StrokeOrder > 0
            ? carrier.StrokeOrder - 1
            : carrierIndex;
        return _fallbackCarrierColors[colorIndex % _fallbackCarrierColors.Length];
    }

    private static SKPoint Interpolate(SKPoint start, SKPoint end, float t) =>
        new(
            start.X + ((end.X - start.X) * t),
            start.Y + ((end.Y - start.Y) * t));

    private static SKPoint ResolvePerpendicularOffset(SKPoint start, SKPoint end, float magnitude)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt((dx * dx) + (dy * dy));
        if (length < 0.001f)
        {
            return SKPoint.Empty;
        }

        return new SKPoint((-dy / length) * magnitude, (dx / length) * magnitude);
    }

    private static SKColor LerpColor(SKColor from, SKColor to, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        byte Lerp(byte a, byte b) => (byte)(a + ((b - a) * t));
        return new SKColor(
            Lerp(from.Red, to.Red),
            Lerp(from.Green, to.Green),
            Lerp(from.Blue, to.Blue),
            Lerp(from.Alpha, to.Alpha));
    }

    private static double SumAlignmentTension(IEnumerable<LetterFormationTension> tensions) =>
        tensions
            .Where(IsAlignmentTension)
            .Sum(tension => ToDouble(tension.Magnitude));

    private static bool IsAlignmentTension(LetterFormationTension tension)
    {
        string source = tension.Source.ToLowerInvariant();
        return source.Contains("vertical", StringComparison.Ordinal) ||
               source.Contains("horizontal", StringComparison.Ordinal) ||
               source.Contains("level", StringComparison.Ordinal) ||
               source.Contains("midline", StringComparison.Ordinal) ||
               source.Contains("rises", StringComparison.Ordinal) ||
               source.Contains("descends", StringComparison.Ordinal) ||
               source.Contains("extends", StringComparison.Ordinal);
    }

    private SiteHandleLayout? HitSiteHandle(SKPoint pixelPoint) =>
        _siteHandles.FirstOrDefault(handle => handle.Rect.Contains(pixelPoint));

    private void DragSites(SKPoint pixelPoint)
    {
        if (_state is null || _draggedSiteIds is null)
        {
            return;
        }

        PlanarPoint target = MapPixelToPoint(pixelPoint);
        HashSet<string> draggedIds = new(_draggedSiteIds, StringComparer.Ordinal);
        IReadOnlyList<LetterFormationSiteState> updatedSites = _state.Sites
            .Select(site => draggedIds.Contains(site.Id)
                ? site with { Position = target, Momentum = PlanarOffset.Zero }
                : site)
            .ToArray();

        _state = LetterFormationTensionEvaluator.Evaluate(_state with
        {
            Sites = updatedSites,
            Tensions = [],
        });
        if (_animateCheck is not null)
        {
            _animateCheck.Checked = true;
        }

        RefreshProposals();
        _canvasHost?.InvalidateCanvas();
    }

    private PlanarPoint MapPixelToPoint(SKPoint pixelPoint)
    {
        if (_state is null || _lastLetterboxRect.Width <= 0f || _lastLetterboxRect.Height <= 0f)
        {
            return new PlanarPoint(Core2.Elements.Proportion.Zero, Core2.Elements.Proportion.Zero);
        }

        double horizontalRatio = Math.Clamp((pixelPoint.X - _lastLetterboxRect.Left) / _lastLetterboxRect.Width, 0d, 1d);
        double verticalRatio = Math.Clamp((pixelPoint.Y - _lastLetterboxRect.Top) / _lastLetterboxRect.Height, 0d, 1d);
        return new PlanarPoint(
            _state.Environment.Left + (_state.Environment.Width * FromDouble(horizontalRatio)),
            _state.Environment.Top + (_state.Environment.Height * FromDouble(verticalRatio)));
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

    private static Core2.Elements.Proportion FromDouble(double value) =>
        new((long)Math.Round(value * 1000d), 1000);

    private static PlanarOffset ToOffset(double horizontal, double vertical) =>
        new(
            new Core2.Elements.Proportion((long)Math.Round(horizontal * 1000d), 1000),
            new Core2.Elements.Proportion((long)Math.Round(vertical * 1000d), 1000));

    private sealed record SiteHandleLayout(SKRect Rect, string[] SiteIds);
}
