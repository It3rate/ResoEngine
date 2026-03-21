using Core2.Symbolics.Expressions;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ResoEngine.Visualizer.Pages;

public sealed class SymbolicWorkbenchPage : IVisualizerPage
{
    private readonly Dictionary<string, string> _examples = new(StringComparer.Ordinal)
    {
        ["Transform"] = "1 * i",
        ["Fold"] = "fold([3/1]i + [12/-1])",
        ["Boolean"] = "xor([0/1]i + [10/1], [-3/1]i + [5/1])",
        ["Multiply"] = "(3i+2) * (4i+5)",
        ["Divide"] = "(3i+2) / (4i+5)",
        ["Constraint"] = "constraints{require(glyph, share(P4.u, P3.u)) | prefer(box, branch{1 | i} == i, 2/1)}",
        ["Commit"] = "let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice",
    };

    private readonly Dictionary<string, Button> _exampleButtons = new(StringComparer.Ordinal);

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private Panel? _inputPanel;
    private Panel? _inspectionPanel;
    private Label? _panelTitleLabel;
    private Label? _examplesLabel;
    private Label? _inputLabel;
    private Label? _inspectionTitleLabel;
    private FlowLayoutPanel? _exampleButtonPanel;
    private TextBox? _inputEditor;
    private TextBox? _inspectionViewer;
    private Button? _inspectionCopyButton;
    private Bitmap? _copyIcon;
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
        var inputRect = new SKRect(margin, 112f, _coords.Width - margin, 312f);
        var outputRect = new SKRect(margin, 332f, _coords.Width - margin, 762f);

        LayoutInputPanel(inputRect);
        LayoutInspectionPanel(outputRect);

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

        _inspectionPanel = new Panel
        {
            BackColor = Color.FromArgb(252, 252, 252),
        };
        _inspectionPanel.Resize += (_, _) => LayoutInspectionPanelContents();

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

