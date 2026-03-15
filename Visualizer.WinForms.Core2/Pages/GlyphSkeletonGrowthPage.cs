using Core2.Dynamic;
using Core2.Geometry.Glyphs;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class GlyphSkeletonGrowthPage : IVisualizerPage
{
    private readonly SKPaint _fieldBitmapPaint = new()
    {
        IsAntialias = false,
        FilterQuality = SKFilterQuality.Medium,
    };

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
        Color = new SKColor(208, 216, 226, 115),
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
    private NumericUpDown? _seedInput;
    private CheckBox? _freezeSeedToggle;
    private SKBitmap? _fieldBitmap;

    private string _selectedLetterKey = "Y";
    private bool _isPlaying;
    private bool _seedInputUpdating;
    private int _currentSeed = Random.Shared.Next(1, 999_999);
    private DynamicMachine<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>? _machine;
    private DynamicTrace<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>? _trace;

    public GlyphSkeletonGrowthPage()
    {
        _timer = new System.Windows.Forms.Timer
        {
            Interval = 33,
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
        _seedInput = null;
        _freezeSeedToggle = null;
        _machine = null;
        _trace = null;
        _canvasHost = null;
        _fieldBitmap?.Dispose();
        _fieldBitmap = null;
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
        _fieldBitmapPaint.Dispose();
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
        _fieldBitmap?.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(286, 206),
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
            Maximum = 600,
            Value = (decimal)GlyphGrowthDefaults.DefaultMaxSteps,
            Increment = 1,
            DecimalPlaces = 0,
            Location = new Point(110, 79),
            Size = new Size(64, 24),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Regular),
        };
        _maxStepsInput.ValueChanged += (_, _) => LoadLetter(_selectedLetterKey);

        var seedLabel = new Label
        {
            Text = "Seed",
            Location = new Point(14, 110),
            Size = new Size(48, 18),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(92, 92, 92),
        };

        _seedInput = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 999999,
            Value = _currentSeed,
            Increment = 1,
            DecimalPlaces = 0,
            Location = new Point(62, 107),
            Size = new Size(78, 24),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Regular),
        };
        _seedInput.ValueChanged += (_, _) =>
        {
            if (_seedInputUpdating)
            {
                return;
            }

            _currentSeed = (int)_seedInput.Value;
            if (_freezeSeedToggle?.Checked == true)
            {
                LoadLetter(_selectedLetterKey);
            }
        };

        _freezeSeedToggle = new CheckBox
        {
            Text = "Freeze seed",
            AutoSize = true,
            Location = new Point(154, 109),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(86, 86, 86),
            Checked = false,
            BackColor = Color.Transparent,
        };
        _freezeSeedToggle.CheckedChanged += (_, _) =>
        {
            if (_freezeSeedToggle.Checked)
            {
                _currentSeed = (int)(_seedInput?.Value ?? _currentSeed);
            }

            LoadLetter(_selectedLetterKey);
        };

        _playButton = CreateUiButton("Play", width: 68);
        _playButton.Location = new Point(14, 150);
        _playButton.Click += (_, _) =>
        {
            if (_machine?.IsCompleted == true)
            {
                LoadLetter(_selectedLetterKey);
            }

            SetPlaying(!_isPlaying);
        };

        _stepButton = CreateUiButton("Step", width: 68);
        _stepButton.Location = new Point(92, 150);
        _stepButton.Click += (_, _) =>
        {
            if (_machine?.IsCompleted == true)
            {
                LoadLetter(_selectedLetterKey);
            }

            AdvanceMachine();
        };

        _resetButton = CreateUiButton("Reset", width: 68);
        _resetButton.Location = new Point(170, 150);
        _resetButton.Click += (_, _) => LoadLetter(_selectedLetterKey);

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(_letterButtonPanel);
        _controlsPanel.Controls.Add(stepsLabel);
        _controlsPanel.Controls.Add(_maxStepsInput);
        _controlsPanel.Controls.Add(seedLabel);
        _controlsPanel.Controls.Add(_seedInput);
        _controlsPanel.Controls.Add(_freezeSeedToggle);
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
        _currentSeed = ResolveSeedForLoad();
        _machine = GlyphGrowthRuntime.CreateMachine(letterKey, (int)(_maxStepsInput?.Value ?? 12m), _currentSeed);
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
        DrawFieldBitmap(canvas, mapper, state);
        canvas.DrawRect(mapper.BoxRect, _glyphBoxPaint);

        DrawLandmarks(canvas, mapper, spec.Environment, state);
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

    private void DrawFieldBitmap(SKCanvas canvas, GlyphSceneMapper mapper, GlyphGrowthState state)
    {
        if (state.TensionField is null)
        {
            return;
        }

        UpdateFieldBitmap(state.TensionField);
        if (_fieldBitmap is not null)
        {
            canvas.DrawBitmap(_fieldBitmap, mapper.BoxRect, _fieldBitmapPaint);
        }
    }

    private void DrawLandmarks(SKCanvas canvas, GlyphSceneMapper mapper, GlyphEnvironment environment, GlyphGrowthState state)
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
                case GlyphLandmarkKind.StopPoint:
                    break;
            }
        }

        foreach (var signal in (state.AmbientSignals ?? [])
            .Where(signal => signal.TargetPosition is not null)
            .Take(10))
        {
            var point = mapper.Map(signal.Position);
            float radius = 3.5f + MathF.Min(4f, (float)signal.Magnitude * 6f);
            var paint = signal.Kind == global::Core2.Propagation.CouplingKind.Stop
                ? _stopPointPaint
                : _branchPointPaint;
            canvas.DrawCircle(point, radius, paint);
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
            $"seed: {state.RandomSeed}",
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

    private int ResolveSeedForLoad()
    {
        if (_freezeSeedToggle?.Checked == true)
        {
            return (int)(_seedInput?.Value ?? _currentSeed);
        }

        int nextSeed = Random.Shared.Next(1, 999_999);
        _seedInputUpdating = true;
        if (_seedInput is not null)
        {
            _seedInput.Value = nextSeed;
        }

        _seedInputUpdating = false;
        return nextSeed;
    }

    private unsafe void UpdateFieldBitmap(GlyphTensionField field)
    {
        if (_fieldBitmap is null || _fieldBitmap.Width != field.Width || _fieldBitmap.Height != field.Height)
        {
            _fieldBitmap?.Dispose();
            _fieldBitmap = new SKBitmap(field.Width, field.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        }

        ReadOnlySpan<float> grow = field.GrowChannel.Span;
        ReadOnlySpan<float> stop = field.StopChannel.Span;
        ReadOnlySpan<float> branch = field.BranchChannel.Span;

        byte* pixels = (byte*)_fieldBitmap.GetPixels().ToPointer();
        int rowBytes = _fieldBitmap.RowBytes;
        for (int y = 0; y < field.Height; y++)
        {
            byte* row = pixels + (y * rowBytes);
            int sourceY = field.Height - 1 - y;
            for (int x = 0; x < field.Width; x++)
            {
                int index = sourceY * field.Width + x;
                float cool = grow[index];
                float warm = stop[index];
                float branchValue = branch[index];
                float total = cool + warm + branchValue;
                float alphaFactor = Math.Clamp(total * 1.15f, 0f, 0.9f);

                row[x * 4 + 0] = (byte)Math.Clamp((int)MathF.Round(248f - warm * 56f + cool * 18f + branchValue * 28f), 170, 255);
                row[x * 4 + 1] = (byte)Math.Clamp((int)MathF.Round(246f - warm * 26f + cool * 38f + branchValue * 12f), 176, 255);
                row[x * 4 + 2] = (byte)Math.Clamp((int)MathF.Round(244f + warm * 44f - cool * 20f + branchValue * 10f), 176, 255);
                row[x * 4 + 3] = (byte)Math.Clamp((int)MathF.Round(alphaFactor * 140f), 0, 150);
            }
        }
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
