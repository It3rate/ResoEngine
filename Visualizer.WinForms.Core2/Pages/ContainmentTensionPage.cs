using Core2.Elements;
using Core2.Support;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class ContainmentTensionPage : IVisualizerPage
{
    private const float ParentY = 4.8f;
    private const float ChildY = 2.0f;
    private const float ContextY = -0.8f;

    private readonly AxisDisplayMapper _parent = new(
        Axis.FromCoordinates((Scalar)(-4m), (Scalar)7m, Scalar.One, Scalar.One),
        "Parent");

    private readonly AxisDisplayMapper _child = new(
        Axis.FromCoordinates((Scalar)(-2m), (Scalar)3.5m, Scalar.One, Scalar.One),
        "Child");

    private readonly AxisDisplayMapper _contextualChild = new(Axis.Zero, "In Parent");

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

    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _cardFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(248, 248, 248),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(190, 190, 190),
        IsAntialias = true,
    };

    private readonly SKPaint _cardTitlePaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _cardTextPaint = new()
    {
        Color = new SKColor(86, 86, 86),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _okPaint = new()
    {
        Color = new SKColor(50, 120, 70),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _tensionPaint = new()
    {
        Color = new SKColor(184, 88, 52),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
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
    private SegmentRenderer? _parentRenderer;
    private SegmentRenderer? _childRenderer;
    private SegmentRenderer? _contextRenderer;

    private Panel? _controlsPanel;
    private ComboBox? _perspectiveCombo;
    private ComboBox? _recessiveSupportCombo;
    private ComboBox? _dominantSupportCombo;

    public string Title => "Containment / Tension";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width * 0.34f;
        coords.OriginY = 360f;

        _parentRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: ParentY);
        _childRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Blue, crossPosition: ChildY);
        _contextRenderer = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Green, crossPosition: ContextY);

        hitTest.Register(_parentRenderer, _parent);
        hitTest.Register(_childRenderer, _child);

        EnsureControls();
        ApplyChildSupports();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        var relation = BuildRelation();
        var contextual = relation.ChildInParentContext as Axis ?? Axis.Zero;
        _contextualChild.SetAxis(contextual);

        canvas.DrawText("Containment and Tension", 34f, 42f, _headingPaint);
        canvas.DrawText("Drag the parent and child. Flip the parent perspective to reinterpret the child, or change child support to create tension instead of failure.", 34f, 68f, _bodyPaint);

        DrawSegmentLabel(canvas, "Parent Frame", ParentY);
        DrawSegmentLabel(canvas, "Child Raw", ChildY);
        DrawSegmentLabel(canvas, "Child In Parent Context", ContextY);

        _parentRenderer?.Render(canvas, _parent);
        _childRenderer?.Render(canvas, _child);
        _contextRenderer?.Render(canvas, _contextualChild);

        DrawRelationCard(canvas, relation, contextual);

        var originPx = _coords.MathToPixel(0f, 0f);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawSegmentLabel(SKCanvas canvas, string label, float y)
    {
        if (_coords == null)
        {
            return;
        }

        var point = _coords.MathToPixel(0f, y);
        canvas.DrawText(label, 38f, point.Y - 16f, _labelPaint);
    }

    private void DrawRelationCard(SKCanvas canvas, Containment relation, Axis contextual)
    {
        if (_coords == null)
        {
            return;
        }

        var rect = new SKRect(_coords.Width - 390f, 132f, _coords.Width - 36f, 520f);
        canvas.DrawRoundRect(rect, 16f, 16f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 16f, 16f, _cardBorderPaint);

        float x = rect.Left + 18f;
        float y = rect.Top + 28f;
        canvas.DrawText("Containment Relation", x, y, _cardTitlePaint);
        y += 30f;

        DrawWrappedText(canvas, $"Parent perspective: {SelectedPerspective}", x, ref y, rect.Width - 36f, _cardTextPaint);
        DrawWrappedText(canvas, $"Child in context: {FormatAxis(contextual)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        y += 6f;
        DrawWrappedText(canvas, relation.ToString(), x, ref y, rect.Width - 36f, _cardTextPaint);
        y += 12f;

        if (relation.Tensions.Count == 0)
        {
            canvas.DrawText("No tension", x, y, _okPaint);
            y += 24f;
            DrawWrappedText(canvas, "The child fits inside the parent envelope and shares the parent support/resolution.", x, ref y, rect.Width - 36f, _cardTextPaint);
            return;
        }

        canvas.DrawText("Tensions", x, y, _tensionPaint);
        y += 24f;

        foreach (var tension in relation.Tensions)
        {
            string label = string.IsNullOrWhiteSpace(tension.Path)
                ? tension.Kind.ToString()
                : $"{tension.Kind} ({tension.Path})";

            canvas.DrawText(label, x, y, _tensionPaint);
            y += 18f;
            DrawWrappedText(canvas, tension.Message, x, ref y, rect.Width - 36f, _cardTextPaint);
            y += 10f;

            if (y > rect.Bottom - 28f)
            {
                break;
            }
        }
    }

    private void DrawWrappedText(SKCanvas canvas, string text, float x, ref float y, float width, SKPaint paint)
    {
        foreach (var line in WrapText(text, width, paint))
        {
            canvas.DrawText(line, x, y, paint);
            y += paint.TextSize + 4f;
        }
    }

    private static IReadOnlyList<string> WrapText(string text, float width, SKPaint paint)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = string.Empty;

        foreach (var word in words)
        {
            var candidate = string.IsNullOrEmpty(current) ? word : $"{current} {word}";
            if (paint.MeasureText(candidate) <= width)
            {
                current = candidate;
                continue;
            }

            if (!string.IsNullOrEmpty(current))
            {
                lines.Add(current);
            }

            current = word;
        }

        if (!string.IsNullOrEmpty(current))
        {
            lines.Add(current);
        }

        return lines;
    }

    private Containment BuildRelation()
    {
        ApplyChildSupports();
        var parentNode = _parent.Axis.AsNode(SelectedPerspective);
        return parentNode.AddChild(_child.Axis);
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
            Size = new Size(304, 112),
            Location = new Point(690, 20),
        };

        var perspectiveLabel = new Label
        {
            Text = "Parent Perspective",
            AutoSize = true,
            Location = new Point(12, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _perspectiveCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(12, 32),
            Width = 130,
        };
        _perspectiveCombo.Items.AddRange(["Dominant", "Opposite"]);
        _perspectiveCombo.SelectedIndex = 0;
        _perspectiveCombo.SelectedIndexChanged += (_, _) => _canvasHost?.InvalidateCanvas();

        var recessiveLabel = new Label
        {
            Text = "Child i support",
            AutoSize = true,
            Location = new Point(156, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _recessiveSupportCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(156, 32),
            Width = 58,
        };
        _recessiveSupportCombo.Items.AddRange(["1", "2", "3"]);
        _recessiveSupportCombo.SelectedIndex = 0;
        _recessiveSupportCombo.SelectedIndexChanged += (_, _) =>
        {
            ApplyChildSupports();
            _canvasHost?.InvalidateCanvas();
        };

        var dominantLabel = new Label
        {
            Text = "Child r support",
            AutoSize = true,
            Location = new Point(224, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _dominantSupportCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(224, 32),
            Width = 58,
        };
        _dominantSupportCombo.Items.AddRange(["1", "2", "3"]);
        _dominantSupportCombo.SelectedIndex = 0;
        _dominantSupportCombo.SelectedIndexChanged += (_, _) =>
        {
            ApplyChildSupports();
            _canvasHost?.InvalidateCanvas();
        };

        var note = new Label
        {
            Text = "Drag the child beyond the parent to create range tension. Change either support to create resolution tension.",
            AutoSize = false,
            Location = new Point(12, 66),
            Size = new Size(274, 34),
            Font = new Font("Arial", 8f, FontStyle.Regular),
            ForeColor = Color.FromArgb(105, 105, 105),
        };

        _controlsPanel.Controls.Add(perspectiveLabel);
        _controlsPanel.Controls.Add(_perspectiveCombo);
        _controlsPanel.Controls.Add(recessiveLabel);
        _controlsPanel.Controls.Add(_recessiveSupportCombo);
        _controlsPanel.Controls.Add(dominantLabel);
        _controlsPanel.Controls.Add(_dominantSupportCombo);
        _controlsPanel.Controls.Add(note);
        _canvasHost.Controls.Add(_controlsPanel);
        _controlsPanel.BringToFront();
    }

    private void ApplyChildSupports()
    {
        var child = _child.Axis;
        var updated = Axis.FromCoordinates(
            child.Start,
            child.End,
            SelectedRecessiveSupport,
            SelectedDominantSupport,
            child.Basis);
        _child.SetAxis(updated);
    }

    private Perspective SelectedPerspective =>
        _perspectiveCombo?.SelectedItem?.ToString() == "Opposite"
            ? Perspective.Opposite
            : Perspective.Dominant;

    private Scalar SelectedRecessiveSupport => SelectedSupport(_recessiveSupportCombo);
    private Scalar SelectedDominantSupport => SelectedSupport(_dominantSupportCombo);

    private static Scalar SelectedSupport(ComboBox? combo) =>
        combo?.SelectedItem?.ToString() switch
        {
            "2" => (Scalar)2m,
            "3" => (Scalar)3m,
            _ => Scalar.One,
        };

    private static string FormatAxis(Axis axis)
    {
        decimal recessive = axis.Recessive.Fold();
        decimal dominant = axis.Dominant.Fold();

        if (recessive == 0m)
        {
            return dominant.ToString("0.0");
        }

        if (dominant == 0m)
        {
            return $"{recessive:0.0}i";
        }

        string sign = dominant >= 0m ? "+" : "-";
        return $"{recessive:0.0}i {sign} {Math.Abs(dominant):0.0}";
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        return SKPoint.Distance(pixelPoint, _coords.MathToPixel(0f, 0f)) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_parent, _child];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0f, 0f);

    public void Destroy()
    {
        _parentRenderer?.Dispose();
        _parentRenderer = null;
        _childRenderer?.Dispose();
        _childRenderer = null;
        _contextRenderer?.Dispose();
        _contextRenderer = null;

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
        _labelPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _cardTitlePaint.Dispose();
        _cardTextPaint.Dispose();
        _okPaint.Dispose();
        _tensionPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }
}