        _inspectionTitleLabel = new Label
        {
            Text = "Inspection",
            Font = new Font(VisualStyle.UiFontFamily, 10.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(56, 56, 56),
            AutoSize = false,
        };

        _inspectionViewer = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            WordWrap = false,
            ScrollBars = ScrollBars.Both,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 10.25f, FontStyle.Regular),
            BackColor = Color.FromArgb(252, 252, 252),
            ForeColor = Color.FromArgb(48, 48, 48),
            ShortcutsEnabled = true,
        };

        _inputPanel.Controls.Add(_panelTitleLabel);
        _inputPanel.Controls.Add(_examplesLabel);
        _inputPanel.Controls.Add(_exampleButtonPanel);
        _inputPanel.Controls.Add(_inputLabel);
        _inputPanel.Controls.Add(_inputEditor);
        _canvasHost.Controls.Add(_inputPanel);
        _inputPanel.BringToFront();

        _copyIcon = CreateCopyIcon();
        _inspectionCopyButton = new Button
        {
            Width = 24,
            Height = 24,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(252, 252, 252),
            Image = _copyIcon,
            TabStop = false,
            Cursor = Cursors.Hand,
            Visible = true,
        };
        _inspectionCopyButton.FlatAppearance.BorderSize = 0;
        _inspectionCopyButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(232, 232, 232);
        _inspectionCopyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        _inspectionCopyButton.Click += OnInspectionCopyClicked;

        _inspectionPanel.Controls.Add(_inspectionTitleLabel);
        _inspectionPanel.Controls.Add(_inspectionCopyButton);
        _inspectionPanel.Controls.Add(_inspectionViewer);
        _canvasHost.Controls.Add(_inspectionPanel);
        _inspectionPanel.BringToFront();

        _inputEditor.Text = _examples[_activeExample];
        UpdateExampleButtonStyles();
        LayoutInputPanelContents();
        LayoutInspectionPanelContents();
    }

    private Button CreateExampleButton(string name, string source)
    {
        var button = new Button
        {
            Text = name,
            AutoSize = false,
            Width = 110,
            Height = 32,
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

        if (_inspectionPanel is not null)
        {
            if (_canvasHost is not null)
            {
                _canvasHost.Controls.Remove(_inspectionPanel);
            }

            _inspectionPanel.Dispose();
            _inspectionPanel = null;
        }

        if (_inspectionCopyButton is not null)
        {
            _inspectionCopyButton.Click -= OnInspectionCopyClicked;
            _inspectionCopyButton = null;
        }

        _copyIcon?.Dispose();
        _copyIcon = null;

        _panelTitleLabel = null;
        _examplesLabel = null;
        _inputLabel = null;
        _inspectionTitleLabel = null;
        _exampleButtonPanel = null;
        _inputEditor = null;
        _inspectionViewer = null;
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
        if (_inspectionCopyButton is not null)
        {
            _inspectionCopyButton.Enabled = _report is not null;
        }
        RefreshInspectionViewer();
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

    private void LayoutInspectionPanel(SKRect viewRect)
    {
        if (_canvasHost is null || _coords is null || _inspectionPanel is null)
        {
            return;
        }

        var pixelRect = ViewToPixelRect(viewRect);
        _inspectionPanel.SetBounds(pixelRect.Left + 10, pixelRect.Top + 18, pixelRect.Width - 20, pixelRect.Height - 28);
        _inspectionPanel.BringToFront();
        LayoutInspectionPanelContents();
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
        _exampleButtonPanel.SetBounds(padding, y, width, 80);
        y += 86;
        _inputLabel.SetBounds(padding, y, width, 18);
        y += 22;
        _inputEditor.SetBounds(padding, y, width, Math.Max(80, _inputPanel.ClientSize.Height - y - padding));
    }

    private void LayoutInspectionPanelContents()
    {
        if (_inspectionPanel is null ||
            _inspectionTitleLabel is null ||
            _inspectionCopyButton is null ||
            _inspectionViewer is null)
        {
            return;
        }

        int padding = 12;
        int titleHeight = 22;
        int copySize = 24;

        _inspectionTitleLabel.SetBounds(padding, padding, Math.Max(100, _inspectionPanel.ClientSize.Width - padding * 3 - copySize), titleHeight);
        _inspectionCopyButton.Location = new Point(
            Math.Max(padding, _inspectionPanel.ClientSize.Width - padding - copySize),
            padding - 1);

        int editorTop = padding + titleHeight + 10;
        _inspectionViewer.SetBounds(
            padding,
            editorTop,
            Math.Max(100, _inspectionPanel.ClientSize.Width - padding * 2),
            Math.Max(80, _inspectionPanel.ClientSize.Height - editorTop - padding));

        RefreshInspectionViewer();
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

    private void RefreshInspectionViewer()
    {
        if (_inspectionViewer is null)
        {
            return;
        }

        _inspectionViewer.Text = BuildInspectionDisplayText();
        _inspectionViewer.SelectionStart = 0;
        _inspectionViewer.SelectionLength = 0;
        _inspectionViewer.ScrollToCaret();
    }

    private void DrawCard(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _cardStrokePaint);
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n');

    private void OnInspectionCopyClicked(object? sender, EventArgs e)
    {
        string text = _report is not null
            ? SymbolicInspectionExporter.Export(_report)
            : _inputEditor?.Text ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(text))
        {
            Clipboard.SetText(text);
        }
    }

    private static string FitSingleLineWithEllipsis(string text, float width, SKPaint paint)
        => text;

    private static Bitmap CreateCopyIcon()
    {
        var bitmap = new Bitmap(16, 16);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var shadowPen = new Pen(Color.FromArgb(160, 160, 160), 1.3f);
        using var frontPen = new Pen(Color.FromArgb(110, 110, 110), 1.3f);

        graphics.DrawRectangle(shadowPen, 5, 2, 7, 9);
        graphics.DrawRectangle(frontPen, 3, 5, 7, 9);
        return bitmap;
    }

    private string BuildInspectionDisplayText()
    {
        if (_report is null)
        {
            return "Enter a symbolic expression to inspect.";
        }

        string text = SymbolicInspectionDisplayFormatter.Format(_report);
        if (_inspectionViewer is null)
        {
            return text;
        }

        var lines = NormalizeLineEndings(text).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("Canonical: ", StringComparison.Ordinal))
            {
                lines[i] = FitViewerLineWithEllipsis(lines[i]);
                break;
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string FitViewerLineWithEllipsis(string text)
    {
        const int maxVisibleCharacters = 200;
        if (text.Length <= maxVisibleCharacters)
        {
            return text;
        }

        return string.Concat(text.AsSpan(0, maxVisibleCharacters - 3), "...");
    }
}
