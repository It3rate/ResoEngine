using Core2.Dynamic;
using Core2.Geometry.Glyphs;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class GlyphSkeletonGrowthPage : IVisualizerPage
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
        Color = new SKColor(249, 249, 250),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(206, 206, 206),
        IsAntialias = true,
    };

    private readonly SKPaint _stageFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _stageBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(220, 220, 220),
        IsAntialias = true,
    };

    private readonly SKPaint _glyphBoxPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(182, 182, 182),
        IsAntialias = true,
    };

    private readonly SKPaint _fieldLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(202, 212, 224),
        PathEffect = SKPathEffect.CreateDash([5f, 5f], 0f),
        IsAntialias = true,
    };

    private readonly SKPaint _stopPointPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(240, 156, 126, 180),
        IsAntialias = true,
    };

    private readonly SKPaint _branchPointPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(147, 188, 255, 190),
        IsAntialias = true,
    };

    private readonly SKPaint _carrierGlowPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 8f,
        Color = new SKColor(74, 209, 201, 50),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _carrierPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 3f,
        Color = new SKColor(35, 132, 127),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _activeTipPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(24, 118, 118),
        IsAntialias = true,
    };

    private readonly SKPaint _inactiveTipPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(155, 168, 172),
        IsAntialias = true,
    };

    private readonly SKPaint _junctionFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _junctionStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(94, 146, 231),
        IsAntialias = true,
    };

    private readonly SKPaint _terminalStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(214, 133, 96),
        IsAntialias = true,
    };

    private readonly SKPaint _sectionTitlePaint = new()
    {
        Color = new SKColor(61, 61, 61),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _detailPaint = new()
    {
        Color = new SKColor(98, 98, 98),
        TextSize = 12.5f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _legendSwatchPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(61, 61, 61),
        IsAntialias = true,
    };

    private readonly System.Windows.Forms.Timer _timer;
    private readonly List<Button> _letterButtons = [];
    private readonly string[] _letterKeys = ["Y", "V", "T", "O"];

    private SkiaCanvas? _canvasHost;
    private Panel? _controlsPanel;
    private FlowLayoutPanel? _letterButtonPanel;
    private Button? _playButton;
    private Button? _stepButton;
    private Button? _resetButton;
    private NumericUpDown? _maxStepsInput;

    private string _selectedLetterKey = "Y";
    private bool _isPlaying;
    private DynamicMachine<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>? _machine;
    private DynamicTrace<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>? _trace;

    public GlyphSkeletonGrowthPage()
    {
        _timer = new System.Windows.Forms.Timer
        {
            Interval = 420,
        };
        _timer.Tick += (_, _) =>
        {
            if (!AdvanceMachine())
            {
                SetPlaying(false);
            }
        };
    }

    public string Title => "Glyph Skeleton Growth";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureControls();
        LoadLetter(_selectedLetterKey);
    }

    public void Render(SKCanvas canvas)
    {
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);

        canvas.DrawText("Glyph Skeleton Growth", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Phase 5 seeds each glyph with a small number of charged tips and landmarks, then lets the Phase 4 growth resolver evolve carriers, joins, splits, and stops inside a letter box.",
            34f,
            ref subtitleY,
            760f,
            _bodyPaint);

        var layout = ComputeLayout();
        canvas.DrawRoundRect(layout.Card, _cardFillPaint);
        canvas.DrawRoundRect(layout.Card, _cardBorderPaint);
        canvas.DrawRoundRect(layout.StageRect, 18f, 18f, _stageFillPaint);
        canvas.DrawRoundRect(layout.StageRect, 18f, 18f, _stageBorderPaint);
        canvas.DrawRoundRect(layout.DetailRect, 18f, 18f, _stageFillPaint);
        canvas.DrawRoundRect(layout.DetailRect, 18f, 18f, _stageBorderPaint);

        if (_trace is null)
        {
            return;
        }

        var context = _trace.SelectedContext ?? _trace.CurrentContexts.FirstOrDefault();
        if (context is null)
        {
            return;
        }

        var spec = GlyphLetterCatalog.Get(_selectedLetterKey);
        DrawGlyphStage(canvas, layout.StageRect, spec, context.State);
        DrawDetails(canvas, layout.DetailRect, spec, context.State, _trace);
    }

    public void Destroy()
    {
        SetPlaying(false);

        if (_controlsPanel is not null)
        {
            _canvasHost?.Controls.Remove(_controlsPanel);
            _controlsPanel.Dispose();
            _controlsPanel = null;
        }

        _letterButtons.Clear();
        _letterButtonPanel = null;
        _playButton = null;
        _stepButton = null;
        _resetButton = null;
        _maxStepsInput = null;
        _machine = null;
        _trace = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _timer.Dispose();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _stageFillPaint.Dispose();
        _stageBorderPaint.Dispose();
        _glyphBoxPaint.Dispose();
        _fieldLinePaint.Dispose();
        _stopPointPaint.Dispose();
        _branchPointPaint.Dispose();
        _carrierGlowPaint.Dispose();
        _carrierPaint.Dispose();
        _activeTipPaint.Dispose();
        _inactiveTipPaint.Dispose();
        _junctionFillPaint.Dispose();
        _junctionStrokePaint.Dispose();
        _terminalStrokePaint.Dispose();
        _sectionTitlePaint.Dispose();
        _detailPaint.Dispose();
        _legendSwatchPaint.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(286, 168),
            BackColor = Color.FromArgb(248, 248, 248),
        };

        var title = new Label
        {
            Text = "Glyph Seeds",
            Location = new Point(14, 10),
            Size = new Size(180, 18),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(62, 62, 62),
        };

        _letterButtonPanel = new FlowLayoutPanel
        {
            Location = new Point(14, 36),
            Size = new Size(258, 34),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
        };

        foreach (string letterKey in _letterKeys)
        {
            string captured = letterKey;
            var button = CreateUiButton(letterKey, width: 54);
            button.Click += (_, _) => LoadLetter(captured);
            _letterButtonPanel.Controls.Add(button);
            _letterButtons.Add(button);
        }

        var stepsLabel = new Label
        {
            Text = "Max Steps",
            Location = new Point(14, 82),
            Size = new Size(88, 18),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(92, 92, 92),
        };

        _maxStepsInput = new NumericUpDown
        {
            Minimum = 4,
            Maximum = 60,
            Value = (decimal)GlyphGrowthDefaults.DefaultMaxSteps,
            Increment = 1,
            DecimalPlaces = 0,
            Location = new Point(110, 79),
            Size = new Size(64, 24),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Regular),
        };
        _maxStepsInput.ValueChanged += (_, _) => LoadLetter(_selectedLetterKey);

        _playButton = CreateUiButton("Play", width: 68);
        _playButton.Location = new Point(14, 118);
        _playButton.Click += (_, _) =>
        {
            if (_machine?.IsCompleted == true)
            {
                LoadLetter(_selectedLetterKey);
            }

            SetPlaying(!_isPlaying);
        };

        _stepButton = CreateUiButton("Step", width: 68);
        _stepButton.Location = new Point(92, 118);
        _stepButton.Click += (_, _) =>
        {
            if (_machine?.IsCompleted == true)
            {
                LoadLetter(_selectedLetterKey);
            }

            AdvanceMachine();
        };

        _resetButton = CreateUiButton("Reset", width: 68);
        _resetButton.Location = new Point(170, 118);
        _resetButton.Click += (_, _) => LoadLetter(_selectedLetterKey);

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(_letterButtonPanel);
        _controlsPanel.Controls.Add(stepsLabel);
        _controlsPanel.Controls.Add(_maxStepsInput);
        _controlsPanel.Controls.Add(_playButton);
        _controlsPanel.Controls.Add(_stepButton);
        _controlsPanel.Controls.Add(_resetButton);
        _canvasHost.Controls.Add(_controlsPanel);
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);
        RefreshLetterButtons();
    }

    private void LoadLetter(string letterKey)
    {
        _selectedLetterKey = letterKey;
        SetPlaying(false);
        _machine = GlyphGrowthRuntime.CreateMachine(letterKey, (int)(_maxStepsInput?.Value ?? 12m));
        _trace = _machine.Snapshot();
        RefreshLetterButtons();
        UpdateControlState();
        _canvasHost?.InvalidateCanvas();
    }

    private bool AdvanceMachine()
    {
        if (_machine is null)
        {
            return false;
        }

        bool advanced = _machine.Step();
        _trace = _machine.Snapshot();
        if (!advanced)
        {
            UpdateControlState();
        }

        _canvasHost?.InvalidateCanvas();
        return advanced;
    }

    private void SetPlaying(bool playing)
    {
        _isPlaying = playing;
        if (_isPlaying)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        UpdateControlState();
    }

    private void UpdateControlState()
    {
        if (_playButton is null || _stepButton is null)
        {
            return;
        }

        bool completed = _machine?.IsCompleted == true;
        _playButton.Text = _isPlaying ? "Pause" : completed ? "Replay" : "Play";
        _stepButton.Text = completed ? "Restart" : "Step";
    }

    private void RefreshLetterButtons()
    {
        for (int index = 0; index < _letterButtons.Count; index++)
        {
            bool selected = string.Equals(_letterKeys[index], _selectedLetterKey, StringComparison.OrdinalIgnoreCase);
            var button = _letterButtons[index];
            button.BackColor = selected ? Color.FromArgb(215, 241, 246) : Color.White;
            button.FlatAppearance.BorderColor = selected ? Color.FromArgb(122, 190, 201) : Color.FromArgb(210, 210, 210);
        }
    }

    private void DrawGlyphStage(SKCanvas canvas, SKRect stageRect, GlyphLetterSpec spec, GlyphGrowthState state)
    {
        var mapper = GlyphSceneMapper.Create(stageRect, spec.Environment.Box);
        DrawFieldWash(canvas, mapper, spec.Environment, state);
        canvas.DrawRect(mapper.BoxRect, _glyphBoxPaint);

        DrawLandmarks(canvas, mapper, spec.Environment);
        DrawPacketGlow(canvas, mapper, state);

        foreach (var carrier in state.Carriers)
        {
            var start = mapper.Map(carrier.Start);
            var end = mapper.Map(carrier.End);
            canvas.DrawLine(start, end, _carrierGlowPaint);
            canvas.DrawLine(start, end, _carrierPaint);
        }

        foreach (var junction in state.Junctions)
        {
            DrawJunction(canvas, mapper, junction);
        }

        foreach (var tip in state.ActiveTips)
        {
            DrawTip(canvas, mapper, tip);
        }
    }

    private void DrawFieldWash(SKCanvas canvas, GlyphSceneMapper mapper, GlyphEnvironment environment, GlyphGrowthState state)
    {
        int columns = 16;
        int rows = 18;
        float cellWidth = mapper.BoxRect.Width / columns;
        float cellHeight = mapper.BoxRect.Height / rows;

        for (int yIndex = 0; yIndex < rows; yIndex++)
        {
            for (int xIndex = 0; xIndex < columns; xIndex++)
            {
                var glyphPoint = new GlyphVector(
                    environment.Box.Left + environment.Box.Width * ((decimal)xIndex + 0.5m) / columns,
                    environment.Box.Bottom + environment.Box.Height * ((decimal)yIndex + 0.5m) / rows);

                decimal cool = 0m;
                decimal warm = 0m;

                foreach (var influence in environment.SampleInfluencesAt(glyphPoint))
                {
                    switch (influence.Rule.Kind)
                    {
                        case global::Core2.Propagation.CouplingKind.Align:
                        case global::Core2.Propagation.CouplingKind.Split:
                        case global::Core2.Propagation.CouplingKind.Grow:
                            cool += influence.Weight;
                            break;
                        case global::Core2.Propagation.CouplingKind.Stop:
                            warm += influence.Weight;
                            break;
                        case global::Core2.Propagation.CouplingKind.Attract:
                            cool += influence.Weight * 0.45m;
                            warm += influence.Weight * 0.1m;
                            break;
                    }
                }

                foreach (var signal in state.AmbientSignals ?? [])
                {
                    decimal distance = glyphPoint.DistanceTo(signal.Position);
                    if (distance > signal.Radius || signal.Radius <= 0m)
                    {
                        continue;
                    }

                    decimal weight = signal.Magnitude * (1m - decimal.Clamp(distance / signal.Radius, 0m, 1m));
                    switch (signal.Kind)
                    {
                        case global::Core2.Propagation.CouplingKind.Stop:
                        case global::Core2.Propagation.CouplingKind.Repel:
                            warm += weight;
                            break;
                        default:
                            cool += weight;
                            break;
                    }
                }

                int alpha = (int)Math.Round(Math.Clamp((double)((cool + warm) * 42m), 0d, 70d));
                if (alpha <= 2)
                {
                    continue;
                }

                byte red = (byte)Math.Clamp((int)Math.Round(236 + warm * 22m), 170, 248);
                byte green = (byte)Math.Clamp((int)Math.Round(244 - warm * 18m + cool * 6m), 188, 248);
                byte blue = (byte)Math.Clamp((int)Math.Round(247 - warm * 24m + cool * 18m), 188, 252);

                using var wash = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = new SKColor(red, green, blue, (byte)alpha),
                    IsAntialias = true,
                };

                float left = mapper.BoxRect.Left + xIndex * cellWidth;
                float top = mapper.BoxRect.Top + (rows - yIndex - 1) * cellHeight;
                canvas.DrawRect(new SKRect(left, top, left + cellWidth + 0.5f, top + cellHeight + 0.5f), wash);
            }
        }
    }

    private void DrawLandmarks(SKCanvas canvas, GlyphSceneMapper mapper, GlyphEnvironment environment)
    {
        foreach (var landmark in environment.Landmarks)
        {
            switch (landmark.Kind)
            {
                case GlyphLandmarkKind.Centerline:
                    {
                        var top = mapper.Map(new GlyphVector(landmark.Position.X, environment.Box.Top));
                        var bottom = mapper.Map(new GlyphVector(landmark.Position.X, environment.Box.Bottom));
                        canvas.DrawLine(top, bottom, _fieldLinePaint);
                        break;
                    }
                case GlyphLandmarkKind.Midline:
                case GlyphLandmarkKind.Capline:
                case GlyphLandmarkKind.Baseline:
                    {
                        var left = mapper.Map(new GlyphVector(environment.Box.Left, landmark.Position.Y));
                        var right = mapper.Map(new GlyphVector(environment.Box.Right, landmark.Position.Y));
                        canvas.DrawLine(left, right, _fieldLinePaint);
                        break;
                    }
                case GlyphLandmarkKind.BranchPoint:
                    {
                        var point = mapper.Map(landmark.Position);
                        canvas.DrawCircle(point, 5f, _branchPointPaint);
                        break;
                    }
                case GlyphLandmarkKind.StopPoint:
                    {
                        var point = mapper.Map(landmark.Position);
                        canvas.DrawCircle(point, 5f, _stopPointPaint);
                        break;
                    }
            }
        }
    }

    private void DrawPacketGlow(SKCanvas canvas, GlyphSceneMapper mapper, GlyphGrowthState state)
    {
        foreach (var tip in state.ActiveTips.Where(tip => tip.IsActive))
        {
            decimal magnitude = state.Packets
                .Where(packet => packet.CarrierKey == tip.Key)
                .Sum(packet => packet.Magnitude);

            if (magnitude <= 0m)
            {
                continue;
            }

            byte alpha = (byte)Math.Clamp((int)Math.Round(48m + magnitude * 84m), 32, 160);
            float radius = 10f + (float)Math.Clamp(magnitude * 8m, 0m, 18m);
            var point = mapper.Map(tip.Position);

            using var glowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(96, 207, 214, alpha),
                IsAntialias = true,
            };

            canvas.DrawCircle(point, radius, glowPaint);
        }
    }

    private void DrawJunction(SKCanvas canvas, GlyphSceneMapper mapper, GlyphJunction junction)
    {
        var point = mapper.Map(junction.Position);
        float radius = junction.Kind == GlyphJunctionKind.Split ? 7.5f : 6.5f;
        canvas.DrawCircle(point, radius, _junctionFillPaint);
        canvas.DrawCircle(point, radius, junction.Kind == GlyphJunctionKind.Terminal ? _terminalStrokePaint : _junctionStrokePaint);
    }

    private void DrawTip(SKCanvas canvas, GlyphSceneMapper mapper, GlyphTip tip)
    {
        var point = mapper.Map(tip.Position);
        canvas.DrawCircle(point, tip.IsActive ? 5.5f : 4.5f, tip.IsActive ? _activeTipPaint : _inactiveTipPaint);
    }

    private void DrawDetails(
        SKCanvas canvas,
        SKRect detailRect,
        GlyphLetterSpec spec,
        GlyphGrowthState state,
        DynamicTrace<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> trace)
    {
        float x = detailRect.Left + 16f;
        float y = detailRect.Top + 24f;

        canvas.DrawText($"{spec.DisplayName} skeleton", x, y, _sectionTitlePaint);
        y += 24f;
        PageChrome.DrawWrappedText(canvas, spec.Description, x, ref y, detailRect.Width - 32f, _detailPaint);

        y += 10f;
        canvas.DrawText("Current state", x, y, _sectionTitlePaint);
        y += 22f;

        var lines = new[]
        {
            $"macro step: {state.MacroStep}",
            $"active tips: {state.ActiveTips.Count(tip => tip.IsActive)} / {state.ActiveTips.Count}",
            $"carriers: {state.Carriers.Count}",
            $"junctions: {state.Junctions.Count}",
            $"packets: {state.Packets.Count}",
            $"ambient signals: {(state.AmbientSignals?.Count ?? 0)}",
            $"residual tension: {state.ResidualTension:0.00}",
            $"last adjustment: {state.LastAdjustment:0.00}",
            $"graph nodes: {trace.Graph.Nodes.Count}",
            $"frontier: {trace.CurrentContexts.Count}",
            $"status: {ResolveStatus()}",
        };

        foreach (var line in lines)
        {
            canvas.DrawText(line, x, y, _detailPaint);
            y += 18f;
        }

        y += 12f;
        canvas.DrawText("Legend", x, y, _sectionTitlePaint);
        y += 22f;
        DrawLegendRow(canvas, x, y, new SKColor(35, 132, 127), "carrier");
        y += 20f;
        DrawLegendRow(canvas, x, y, new SKColor(24, 118, 118), "active tip");
        y += 20f;
        DrawLegendRow(canvas, x, y, new SKColor(94, 146, 231), "join/split junction");
        y += 20f;
        DrawLegendRow(canvas, x, y, new SKColor(214, 133, 96), "terminal or stop capture");
        y += 20f;
        DrawLegendRow(canvas, x, y, new SKColor(96, 207, 214), "propagating tension glow");
        y += 20f;
        DrawLegendRow(canvas, x, y, new SKColor(214, 234, 244), "frame and ambient field wash");

        var activeTips = state.ActiveTips
            .Where(tip => tip.IsActive)
            .Take(4)
            .ToArray();
        if (activeTips.Length > 0)
        {
            y += 28f;
            canvas.DrawText("Active tips", x, y, _sectionTitlePaint);
            y += 20f;

            foreach (var tip in activeTips)
            {
                canvas.DrawText(
                    $"{tip.Key}: ({tip.Position.X:0.#}, {tip.Position.Y:0.#})",
                    x,
                    y,
                    _detailPaint);
                y += 18f;
            }
        }
    }

    private void DrawLegendRow(SKCanvas canvas, float x, float y, SKColor color, string label)
    {
        _legendSwatchPaint.Color = color;
        canvas.DrawCircle(x + 5f, y - 4f, 5f, _legendSwatchPaint);
        canvas.DrawText(label, x + 18f, y, _detailPaint);
    }

    private string ResolveStatus()
    {
        if (_isPlaying)
        {
            return "running";
        }

        if (_machine?.IsCompleted == true)
        {
            return "settled";
        }

        if ((_trace?.SelectedContext?.State.ResidualTension ?? 0m) > GlyphGrowthDefaults.ResidualTensionThreshold)
        {
            return "relaxing";
        }

        return "paused";
    }

    private GlyphPageLayout ComputeLayout()
    {
        float cardLeft = 20f;
        float cardTop = 124f;
        float cardRight = 900f;
        float cardBottom = 790f;
        var card = new SKRoundRect(new SKRect(cardLeft, cardTop, cardRight, cardBottom), 24f, 24f);

        var stageRect = new SKRect(cardLeft + 18f, cardTop + 18f, cardLeft + 600f, cardBottom - 18f);
        var detailRect = new SKRect(stageRect.Right + 16f, cardTop + 18f, cardRight - 18f, cardBottom - 18f);
        return new GlyphPageLayout(card, stageRect, detailRect);
    }

    private Button CreateUiButton(string text, int width)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 28,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(58, 58, 58),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleCenter,
            TabStop = false,
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(236, 244, 246);
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 249, 250);
        return button;
    }

    private sealed record GlyphPageLayout(
        SKRoundRect Card,
        SKRect StageRect,
        SKRect DetailRect);

    private sealed record GlyphSceneMapper(
        SKRect BoxRect,
        float Scale,
        float OffsetX,
        float OffsetY,
        GlyphBox Box)
    {
        public static GlyphSceneMapper Create(SKRect rect, GlyphBox box)
        {
            float padding = 28f;
            float usableWidth = rect.Width - padding * 2f;
            float usableHeight = rect.Height - padding * 2f;
            float scale = Math.Min(usableWidth / (float)box.Width, usableHeight / (float)box.Height);
            float offsetX = rect.Left + padding + (usableWidth - (float)box.Width * scale) * 0.5f;
            float offsetY = rect.Bottom - padding - (usableHeight - (float)box.Height * scale) * 0.5f;
            var boxRect = new SKRect(
                offsetX,
                offsetY - (float)box.Height * scale,
                offsetX + (float)box.Width * scale,
                offsetY);
            return new GlyphSceneMapper(boxRect, scale, offsetX, offsetY, box);
        }

        public SKPoint Map(GlyphVector point) =>
            new(
                OffsetX + (float)(point.X - Box.Left) * Scale,
                OffsetY - (float)(point.Y - Box.Bottom) * Scale);
    }
}
