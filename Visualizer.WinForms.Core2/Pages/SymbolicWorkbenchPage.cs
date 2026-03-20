using Core2.Symbolics.Expressions;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;
using System.Drawing;
using System.Windows.Forms;

namespace ResoEngine.Visualizer.Pages;

public sealed class SymbolicWorkbenchPage : IVisualizerPage
{
    private readonly Dictionary<string, string> _examples = new(StringComparer.Ordinal)
    {
        ["Transform"] = "1 * i",
        ["Fold"] = "fold([3/1]i + [12/-1])",
        ["Boolean"] = "xor([0/1]i + [10/1], [-3/1]i + [5/1])",
        ["Constraint"] = "constraints{require(glyph, share(P4.u, P3.u)) | prefer(box, branch{1 | i} == i, 2/1)}",
        ["Commit"] = "let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice",
    };

    private readonly Dictionary<string, Button> _exampleButtons = new(StringComparer.Ordinal);

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private Panel? _inputPanel;
    private Label? _panelTitleLabel;
    private Label? _examplesLabel;
    private Label? _inputLabel;
    private FlowLayoutPanel? _exampleButtonPanel;
    private TextBox? _inputEditor;
    private SymbolicInspectionReport? _report;
    private string _activeExample = "Commit";

    private readonly SKPaint _headingPaint = new()
    {
        Color = new SKColor(38, 38, 38),
        TextSize = 25f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _bodyPaint = new()
    {
        Color = new SKColor(96, 96, 96),
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
    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(58, 58, 58),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _smallLabelPaint = new()
    {
        Color = new SKColor(94, 94, 94),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _monoPaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _errorPaint = new()
    {
        Color = new SKColor(184, 58, 58),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    public string Title => "Symbolic Workbench";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
        EnsureControls();
        RefreshReport();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords is null)
        {
            return;
        }

        const float margin = 28f;
        var inputRect = new SKRect(margin, 112f, 258f, 762f);
        var outputRect = new SKRect(276f, 112f, _coords.Width - margin, 762f);

        LayoutInputPanel(inputRect);

        canvas.DrawText("Symbolic Workbench", margin, 42f, _headingPaint);
        float introY = 74f;
        PageChrome.DrawWrappedText(
            canvas,
            "Parse, elaborate, reduce, evaluate, and negotiate native Core 2 symbolic terms. The input panel is live, so you can use it as a small grammar workbench instead of reading the symbolic layer only through tests.",
            margin,
            ref introY,
            _coords.Width - margin * 2f,
            _bodyPaint);

        DrawCard(canvas, inputRect);
        DrawCard(canvas, outputRect);
        DrawInputCardText(canvas, inputRect);
        DrawOutputArea(canvas, outputRect);
    }

    public void Destroy()
    {
        RemoveControls();
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardStrokePaint.Dispose();
        _labelPaint.Dispose();
        _smallLabelPaint.Dispose();
        _monoPaint.Dispose();
        _errorPaint.Dispose();
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _inputPanel is not null)
        {
            return;
        }

        _inputPanel = new Panel
        {
            BackColor = Color.FromArgb(252, 252, 252),
        };
        _inputPanel.Resize += (_, _) => LayoutInputPanelContents();

        _panelTitleLabel = new Label
        {
            Text = "Input",
            Font = new Font(VisualStyle.UiFontFamily, 10.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(56, 56, 56),
            AutoSize = false,
        };

        _examplesLabel = new Label
        {
            Text = "Examples",
            Font = new Font(VisualStyle.UiFontFamily, 8.8f, FontStyle.Bold),
            ForeColor = Color.FromArgb(108, 108, 108),
            AutoSize = false,
        };

        _exampleButtonPanel = new FlowLayoutPanel
        {
            WrapContents = true,
            AutoScroll = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };

        foreach (var example in _examples)
        {
            var button = CreateExampleButton(example.Key, example.Value);
            _exampleButtons[example.Key] = button;
            _exampleButtonPanel.Controls.Add(button);
        }

        _inputLabel = new Label
        {
            Text = "Expression",
            Font = new Font(VisualStyle.UiFontFamily, 8.8f, FontStyle.Bold),
            ForeColor = Color.FromArgb(108, 108, 108),
            AutoSize = false,
        };

        _inputEditor = new TextBox
        {
            Multiline = true,
            AcceptsReturn = true,
            AcceptsTab = true,
            WordWrap = false,
            ScrollBars = ScrollBars.Both,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 10.5f, FontStyle.Regular),
            BackColor = Color.FromArgb(252, 252, 252),
            ForeColor = Color.FromArgb(48, 48, 48),
        };
        _inputEditor.TextChanged += OnInputChanged;

        _inputPanel.Controls.Add(_panelTitleLabel);
        _inputPanel.Controls.Add(_examplesLabel);
        _inputPanel.Controls.Add(_exampleButtonPanel);
        _inputPanel.Controls.Add(_inputLabel);
        _inputPanel.Controls.Add(_inputEditor);
        _canvasHost.Controls.Add(_inputPanel);
        _inputPanel.BringToFront();

        _inputEditor.Text = _examples[_activeExample];
        UpdateExampleButtonStyles();
        LayoutInputPanelContents();
    }

    private Button CreateExampleButton(string name, string source)
    {
        var button = new Button
        {
            Text = name,
            AutoSize = false,
            Width = 78,
            Height = 28,
            Margin = new Padding(0, 0, 8, 8),
            FlatStyle = FlatStyle.Flat,
            Font = new Font(VisualStyle.UiFontFamily, 8.8f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        button.FlatAppearance.BorderSize = 1;
        button.Click += (_, _) =>
        {
            _activeExample = name;
            if (_inputEditor is not null)
            {
                _inputEditor.Text = source;
                _inputEditor.Focus();
                _inputEditor.SelectionStart = _inputEditor.TextLength;
            }

            UpdateExampleButtonStyles();
        };

        return button;
    }

    private void RemoveControls()
    {
        if (_inputEditor is not null)
        {
            _inputEditor.TextChanged -= OnInputChanged;
        }

        if (_inputPanel is not null)
        {
            if (_canvasHost is not null)
            {
                _canvasHost.Controls.Remove(_inputPanel);
            }

            _inputPanel.Dispose();
            _inputPanel = null;
        }

        _panelTitleLabel = null;
        _examplesLabel = null;
        _inputLabel = null;
        _exampleButtonPanel = null;
        _inputEditor = null;
        _exampleButtons.Clear();
    }

    private void OnInputChanged(object? sender, EventArgs e)
    {
        RefreshReport();
        UpdateExampleButtonStyles();
    }

    private void RefreshReport()
    {
        string text = _inputEditor?.Text ?? string.Empty;
        _report = string.IsNullOrWhiteSpace(text)
            ? null
            : SymbolicInspector.Inspect(text);
        _canvasHost?.InvalidateCanvas();
    }

    private void UpdateExampleButtonStyles()
    {
        if (_inputEditor is null)
        {
            return;
        }

        string current = NormalizeLineEndings(_inputEditor.Text).Trim();
        foreach (var entry in _examples)
        {
            bool isActive = string.Equals(current, NormalizeLineEndings(entry.Value).Trim(), StringComparison.Ordinal);
            if (_exampleButtons.TryGetValue(entry.Key, out var button))
            {
                button.BackColor = isActive ? Color.FromArgb(239, 246, 255) : Color.FromArgb(255, 255, 255);
                button.ForeColor = isActive ? Color.FromArgb(42, 94, 165) : Color.FromArgb(74, 74, 74);
                button.FlatAppearance.BorderColor = isActive
                    ? Color.FromArgb(110, 158, 220)
                    : Color.FromArgb(214, 214, 214);
            }
        }
    }

    private void LayoutInputPanel(SKRect viewRect)
    {
        if (_canvasHost is null || _coords is null || _inputPanel is null)
        {
            return;
        }

        var pixelRect = ViewToPixelRect(viewRect);
        _inputPanel.SetBounds(pixelRect.Left + 10, pixelRect.Top + 18, pixelRect.Width - 20, pixelRect.Height - 28);
        _inputPanel.BringToFront();
        LayoutInputPanelContents();
    }

    private void LayoutInputPanelContents()
    {
        if (_inputPanel is null ||
            _panelTitleLabel is null ||
            _examplesLabel is null ||
            _exampleButtonPanel is null ||
            _inputLabel is null ||
            _inputEditor is null)
        {
            return;
        }

        int padding = 12;
        int y = padding;
        int width = Math.Max(100, _inputPanel.ClientSize.Width - padding * 2);

        _panelTitleLabel.SetBounds(padding, y, width, 22);
        y += 28;
        _examplesLabel.SetBounds(padding, y, width, 18);
        y += 22;
        _exampleButtonPanel.SetBounds(padding, y, width, 72);
        y += 78;
        _inputLabel.SetBounds(padding, y, width, 18);
        y += 22;
        _inputEditor.SetBounds(padding, y, width, Math.Max(80, _inputPanel.ClientSize.Height - y - padding));
    }

    private Rectangle ViewToPixelRect(SKRect rect)
    {
        if (_canvasHost is null || _coords is null)
        {
            return Rectangle.Empty;
        }

        float scaleX = _canvasHost.ClientSize.Width / _coords.Width;
        float scaleY = _canvasHost.ClientSize.Height / _coords.Height;

        return Rectangle.FromLTRB(
            (int)MathF.Round(rect.Left * scaleX),
            (int)MathF.Round(rect.Top * scaleY),
            (int)MathF.Round(rect.Right * scaleX),
            (int)MathF.Round(rect.Bottom * scaleY));
    }

    private void DrawInputCardText(SKCanvas canvas, SKRect rect)
    {
        float x = rect.Left + 18f;
        float y = rect.Top + 28f;
        canvas.DrawText("Live grammar input", x, y, _labelPaint);
        y += 26f;
        PageChrome.DrawWrappedText(
            canvas,
            "The editor on the left parses the current shorthand, then traces elaboration, reduction, constraint evaluation, and negotiation using the same symbolic layer the tests are exercising.",
            x,
            ref y,
            rect.Width - 36f,
            _bodyPaint);
        y += 14f;
        canvas.DrawText("Stages", x, y, _smallLabelPaint);
        y += 20f;
        PageChrome.DrawWrappedText(
            canvas,
            "Parse: build the native symbolic term. Elaborate: resolve names through the environment. Reduce: execute the currently lawful subset. Evaluate/Negotiate: assess distributed requirements and preferences without forcing false certainty.",
            x,
            ref y,
            rect.Width - 36f,
            _bodyPaint);
    }

    private void DrawOutputArea(SKCanvas canvas, SKRect rect)
    {
        float x = rect.Left + 18f;
        float y = rect.Top + 28f;
        float width = rect.Width - 36f;

        canvas.DrawText("Inspection", x, y, _labelPaint);
        y += 24f;

        if (_report is null)
        {
            DrawMessageCard(canvas, x, y, width, "Enter a symbolic expression to inspect.", _bodyPaint);
            return;
        }

        if (_report.HasError)
        {
            DrawErrorCard(canvas, x, y, width, _report.Error ?? "Unknown symbolic error.");
            return;
        }

        if (_report.Parsed is null)
        {
            DrawErrorCard(canvas, x, y, width, "The symbolic input did not produce a parsed term.");
            return;
        }

        y = DrawSummaryCard(canvas, x, y, width, _report);
        y += 12f;

        int maxSteps = Math.Min(3, _report.Steps.Count);
        for (int i = 0; i < maxSteps; i++)
        {
            y = DrawStepCard(canvas, x, y, width, _report.Steps[i]);
            y += 12f;
        }

        if (_report.Steps.Count > maxSteps)
        {
            y = DrawMessageCard(
                canvas,
                x,
                y,
                width,
                $"+{_report.Steps.Count - maxSteps} additional step(s) hidden in this preview. The reduction and final environment below still reflect the full trace.",
                _bodyPaint);
            y += 12f;
        }

        DrawEnvironmentCard(canvas, x, y, width, _report.FinalEnvironment);
    }

    private float DrawSummaryCard(SKCanvas canvas, float x, float y, float width, SymbolicInspectionReport report)
    {
        string parsedFriendly = SymbolicTermFormatter.Format(report.Parsed!);
        string canonical = CanonicalSymbolicSerializer.Serialize(report.Parsed!);
        string finalOutput = report.FinalStep?.Reduction.Output is null
            ? "(none)"
            : SymbolicTermFormatter.Format(report.FinalStep.Reduction.Output);

        float bodyWidth = width - 26f;
        float height = 26f
            + MeasureFieldHeight("parsed", parsedFriendly, bodyWidth, 4)
            + MeasureFieldHeight("canonical", canonical, bodyWidth, 4)
            + MeasureFieldHeight("final", finalOutput, bodyWidth, 3)
            + 10f;

        DrawCard(canvas, new SKRect(x, y, x + width, y + height));
        canvas.DrawText("Top-Level Term", x + 14f, y + 18f, _smallLabelPaint);

        float contentY = y + 26f;
        contentY = DrawField(canvas, "parsed", parsedFriendly, x + 14f, contentY, bodyWidth, 4);
        contentY = DrawField(canvas, "canonical", canonical, x + 14f, contentY, bodyWidth, 4);
        DrawField(canvas, "final", finalOutput, x + 14f, contentY, bodyWidth, 3);
        return y + height;
    }

    private float DrawStepCard(SKCanvas canvas, float x, float y, float width, SymbolicInspectionStep step)
    {
        string source = SymbolicTermFormatter.Format(step.Source);
        string elaborated = step.Elaboration.Output is null ? "(none)" : SymbolicTermFormatter.Format(step.Elaboration.Output);
        string reduced = step.Reduction.Output is null ? "(none)" : SymbolicTermFormatter.Format(step.Reduction.Output);

        var metaLines = new List<string>
        {
            $"label: {step.Label}",
        };

        if (step.Evaluation is not null)
        {
            metaLines.Add(
                $"constraints: {step.Evaluation.Items.Count} item(s), unresolved req {step.Evaluation.HasUnresolvedRequirements.ToString().ToLowerInvariant()}, pref {step.Evaluation.SatisfiedPreferenceWeight}/{step.Evaluation.UnsatisfiedPreferenceWeight}/{step.Evaluation.UnresolvedPreferenceWeight}");

            foreach (var participant in step.Evaluation.ParticipantSummaries.Take(3))
            {
                string name = participant.ParticipantName ?? "(unscoped)";
                metaLines.Add(
                    $"{name}: req +{participant.SatisfiedRequirements} -{participant.UnsatisfiedRequirements} ?{participant.UnresolvedRequirements}, pref +{participant.SatisfiedPreferenceWeight} -{participant.UnsatisfiedPreferenceWeight} ?{participant.UnresolvedPreferenceWeight}");
            }
        }

        if (step.Negotiation is not null)
        {
            string negotiationText = step.Negotiation.Status switch
            {
                ConstraintNegotiationStatus.Selected when step.Negotiation.SelectedCandidate is not null
                    => $"negotiation: Selected -> {SymbolicTermFormatter.Format(step.Negotiation.SelectedCandidate)}",
                ConstraintNegotiationStatus.PreservedCandidates when step.Negotiation.PreservedCandidateFamily is not null
                    => $"negotiation: Preserved -> {SymbolicTermFormatter.Format(step.Negotiation.PreservedCandidateFamily)}",
                _ => $"negotiation: {step.Negotiation.Status}",
            };
            metaLines.Add(negotiationText);
            if (!string.IsNullOrWhiteSpace(step.Negotiation.Note))
            {
                metaLines.Add(step.Negotiation.Note!);
            }
        }

        string notes = string.Join(Environment.NewLine, metaLines);
        float bodyWidth = width - 26f;
        float height = 26f
            + MeasureFieldHeight("source", source, bodyWidth, 3)
            + MeasureFieldHeight("elaborated", elaborated, bodyWidth, 3)
            + MeasureFieldHeight("reduced", reduced, bodyWidth, 3)
            + MeasureFieldHeight("notes", notes, bodyWidth, 5)
            + 10f;

        DrawCard(canvas, new SKRect(x, y, x + width, y + height));
        canvas.DrawText($"Step {step.Index}", x + 14f, y + 18f, _smallLabelPaint);

        float contentY = y + 26f;
        contentY = DrawField(canvas, "source", source, x + 14f, contentY, bodyWidth, 3);
        contentY = DrawField(canvas, "elaborated", elaborated, x + 14f, contentY, bodyWidth, 3);
        contentY = DrawField(canvas, "reduced", reduced, x + 14f, contentY, bodyWidth, 3);
        DrawField(canvas, "notes", notes, x + 14f, contentY, bodyWidth, 5);
        return y + height;
    }

    private void DrawEnvironmentCard(SKCanvas canvas, float x, float y, float width, SymbolicEnvironment environment)
    {
        string text = FormatEnvironmentBindings(environment);
        float bodyWidth = width - 26f;
        float height = 26f + MeasureFieldHeight("bindings", text, bodyWidth, 14) + 10f;

        DrawCard(canvas, new SKRect(x, y, x + width, y + height));
        canvas.DrawText("Final Environment", x + 14f, y + 18f, _smallLabelPaint);
        DrawField(canvas, "bindings", text, x + 14f, y + 26f, bodyWidth, 14);
    }

    private float DrawMessageCard(SKCanvas canvas, float x, float y, float width, string text, SKPaint paint)
    {
        var lines = WrapCodeText(text, width - 28f, paint, 4);
        float height = 28f + lines.Count * (paint.TextSize + 4f) + 12f;
        DrawCard(canvas, new SKRect(x, y, x + width, y + height));

        float lineY = y + 22f;
        foreach (var line in lines)
        {
            canvas.DrawText(line, x + 14f, lineY, paint);
            lineY += paint.TextSize + 4f;
        }

        return y + height;
    }

    private void DrawErrorCard(SKCanvas canvas, float x, float y, float width, string error)
    {
        var lines = WrapCodeText(error, width - 28f, _bodyPaint, 6);
        float height = 34f + lines.Count * (_bodyPaint.TextSize + 4f) + 14f;
        DrawCard(canvas, new SKRect(x, y, x + width, y + height));
        canvas.DrawText("Parse / Inspection Error", x + 14f, y + 20f, _errorPaint);

        float lineY = y + 42f;
        foreach (var line in lines)
        {
            canvas.DrawText(line, x + 14f, lineY, _bodyPaint);
            lineY += _bodyPaint.TextSize + 4f;
        }
    }

    private void DrawCard(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _cardStrokePaint);
    }

    private float DrawField(
        SKCanvas canvas,
        string label,
        string value,
        float x,
        float y,
        float width,
        int maxLines)
    {
        var lines = WrapCodeText($"{label}: {value}", width, _monoPaint, maxLines);
        float lineY = y;

        foreach (var line in lines)
        {
            canvas.DrawText(line, x, lineY + _monoPaint.TextSize, _monoPaint);
            lineY += _monoPaint.TextSize + 4f;
        }

        return lineY + 8f;
    }

    private float MeasureFieldHeight(string label, string value, float width, int maxLines)
    {
        int lineCount = WrapCodeText($"{label}: {value}", width, _monoPaint, maxLines).Count;
        return lineCount * (_monoPaint.TextSize + 4f) + 8f;
    }

    private static List<string> WrapCodeText(string text, float width, SKPaint paint, int maxLines)
    {
        if (string.IsNullOrEmpty(text))
        {
            return ["(empty)"];
        }

        var lines = new List<string>();
        var paragraphs = NormalizeLineEndings(text).Split('\n');

        foreach (var paragraph in paragraphs)
        {
            if (string.IsNullOrEmpty(paragraph))
            {
                lines.Add(string.Empty);
                if (lines.Count >= maxLines)
                {
                    return ClampLines(lines, maxLines);
                }

                continue;
            }

            string current = string.Empty;
            foreach (char ch in paragraph)
            {
                string candidate = string.Concat(current, ch);
                if (string.IsNullOrEmpty(current) || paint.MeasureText(candidate) <= width)
                {
                    current = candidate;
                    continue;
                }

                lines.Add(current);
                if (lines.Count >= maxLines)
                {
                    return ClampLines(lines, maxLines);
                }

                current = ch.ToString();
            }

            if (!string.IsNullOrEmpty(current))
            {
                lines.Add(current);
                if (lines.Count >= maxLines)
                {
                    return ClampLines(lines, maxLines);
                }
            }
        }

        return lines;
    }

    private static List<string> ClampLines(List<string> lines, int maxLines)
    {
        if (lines.Count <= maxLines)
        {
            return lines;
        }

        return
        [
            .. lines.Take(maxLines - 1),
            $"{lines[maxLines - 1]}...",
        ];
    }

    private static string FormatEnvironmentBindings(SymbolicEnvironment environment)
    {
        var bindings = environment.Bindings.ToArray();
        if (bindings.Length == 0)
        {
            return "(no bindings)";
        }

        return string.Join(
            Environment.NewLine,
            bindings
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => $"{pair.Key} = {SymbolicTermFormatter.Format(pair.Value)}"));
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n');
}
