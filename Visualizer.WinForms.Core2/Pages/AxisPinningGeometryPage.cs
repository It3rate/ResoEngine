using Core2.Elements;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed class AxisPinningGeometryPage : IVisualizerPage
{
    private const int MinUnitTicks = -8;
    private const int MaxUnitTicks = 8;
    private const int MinCoefficient = -8;
    private const int MaxCoefficient = 8;
    private const int FixedHorizontalExtentTicks = 25;
    private const float LaneOffset = 24f;
    private const float UnitBoxHeight = 30f;
    private const float HandleRadius = 12f;
    private const float LabelPadding = 12f;
    private const float UnresolvedAlpha = 52f;

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private readonly List<HandleLayout> _handles = [];
    private SceneLayout? _sceneLayout;
    private DragTargetKind _dragTarget;
    private SKRect _resetButtonRect;

    private int _recessiveUnitTicks = 5;
    private int _recessiveCoefficient = 3;
    private int _dominantUnitTicks = 5;
    private int _dominantCoefficient = 2;

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
    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(55, 55, 55),
        TextSize = 15f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _captionPaint = new()
    {
        Color = new SKColor(120, 120, 120),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _rulerBandLeftPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 210, 210, 110),
        IsAntialias = true,
    };
    private readonly SKPaint _rulerBandRightPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(204, 228, 255, 120),
        IsAntialias = true,
    };
    private readonly SKPaint _rulerLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 4f,
        Color = new SKColor(145, 110, 110),
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true,
    };
    private readonly SKPaint _tickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.5f,
        Color = new SKColor(162, 162, 162),
        IsAntialias = true,
    };
    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(188, 188, 188, 132),
        IsAntialias = true,
        PathEffect = SKPathEffect.CreateDash([5f, 7f], 0f),
    };
    private readonly SKPaint _infoFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255, 240),
        IsAntialias = true,
    };
    private readonly SKPaint _infoStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(220, 220, 220),
        IsAntialias = true,
    };
    private readonly SKPaint _infoTextPaint = new()
    {
        Color = new SKColor(56, 56, 56),
        TextSize = 21f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
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
        StrokeWidth = 2f,
        Color = new SKColor(152, 152, 152),
        IsAntialias = true,
    };
    private readonly SKPaint _valueLabelPaint = new()
    {
        TextSize = 20f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _unitLabelPaint = new()
    {
        TextSize = 18f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(214, 214, 214),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonTextPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private static readonly SKColor RecessiveUnitStroke = new(176, 176, 176);
    private static readonly SKColor RecessiveUnitFill = new(173, 233, 255, 84);
    private static readonly SKColor RecessiveUnitSegmentColor = SKColor.Parse("#3797DC");
    private static readonly SKColor RecessiveValueColor = SegmentColors.Blue.Solid;
    private static readonly SKColor DominantUnitStroke = new(176, 176, 176);
    private static readonly SKColor DominantUnitFill = new(255, 216, 232, 84);
    private static readonly SKColor DominantUnitSegmentColor = SKColor.Parse("#E85D3A");
    private static readonly SKColor DominantValueColor = SegmentColors.Red.Solid;

    public string Title => "Axis Pinning Geometry";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
    }

    public void Render(SKCanvas canvas)
    {
        _handles.Clear();
        _resetButtonRect = SKRect.Empty;

        float width = _coords?.Width ?? 1100f;
        float height = _coords?.Height ?? 880f;

        canvas.DrawText("Axis Pinning Geometry", 32f, 44f, _headingPaint);
        float textY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "Drag the four arrow tips. The unit arrows stay anchored at 0, the unit boxes stay on their recessive and dominant sides, and the value arrows preserve their coefficients as the units resize.",
            32f,
            ref textY,
            width - 80f,
            _bodyPaint);

        var cardRect = new SKRect(24f, 104f, width - 18f, height - 24f);
        var infoRect = new SKRect(cardRect.Left + 20f, cardRect.Top + 18f, cardRect.Left + 720f, cardRect.Top + 66f);
        _resetButtonRect = new SKRect(cardRect.Right - 128f, cardRect.Top + 16f, cardRect.Right - 22f, cardRect.Top + 52f);
        var plotRect = new SKRect(cardRect.Left + 22f, infoRect.Bottom + 12f, cardRect.Right - 22f, cardRect.Bottom - 24f);

        canvas.DrawRoundRect(cardRect, 22f, 22f, _cardFillPaint);
        canvas.DrawRoundRect(cardRect, 22f, 22f, _cardStrokePaint);

        _sceneLayout = BuildSceneLayout(plotRect);

        DrawConversionBox(canvas, infoRect);
        DrawResetButton(canvas, _resetButtonRect);
        DrawPlot(canvas, _sceneLayout);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        if (_resetButtonRect.Contains(pixelPoint))
        {
            ResetEquation();
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var handle = HitHandle(pixelPoint);
        if (handle is null)
        {
            return false;
        }

        _dragTarget = handle.Target;
        _canvasHost?.InvalidateCanvas();
        return true;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost is null)
        {
            return;
        }

        if (_dragTarget != DragTargetKind.None)
        {
            UpdateDrag(pixelPoint);
            _canvasHost.Cursor = Cursors.Hand;
            _canvasHost.InvalidateCanvas();
            return;
        }

        _canvasHost.Cursor =
            HitHandle(pixelPoint) != null || _resetButtonRect.Contains(pixelPoint)
                ? Cursors.Hand
                : Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        _dragTarget = DragTargetKind.None;
        if (_canvasHost is not null)
        {
            _canvasHost.Cursor = Cursors.Default;
        }
    }

    public void Destroy()
    {
        _handles.Clear();
        _sceneLayout = null;
        _dragTarget = DragTargetKind.None;
        _coords = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardStrokePaint.Dispose();
        _labelPaint.Dispose();
        _captionPaint.Dispose();
        _rulerBandLeftPaint.Dispose();
        _rulerBandRightPaint.Dispose();
        _rulerLinePaint.Dispose();
        _tickPaint.Dispose();
        _guidePaint.Dispose();
        _infoFillPaint.Dispose();
        _infoStrokePaint.Dispose();
        _infoTextPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _valueLabelPaint.Dispose();
        _unitLabelPaint.Dispose();
        _buttonFillPaint.Dispose();
        _buttonStrokePaint.Dispose();
        _buttonTextPaint.Dispose();
    }

    private void DrawConversionBox(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _infoFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _infoStrokePaint);

        string formula = $"({ValueNumerator(_recessiveUnitTicks, _recessiveCoefficient)}/{_recessiveUnitTicks}i)+({ValueNumerator(_dominantUnitTicks, _dominantCoefficient)}/{_dominantUnitTicks}u):({FormatImaginaryTerm(_recessiveCoefficient)}{FormatRealTerm(_dominantCoefficient)})";

        canvas.DrawText(formula, rect.Left + 18f, rect.Top + 33f, _infoTextPaint);
    }

    private void DrawPlot(SKCanvas canvas, SceneLayout scene)
    {
        DrawRuler(canvas, scene);
        DrawUnitBoxes(canvas, scene);
        DrawOriginMarkers(canvas, scene);

        DrawUnitSegment(canvas, scene, PinSideRole.Recessive, _recessiveUnitTicks);
        DrawUnitSegment(canvas, scene, PinSideRole.Dominant, _dominantUnitTicks);
        DrawValueSegment(canvas, scene, PinSideRole.Recessive, _recessiveUnitTicks, _recessiveCoefficient);
        DrawValueSegment(canvas, scene, PinSideRole.Dominant, _dominantUnitTicks, _dominantCoefficient);
    }

    private void DrawRuler(SKCanvas canvas, SceneLayout scene)
    {
        float bandHeight = 20f;
        var leftBand = new SKRect(scene.AxisLeft, scene.AxisY - bandHeight / 2f, scene.OriginX, scene.AxisY + bandHeight / 2f);
        var rightBand = new SKRect(scene.OriginX, scene.AxisY - bandHeight / 2f, scene.AxisRight, scene.AxisY + bandHeight / 2f);

        canvas.DrawRect(leftBand, _rulerBandLeftPaint);
        canvas.DrawRect(rightBand, _rulerBandRightPaint);
        canvas.DrawLine(scene.AxisLeft, scene.AxisY, scene.AxisRight, scene.AxisY, _rulerLinePaint);
        canvas.DrawLine(scene.OriginX, scene.PlotRect.Top + 10f, scene.OriginX, scene.PlotRect.Bottom - 10f, _guidePaint);

        for (int tick = -scene.HorizontalExtentTicks; tick <= scene.HorizontalExtentTicks; tick++)
        {
            float x = scene.OriginX + tick * scene.TickSpacing;
            float tickHeight = tick == 0 ? 18f : 12f;
            canvas.DrawLine(x, scene.AxisY - tickHeight / 2f, x, scene.AxisY + tickHeight / 2f, _tickPaint);
        }
    }

    private void DrawUnitBoxes(SKCanvas canvas, SceneLayout scene)
    {
        float recessiveWidth = Math.Abs(_recessiveUnitTicks) * scene.TickSpacing;
        if (recessiveWidth > 0.001f)
        {
            using var fill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = RecessiveUnitFill,
                IsAntialias = true,
            };
            using var stroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                Color = RecessiveUnitStroke,
                IsAntialias = true,
            };

            var rect = new SKRect(
                scene.OriginX - recessiveWidth,
                scene.TopLaneY - UnitBoxHeight - 6f,
                scene.OriginX,
                scene.TopLaneY + 4f);
            canvas.DrawRect(rect, fill);
            canvas.DrawRect(rect, stroke);
        }

        float dominantWidth = Math.Abs(_dominantUnitTicks) * scene.TickSpacing;
        if (dominantWidth > 0.001f)
        {
            using var fill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = DominantUnitFill,
                IsAntialias = true,
            };
            using var stroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                Color = DominantUnitStroke,
                IsAntialias = true,
            };

            var rect = new SKRect(
                scene.OriginX,
                scene.BottomLaneY - 4f,
                scene.OriginX + dominantWidth,
                scene.BottomLaneY + UnitBoxHeight + 6f);
            canvas.DrawRect(rect, fill);
            canvas.DrawRect(rect, stroke);
        }
    }

    private void DrawOriginMarkers(SKCanvas canvas, SceneLayout scene)
    {
        DrawOriginMarker(canvas, new SKPoint(scene.OriginX, scene.TopLaneY), RecessiveUnitStroke);
        DrawOriginMarker(canvas, new SKPoint(scene.OriginX, scene.BottomLaneY), DominantUnitStroke);
    }

    private void DrawOriginMarker(SKCanvas canvas, SKPoint center, SKColor strokeColor)
    {
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.6f,
            Color = new SKColor(152, 152, 152),
            IsAntialias = true,
        };
        canvas.DrawCircle(center, 11f, _originFillPaint);
        canvas.DrawCircle(center, 11f, stroke);
    }

    private void DrawUnitSegment(SKCanvas canvas, SceneLayout scene, PinSideRole role, int unitTicks)
    {
        SKPoint start = role == PinSideRole.Recessive
            ? new(scene.OriginX, scene.TopLaneY)
            : new(scene.OriginX, scene.BottomLaneY);

        SKColor strokeColor = role == PinSideRole.Recessive ? RecessiveUnitSegmentColor : DominantUnitSegmentColor;
        SKColor labelColor = strokeColor;
        string label = role == PinSideRole.Recessive ? "i" : "u";
        float signedTicks = role == PinSideRole.Recessive ? -unitTicks : unitTicks;
        SKPoint end = new(scene.OriginX + signedTicks * scene.TickSpacing, start.Y);

        if (role == PinSideRole.Recessive)
        {
            DrawDotSegment(canvas, start, end, strokeColor, 4.6f);
        }
        else
        {
            DrawArrowSegment(canvas, start, end, strokeColor, 4.6f);
        }

        DrawLabel(canvas, label, end, start, labelColor, true, role == PinSideRole.Dominant);
        RegisterHandle(role == PinSideRole.Recessive ? DragTargetKind.RecessiveUnit : DragTargetKind.DominantUnit, end);
    }

    private void DrawValueSegment(SKCanvas canvas, SceneLayout scene, PinSideRole role, int unitTicks, int coefficient)
    {
        SKPoint start = role == PinSideRole.Recessive
            ? new(scene.OriginX, scene.BottomLaneY)
            : new(scene.OriginX, scene.TopLaneY);

        if (coefficient == 0)
        {
            RegisterHandle(role == PinSideRole.Recessive ? DragTargetKind.RecessiveValue : DragTargetKind.DominantValue, start);
            return;
        }

        SKColor strokeColor = role == PinSideRole.Recessive ? RecessiveValueColor : DominantValueColor;
        string label = $"{coefficient}{(role == PinSideRole.Recessive ? "iV" : "uV")}";
        var segment = BuildValueSegment(scene, role, start, unitTicks, coefficient);

        if (segment.IsUnresolved)
        {
            DrawUnresolvedValue(canvas, start, segment, strokeColor, label, role);
            foreach (var tip in segment.UnresolvedTips)
            {
                RegisterHandle(role == PinSideRole.Recessive ? DragTargetKind.RecessiveValue : DragTargetKind.DominantValue, tip);
            }

            return;
        }

        if (role == PinSideRole.Recessive)
        {
            DrawDotSegment(canvas, start, segment.PrimaryTip, strokeColor, 5.2f);
        }
        else
        {
            DrawArrowSegment(canvas, start, segment.PrimaryTip, strokeColor, 5.2f);
        }

        DrawLabel(canvas, label, segment.PrimaryTip, start, strokeColor, false, role == PinSideRole.Recessive);
        RegisterHandle(role == PinSideRole.Recessive ? DragTargetKind.RecessiveValue : DragTargetKind.DominantValue, segment.PrimaryTip);
    }

    private void DrawUnresolvedValue(SKCanvas canvas, SKPoint start, ValueSegment segment, SKColor color, string label, PinSideRole role)
    {
        using var faded = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4.6f,
            Color = color.WithAlpha((byte)UnresolvedAlpha),
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };

        foreach (var tip in segment.UnresolvedTips)
        {
            if (role == PinSideRole.Recessive)
            {
                DrawDotSegment(canvas, start, tip, faded.Color, faded.StrokeWidth);
            }
            else
            {
                DrawArrowSegment(canvas, start, tip, faded.Color, faded.StrokeWidth);
            }
        }

        if (segment.UnresolvedTips.Count > 0)
        {
            DrawLabel(canvas, label, segment.UnresolvedTips[0], start, color.WithAlpha((byte)UnresolvedAlpha), false, true);
        }
    }

    private void DrawArrowSegment(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        var vector = new SKPoint(end.X - start.X, end.Y - start.Y);
        float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        if (length > 0.001f)
        {
            float ux = vector.X / length;
            float uy = vector.Y / length;
            float headLength = 18f;
            float headWidth = 10f;
            var back = new SKPoint(end.X - ux * headLength, end.Y - uy * headLength);
            using var stroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = color,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
            };
            canvas.DrawLine(start, back, stroke);

            var left = new SKPoint(back.X - uy * headWidth, back.Y + ux * headWidth);
            var right = new SKPoint(back.X + uy * headWidth, back.Y - ux * headWidth);

            using var head = new SKPath();
            head.MoveTo(end);
            head.LineTo(left);
            head.LineTo(right);
            head.Close();

            using var fill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color,
                IsAntialias = true,
            };

            canvas.DrawPath(head, fill);
        }
        else
        {
            using var stroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = color,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
            };
            canvas.DrawLine(start, end, stroke);
        }
    }

    private void DrawDotSegment(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeWidth,
            Color = color,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };

        canvas.DrawLine(start, end, stroke);
        canvas.DrawCircle(end, 10f, fill);
    }

    private void DrawLabel(SKCanvas canvas, string label, SKPoint tip, SKPoint start, SKColor color, bool unitLabel, bool belowSegment)
    {
        using var paint = new SKPaint
        {
            Color = color,
            TextSize = unitLabel ? _unitLabelPaint.TextSize : _valueLabelPaint.TextSize,
            Typeface = unitLabel ? _unitLabelPaint.Typeface : _valueLabelPaint.Typeface,
            IsAntialias = true,
        };

        SKPoint offset;
        float dx = tip.X - start.X;
        float dy = tip.Y - start.Y;
        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            float yOffset = belowSegment ? 26f : -12f;
            offset = new SKPoint(dx < 0 ? -LabelPadding - paint.MeasureText(label) : LabelPadding, yOffset);
        }
        else
        {
            offset = new SKPoint(LabelPadding, dy < 0 ? -LabelPadding : LabelPadding + 18f);
        }

        canvas.DrawText(label, tip.X + offset.X, tip.Y + offset.Y, paint);
    }

    private ValueSegment BuildValueSegment(SceneLayout scene, PinSideRole role, SKPoint start, int unitTicks, int coefficient)
    {
        if (coefficient == 0)
        {
            return new ValueSegment(start, [], true);
        }

        if (unitTicks == 0)
        {
            float unresolvedMagnitude = Math.Abs(coefficient) * scene.TickSpacing;
            SKPoint horizontalTip = role == PinSideRole.Recessive
                ? new(start.X - coefficient * scene.TickSpacing, start.Y)
                : new(start.X + coefficient * scene.TickSpacing, start.Y);
            float verticalDirection = role == PinSideRole.Recessive ? -coefficient : coefficient;
            SKPoint verticalTip = new(start.X, start.Y - verticalDirection * scene.TickSpacing);
            return new ValueSegment(start, [horizontalTip, verticalTip], true);
        }

        float magnitude = Math.Abs(unitTicks) * Math.Abs(coefficient) * scene.TickSpacing;
        int nativePositiveDirection = role == PinSideRole.Recessive ? -1 : 1;
        int directionSign = nativePositiveDirection * Math.Sign(coefficient);

        if (unitTicks > 0)
        {
            SKPoint tip = new(start.X + directionSign * magnitude, start.Y);
            return new ValueSegment(tip, [], false);
        }

        SKPoint vertical = new(start.X, start.Y - directionSign * magnitude);
        return new ValueSegment(vertical, [], false);
    }

    private SceneLayout BuildSceneLayout(SKRect plotRect)
    {
        float availableWidth = plotRect.Width - 60f;
        float tickSpacing = MathF.Max(16f, availableWidth / (FixedHorizontalExtentTicks * 2f));

        float axisHalfWidth = FixedHorizontalExtentTicks * tickSpacing;
        int verticalExtentTicks = Math.Max(6, (int)MathF.Floor((plotRect.Height - 60f) / (2f * tickSpacing)));
        float verticalHalf = verticalExtentTicks * tickSpacing;
        float originX = plotRect.MidX;
        float axisY = plotRect.MidY + 8f;
        float topLaneY = axisY - LaneOffset;
        float bottomLaneY = axisY + LaneOffset;

        return new SceneLayout(
            plotRect,
            originX,
            axisY,
            topLaneY,
            bottomLaneY,
            tickSpacing,
            FixedHorizontalExtentTicks,
            verticalExtentTicks,
            originX - axisHalfWidth,
            originX + axisHalfWidth,
            axisY - verticalHalf,
            axisY + verticalHalf);
    }

    private void DrawResetButton(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 16f, 16f, _buttonFillPaint);
        canvas.DrawRoundRect(rect, 16f, 16f, _buttonStrokePaint);
        canvas.DrawText("Reset", rect.MidX, rect.MidY + 4f, _buttonTextPaint);
    }

    private void UpdateDrag(SKPoint pixelPoint)
    {
        if (_sceneLayout is null)
        {
            return;
        }

        switch (_dragTarget)
        {
            case DragTargetKind.RecessiveUnit:
                _recessiveUnitTicks = Clamp((int)MathF.Round((_sceneLayout.OriginX - pixelPoint.X) / _sceneLayout.TickSpacing), MinUnitTicks, MaxUnitTicks);
                break;

            case DragTargetKind.DominantUnit:
                _dominantUnitTicks = Clamp((int)MathF.Round((pixelPoint.X - _sceneLayout.OriginX) / _sceneLayout.TickSpacing), MinUnitTicks, MaxUnitTicks);
                break;

            case DragTargetKind.RecessiveValue:
                _recessiveCoefficient = ResolveCoefficientFromPointer(PinSideRole.Recessive, pixelPoint, _recessiveUnitTicks, _sceneLayout.BottomLaneY);
                break;

            case DragTargetKind.DominantValue:
                _dominantCoefficient = ResolveCoefficientFromPointer(PinSideRole.Dominant, pixelPoint, _dominantUnitTicks, _sceneLayout.TopLaneY);
                break;
        }
    }

    private int ResolveCoefficientFromPointer(PinSideRole role, SKPoint pixelPoint, int unitTicks, float laneY)
    {
        if (_sceneLayout is null)
        {
            return 0;
        }

        int coefficient;
        float unitMagnitude = Math.Max(1f, Math.Abs(unitTicks)) * _sceneLayout.TickSpacing;

        if (unitTicks > 0)
        {
            coefficient = role == PinSideRole.Recessive
                ? (int)MathF.Round((_sceneLayout.OriginX - pixelPoint.X) / unitMagnitude)
                : (int)MathF.Round((pixelPoint.X - _sceneLayout.OriginX) / unitMagnitude);
        }
        else if (unitTicks < 0)
        {
            coefficient = role == PinSideRole.Recessive
                ? (int)MathF.Round((pixelPoint.Y - laneY) / unitMagnitude)
                : (int)MathF.Round((laneY - pixelPoint.Y) / unitMagnitude);
        }
        else
        {
            float dx = pixelPoint.X - _sceneLayout.OriginX;
            float dy = pixelPoint.Y - laneY;
            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                coefficient = role == PinSideRole.Recessive
                    ? (int)MathF.Round((-dx) / _sceneLayout.TickSpacing)
                    : (int)MathF.Round(dx / _sceneLayout.TickSpacing);
            }
            else
            {
                coefficient = role == PinSideRole.Recessive
                    ? (int)MathF.Round(dy / _sceneLayout.TickSpacing)
                    : (int)MathF.Round((-dy) / _sceneLayout.TickSpacing);
            }
        }

        return Clamp(coefficient, MinCoefficient, MaxCoefficient);
    }

    private HandleLayout? HitHandle(SKPoint point)
    {
        float threshold = HandleRadius + 6f;
        return _handles.FirstOrDefault(handle => Distance(point, handle.Center) <= threshold);
    }

    private void RegisterHandle(DragTargetKind target, SKPoint center) =>
        _handles.Add(new HandleLayout(target, center));

    private static int ValueNumerator(int unitTicks, int coefficient) =>
        unitTicks == 0 ? 0 : Math.Abs(unitTicks) * coefficient;

    private static string FormatImaginaryTerm(int value) => $"{value}i";

    private static string FormatRealTerm(int value) =>
        value >= 0 ? $" + {value}" : $" - {Math.Abs(value)}";

    private static float Distance(SKPoint a, SKPoint b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    private static int Clamp(int value, int min, int max) =>
        Math.Min(max, Math.Max(min, value));

    private void ResetEquation()
    {
        _recessiveUnitTicks = 5;
        _recessiveCoefficient = 3;
        _dominantUnitTicks = 5;
        _dominantCoefficient = 2;
        _dragTarget = DragTargetKind.None;
    }

    private sealed record SceneLayout(
        SKRect PlotRect,
        float OriginX,
        float AxisY,
        float TopLaneY,
        float BottomLaneY,
        float TickSpacing,
        int HorizontalExtentTicks,
        int VerticalExtentTicks,
        float AxisLeft,
        float AxisRight,
        float PlotTop,
        float PlotBottom);

    private sealed record HandleLayout(DragTargetKind Target, SKPoint Center);

    private readonly record struct ValueSegment(SKPoint PrimaryTip, IReadOnlyList<SKPoint> UnresolvedTips, bool IsUnresolved);

    private enum DragTargetKind
    {
        None,
        RecessiveUnit,
        RecessiveValue,
        DominantValue,
        DominantUnit,
    }
}
