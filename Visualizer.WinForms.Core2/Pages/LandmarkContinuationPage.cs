using Core2.Elements;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed class LandmarkContinuationPage : IVisualizerPage
{
    private const long MinEncoding = -1;
    private const long MaxEncoding = 1;
    private const int MinDisplayHalfSteps = -8;
    private const int MaxDisplayHalfSteps = 8;
    private const int MinSegmentSpanHalfSteps = 2;
    private const byte SelectedOverlayAlpha = 36;
    private const byte SecondaryOverlayAlpha = 24;
    private const float LandmarkForceLengthUnits = 2f;

    private static readonly SKColor[] LandmarkAccentPalette =
    [
        SegmentColors.Green.Solid,
        SegmentColors.Orange.Solid,
        SegmentColors.Purple.Solid,
        SKColor.Parse("#0F8A8A"),
        SKColor.Parse("#8A5A00"),
        SKColor.Parse("#A02060"),
    ];

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;

    private readonly List<FieldLayout> _fieldLayouts = [];
    private readonly List<ButtonLayout> _buttonLayouts = [];
    private readonly List<PinLayout> _pinLayouts = [];

    private SceneLayout? _sceneLayout;
    private SKRect _seedRect;
    private SKRect _trashRect;
    private string? _activeFieldKey;
    private DragMode _dragMode;
    private int? _dragPinId;
    private SKPoint _dragPointer;

    private readonly List<LandmarkPinModel> _pins = [new(1, 0, 1, 1, 1, 1)];
    private int _nextPinId = 2;
    private int? _selectedPinId = 1;
    private int _segmentStartHalfSteps = -6;
    private int _segmentEndHalfSteps = 6;

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
    private readonly SKPaint _trackPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(195, 195, 195),
        IsAntialias = true,
        StrokeCap = SKStrokeCap.Round,
    };
    private readonly SKPaint _tickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(188, 188, 188),
        IsAntialias = true,
    };
    private readonly SKPaint _zeroTickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(118, 118, 118),
        IsAntialias = true,
    };
    private readonly SKPaint _tickTextPaint = new()
    {
        Color = new SKColor(140, 140, 140),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKPaint _badgeFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255, 242),
        IsAntialias = true,
    };
    private readonly SKPaint _badgeStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.1f,
        Color = new SKColor(214, 214, 214, 235),
        IsAntialias = true,
    };
    private readonly SKPaint _badgeTextPaint = new()
    {
        Color = new SKColor(72, 72, 72),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
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
    private readonly SKPaint _sceneFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(250, 250, 250),
        IsAntialias = true,
    };
    private readonly SKPaint _sceneStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(222, 222, 222),
        IsAntialias = true,
    };
    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(185, 185, 185, 130),
        IsAntialias = true,
        PathEffect = SKPathEffect.CreateDash([6f, 6f], 0f),
    };
    private readonly SKPaint _rulerPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.6f,
        Color = new SKColor(184, 184, 184),
        IsAntialias = true,
    };
    private readonly SKPaint _carrierPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 4f,
        Color = new SKColor(100, 100, 100),
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true,
    };
    private readonly SKPaint _carrierGhostPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(160, 160, 160, 120),
        StrokeCap = SKStrokeCap.Round,
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
        Color = new SKColor(72, 72, 72),
        IsAntialias = true,
    };
    private readonly SKPaint _originDotPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(72, 72, 72),
        IsAntialias = true,
    };
    private readonly SKPaint _cellFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 240, 184, 145),
        IsAntialias = true,
    };
    private readonly SKPaint _cellStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = new SKColor(208, 176, 62, 210),
        IsAntialias = true,
    };
    private readonly SKPaint _handleStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2f,
        Color = SKColors.White,
        IsAntialias = true,
    };
    private readonly SKPaint _pinLabelPaint = new()
    {
        Color = new SKColor(72, 72, 72),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _tokenTextPaint = new()
    {
        Color = new SKColor(84, 84, 84),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };
    private readonly SKColor _frameEndpointColor = new(132, 132, 132);

    public string Title => "Landmark Continuation Explorer";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
    }

    public void Render(SKCanvas canvas)
    {
        _fieldLayouts.Clear();
        _buttonLayouts.Clear();
        _pinLayouts.Clear();

        float width = _coords?.Width ?? 900f;
        float height = _coords?.Height ?? 800f;

        var inputRect = new SKRect(24f, 104f, 360f, height - 40f);
        var sceneRect = new SKRect(388f, 104f, width - 18f, height - 24f);

        canvas.DrawText("Landmark Continuation Explorer", 32f, 44f, _headingPaint);
        float textY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "Drag the carrier endpoints on the ruler, slide landmarks along the segment, and edit the selected landmark. " +
            "The local pin is attached to a straight horizontal carrier so we can inspect reflection, reinforcement, orthogonal breakout, and noise clearly.",
            32f,
            ref textY,
            860f,
            _bodyPaint);

        DrawInputPanel(canvas, inputRect);
        DrawScenePanel(canvas, sceneRect);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        var button = HitButton(pixelPoint);
        if (button != null)
        {
            button.OnClick();
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var field = HitField(pixelPoint);
        if (field != null && SelectedPin is not null)
        {
            _activeFieldKey = field.Key;
            UpdateFieldValue(field, pixelPoint.X);
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        if (_seedRect.Contains(pixelPoint))
        {
            _dragMode = DragMode.NewPin;
            _dragPointer = pixelPoint;
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        if (_sceneLayout is { } scene)
        {
            if (scene.StartHandleRect.Contains(pixelPoint))
            {
                _dragMode = DragMode.SegmentStart;
                return true;
            }

            if (scene.EndHandleRect.Contains(pixelPoint))
            {
                _dragMode = DragMode.SegmentEnd;
                return true;
            }
        }

        var pin = HitPin(pixelPoint);
        if (pin != null)
        {
            _selectedPinId = pin.PinId;
            _dragMode = DragMode.ExistingPin;
            _dragPinId = pin.PinId;
            _dragPointer = pixelPoint;
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        return false;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost is null)
        {
            return;
        }

        if (_activeFieldKey is not null)
        {
            var field = _fieldLayouts.FirstOrDefault(layout => layout.Key == _activeFieldKey);
            if (field != null)
            {
                UpdateFieldValue(field, pixelPoint.X);
                _canvasHost.Cursor = Cursors.SizeWE;
                _canvasHost.InvalidateCanvas();
                return;
            }
        }

        switch (_dragMode)
        {
            case DragMode.SegmentStart:
                _segmentStartHalfSteps = ClampHalfStep(XToHalfStep(pixelPoint.X), MinDisplayHalfSteps, _segmentEndHalfSteps - MinSegmentSpanHalfSteps);
                ClampPinsToSegment();
                _canvasHost.Cursor = Cursors.SizeWE;
                _canvasHost.InvalidateCanvas();
                return;

            case DragMode.SegmentEnd:
                _segmentEndHalfSteps = ClampHalfStep(XToHalfStep(pixelPoint.X), _segmentStartHalfSteps + MinSegmentSpanHalfSteps, MaxDisplayHalfSteps);
                ClampPinsToSegment();
                _canvasHost.Cursor = Cursors.SizeWE;
                _canvasHost.InvalidateCanvas();
                return;

            case DragMode.ExistingPin:
                _dragPointer = pixelPoint;
                if (_dragPinId is int pinId && TryProjectToCarrier(pixelPoint, out int halfSteps))
                {
                    var pin = _pins.FirstOrDefault(candidate => candidate.Id == pinId);
                    if (pin != null)
                    {
                        pin.LocationHalfSteps = ClampHalfStep(halfSteps, _segmentStartHalfSteps, _segmentEndHalfSteps);
                    }
                }

                _canvasHost.Cursor = _trashRect.Contains(pixelPoint) ? Cursors.Hand : Cursors.SizeWE;
                _canvasHost.InvalidateCanvas();
                return;

            case DragMode.NewPin:
                _dragPointer = pixelPoint;
                _canvasHost.Cursor = Cursors.Cross;
                _canvasHost.InvalidateCanvas();
                return;
        }

        if (HitField(pixelPoint) != null)
        {
            _canvasHost.Cursor = Cursors.SizeWE;
            return;
        }

        if (_seedRect.Contains(pixelPoint) || _trashRect.Contains(pixelPoint) || HitButton(pixelPoint) != null)
        {
            _canvasHost.Cursor = Cursors.Hand;
            return;
        }

        if (_sceneLayout is { } layout &&
            (layout.StartHandleRect.Contains(pixelPoint) || layout.EndHandleRect.Contains(pixelPoint)))
        {
            _canvasHost.Cursor = Cursors.SizeWE;
            return;
        }

        if (HitPin(pixelPoint) != null)
        {
            _canvasHost.Cursor = Cursors.Hand;
            return;
        }

        _canvasHost.Cursor = Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        switch (_dragMode)
        {
            case DragMode.NewPin:
                if (TryProjectToCarrier(pixelPoint, out int halfSteps))
                {
                    var pin = new LandmarkPinModel(_nextPinId++, ClampHalfStep(halfSteps, _segmentStartHalfSteps, _segmentEndHalfSteps), 1, 1, 1, 1);
                    _pins.Add(pin);
                    _selectedPinId = pin.Id;
                }
                break;

            case DragMode.ExistingPin:
                if (_dragPinId is int pinId && _trashRect.Contains(pixelPoint))
                {
                    _pins.RemoveAll(pin => pin.Id == pinId);
                    if (_selectedPinId == pinId)
                    {
                        _selectedPinId = _pins.LastOrDefault()?.Id;
                    }
                }
                break;
        }

        _activeFieldKey = null;
        _dragMode = DragMode.None;
        _dragPinId = null;
        if (_canvasHost != null)
        {
            _canvasHost.Cursor = Cursors.Default;
            _canvasHost.InvalidateCanvas();
        }
    }

    public void Destroy()
    {
        _coords = null;
        _canvasHost = null;
        _sceneLayout = null;
        _fieldLayouts.Clear();
        _buttonLayouts.Clear();
        _pinLayouts.Clear();
        _activeFieldKey = null;
        _dragMode = DragMode.None;
        _dragPinId = null;
        _seedRect = SKRect.Empty;
        _trashRect = SKRect.Empty;
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
        _trackPaint.Dispose();
        _tickPaint.Dispose();
        _zeroTickPaint.Dispose();
        _tickTextPaint.Dispose();
        _badgeFillPaint.Dispose();
        _badgeStrokePaint.Dispose();
        _badgeTextPaint.Dispose();
        _buttonTextPaint.Dispose();
        _buttonFillPaint.Dispose();
        _buttonStrokePaint.Dispose();
        _sceneFillPaint.Dispose();
        _sceneStrokePaint.Dispose();
        _guidePaint.Dispose();
        _rulerPaint.Dispose();
        _carrierPaint.Dispose();
        _carrierGhostPaint.Dispose();
        _originFillPaint.Dispose();
        _originStrokePaint.Dispose();
        _originDotPaint.Dispose();
        _cellFillPaint.Dispose();
        _cellStrokePaint.Dispose();
        _handleStrokePaint.Dispose();
        _pinLabelPaint.Dispose();
        _tokenTextPaint.Dispose();
    }

    private LandmarkPinModel? SelectedPin =>
        _selectedPinId is int selectedId
            ? _pins.FirstOrDefault(pin => pin.Id == selectedId)
            : null;

    private void DrawInputPanel(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 20f, 20f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 20f, 20f, _cardStrokePaint);
        canvas.DrawText("Selected Landmark", rect.Left + 20f, rect.Top + 28f, _labelPaint);

        float captionY = rect.Top + 50f;
        PageChrome.DrawWrappedText(
            canvas,
            "Drag the seed onto the carrier to create a landmark. Click a landmark to select and edit it.",
            rect.Left + 20f,
            ref captionY,
            rect.Width - 40f,
            _captionPaint);

        var selected = SelectedPin;
        if (selected is not null)
        {
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, rect.Top + 94f), $"Landmark P{selected.Id}");
            DrawBadge(canvas, new SKPoint(rect.Left + 160f, rect.Top + 94f), $"At {FormatHalfStep(selected.LocationHalfSteps)}");
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, rect.Top + 128f), DescribePinBehavior(selected));
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, rect.Top + 162f), $"Relation: {selected.BuildDescriptor().Relation}");
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, rect.Top + 196f), FormatDescriptor(selected.BuildDescriptor()));

            float gridTop = rect.Top + 238f;
            float cardGap = 16f;
            float fieldWidth = (rect.Width - 20f * 2f - cardGap) * 0.5f;
            float fieldHeight = 110f;
            float left = rect.Left + 20f;
            float right = left + fieldWidth + cardGap;

            DrawFieldCard(canvas, new SKRect(left, gridTop, left + fieldWidth, gridTop + fieldHeight), "i", "recessive unit", selected.RecessiveUnit, SegmentColors.Red.Solid, "recessive-unit", MinEncoding, MaxEncoding);
            DrawFieldCard(canvas, new SKRect(right, gridTop, right + fieldWidth, gridTop + fieldHeight), "uV", "dominant value", selected.DominantValue, SegmentColors.Blue.Solid, "dominant-value", MinEncoding, MaxEncoding);
            DrawFieldCard(canvas, new SKRect(left, gridTop + fieldHeight + cardGap, left + fieldWidth, gridTop + fieldHeight * 2f + cardGap), "iV", "recessive value", selected.RecessiveValue, SegmentColors.Red.Solid, "recessive-value", MinEncoding, MaxEncoding);
            DrawFieldCard(canvas, new SKRect(right, gridTop + fieldHeight + cardGap, right + fieldWidth, gridTop + fieldHeight * 2f + cardGap), "u", "dominant unit", selected.DominantUnit, SegmentColors.Blue.Solid, "dominant-unit", MinEncoding, MaxEncoding);

            float buttonsTop = gridTop + fieldHeight * 2f + cardGap + 18f;
            DrawActionButton(canvas, new SKRect(left, buttonsTop, left + fieldWidth, buttonsTop + 34f), "Directed", false, () => ApplyPresetToSelected(1, 1, 1, 1));
            DrawActionButton(canvas, new SKRect(right, buttonsTop, right + fieldWidth, buttonsTop + 34f), "Acceleration", false, () => ApplyPresetToSelected(1, -1, 1, 1));
            DrawActionButton(canvas, new SKRect(left, buttonsTop + 44f, left + fieldWidth, buttonsTop + 78f), "Bent", false, () => ApplyPresetToSelected(-1, 1, 1, 1));
            DrawActionButton(canvas, new SKRect(right, buttonsTop + 44f, right + fieldWidth, buttonsTop + 78f), "Noise Hold", false, () => ApplyPresetToSelected(0, 1, 1, 1));
        }
        else
        {
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, rect.Top + 94f), "No landmark selected");
            float messageY = rect.Top + 138f;
            PageChrome.DrawWrappedText(
                canvas,
                "Create a new landmark by dragging the seed token onto the carrier.",
                rect.Left + 20f,
                ref messageY,
                rect.Width - 40f,
                _captionPaint);
        }

        _seedRect = SKRect.Empty;
        _trashRect = SKRect.Empty;
    }

    private void DrawSeedToken(SKCanvas canvas, SKRect rect)
    {
        bool active = _dragMode == DragMode.NewPin;
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = active ? new SKColor(238, 247, 255) : new SKColor(255, 255, 255),
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = active ? new SKColor(110, 154, 220) : new SKColor(214, 214, 214),
            IsAntialias = true,
        };
        using var seedFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(245, 245, 245),
            IsAntialias = true,
        };
        using var seedStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.4f,
            Color = new SKColor(112, 112, 112),
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 16f, 16f, fill);
        canvas.DrawRoundRect(rect, 16f, 16f, stroke);
        var center = new SKPoint(rect.Left + 28f, rect.MidY);
        canvas.DrawCircle(center, 10f, seedFill);
        canvas.DrawCircle(center, 10f, seedStroke);
        canvas.DrawLine(center.X - 4f, center.Y, center.X + 4f, center.Y, seedStroke);
        canvas.DrawLine(center.X, center.Y - 4f, center.X, center.Y + 4f, seedStroke);
        canvas.DrawText("Drag Landmark", rect.MidX + 18f, rect.MidY + 4f, _tokenTextPaint);
        _seedRect = rect;
    }

    private void DrawTrashZone(SKCanvas canvas, SKRect rect)
    {
        bool active = _dragMode == DragMode.ExistingPin && _trashRect.Contains(_dragPointer);
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = active ? new SKColor(255, 237, 237) : new SKColor(255, 255, 255),
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = active ? new SKColor(214, 98, 98) : new SKColor(214, 214, 214),
            IsAntialias = true,
        };
        using var iconPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.8f,
            Color = new SKColor(124, 124, 124),
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
        };

        canvas.DrawRoundRect(rect, 16f, 16f, fill);
        canvas.DrawRoundRect(rect, 16f, 16f, stroke);
        float left = rect.Left + 26f;
        float right = left + 16f;
        float top = rect.MidY - 7f;
        float bottom = rect.MidY + 8f;
        canvas.DrawLine(left + 2f, top - 3f, right - 2f, top - 3f, iconPaint);
        canvas.DrawRect(new SKRect(left, top, right, bottom), iconPaint);
        canvas.DrawLine(left + 4f, top + 3f, left + 4f, bottom - 3f, iconPaint);
        canvas.DrawLine(right - 4f, top + 3f, right - 4f, bottom - 3f, iconPaint);
        canvas.DrawText("Trash", rect.MidX + 14f, rect.MidY + 4f, _tokenTextPaint);
        _trashRect = rect;
    }

    private void DrawFieldCard(
        SKCanvas canvas,
        SKRect rect,
        string label,
        string caption,
        long value,
        SKColor accent,
        string key,
        long minValue,
        long maxValue)
    {
        canvas.DrawRoundRect(rect, 18f, 18f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 18f, 18f, _cardStrokePaint);

        using var accentPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accent,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(new SKRect(rect.Left, rect.Top, rect.Left + 8f, rect.Bottom), 18f, 18f, accentPaint);
        using var accentTextPaint = new SKPaint
        {
            Color = accent,
            TextSize = 15f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };
        float labelY = rect.Top + 26f;
        canvas.DrawText(label, rect.Left + 18f, labelY, accentTextPaint);
        float labelWidth = accentTextPaint.MeasureText(label);
        canvas.DrawText(caption, rect.Left + 24f + labelWidth, labelY, _captionPaint);

        float trackLeft = rect.Left + 20f;
        float trackRight = rect.Right - 18f;
        float trackY = rect.Bottom - 30f;
        DrawField(canvas, key, value, accent, trackLeft, trackRight, trackY, minValue, maxValue);
    }

    private void DrawField(
        SKCanvas canvas,
        string key,
        long value,
        SKColor accent,
        float left,
        float right,
        float y,
        long minValue,
        long maxValue)
    {
        float zeroX = ValueToX(0, left, right, minValue, maxValue);
        float handleX = ValueToX(value, left, right, minValue, maxValue);

        canvas.DrawLine(left, y, right, y, _trackPaint);
        for (long tick = minValue; tick <= maxValue; tick++)
        {
            float x = ValueToX(tick, left, right, minValue, maxValue);
            float tickTop = tick == 0 ? y - 11f : y - 7f;
            float tickBottom = tick == 0 ? y + 11f : y + 7f;
            canvas.DrawLine(x, tickTop, x, tickBottom, tick == 0 ? _zeroTickPaint : _tickPaint);
            if (tick != 0)
            {
                canvas.DrawText(tick.ToString(), x, y + 24f, _tickTextPaint);
            }
        }

        canvas.DrawLine(zeroX, y - 14f, zeroX, y + 14f, _zeroTickPaint);

        using var valuePaint = new SKPaint
        {
            Color = new SKColor(72, 72, 72),
            TextSize = 13f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };
        using var handleFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accent,
            IsAntialias = true,
        };

        float bubbleCenterX = (left + right) * 0.5f;
        var bubbleRect = new SKRect(bubbleCenterX - 24f, y - 50f, bubbleCenterX + 24f, y - 22f);
        canvas.DrawRoundRect(bubbleRect, 12f, 12f, _badgeFillPaint);
        canvas.DrawRoundRect(bubbleRect, 12f, 12f, _badgeStrokePaint);
        canvas.DrawText(value.ToString(), bubbleRect.MidX, bubbleRect.MidY + 5f, valuePaint);

        canvas.DrawCircle(handleX, y, 10f, handleFill);
        canvas.DrawCircle(handleX, y, 10f, _handleStrokePaint);

        var interactiveRect = new SKRect(left - 14f, y - 20f, right + 14f, y + 20f);
        _fieldLayouts.Add(new FieldLayout(key, interactiveRect, left, right, minValue, maxValue));
    }

    private void DrawActionButton(SKCanvas canvas, SKRect rect, string text, bool isActive, Action onClick)
    {
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = isActive ? new SKColor(236, 244, 255) : _buttonFillPaint.Color,
            IsAntialias = true,
        };
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = isActive ? new SKColor(110, 154, 220) : _buttonStrokePaint.Color,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 14f, 14f, fillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, strokePaint);
        canvas.DrawText(text, rect.MidX, rect.MidY + 4f, _buttonTextPaint);
        _buttonLayouts.Add(new ButtonLayout(rect, onClick));
    }

    private void DrawScenePanel(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 22f, 22f, _sceneFillPaint);
        canvas.DrawRoundRect(rect, 22f, 22f, _sceneStrokePaint);

        canvas.DrawText("Carrier + Landmark Response", rect.Left + 22f, rect.Top + 30f, _labelPaint);
        DrawBadge(canvas, new SKPoint(rect.Left + 22f, rect.Top + 58f), $"Segment: {FormatHalfStep(_segmentStartHalfSteps)} to {FormatHalfStep(_segmentEndHalfSteps)}");
        if (SelectedPin is { } selected)
        {
            DrawBadge(canvas, new SKPoint(rect.Left + 320f, rect.Top + 58f), $"Selected: P{selected.Id}");
            DrawBadge(canvas, new SKPoint(rect.Left + 22f, rect.Top + 92f), DescribePinBehavior(selected));
            DrawBadge(canvas, new SKPoint(rect.Left + 240f, rect.Top + 92f), $"Relation: {selected.BuildDescriptor().Relation}");
        }

        var viewport = new SKRect(rect.Left + 18f, rect.Top + 126f, rect.Right - 18f, rect.Bottom - 96f);
        DrawSceneViewport(canvas, viewport);

        float tokenTop = rect.Bottom - 72f;
        DrawSeedToken(canvas, new SKRect(rect.Left + 22f, tokenTop, rect.Left + 168f, tokenTop + 44f));
        DrawTrashZone(canvas, new SKRect(rect.Right - 168f, tokenTop, rect.Right - 22f, tokenTop + 44f));
    }

    private void DrawSceneViewport(SKCanvas canvas, SKRect rect)
    {
        using var viewportFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 190),
            IsAntialias = true,
        };
        using var viewportStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = new SKColor(225, 225, 225),
            IsAntialias = true,
        };
        canvas.DrawRoundRect(rect, 18f, 18f, viewportFill);
        canvas.DrawRoundRect(rect, 18f, 18f, viewportStroke);

        var layout = ComputeSceneLayout(rect);
        _sceneLayout = layout;

        canvas.DrawLine(layout.ZeroX, layout.Inner.Top, layout.ZeroX, layout.Inner.Bottom, _guidePaint);
        canvas.DrawLine(layout.Inner.Left, layout.CarrierY, layout.Inner.Right, layout.CarrierY, _rulerPaint);

        for (int tick = MinDisplayHalfSteps / 2; tick <= MaxDisplayHalfSteps / 2; tick++)
        {
            float x = ValueToX(tick, layout);
            canvas.DrawLine(x, layout.CarrierY - 8f, x, layout.CarrierY + 8f, tick == 0 ? _zeroTickPaint : _tickPaint);
            canvas.DrawText(tick.ToString("+0;-0;0"), x, layout.CarrierY + 26f, _tickTextPaint);
        }

        var pinInfos = _pins
            .Select(pin => BuildRenderInfo(layout, pin))
            .OrderBy(info => info.Origin.X)
            .ToList();

        CarrierFlowState flowState = DrawCarrierBaseline(canvas, layout, pinInfos);

        foreach (var info in pinInfos.Where(info => info.Pin.Id != _selectedPinId))
        {
            DrawLandmarkForceLines(canvas, layout, info);
        }

        if (pinInfos.FirstOrDefault(info => info.Pin.Id == _selectedPinId) is { } selectedInfo && selectedInfo.Pin is not null)
        {
            DrawLandmarkForceLines(canvas, layout, selectedInfo);
        }

        foreach (var info in pinInfos.Where(info => info.Pin.Id != _selectedPinId))
        {
            DrawPinStructure(canvas, layout.UnitScale, info, showPinTag: false, alpha: SecondaryOverlayAlpha);
        }

        if (pinInfos.FirstOrDefault(info => info.Pin.Id == _selectedPinId) is { } selectedOverlay && selectedOverlay.Pin is not null)
        {
            DrawPinStructure(canvas, layout.UnitScale, selectedOverlay, showPinTag: true, alpha: SelectedOverlayAlpha);
        }

        DrawEndpointDot(
            canvas,
            layout.StartPoint,
            flowState.FadeLeft
                ? new SKColor(_frameEndpointColor.Red, _frameEndpointColor.Green, _frameEndpointColor.Blue, 82)
                : _frameEndpointColor);
        DrawEndpointArrowhead(
            canvas,
            layout.EndPoint,
            flowState.FadeRight
                ? new SKColor(_frameEndpointColor.Red, _frameEndpointColor.Green, _frameEndpointColor.Blue, 82)
                : _frameEndpointColor);

        if (_dragMode == DragMode.NewPin)
        {
            DrawGhostPin(canvas, layout, _dragPointer);
        }
    }

    private void DrawEndpointDot(SKCanvas canvas, SKPoint point, SKColor color)
    {
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = new SKColor(88, 88, 88, color.Alpha),
            IsAntialias = true,
        };
        canvas.DrawCircle(point, 10f, fill);
        canvas.DrawCircle(point, 10f, stroke);
    }

    private void DrawEndpointArrowhead(SKCanvas canvas, SKPoint point, SKColor color)
    {
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = new SKColor(88, 88, 88, color.Alpha),
            IsAntialias = true,
        };

        using var path = new SKPath();
        path.MoveTo(point.X + 11f, point.Y);
        path.LineTo(point.X - 6f, point.Y - 10f);
        path.LineTo(point.X - 6f, point.Y + 10f);
        path.Close();
        canvas.DrawPath(path, fill);
        canvas.DrawPath(path, stroke);
    }

    private void DrawGhostPin(SKCanvas canvas, SceneLayout layout, SKPoint pointer)
    {
        var ghostDescriptor = new LandmarkPinModel(-1, 0, 1, 1, 1, 1);
        if (TryProjectToCarrier(pointer, out int halfSteps))
        {
            ghostDescriptor.LocationHalfSteps = ClampHalfStep(halfSteps, _segmentStartHalfSteps, _segmentEndHalfSteps);
            DrawPinStructure(canvas, layout.UnitScale, BuildRenderInfo(layout, ghostDescriptor), showPinTag: false, alpha: 110);
            return;
        }

        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 190),
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.4f,
            Color = new SKColor(112, 112, 112, 180),
            IsAntialias = true,
        };
        canvas.DrawCircle(pointer, 10f, fill);
        canvas.DrawCircle(pointer, 10f, stroke);
    }

    private void DrawPinStructure(SKCanvas canvas, float scale, LandmarkRenderInfo info, bool showPinTag, byte alpha)
    {
        _pinLayouts.Add(new PinLayout(info.Pin.Id, new SKRect(info.Origin.X - 14f, info.Origin.Y - 14f, info.Origin.X + 14f, info.Origin.Y + 14f)));

        DrawUnitDisplay(canvas, info.Origin, scale, info.Geometry, alpha);

        DrawRay(
            canvas,
            info.Origin,
            scale,
            info.Geometry.RecessiveRay,
            WithAlpha(SegmentColors.Red.Solid, alpha),
            incoming: true);
        DrawRay(
            canvas,
            info.Origin,
            scale,
            info.Geometry.DominantRay,
            WithAlpha(SegmentColors.Blue.Solid, alpha),
            incoming: false);

        DrawNoiseIndicators(canvas, info.Origin, info.Pin, alpha);

        canvas.DrawCircle(info.Origin, VisualStyle.OriginDotRadius, _originFillPaint);
        canvas.DrawCircle(info.Origin, VisualStyle.OriginDotRadius, _originStrokePaint);
        canvas.DrawCircle(info.Origin, 3f, _originDotPaint);

        if (showPinTag)
        {
            canvas.DrawText($"P{info.Pin.Id}", info.Origin.X + 10f, info.Origin.Y - 16f, _pinLabelPaint);
        }
    }

    private void DrawUnitDisplay(SKCanvas canvas, SKPoint origin, float scale, PinAxisDisplayGeometry geometry, byte alpha)
    {
        bool recessiveNoise = geometry.Resolution.RecessiveSide.ValueSign == 0 || geometry.Resolution.RecessiveSide.UnitSign == 0;
        bool dominantNoise = geometry.Resolution.DominantSide.ValueSign == 0 || geometry.Resolution.DominantSide.UnitSign == 0;
        SKPoint recessiveDisplayBasis = recessiveNoise ? SKPoint.Empty : geometry.RecessiveBasis;
        SKPoint dominantDisplayBasis = dominantNoise ? SKPoint.Empty : geometry.DominantBasis;
        SKPoint recessiveUnitBasis = GetUnitBasis(geometry.Resolution.RecessiveSide);
        SKPoint dominantUnitBasis = GetUnitBasis(geometry.Resolution.DominantSide);

        bool showUnitCell =
            !recessiveNoise &&
            !dominantNoise &&
            recessiveDisplayBasis != SKPoint.Empty &&
            dominantDisplayBasis != SKPoint.Empty &&
            AreOrthogonal(recessiveDisplayBasis, dominantDisplayBasis);

        if (showUnitCell)
        {
            DrawUnitCell(canvas, origin, scale, recessiveDisplayBasis, dominantDisplayBasis, alpha);
            return;
        }

        if (!recessiveNoise && recessiveDisplayBasis != SKPoint.Empty && recessiveUnitBasis != SKPoint.Empty)
        {
            SKRect? recessiveRect = GetUnitMarkerRect(origin, scale, recessiveDisplayBasis, recessiveUnitBasis);
            if (recessiveRect.HasValue)
            {
                DrawUnitMarker(canvas, recessiveRect.Value, WithAlpha(SegmentColors.Red.Solid, alpha));
            }
        }

        if (!dominantNoise && dominantDisplayBasis != SKPoint.Empty && dominantUnitBasis != SKPoint.Empty)
        {
            SKRect? dominantRect = GetUnitMarkerRect(origin, scale, dominantDisplayBasis, dominantUnitBasis);
            if (dominantRect.HasValue)
            {
                DrawUnitMarker(canvas, dominantRect.Value, WithAlpha(SegmentColors.Blue.Solid, alpha));
            }
        }
    }

    private void DrawRay(
        SKCanvas canvas,
        SKPoint origin,
        float scale,
        PinDisplayRay ray,
        SKColor color,
        bool incoming)
    {
        using var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4f,
            Color = color,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
        };
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };

        if (!ray.HasEndpoint)
        {
            return;
        }

        var end = ToPixel(ray.Endpoint, origin, scale);
        if (incoming)
        {
            DrawLineWithCircleEnd(canvas, origin, end, linePaint, fillPaint);
        }
        else
        {
            DrawArrow(canvas, origin, end, linePaint, fillPaint);
        }

        if (ray.IsLifted)
        {
            DrawLiftMarker(canvas, origin, end, color);
        }
    }

    private void DrawUnitMarker(SKCanvas canvas, SKRect rect, SKColor color)
    {
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(_cellFillPaint.Color.Red, _cellFillPaint.Color.Green, _cellFillPaint.Color.Blue, 190),
            IsAntialias = true,
        };
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.6f,
            Color = color,
            IsAntialias = true,
        };
        fillPaint.Color = new SKColor(fillPaint.Color.Red, fillPaint.Color.Green, fillPaint.Color.Blue, 22);
        strokePaint.Color = new SKColor(color.Red, color.Green, color.Blue, 28);

        canvas.DrawRect(rect, fillPaint);
        canvas.DrawRect(rect, strokePaint);
    }

    private void DrawUnitCell(SKCanvas canvas, SKPoint origin, float scale, SKPoint recessiveBasis, SKPoint dominantBasis, byte alpha)
    {
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(_cellFillPaint.Color.Red, _cellFillPaint.Color.Green, _cellFillPaint.Color.Blue, Math.Min(alpha, (byte)22)),
            IsAntialias = true,
        };
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = new SKColor(_cellStrokePaint.Color.Red, _cellStrokePaint.Color.Green, _cellStrokePaint.Color.Blue, Math.Min(alpha, (byte)28)),
            IsAntialias = true,
        };

        using var path = new SKPath();
        SKPoint[] corners =
        [
            origin,
            ToPixel(recessiveBasis, origin, scale),
            ToPixel(new SKPoint(recessiveBasis.X + dominantBasis.X, recessiveBasis.Y + dominantBasis.Y), origin, scale),
            ToPixel(dominantBasis, origin, scale),
        ];

        path.MoveTo(corners[0]);
        for (int index = 1; index < corners.Length; index++)
        {
            path.LineTo(corners[index]);
        }

        path.Close();
        canvas.DrawPath(path, fillPaint);
        canvas.DrawPath(path, strokePaint);
    }

    private SKRect? GetUnitMarkerRect(SKPoint origin, float scale, SKPoint displayBasis, SKPoint unitBasis)
    {
        if (displayBasis == SKPoint.Empty || unitBasis == SKPoint.Empty)
        {
            return null;
        }

        float length = scale;
        float thickness = scale * 0.09f;
        float offset = thickness * 1.15f;
        var center = new SKPoint(origin.X + unitBasis.Y * offset, origin.Y + unitBasis.X * offset);
        var end = new SKPoint(center.X + displayBasis.X * length, center.Y - displayBasis.Y * length);
        float minX = MathF.Min(center.X, end.X);
        float maxX = MathF.Max(center.X, end.X);
        float minY = MathF.Min(center.Y, end.Y);
        float maxY = MathF.Max(center.Y, end.Y);

        return displayBasis.X != 0f
            ? new SKRect(minX, center.Y - thickness, maxX, center.Y + thickness)
            : new SKRect(center.X - thickness, minY, center.X + thickness, maxY);
    }

    private void DrawLiftMarker(SKCanvas canvas, SKPoint origin, SKPoint end, SKColor color)
    {
        using var markerPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = new SKColor(color.Red, color.Green, color.Blue, 180),
            IsAntialias = true,
        };

        float radius = 16f;
        var rect = new SKRect(origin.X - radius, origin.Y - radius, origin.X + radius, origin.Y + radius);
        float start = end.Y < origin.Y ? 180f : 0f;
        canvas.DrawArc(rect, start, 90f, false, markerPaint);
    }

    private void DrawNoiseIndicators(SKCanvas canvas, SKPoint origin, LandmarkPinModel pin, byte alpha)
    {
        bool recessiveNoise = pin.RecessiveUnit == 0 || pin.RecessiveValue == 0;
        bool dominantNoise = pin.DominantUnit == 0 || pin.DominantValue == 0;

        if (!recessiveNoise && !dominantNoise)
        {
            return;
        }

        if (recessiveNoise && dominantNoise)
        {
            SKColor[] colors =
            [
                WithAlpha(SegmentColors.Blue.Solid, alpha),
                WithAlpha(SegmentColors.Red.Solid, alpha),
                WithAlpha(SegmentColors.Blue.Solid, alpha),
                WithAlpha(SegmentColors.Red.Solid, alpha),
            ];

            for (int index = 0; index < colors.Length; index++)
            {
                DrawNoiseRing(canvas, origin, 22f + index * 12f, colors[index]);
            }

            return;
        }

        DrawNoiseHalo(canvas, origin, recessiveNoise ? WithAlpha(SegmentColors.Red.Solid, alpha) : WithAlpha(SegmentColors.Blue.Solid, alpha), 3);
    }

    private void DrawNoiseHalo(SKCanvas canvas, SKPoint origin, SKColor color, int ringCount)
    {
        for (int index = 0; index < ringCount; index++)
        {
            DrawNoiseRing(canvas, origin, 22f + index * 12f, color);
        }
    }

    private void DrawNoiseRing(SKCanvas canvas, SKPoint origin, float radius, SKColor color)
    {
        using var noisePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.6f,
            Color = color,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash([2f, 5f], 0f),
        };

        canvas.DrawCircle(origin, radius, noisePaint);
    }

    private void DrawArrow(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint linePaint, SKPaint fillPaint)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length < 0.001f)
        {
            return;
        }

        float ux = dx / length;
        float uy = dy / length;
        float arrowLength = 13f;
        float arrowWidth = 8f;
        var lineEnd = new SKPoint(end.X - ux * arrowLength, end.Y - uy * arrowLength);

        canvas.DrawLine(start, lineEnd, linePaint);

        float perpX = -uy;
        float perpY = ux;
        float baseX = end.X - ux * arrowLength;
        float baseY = end.Y - uy * arrowLength;

        using var arrow = new SKPath();
        arrow.MoveTo(end);
        arrow.LineTo(baseX + perpX * arrowWidth, baseY + perpY * arrowWidth);
        arrow.LineTo(baseX - perpX * arrowWidth, baseY - perpY * arrowWidth);
        arrow.Close();
        canvas.DrawPath(arrow, fillPaint);
    }

    private void DrawLineWithCircleEnd(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint linePaint, SKPaint fillPaint)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length < 0.001f)
        {
            return;
        }

        float ux = dx / length;
        float uy = dy / length;
        float radius = 6f;
        var lineEnd = new SKPoint(end.X - ux * radius, end.Y - uy * radius);
        canvas.DrawLine(start, lineEnd, linePaint);
        canvas.DrawCircle(end, radius, fillPaint);
    }

    private void DrawBadge(SKCanvas canvas, SKPoint topLeft, string text)
    {
        var bounds = new SKRect();
        _badgeTextPaint.MeasureText(text, ref bounds);
        float width = bounds.Width + 24f;
        float height = 28f;
        var rect = new SKRect(topLeft.X, topLeft.Y, topLeft.X + width, topLeft.Y + height);
        canvas.DrawRoundRect(rect, 14f, 14f, _badgeFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _badgeStrokePaint);
        canvas.DrawText(text, rect.Left + 12f, rect.Top + 19f, _badgeTextPaint);
    }

    private void UpdateFieldValue(FieldLayout layout, float pixelX)
    {
        if (SelectedPin is null)
        {
            return;
        }

        float clamped = Math.Clamp(pixelX, layout.Left, layout.Right);
        float ratio = (clamped - layout.Left) / (layout.Right - layout.Left);
        long value = (long)Math.Round(layout.MinValue + (layout.MaxValue - layout.MinValue) * ratio, MidpointRounding.AwayFromZero);

        switch (layout.Key)
        {
            case "recessive-unit":
                SelectedPin.RecessiveUnit = value;
                break;
            case "recessive-value":
                SelectedPin.RecessiveValue = value;
                break;
            case "dominant-unit":
                SelectedPin.DominantUnit = value;
                break;
            case "dominant-value":
                SelectedPin.DominantValue = value;
                break;
        }
    }

    private void ApplyPresetToSelected(long recessiveUnit, long recessiveValue, long dominantUnit, long dominantValue)
    {
        if (SelectedPin is null)
        {
            return;
        }

        SelectedPin.RecessiveUnit = recessiveUnit;
        SelectedPin.RecessiveValue = recessiveValue;
        SelectedPin.DominantUnit = dominantUnit;
        SelectedPin.DominantValue = dominantValue;
        _canvasHost?.InvalidateCanvas();
    }

    private void ClampPinsToSegment()
    {
        foreach (var pin in _pins)
        {
            pin.LocationHalfSteps = ClampHalfStep(pin.LocationHalfSteps, _segmentStartHalfSteps, _segmentEndHalfSteps);
        }
    }

    private static float ValueToX(long value, float left, float right, long minValue, long maxValue) =>
        left + (value - minValue) * (right - left) / (float)(maxValue - minValue);

    private float ValueToX(float value, SceneLayout layout) =>
        layout.Inner.Left + (value - MinDisplayHalfSteps / 2f) * layout.Inner.Width / ((MaxDisplayHalfSteps - MinDisplayHalfSteps) / 2f);

    private int XToHalfStep(float x)
    {
        if (_sceneLayout is null)
        {
            return 0;
        }

        float ratio = (x - _sceneLayout.Inner.Left) / _sceneLayout.Inner.Width;
        float value = MinDisplayHalfSteps / 2f + ratio * ((MaxDisplayHalfSteps - MinDisplayHalfSteps) / 2f);
        return (int)Math.Round(value * 2f, MidpointRounding.AwayFromZero);
    }

    private bool TryProjectToCarrier(SKPoint point, out int halfSteps)
    {
        if (_sceneLayout is null)
        {
            halfSteps = 0;
            return false;
        }

        float minX = Math.Min(_sceneLayout.StartPoint.X, _sceneLayout.EndPoint.X) - 10f;
        float maxX = Math.Max(_sceneLayout.StartPoint.X, _sceneLayout.EndPoint.X) + 10f;
        if (point.X < minX || point.X > maxX || MathF.Abs(point.Y - _sceneLayout.CarrierY) > 42f)
        {
            halfSteps = 0;
            return false;
        }

        halfSteps = ClampHalfStep(XToHalfStep(point.X), _segmentStartHalfSteps, _segmentEndHalfSteps);
        return true;
    }

    private SceneLayout ComputeSceneLayout(SKRect rect)
    {
        float padding = 28f;
        var inner = new SKRect(rect.Left + padding, rect.Top + 28f, rect.Right - padding, rect.Bottom - padding);
        float carrierY = inner.MidY + 24f;
        float unitScale = inner.Width / ((MaxDisplayHalfSteps - MinDisplayHalfSteps) / 2f);
        float zeroX = inner.Left + (0f - MinDisplayHalfSteps / 2f) * inner.Width / ((MaxDisplayHalfSteps - MinDisplayHalfSteps) / 2f);
        var layout = new SceneLayout(rect, inner, carrierY, unitScale, zeroX, SKPoint.Empty, SKPoint.Empty, SKRect.Empty, SKRect.Empty);
        var startPoint = new SKPoint(ValueToX(_segmentStartHalfSteps / 2f, layout), carrierY);
        var endPoint = new SKPoint(ValueToX(_segmentEndHalfSteps / 2f, layout), carrierY);
        var startHandleRect = new SKRect(startPoint.X - 14f, startPoint.Y - 14f, startPoint.X + 14f, startPoint.Y + 14f);
        var endHandleRect = new SKRect(endPoint.X - 14f, endPoint.Y - 14f, endPoint.X + 14f, endPoint.Y + 14f);
        return layout with { StartPoint = startPoint, EndPoint = endPoint, StartHandleRect = startHandleRect, EndHandleRect = endHandleRect };
    }

    private FieldLayout? HitField(SKPoint pixelPoint) =>
        _fieldLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint.X, pixelPoint.Y));

    private ButtonLayout? HitButton(SKPoint pixelPoint) =>
        _buttonLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint.X, pixelPoint.Y));

    private PinLayout? HitPin(SKPoint pixelPoint) =>
        _pinLayouts.LastOrDefault(layout => layout.Rect.Contains(pixelPoint.X, pixelPoint.Y));

    private static int ClampHalfStep(int value, int minValue, int maxValue) =>
        Math.Clamp(value, minValue, maxValue);

    private static SKPoint ToPixel(SKPoint logicalPoint, SKPoint origin, float scale) =>
        new(origin.X + logicalPoint.X * scale, origin.Y - logicalPoint.Y * scale);

    private LandmarkRenderInfo BuildRenderInfo(SceneLayout layout, LandmarkPinModel pin)
    {
        var geometry = new PinAxisDisplayGeometry(pin.BuildDescriptor());
        var origin = new SKPoint(ValueToX(pin.LocationHalfSteps / 2f, layout), layout.CarrierY);
        return new LandmarkRenderInfo(pin, geometry, origin, GetLandmarkAccentColor(pin.Id));
    }

    private CarrierFlowState DrawCarrierBaseline(SKCanvas canvas, SceneLayout layout, IReadOnlyList<LandmarkRenderInfo> pinInfos)
    {
        canvas.DrawLine(layout.StartPoint, layout.EndPoint, _carrierGhostPaint);

        CarrierFlowState flowState = ComputeCarrierFlowState(layout, pinInfos);
        if (!flowState.FadeLeft && !flowState.FadeRight)
        {
            canvas.DrawLine(layout.StartPoint, layout.EndPoint, _carrierPaint);
            return flowState;
        }

        using var cutoffPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f,
            Color = new SKColor(168, 168, 168, 82),
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };

        SKPoint activeStart = new(flowState.ActiveLeftX, layout.CarrierY);
        SKPoint activeEnd = new(flowState.ActiveRightX, layout.CarrierY);

        if (flowState.FadeLeft)
        {
            canvas.DrawLine(layout.StartPoint, activeStart, cutoffPaint);
        }

        if (flowState.FadeRight)
        {
            canvas.DrawLine(activeEnd, layout.EndPoint, cutoffPaint);
        }

        if (flowState.ActiveLeftX < flowState.ActiveRightX - 0.01f)
        {
            canvas.DrawLine(activeStart, activeEnd, _carrierPaint);
        }

        return flowState;
    }

    private void DrawLandmarkForceLines(SKCanvas canvas, SceneLayout layout, LandmarkRenderInfo info)
    {
        DrawLandmarkForceLine(
            canvas,
            info,
            info.Geometry.Resolution.RecessiveSide,
            info.Geometry.RecessiveBasis,
            incoming: true,
            layout.UnitScale);
        DrawLandmarkForceLine(
            canvas,
            info,
            info.Geometry.Resolution.DominantSide,
            info.Geometry.DominantBasis,
            incoming: false,
            layout.UnitScale);
    }

    private void DrawLandmarkForceLine(
        SKCanvas canvas,
        LandmarkRenderInfo info,
        PinResolvedSide side,
        SKPoint displayBasis,
        bool incoming,
        float scale)
    {
        if (!ShouldShowForceLine(side))
        {
            return;
        }

        SKPoint unitBasis = GetUnitBasis(side);
        if (unitBasis == SKPoint.Empty || displayBasis == SKPoint.Empty)
        {
            return;
        }

        SKPoint anchor = GetOffsetAnchor(info.Origin, unitBasis, scale);
        SKPoint end = new(
            anchor.X + displayBasis.X * scale * LandmarkForceLengthUnits,
            anchor.Y - displayBasis.Y * scale * LandmarkForceLengthUnits);

        byte forceAlpha = IsOpposingTimeline(side) ? (byte)102 : (byte)255;
        using var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3.2f,
            Color = WithAlpha(info.AccentColor, forceAlpha),
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = WithAlpha(info.AccentColor, forceAlpha),
            IsAntialias = true,
        };

        if (incoming)
        {
            DrawLineWithCircleEnd(canvas, anchor, end, linePaint, fillPaint);
        }
        else
        {
            DrawArrow(canvas, anchor, end, linePaint, fillPaint);
        }
    }

    private CarrierFlowState ComputeCarrierFlowState(SceneLayout layout, IReadOnlyList<LandmarkRenderInfo> pinInfos)
    {
        float activeLeftX = layout.StartPoint.X;
        float activeRightX = layout.EndPoint.X;
        bool fadeLeft = false;
        bool fadeRight = false;

        foreach (var info in pinInfos)
        {
            if (IsLeftBlocking(info.Geometry.Resolution.DominantSide))
            {
                activeLeftX = MathF.Max(activeLeftX, info.Origin.X);
                fadeLeft = true;
            }

            if (IsRightBlocking(info.Geometry.Resolution.RecessiveSide))
            {
                activeRightX = MathF.Min(activeRightX, info.Origin.X);
                fadeRight = true;
            }
        }

        if (activeRightX < activeLeftX)
        {
            float midpoint = (activeLeftX + activeRightX) * 0.5f;
            activeLeftX = midpoint;
            activeRightX = midpoint;
        }

        return new CarrierFlowState(activeLeftX, activeRightX, fadeLeft, fadeRight);
    }

    private static bool IsLeftBlocking(PinResolvedSide side) =>
        side.Role == PinSideRole.Dominant && IsOpposingTimeline(side);

    private static bool IsRightBlocking(PinResolvedSide side) =>
        side.Role == PinSideRole.Recessive && IsOpposingTimeline(side);

    private static bool IsOpposingTimeline(PinResolvedSide side) =>
        GetCarrierFlowSign(side) < 0;

    private static int GetCarrierFlowSign(PinResolvedSide side)
    {
        if (side.CarrierRank != 0 || side.UnitSign == 0 || side.ValueSign == 0 || side.DirectionSign == 0)
        {
            return 0;
        }

        return side.Role == PinSideRole.Recessive
            ? -side.DirectionSign
            : side.DirectionSign;
    }

    private static bool ShouldShowForceLine(PinResolvedSide side) =>
        side.HasCarrier &&
        side.UnitSign != 0 &&
        side.ValueSign != 0 &&
        side.DirectionSign != 0;

    private static SKPoint GetOffsetAnchor(SKPoint origin, SKPoint unitBasis, float scale)
    {
        float offset = scale * 0.12f;
        return new(
            origin.X + unitBasis.Y * offset,
            origin.Y + unitBasis.X * offset);
    }

    private static SKPoint GetUnitBasis(PinResolvedSide side)
    {
        if (!side.HasCarrier)
        {
            return SKPoint.Empty;
        }

        int naturalDirection = side.Role == PinSideRole.Recessive ? -1 : 1;
        return side.CarrierRank switch
        {
            0 => new SKPoint(naturalDirection, 0f),
            1 => new SKPoint(0f, naturalDirection),
            _ => SKPoint.Empty,
        };
    }

    private static bool AreOrthogonal(SKPoint left, SKPoint right) =>
        Math.Abs(left.X * right.X + left.Y * right.Y) < 0.01f;

    private static string DescribePinBehavior(LandmarkPinModel pin)
    {
        bool recessiveNoise = pin.RecessiveUnit == 0 || pin.RecessiveValue == 0;
        bool dominantNoise = pin.DominantUnit == 0 || pin.DominantValue == 0;
        if (recessiveNoise && dominantNoise)
        {
            return "Pure noise";
        }

        if (recessiveNoise || dominantNoise)
        {
            return "Noisy segment";
        }

        return pin.BuildDescriptor().PinResolution.Behavior switch
        {
            PinBehaviorKind.DirectedSegment => "Directed segment",
            PinBehaviorKind.SequentialReinforcement => "Acceleration",
            PinBehaviorKind.OrthogonalStructure => "Bent segment",
            _ => "Unresolved structure",
        };
    }

    private static string FormatDescriptor(Axis axis) =>
        $"[{axis.Recessive.Dominant}/{axis.Recessive.Recessive}]i + [{axis.Dominant.Dominant}/{axis.Dominant.Recessive}]";

    private static string FormatHalfStep(int halfSteps) =>
        halfSteps % 2 == 0 ? (halfSteps / 2).ToString() : $"{halfSteps}/2";

    private static SKColor WithAlpha(SKColor color, byte alpha) =>
        new(color.Red, color.Green, color.Blue, alpha);

    private static SKColor GetLandmarkAccentColor(int pinId) =>
        LandmarkAccentPalette[Math.Abs(pinId - 1) % LandmarkAccentPalette.Length];

    private sealed class LandmarkPinModel
    {
        public LandmarkPinModel(
            int id,
            int locationHalfSteps,
            long recessiveUnit,
            long recessiveValue,
            long dominantUnit,
            long dominantValue)
        {
            Id = id;
            LocationHalfSteps = locationHalfSteps;
            RecessiveUnit = recessiveUnit;
            RecessiveValue = recessiveValue;
            DominantUnit = dominantUnit;
            DominantValue = dominantValue;
        }

        public int Id { get; }
        public int LocationHalfSteps { get; set; }
        public long RecessiveUnit { get; set; }
        public long RecessiveValue { get; set; }
        public long DominantUnit { get; set; }
        public long DominantValue { get; set; }

        public Axis BuildDescriptor() => new(RecessiveValue, RecessiveUnit, DominantValue, DominantUnit);
    }

    private enum DragMode
    {
        None,
        SegmentStart,
        SegmentEnd,
        ExistingPin,
        NewPin,
    }

    private sealed record FieldLayout(
        string Key,
        SKRect Rect,
        float Left,
        float Right,
        long MinValue,
        long MaxValue);

    private sealed record ButtonLayout(SKRect Rect, Action OnClick);

    private sealed record PinLayout(int PinId, SKRect Rect);

    private sealed record LandmarkRenderInfo(
        LandmarkPinModel Pin,
        PinAxisDisplayGeometry Geometry,
        SKPoint Origin,
        SKColor AccentColor);

    private sealed record CarrierFlowState(
        float ActiveLeftX,
        float ActiveRightX,
        bool FadeLeft,
        bool FadeRight);

    private sealed record SceneLayout(
        SKRect Rect,
        SKRect Inner,
        float CarrierY,
        float UnitScale,
        float ZeroX,
        SKPoint StartPoint,
        SKPoint EndPoint,
        SKRect StartHandleRect,
        SKRect EndHandleRect);
}
