using System.Drawing;
using System.Drawing.Drawing2D;
using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class OrthogonalAxesPage : IVisualizerPage
{
    public string Title => "Orthogonal Axes (Core2)";

    private readonly AxisDisplayMapper _axisA = new(
        new Axis(new Proportion(3, 1), new Proportion(5, 1)),
        "A");
    private readonly AxisDisplayMapper _axisB = new(
        new Axis(new Proportion(2, 1), new Proportion(5, 1)),
        "B");

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private GridRenderer? _gridRenderer;

    private Panel? _formulaPanel;
    private readonly List<FormulaRowUi> _formulaRows = [];
    private Bitmap? _copyIcon;

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
    private readonly SKPaint _quadrantTextPaint = new()
    {
        Color = new SKColor(50, 50, 50),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _quadrantBgPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255, 235),
        IsAntialias = true,
    };
    private readonly SKPaint _quadrantBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205, 230),
        IsAntialias = true,
    };

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width / 2;
        coords.OriginY = coords.Height / 2;

        _gridRenderer = new GridRenderer(coords);
        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: 0);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Vertical, SegmentColors.Blue, crossPosition: 0);

        hitTest.Register(_rendererA, _axisA);
        hitTest.Register(_rendererB, _axisB);

        EnsureFormulaOverlay();
        UpdateFormulaOverlay();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        SyncInputsFromDisplay();
        var area = _axisA.Axis.Pin(_axisB.Axis);

        _gridRenderer?.Render(canvas, _axisA, _axisB, SegmentColors.Red, SegmentColors.Blue);
        DrawQuadrantValues(canvas, area);
        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);

        UpdateFormulaOverlay();
    }

    private void SyncInputsFromDisplay()
    {
    }

    private void DrawQuadrantValues(SKCanvas canvas, Area area)
    {
        if (_coords == null)
        {
            return;
        }

        var terms = area.ExpandTerms();

        var hImag = CreateZeroRange(_axisA.Imaginary);
        var hReal = CreateZeroRange(_axisA.Real);
        var vImag = CreateZeroRange(_axisB.Imaginary);
        var vReal = CreateZeroRange(_axisB.Real);

        DrawQuadrantValue(canvas, hImag, vImag, $"i*i = {N(terms.ii.Fold())}");
        DrawQuadrantValue(canvas, hImag, vReal, $"i*r = {N(terms.ir.Fold())}i");
        DrawQuadrantValue(canvas, hReal, vImag, $"r*i = {N(terms.ri.Fold())}i");
        DrawQuadrantValue(canvas, hReal, vReal, $"r*r = {N(terms.rr.Fold())}");
    }

    private void DrawQuadrantValue(SKCanvas canvas, AxisRange xRange, AxisRange yRange, string text)
    {
        if (_coords == null || !xRange.HasSpan || !yRange.HasSpan)
        {
            return;
        }

        float centerX = (xRange.Min + xRange.Max) * 0.5f;
        float centerY = (yRange.Min + yRange.Max) * 0.5f;
        var center = _coords.MathToPixel(centerX, centerY);

        var bounds = new SKRect();
        _quadrantTextPaint.MeasureText(text, ref bounds);

        const float padX = 10f;
        const float padY = 6f;
        var rect = new SKRect(
            center.X - bounds.Width * 0.5f - padX,
            center.Y - bounds.Height * 0.5f - padY,
            center.X + bounds.Width * 0.5f + padX,
            center.Y + bounds.Height * 0.5f + padY);

        canvas.DrawRoundRect(rect, 8f, 8f, _quadrantBgPaint);
        canvas.DrawRoundRect(rect, 8f, 8f, _quadrantBorderPaint);
        canvas.DrawText(text, center.X, center.Y + bounds.Height * 0.35f, _quadrantTextPaint);
    }

    private void EnsureFormulaOverlay()
    {
        if (_canvasHost == null || _formulaPanel != null)
        {
            return;
        }

        _copyIcon ??= CreateCopyIcon();

        _formulaPanel = new Panel
        {
            BackColor = Color.Transparent,
            Visible = true,
            TabStop = false,
        };

        for (int i = 0; i < 3; i++)
        {
            var rowPanel = new Panel
            {
                BackColor = Color.White,
                Height = 32,
                Padding = new Padding(6, 6, 4, 4),
            };

            var textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 11f, FontStyle.Regular),
                TextAlign = HorizontalAlignment.Center,
                TabStop = true,
                ShortcutsEnabled = true,
            };

            var button = new Button
            {
                Width = 26,
                Height = 26,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = _copyIcon,
                TabStop = false,
                Cursor = Cursors.Hand,
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 235, 235);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);

            button.Click += (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    Clipboard.SetText(textBox.Text);
                }
            };

            rowPanel.Controls.Add(textBox);
            rowPanel.Controls.Add(button);
            _formulaPanel.Controls.Add(rowPanel);
            _formulaRows.Add(new FormulaRowUi(rowPanel, textBox, button));
        }

        _canvasHost.Controls.Add(_formulaPanel);
        _formulaPanel.BringToFront();
    }

    private void UpdateFormulaOverlay()
    {
        if (_canvasHost == null || _formulaPanel == null)
        {
            return;
        }

        string[] lines = BuildFormulaLines();
        using var textMeasureGraphics = _formulaPanel.CreateGraphics();

        int maxWidth = 0;
        for (int i = 0; i < _formulaRows.Count; i++)
        {
            var row = _formulaRows[i];
            row.TextBox.Text = lines[i];
            var textSize = TextRenderer.MeasureText(
                textMeasureGraphics,
                lines[i],
                row.TextBox.Font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding);
            int rowWidth = textSize.Width + row.CopyButton.Width + 28;
            maxWidth = Math.Max(maxWidth, rowWidth);
        }

        int rowHeight = 34;
        int rowGap = 10;
        int innerWidth = Math.Max(220, maxWidth);
        for (int i = 0; i < _formulaRows.Count; i++)
        {
            var row = _formulaRows[i];
            int top = i * (rowHeight + rowGap);
            row.RowPanel.SetBounds(0, top, innerWidth, rowHeight);
            row.CopyButton.SetBounds(innerWidth - row.CopyButton.Width - 4, 4, row.CopyButton.Width, row.CopyButton.Height);
            row.TextBox.SetBounds(8, 9, innerWidth - row.CopyButton.Width - 18, 18);
        }

        _formulaPanel.Size = new Size(innerWidth, _formulaRows.Count * rowHeight + (_formulaRows.Count - 1) * rowGap);
        PositionFormulaOverlay();
        _formulaPanel.BringToFront();
    }

    private void PositionFormulaOverlay()
    {
        if (_canvasHost == null || _formulaPanel == null)
        {
            return;
        }

        int panelWidth = _formulaPanel.Width;
        int panelHeight = _formulaPanel.Height;
        int x = (_canvasHost.ClientSize.Width - panelWidth) / 2;
        int y = _canvasHost.ClientSize.Height - panelHeight - 18;

        x = Math.Max(8, x);
        y = Math.Max(8, y);

        _formulaPanel.Location = new Point(x, y);
    }

    private string[] BuildFormulaLines()
    {
        var axisA = _axisA.Axis;
        var axisB = _axisB.Axis;
        var area = axisA.Pin(axisB);
        var terms = area.ExpandTerms();
        var folded = area.Fold();

        decimal ai = axisA.Recessive.Fold();
        decimal ar = axisA.Dominant.Fold();
        decimal bi = axisB.Recessive.Fold();
        decimal br = axisB.Dominant.Fold();
        decimal ii = terms.ii.Fold();
        decimal ir = terms.ir.Fold();
        decimal ri = terms.ri.Fold();
        decimal rr = terms.rr.Fold();
        decimal resultI = folded.Recessive.Fold();
        decimal resultR = folded.Dominant.Fold();

        string line1 = $"({N(ai)}i {Pm(ar)} {A(ar)})({N(bi)}i {Pm(br)} {A(br)})";
        string line2 = $"= ({N(ir)}i {Pm(ri)} {A(ri)}i)({N(rr)} {PmNeg(ii)} {A(ii)})";
        string line3 = $"= ({N(resultI)}i {Pm(resultR)} {A(resultR)})";
        return [line1, line2, line3];
    }

    private static AxisRange CreateZeroRange(float value) => new(MathF.Min(0f, value), MathF.Max(0f, value));

    private static Bitmap CreateCopyIcon()
    {
        var bitmap = new Bitmap(16, 16);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var shadowPen = new Pen(Color.FromArgb(140, 140, 140), 1.4f);
        using var frontPen = new Pen(Color.FromArgb(70, 70, 70), 1.4f);

        graphics.DrawRectangle(shadowPen, 5, 2, 7, 9);
        graphics.DrawRectangle(frontPen, 3, 5, 7, 9);
        return bitmap;
    }

    private static string N(decimal v) => decimal.Round(v, 1).ToString("0.0");
    private static string A(decimal v) => N(decimal.Abs(v));
    private static string Pm(decimal v) => v >= 0m ? "+" : "-";
    private static string PmNeg(decimal v) => v >= 0m ? "-" : "+";

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        var originPx = _coords.MathToPixel(0, 0);
        return SKPoint.Distance(pixelPoint, originPx) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_axisA, _axisB];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0, 0);

    public void Destroy()
    {
        _rendererA?.Dispose();
        _rendererA = null;
        _rendererB?.Dispose();
        _rendererB = null;
        _gridRenderer?.Dispose();
        _gridRenderer = null;

        if (_formulaPanel != null && _canvasHost != null)
        {
            _canvasHost.Controls.Remove(_formulaPanel);
            _formulaPanel.Dispose();
            _formulaPanel = null;
        }

        _formulaRows.Clear();
        _coords = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _copyIcon?.Dispose();
        _quadrantTextPaint.Dispose();
        _quadrantBgPaint.Dispose();
        _quadrantBorderPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

    private readonly record struct AxisRange(float Min, float Max)
    {
        public bool HasSpan => Max > Min;
    }

    private sealed record FormulaRowUi(Panel RowPanel, TextBox TextBox, Button CopyButton);
}
