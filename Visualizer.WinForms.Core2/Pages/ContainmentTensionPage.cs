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
    private const float ChildY = 1.9f;
    private const float ContextY = -2.15f;

    private readonly AxisDisplayMapper _parent = new(
        Axis.FromCoordinates((Scalar)(-4m), (Scalar)7m, Scalar.One, Scalar.One),
        string.Empty);

    private readonly AxisDisplayMapper _child = new(
        Axis.FromCoordinates((Scalar)(-2m), (Scalar)3.5m, Scalar.One, Scalar.One),
        string.Empty);

    private readonly AxisDisplayMapper _contextualChild = new(Axis.Zero, string.Empty);

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

    private readonly SKPaint _tensionOverlayPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(232, 128, 72, 0),
        IsAntialias = true,
    };

    private readonly SKPaint _rulerLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(176, 176, 176),
        IsAntialias = true,
    };

    private readonly SKPaint _zeroAxisPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.3f,
        Color = new SKColor(176, 176, 176),
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

    private readonly SKPaint _tickTextPaint = new()
    {
        Color = new SKColor(132, 132, 132),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
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

    private readonly SKPaint _tensionBandStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(214, 117, 63, 176),
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

        coords.OriginX = coords.Width * 0.5f;
        coords.OriginY = 396f;

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
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Every number is also a frame, and it can be in the context of a frame.\n" +
            "When elements overflow the parent bounds, or have resolution mismatches " +
            "this creates tension information rather than failure. This can be resolved within the relevant context.\n" +
            "The two number boxes change the child's i-side and r-side unit resolution, " +
            "which acts like a resolution/unit mismatch.",
            34f,
            ref subtitleY,
            480f,
            _bodyPaint);

        var metrics = relation.TensionMetrics;
        DrawGraphFrame(canvas, relation, metrics);

        _parentRenderer?.Render(canvas, _parent);
        DrawSegmentLabel(canvas, "Parent Frame", ParentY, SegmentColors.Red);
        _childRenderer?.Render(canvas, _child);
        DrawSegmentLabel(canvas, "Child Raw", ChildY, SegmentColors.Blue);
        _contextRenderer?.Render(canvas, _contextualChild);
        DrawSegmentLabel(canvas, "Child In Parent Context", ContextY, SegmentColors.Green);

        DrawRelationCard(canvas, relation, contextual, metrics);

        var originPx = _coords.MathToPixel(0f, 0f);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawSegmentLabel(SKCanvas canvas, string label, float y, SegmentColorSet colors)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out _, out _, out var outerRect);
        var point = _coords.MathToPixel(0f, y);
        var bounds = new SKRect();
        _labelPaint.MeasureText(label, ref bounds);
        float width = Math.Max(116f, bounds.Width + 26f);
        var rect = new SKRect(outerRect.Left + 14f, point.Y - 16f, outerRect.Left + 14f + width, point.Y + 16f);

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
        canvas.DrawText(label, rect.MidX, rect.MidY + 5f, textPaint);
    }

    private void DrawRelationCard(SKCanvas canvas, Containment relation, Axis contextual, ContainmentTensionMetrics metrics)
    {
        if (_coords == null)
        {
            return;
        }

        var rect = new SKRect(170f, 558f, _coords.Width - 170f, 760f);
        canvas.DrawRoundRect(rect, 16f, 16f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 16f, 16f, _cardBorderPaint);

        float x = rect.Left + 18f;
        float y = rect.Top + 28f;
        canvas.DrawText("Containment Relation", x, y, _cardTitlePaint);
        y += 30f;

        DrawWrappedText(canvas, $"Parent perspective: {SelectedPerspective}", x, ref y, rect.Width - 36f, _cardTextPaint);
        DrawWrappedText(canvas, $"Child in context: {FormatAxis(contextual)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        DrawWrappedText(canvas, $"Total normalized tension: {metrics.TotalMagnitude:0.###}", x, ref y, rect.Width - 36f, _cardTextPaint);
        if (metrics.StartRange is { } startRange)
        {
            DrawWrappedText(canvas, $"Start range tension: {FormatMeasure(startRange)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        }

        if (metrics.EndRange is { } endRange)
        {
            DrawWrappedText(canvas, $"End range tension: {FormatMeasure(endRange)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        }

        if (metrics.RecessiveSupport is { } recessiveSupport)
        {
            DrawWrappedText(canvas, $"i-side support tension: {FormatMeasure(recessiveSupport)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        }

        if (metrics.DominantSupport is { } dominantSupport)
        {
            DrawWrappedText(canvas, $"r-side support tension: {FormatMeasure(dominantSupport)}", x, ref y, rect.Width - 36f, _cardTextPaint);
        }

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

    private void DrawGraphFrame(SKCanvas canvas, Containment relation, ContainmentTensionMetrics metrics)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out var minValue, out var maxValue, out var outerRect);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphFillPaint);
        DrawLocalizedTensionOverlays(canvas, relation, metrics, minValue, maxValue, outerRect);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphBorderPaint);

        float top = _coords.MathToPixel(0f, ParentY + 0.95f).Y;
        float bottom = _coords.MathToPixel(0f, ContextY - 0.95f).Y;
        float axisY = _coords.MathToPixel(0f, 0f).Y;
        PageChrome.DrawRuler(
            canvas,
            _coords,
            minValue,
            maxValue,
            axisY,
            top,
            bottom,
            _rulerLinePaint,
            _zeroAxisPaint,
            _topTickPaint,
            _bottomTickPaint,
            _tickTextPaint,
            _tickTextPaint);
    }

    private void DrawLocalizedTensionOverlays(
        SKCanvas canvas,
        Containment relation,
        ContainmentTensionMetrics metrics,
        decimal minValue,
        decimal maxValue,
        SKRect outerRect)
    {
        if (_coords == null)
        {
            return;
        }

        var parent = relation.Parent.Element as Axis;
        var contextualChild = relation.ChildInParentContext as Axis;
        if (parent is null || contextualChild is null)
        {
            return;
        }

        decimal childStart = contextualChild.Start.Value;
        decimal childEnd = contextualChild.End.Value;
        decimal childLeft = Math.Min(childStart, childEnd);
        decimal childRight = Math.Max(childStart, childEnd);
        decimal parentLeft = Math.Min(parent.Start.Value, parent.End.Value);
        decimal parentRight = Math.Max(parent.Start.Value, parent.End.Value);

        float startIntensity = Clamp01(
            ToMagnitude(metrics.GetAmount(ContainmentTensionKind.OutsideExpectedRange, "recessive.boundary")) +
            ToMagnitude(metrics.GetAmount(ContainmentTensionKind.ResolutionMismatch, "recessive.support")) * 0.5f);
        float endIntensity = Clamp01(
            ToMagnitude(metrics.GetAmount(ContainmentTensionKind.OutsideExpectedRange, "dominant.boundary")) +
            ToMagnitude(metrics.GetAmount(ContainmentTensionKind.ResolutionMismatch, "dominant.support")) * 0.5f);

        float top = outerRect.Top + 8f;
        float bottom = outerRect.Bottom - 8f;

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(outerRect, 16f, 16f), antialias: true);

        if (startIntensity > 0f)
        {
            float left = ValueToGraphX(Math.Min(childLeft, parentLeft), minValue, maxValue, outerRect);
            float right = ValueToGraphX(parentLeft, minValue, maxValue, outerRect);
            DrawTensionBand(canvas, left, right, top, bottom, startIntensity);

            if (metrics.GetAmount(ContainmentTensionKind.ResolutionMismatch, "recessive.support") is not null)
            {
                float markerX = ValueToGraphX(childStart, minValue, maxValue, outerRect);
                DrawTensionBand(canvas, markerX - 14f, markerX + 14f, top, bottom, Math.Max(0.22f, startIntensity * 0.7f));
            }
        }

        if (endIntensity > 0f)
        {
            float left = ValueToGraphX(parentRight, minValue, maxValue, outerRect);
            float right = ValueToGraphX(Math.Max(childRight, parentRight), minValue, maxValue, outerRect);
            DrawTensionBand(canvas, left, right, top, bottom, endIntensity);

            if (metrics.GetAmount(ContainmentTensionKind.ResolutionMismatch, "dominant.support") is not null)
            {
                float markerX = ValueToGraphX(childEnd, minValue, maxValue, outerRect);
                DrawTensionBand(canvas, markerX - 14f, markerX + 14f, top, bottom, Math.Max(0.22f, endIntensity * 0.7f));
            }
        }

        if (startIntensity == 0f && endIntensity == 0f && metrics.TotalMagnitude > 0m)
        {
            byte alpha = (byte)Math.Clamp(16m + metrics.TotalMagnitude * 24m, 0m, 72m);
            _tensionOverlayPaint.Color = new SKColor(232, 128, 72, alpha);
            canvas.DrawRoundRect(new SKRect(outerRect.Left + 12f, top, outerRect.Right - 12f, bottom), 12f, 12f, _tensionOverlayPaint);
        }

        canvas.Restore();
    }

    private void DrawTensionBand(SKCanvas canvas, float left, float right, float top, float bottom, float intensity)
    {
        float clampedLeft = Math.Min(left, right);
        float clampedRight = Math.Max(left, right);
        if (clampedRight - clampedLeft < 8f)
        {
            float mid = (clampedLeft + clampedRight) * 0.5f;
            clampedLeft = mid - 4f;
            clampedRight = mid + 4f;
        }

        byte alpha = (byte)Math.Clamp(34f + intensity * 110f, 0f, 156f);
        _tensionOverlayPaint.Color = new SKColor(232, 128, 72, alpha);

        var rect = new SKRect(clampedLeft, top, clampedRight, bottom);
        canvas.DrawRoundRect(rect, 10f, 10f, _tensionOverlayPaint);
        canvas.DrawRoundRect(rect, 10f, 10f, _tensionBandStrokePaint);
    }

    private float ValueToGraphX(decimal value, decimal minValue, decimal maxValue, SKRect outerRect)
    {
        if (maxValue == minValue)
        {
            return outerRect.MidX;
        }

        float t = (float)((value - minValue) / (maxValue - minValue));
        return outerRect.Left + t * outerRect.Width;
    }

    private static float Clamp01(float value) => Math.Max(0f, Math.Min(1f, value));

    private static float ToMagnitude(Proportion? amount) =>
        amount is null ? 0f : (float)Math.Abs((decimal)amount.Fold());

    private static string FormatMeasure(ContainmentTensionMeasure measure) =>
        $"{measure.Amount} ({measure.Magnitude:0.###})";

    private void GetGraphBounds(out decimal minValue, out decimal maxValue, out SKRect outerRect)
    {
        if (_coords == null)
        {
            minValue = 0m;
            maxValue = 1m;
            outerRect = SKRect.Empty;
            return;
        }

        minValue = 0m;
        maxValue = 0m;
        foreach (var axis in new[] { _parent.Axis, _child.Axis, _contextualChild.Axis })
        {
            minValue = Math.Min(minValue, Math.Min(axis.Start.Value, axis.End.Value));
            maxValue = Math.Max(maxValue, Math.Max(axis.Start.Value, axis.End.Value));
        }

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
        float top = _coords.MathToPixel(0f, ParentY + 1.0f).Y;
        float bottom = _coords.MathToPixel(0f, ContextY - 1.0f).Y;
        outerRect = new SKRect(_coords.Width * 0.05f, top - 18f, _coords.Width * 0.95f, bottom + 24f);
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
            Size = new Size(540, 116),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

        var perspectiveLabel = new Label
        {
            Text = "Parent Perspective",
            AutoSize = true,
            Location = new Point(12, 12),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
        };

        _perspectiveCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(12, 32),
            Width = 146,
        };
        _perspectiveCombo.Items.AddRange(["Dominant", "Opposite"]);
        _perspectiveCombo.SelectedIndex = 0;
        _perspectiveCombo.SelectedIndexChanged += (_, _) => _canvasHost?.InvalidateCanvas();

        var recessiveLabel = new Label
        {
            Text = "Child i support",
            AutoSize = true,
            Location = new Point(198, 12),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
        };

        _recessiveSupportCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(198, 32),
            Width = 110,
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
            Location = new Point(344, 12),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
        };

        _dominantSupportCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(344, 32),
            Width = 110,
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
            Text = "Change the support numbers to reinterpret the child's left and right resolution. Range overflow creates boundary tension; support mismatch creates resolution tension.",
            AutoSize = false,
            Location = new Point(12, 66),
            Size = new Size(500, 38),
            Font = new Font(VisualStyle.UiFontFamily, 8f, FontStyle.Regular),
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
        _canvasHost.Resize += CanvasHostOnResize;
    }

    private void CanvasHostOnResize(object? sender, EventArgs e) =>
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

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

    private float CalculateTensionScore(Containment relation)
    {
        float score = 0f;

        foreach (var tension in relation.Tensions)
        {
            score += tension.Kind switch
            {
                ContainmentTensionKind.OutsideExpectedRange => 1.5f,
                ContainmentTensionKind.ResolutionMismatch => 1.0f,
                ContainmentTensionKind.PlacementUnderspecified => 0.75f,
                ContainmentTensionKind.UnsupportedInterpretation => 2.0f,
                _ => 0.5f,
            };
        }

        var parent = relation.Parent.Element as Axis;
        var child = relation.ChildInParentContext as Axis;
        if (parent is not null && child is not null)
        {
            decimal parentLeft = Math.Min(parent.Start.Value, parent.End.Value);
            decimal parentRight = Math.Max(parent.Start.Value, parent.End.Value);
            decimal childLeft = Math.Min(child.Start.Value, child.End.Value);
            decimal childRight = Math.Max(child.Start.Value, child.End.Value);

            if (childLeft < parentLeft)
            {
                score += (float)(parentLeft - childLeft);
            }

            if (childRight > parentRight)
            {
                score += (float)(childRight - parentRight);
            }

            if (child.Recessive.Recessive != parent.Recessive.Recessive)
            {
                score += (float)Math.Abs(child.Recessive.Recessive.Value - parent.Recessive.Recessive.Value) * 0.5f;
            }

            if (child.Dominant.Recessive != parent.Dominant.Recessive)
            {
                score += (float)Math.Abs(child.Dominant.Recessive.Value - parent.Dominant.Recessive.Value) * 0.5f;
            }
        }

        return score;
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
        _rulerLinePaint.Dispose();
        _zeroAxisPaint.Dispose();
        _topTickPaint.Dispose();
        _bottomTickPaint.Dispose();
        _tickTextPaint.Dispose();
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
        _tensionOverlayPaint.Dispose();
        _tensionBandStrokePaint.Dispose();
    }
}
