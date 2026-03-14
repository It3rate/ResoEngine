using Core2.Elements;
using Core2.Units;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class UnitsQuantityPage : IVisualizerPage
{
    private const float InputAY = 4.2f;
    private const float InputBY = 1.7f;

    private static readonly PhysicalReferent DistanceReferent = new("distance", "Distance");
    private static readonly PhysicalReferent TimeReferent = new("time", "Time");
    private static readonly UnitGenerator Length = new("length", "L", DistanceReferent);
    private static readonly UnitGenerator Time = new("time", "T", TimeReferent);

    private readonly AxisDisplayMapper _axisA = new(
        Axis.FromCoordinates((Scalar)(-2m), (Scalar)4m, Scalar.One, Scalar.One),
        string.Empty);

    private readonly AxisDisplayMapper _axisB = new(
        Axis.FromCoordinates((Scalar)(-1.5m), (Scalar)3m, Scalar.One, Scalar.One),
        string.Empty);

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

    private readonly SKPaint _cardFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(247, 247, 247),
        IsAntialias = true,
    };

    private readonly SKPaint _cardBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(188, 188, 188),
        IsAntialias = true,
    };

    private readonly SKPaint _cardTitlePaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _cardTextPaint = new()
    {
        Color = new SKColor(82, 82, 82),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _tensionTextPaint = new()
    {
        Color = new SKColor(180, 68, 68),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        PathEffect = SKPathEffect.CreateDash([6f, 4f], 0f),
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
    private SegmentRenderer? _rendererA;
    private SegmentRenderer? _rendererB;
    private Panel? _controlsPanel;
    private ComboBox? _unitACombo;
    private ComboBox? _unitBCombo;

    public string Title => "Units / Quantity";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;

        coords.OriginX = coords.Width * 0.5f;
        coords.OriginY = 370f;

        _rendererA = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Red, crossPosition: InputAY);
        _rendererB = new SegmentRenderer(coords, SegmentOrientation.Horizontal, SegmentColors.Blue, crossPosition: InputBY);
        hitTest.Register(_rendererA, _axisA);
        hitTest.Register(_rendererB, _axisB);

        EnsureControls();
    }

    public void Render(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        canvas.DrawText("Units and Quantity", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Structure lives in the segment. Unit signatures live beside it and combine independently.",
            34f,
            ref subtitleY,
            430f,
            _bodyPaint);

        DrawGraphFrame(canvas);
        _rendererA?.Render(canvas, _axisA);
        _rendererB?.Render(canvas, _axisB);
        DrawInputBadges(canvas);
        DrawCards(canvas);

        var originPx = _coords.MathToPixel(0, 0);
        float r = VisualStyle.OriginDotRadius;
        canvas.DrawCircle(originPx, r, _originFillPaint);
        canvas.DrawCircle(originPx, r, _originStrokePaint);
        canvas.DrawCircle(originPx, 3f, _originDotPaint);
    }

    private void DrawInputBadges(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        DrawRowBadge(canvas, $"A [{SelectedUnitA.Choice.Symbol}]", InputAY, SegmentColors.Red);
        DrawRowBadge(canvas, $"B [{SelectedUnitB.Choice.Symbol}]", InputBY, SegmentColors.Blue);
    }

    private void DrawCards(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        var qa = _axisA.Axis.AsQuantity(SelectedUnitA.Choice.Signature, SelectedUnitA.Choice);
        var qb = _axisB.Axis.AsQuantity(SelectedUnitB.Choice.Signature, SelectedUnitB.Choice);
        var product = qa.Multiply(qb);
        var square = qa.TryPow(new Proportion(2, 1));
        var sum = qa.TryAdd(qb);
        Quantity<Axis>? squareQuantity = square.Succeeded ? square.PrincipalCandidate : null;
        Quantity<Axis>? rootedQuantity = null;
        if (squareQuantity.HasValue)
        {
            var rooted = squareQuantity.Value.TryPow(new Proportion(1, 2));
            if (rooted.Succeeded)
            {
                rootedQuantity = rooted.PrincipalCandidate;
            }
        }

        const float left = 36f;
        const float top = 412f;
        const float gap = 18f;
        float cardWidth = (_coords.Width - left * 2f - gap) / 2f;
        const float cardHeight = 156f;

        var multiplyRect = new SKRect(left, top, left + cardWidth, top + cardHeight);
        var squareRect = new SKRect(left + cardWidth + gap, top, left + cardWidth * 2f + gap, top + cardHeight);
        var rootRect = new SKRect(left, top + cardHeight + gap, left + cardWidth, top + cardHeight * 2f + gap);
        var addRect = new SKRect(left + cardWidth + gap, top + cardHeight + gap, left + cardWidth * 2f + gap, top + cardHeight * 2f + gap);

        DrawQuantityCard(canvas, multiplyRect, "Multiply", product.Value, product.Signature.ToString(), SegmentColors.Green);

        if (squareQuantity.HasValue)
        {
            DrawQuantityCard(canvas, squareRect, "Square", squareQuantity.Value.Value, squareQuantity.Value.Signature.ToString(), SegmentColors.Orange);
        }
        else
        {
            DrawMessageCard(canvas, squareRect, "Square", "No principal power", false);
        }

        if (rootedQuantity.HasValue)
        {
            DrawQuantityCard(canvas, rootRect, "Square Root", rootedQuantity.Value.Value, rootedQuantity.Value.Signature.ToString(), SegmentColors.Blue);
        }
        else
        {
            DrawMessageCard(canvas, rootRect, "Square Root", "No principal root", false);
        }

        if (sum.Succeeded)
        {
            var quantity = sum.Quantity!.Value;
            DrawQuantityCard(canvas, addRect, "Add", quantity.Value, quantity.Signature.ToString(), SegmentColors.Purple);
        }
        else
        {
            var tension = sum.Tensions.FirstOrDefault();
            string message = tension is null || string.IsNullOrWhiteSpace(tension.Message)
                ? "Signature mismatch"
                : tension.Message;
            DrawMessageCard(canvas, addRect, "Add / Tension", message, true);
        }
    }

    private void DrawGraphFrame(SKCanvas canvas)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out var minValue, out var maxValue, out var outerRect);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphFillPaint);
        canvas.DrawRoundRect(outerRect, 16f, 16f, _graphBorderPaint);

        float top = _coords.MathToPixel(0f, InputAY + 0.95f).Y;
        float bottom = _coords.MathToPixel(0f, -0.9f).Y;
        float axisY = _coords.MathToPixel(0f, 0f).Y;
        PageChrome.DrawRuler(
            canvas,
            _coords,
            minValue,
            maxValue,
            axisY,
            top,
            bottom,
            _guidePaint,
            _guidePaint,
            _topTickPaint,
            _bottomTickPaint,
            _tickTextPaint,
            _tickTextPaint);
    }

    private void DrawRowBadge(SKCanvas canvas, string text, float y, SegmentColorSet colors)
    {
        if (_coords == null)
        {
            return;
        }

        GetGraphBounds(out _, out _, out var outerRect);
        var center = _coords.MathToPixel(0f, y);
        var bounds = new SKRect();
        _cardTitlePaint.MeasureText(text, ref bounds);
        float width = Math.Max(90f, bounds.Width + 24f);
        var rect = new SKRect(outerRect.Left + 14f, center.Y - 16f, outerRect.Left + 14f + width, center.Y + 16f);

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
        canvas.DrawText(text, rect.MidX, rect.MidY + 5f, textPaint);
    }

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
        foreach (var axis in new[] { _axisA.Axis, _axisB.Axis })
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
        float top = _coords.MathToPixel(0f, InputAY + 1.0f).Y;
        float bottom = _coords.MathToPixel(0f, -1.0f).Y;
        outerRect = new SKRect(_coords.Width * 0.05f, top - 18f, _coords.Width * 0.95f, bottom + 24f);
    }

    private void DrawQuantityCard(SKCanvas canvas, SKRect rect, string title, Axis axis, string signatureText, SegmentColorSet colors)
    {
        canvas.DrawRoundRect(rect, 14f, 14f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _cardBorderPaint);
        canvas.DrawText(title, rect.MidX, rect.Top + 24f, _cardTitlePaint);

        var segmentRect = new SKRect(rect.Left + 22f, rect.Top + 38f, rect.Right - 22f, rect.Top + 90f);
        DrawMiniAxis(canvas, segmentRect, axis, colors);

        canvas.DrawText(signatureText, rect.MidX, rect.Bottom - 14f, _cardTextPaint);
    }

    private void DrawMessageCard(SKCanvas canvas, SKRect rect, string title, string message, bool tension)
    {
        canvas.DrawRoundRect(rect, 14f, 14f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _cardBorderPaint);
        canvas.DrawText(title, rect.MidX, rect.Top + 24f, _cardTitlePaint);

        var paint = tension ? _tensionTextPaint : _cardTextPaint;
        canvas.DrawText(message, rect.MidX, rect.MidY, paint);
    }

    private void DrawMiniAxis(SKCanvas canvas, SKRect rect, Axis axis, SegmentColorSet colors)
    {
        decimal start = axis.Start.Value;
        decimal end = axis.End.Value;
        decimal min = Math.Min(Math.Min(start, end), 0m) - 0.5m;
        decimal max = Math.Max(Math.Max(start, end), 0m) + 0.5m;

        float midY = rect.MidY;
        float zeroX = Map(0m, min, max, rect.Left, rect.Right);
        float startX = Map(start, min, max, rect.Left, rect.Right);
        float endX = Map(end, min, max, rect.Left, rect.Right);

        canvas.DrawLine(rect.Left, midY, rect.Right, midY, _guidePaint);
        canvas.DrawLine(zeroX, rect.Top + 8f, zeroX, rect.Bottom - 8f, _guidePaint);

        using var dashPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = colors.Solid,
            PathEffect = SKPathEffect.CreateDash([6f, 4f], 0f),
            IsAntialias = true,
        };
        using var solidPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f,
            Color = colors.Solid,
            IsAntialias = true,
        };
        using var dotPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = colors.Solid,
            IsAntialias = true,
        };
        using var arrowPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = colors.Solid,
            IsAntialias = true,
        };

        canvas.DrawLine(zeroX, midY, startX, midY, dashPaint);
        float direction = endX >= startX ? 1f : -1f;
        float arrowLen = 12f;
        float lineEndX = endX - direction * arrowLen;
        canvas.DrawLine(startX, midY, lineEndX, midY, solidPaint);
        canvas.DrawCircle(startX, midY, 4f, dotPaint);

        using var path = new SKPath();
        path.MoveTo(endX, midY);
        path.LineTo(endX - direction * 12f, midY - 6f);
        path.LineTo(endX - direction * 12f, midY + 6f);
        path.Close();
        canvas.DrawPath(path, arrowPaint);
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
            Size = new Size(240, 104),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

        var labelA = new Label
        {
            Text = "Unit for A",
            AutoSize = true,
            Location = new Point(12, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _unitACombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(12, 32),
            Width = 96,
        };
        _unitACombo.Items.AddRange(UnitOptions.All.Select(option => option.Name).ToArray());
        _unitACombo.SelectedIndex = 0;
        _unitACombo.SelectedIndexChanged += (_, _) => _canvasHost?.InvalidateCanvas();

        var labelB = new Label
        {
            Text = "Unit for B",
            AutoSize = true,
            Location = new Point(126, 12),
            Font = new Font("Arial", 9f, FontStyle.Bold),
        };

        _unitBCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(126, 32),
            Width = 96,
        };
        _unitBCombo.Items.AddRange(UnitOptions.All.Select(option => option.Name).ToArray());
        _unitBCombo.SelectedIndex = 1;
        _unitBCombo.SelectedIndexChanged += (_, _) => _canvasHost?.InvalidateCanvas();

        var note = new Label
        {
            Text = "Multiply and powers carry signatures. Addition requires them to match.",
            AutoSize = false,
            Location = new Point(12, 64),
            Size = new Size(214, 28),
            Font = new Font("Arial", 8f, FontStyle.Regular),
            ForeColor = Color.FromArgb(105, 105, 105),
        };

        _controlsPanel.Controls.Add(labelA);
        _controlsPanel.Controls.Add(_unitACombo);
        _controlsPanel.Controls.Add(labelB);
        _controlsPanel.Controls.Add(_unitBCombo);
        _controlsPanel.Controls.Add(note);
        _canvasHost.Controls.Add(_controlsPanel);
        _controlsPanel.BringToFront();
        _canvasHost.Resize += CanvasHostOnResize;
    }

    private void CanvasHostOnResize(object? sender, EventArgs e) =>
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel, 18);

    private UnitOption SelectedUnitA => UnitOptions.Get(_unitACombo?.SelectedItem?.ToString());
    private UnitOption SelectedUnitB => UnitOptions.Get(_unitBCombo?.SelectedItem?.ToString());

    private static float Map(decimal value, decimal min, decimal max, float left, float right)
    {
        if (max <= min)
        {
            return left;
        }

        return left + (float)((value - min) / (max - min)) * (right - left);
    }

    public bool IsOriginHit(SKPoint pixelPoint)
    {
        if (_coords == null)
        {
            return false;
        }

        return SKPoint.Distance(pixelPoint, _coords.MathToPixel(0f, 0f)) <= VisualStyle.HitPadding;
    }

    public IReadOnlyList<ISegmentValue>? GetDraggableSegments() => [_axisA, _axisB];

    public SKPoint? GetOriginPixel() => _coords?.MathToPixel(0f, 0f);

    public void Destroy()
    {
        _rendererA?.Dispose();
        _rendererA = null;
        _rendererB?.Dispose();
        _rendererB = null;

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
        _topTickPaint.Dispose();
        _bottomTickPaint.Dispose();
        _tickTextPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _cardTitlePaint.Dispose();
        _cardTextPaint.Dispose();
        _tensionTextPaint.Dispose();
        _guidePaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
    }

    private sealed record UnitOption(string Name, UnitChoice Choice);

    private static class UnitOptions
    {
        private static readonly UnitOption[] Values =
        [
            new("Length", new UnitChoice("meter", "m", UnitSignature.From(Length), Scalar.One, DistanceReferent)),
            new("Time", new UnitChoice("second", "s", UnitSignature.From(Time), Scalar.One, TimeReferent)),
            new("Velocity", new UnitChoice("meter/second", "m/s", UnitSignature.From(Length).Divide(UnitSignature.From(Time)), Scalar.One)),
            new("Area", new UnitChoice("square meter", "m²", UnitSignature.From(Length).Pow(2), Scalar.One, DistanceReferent)),
        ];

        public static IEnumerable<UnitOption> All => Values;

        public static UnitOption Get(string? name) =>
            Values.FirstOrDefault(option => string.Equals(option.Name, name, StringComparison.Ordinal))
            ?? Values[0];
    }
}
