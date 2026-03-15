using Core2.Elements;
using Core2.Geometry;
using Core2.Repetition;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class StripOrnamentGalleryPage : IVisualizerPage
{
    private const int MaxPatternSteps = 200;
    private const float EditorScale = 60f;

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

    private readonly SKPaint _rowHighlightPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(225, 249, 246),
        IsAntialias = true,
    };

    private readonly SKPaint _dividerPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(232, 232, 232),
        IsAntialias = true,
    };

    private readonly SKPaint _stripGuidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(225, 225, 225),
        IsAntialias = true,
    };

    private readonly SKPaint _pathGlowPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 8f,
        Color = new SKColor(61, 200, 220, 48),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _pathPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 3.5f,
        Color = new SKColor(34, 130, 140),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly SKPaint _labelPaint = new()
    {
        Color = new SKColor(61, 61, 61),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };

    private readonly SKPaint _mutedPaint = new()
    {
        Color = new SKColor(116, 116, 116),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _detailBodyPaint = new()
    {
        Color = new SKColor(86, 86, 86),
        TextSize = 13f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        IsAntialias = true,
    };

    private readonly SKPaint _editorFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _editorBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(220, 220, 220),
        IsAntialias = true,
    };

    private readonly SKPaint _rulerLinePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        IsAntialias = true,
    };

    private readonly SKPaint _zeroPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(120, 120, 120),
        IsAntialias = true,
    };

    private readonly SKPaint _tickPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(190, 190, 190),
        IsAntialias = true,
    };

    private readonly SKPaint _tickTextPaint = new()
    {
        Color = new SKColor(126, 126, 126),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Normal),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly SKPaint _resetButtonFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(243, 243, 244),
        IsAntialias = true,
    };

    private readonly SKPaint _resetButtonBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        IsAntialias = true,
    };

    private readonly SKPaint _resetButtonTextPaint = new()
    {
        Color = new SKColor(72, 72, 72),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        TextAlign = SKTextAlign.Center,
        IsAntialias = true,
    };

    private readonly EditorSegment[] _editorSegments;

    private SkiaCanvas? _canvasHost;
    private IReadOnlyList<RenderedStrip>? _results;
    private int? _activeIndex;
    private SKRect _resetButtonRect;

    public StripOrnamentGalleryPage()
    {
        var defaults = StripOrnamentCatalog.CreateDefaultSegments().ToDictionary(
            definition => definition.Name,
            StringComparer.OrdinalIgnoreCase);

        _editorSegments =
        [
            new EditorSegment(defaults["X0"], "Short horizontal carrier.", SegmentColors.Red),
            new EditorSegment(defaults["Y0"], "Vertical pulse carrier shown horizontally here.", SegmentColors.Blue),
            new EditorSegment(defaults["XLong"], "Long horizontal carrier.", SegmentColors.Green),
        ];
    }

    public string Title => "Strip Ornament Gallery";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureEditors(hitTest);
        _activeIndex ??= 0;
    }

    public void Render(SKCanvas canvas)
    {
        canvas.DrawText("Strip Ornament Gallery", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Each strip is generated from the same small Core 2 segment language. " +
            "The three live segments below become the shared source for timing, direction, and span across the whole pattern set.",
            34f,
            ref subtitleY,
            760f,
            _bodyPaint);

        var layout = ComputeLayout(_editorSegments.Length);
        RefreshResults(layout);
        if (_results is null || _results.Count == 0)
        {
            return;
        }

        canvas.DrawRoundRect(layout.Card, _cardFillPaint);
        canvas.DrawRoundRect(layout.Card, _cardBorderPaint);

        for (int index = 0; index < _results.Count; index++)
        {
            DrawStripRow(canvas, layout.RowRects[index], _results[index], index == _activeIndex);
        }

        DrawEditorPanel(canvas, layout, SelectedResult());
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        if (_resetButtonRect.Contains(pixelPoint.X, pixelPoint.Y))
        {
            foreach (var editor in _editorSegments)
            {
                editor.Reset();
            }

            _canvasHost?.InvalidateCanvas();
            return true;
        }

        return false;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_results is null || _results.Count == 0)
        {
            return;
        }

        var layout = ComputeLayout(_editorSegments.Length);
        for (int index = 0; index < layout.RowRects.Count; index++)
        {
            if (layout.RowRects[index].Contains(pixelPoint.X, pixelPoint.Y))
            {
                if (_activeIndex != index)
                {
                    _activeIndex = index;
                    _canvasHost?.InvalidateCanvas();
                }

                return;
            }
        }
    }

    public void Destroy()
    {
        foreach (var editor in _editorSegments)
        {
            editor.DisposeRenderer();
        }

        _results = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _rowHighlightPaint.Dispose();
        _dividerPaint.Dispose();
        _stripGuidePaint.Dispose();
        _pathGlowPaint.Dispose();
        _pathPaint.Dispose();
        _labelPaint.Dispose();
        _mutedPaint.Dispose();
        _detailBodyPaint.Dispose();
        _editorFillPaint.Dispose();
        _editorBorderPaint.Dispose();
        _rulerLinePaint.Dispose();
        _zeroPaint.Dispose();
        _tickPaint.Dispose();
        _tickTextPaint.Dispose();
        _resetButtonFillPaint.Dispose();
        _resetButtonBorderPaint.Dispose();
        _resetButtonTextPaint.Dispose();
    }

    private void EnsureEditors(HitTestEngine hitTest)
    {
        foreach (var editor in _editorSegments)
        {
            if (editor.Renderer is not null)
            {
                hitTest.Register(editor.Renderer, editor.Display);
                continue;
            }

            editor.Renderer = new SegmentRenderer(editor.Coords, SegmentOrientation.Horizontal, editor.Colors);
            hitTest.Register(editor.Renderer, editor.Display);
        }
    }

    private void RefreshResults(GalleryLayout layout)
    {
        var definitions = _editorSegments.Select(editor => editor.ToDefinition()).ToArray();
        var patterns = StripOrnamentCatalog.CreateGalleryPatterns(definitions);
        float stripWidth = Math.Max(1f, layout.RowRects[0].Width - 202f);
        float rowHeight = layout.RowRects[0].Height;

        _results = patterns
            .Select(pattern => BuildRenderedStrip(pattern, stripWidth, rowHeight))
            .ToArray();

        if (_activeIndex is null && _results.Count > 0)
        {
            _activeIndex = 0;
        }
        else if (_activeIndex is not null && _activeIndex >= _results.Count)
        {
            _activeIndex = _results.Count - 1;
        }
    }

    private RenderedStrip BuildRenderedStrip(StripOrnamentPattern pattern, float stripWidth, float rowHeight)
    {
        var preview = StripOrnamentComposer.Compose(pattern, 1);
        float previewScale = ComputeStripScale(rowHeight, preview);
        int targetWidth = Math.Max(1, (int)MathF.Floor((stripWidth - 12f) / previewScale));
        var result = StripOrnamentComposer.ComposeToWidth(pattern, targetWidth, MaxPatternSteps);
        return new RenderedStrip(result, ComputeStripScale(rowHeight, result));
    }

    private static float ComputeStripScale(float rowHeight, StripOrnamentResult result)
    {
        float verticalUnits = Math.Max(2f, result.VerticalSpan + 2f);
        return Math.Max(1f, MathF.Min(18f, (rowHeight - 14f) / verticalUnits));
    }

    private RenderedStrip SelectedResult()
    {
        if (_results is null || _results.Count == 0)
        {
            throw new InvalidOperationException("No ornament results are available.");
        }

        int index = Math.Clamp(_activeIndex ?? 0, 0, _results.Count - 1);
        return _results[index];
    }

    private GalleryLayout ComputeLayout(int editorCount)
    {
        float cardLeft = 20f;
        float cardTop = 100f;
        float cardRight = cardLeft + 860f;
        float cardBottom = 782f;
        var card = new SKRoundRect(new SKRect(cardLeft, cardTop, cardRight, cardBottom), 24f, 24f);

        float editorHeight = 246f;
        float rowsTop = cardTop + 12f;
        float rowsBottom = cardBottom - editorHeight - 22f;
        float rowGap = 8f;
        float availableHeight = rowsBottom - rowsTop - rowGap * (StripOrnamentCatalog.GalleryPatterns.Count - 1);
        float rowHeight = availableHeight / StripOrnamentCatalog.GalleryPatterns.Count;

        var rowRects = new List<SKRect>(StripOrnamentCatalog.GalleryPatterns.Count);
        for (int index = 0; index < StripOrnamentCatalog.GalleryPatterns.Count; index++)
        {
            float top = rowsTop + index * (rowHeight + rowGap);
            rowRects.Add(new SKRect(cardLeft + 20f, top, cardRight - 20f, top + rowHeight));
        }

        var editorRect = new SKRect(cardLeft + 20f, rowsBottom + 18f, cardRight - 20f, cardBottom - 20f);
        _resetButtonRect = new SKRect(editorRect.Right - 86f, editorRect.Top + 12f, editorRect.Right - 18f, editorRect.Top + 38f);

        float controlsTop = editorRect.Top + 70f;
        float controlGap = 8f;
        float controlHeight = (editorRect.Bottom - controlsTop - controlGap * (editorCount - 1) - 14f) / editorCount;
        var editorRows = new List<SKRect>(editorCount);
        for (int index = 0; index < editorCount; index++)
        {
            float top = controlsTop + index * (controlHeight + controlGap);
            editorRows.Add(new SKRect(editorRect.Left + 14f, top, editorRect.Right - 14f, top + controlHeight));
        }

        return new GalleryLayout(card, rowRects, editorRect, editorRows, _resetButtonRect);
    }

    private void DrawStripRow(SKCanvas canvas, SKRect rowRect, RenderedStrip rendered, bool isActive)
    {
        if (isActive)
        {
            using var highlight = new SKRoundRect(rowRect, 14f, 14f);
            canvas.DrawRoundRect(highlight, _rowHighlightPaint);
        }

        canvas.DrawLine(rowRect.Left, rowRect.Bottom + 4f, rowRect.Right, rowRect.Bottom + 4f, _dividerPaint);

        float labelX = rowRect.Left + 12f;
        canvas.DrawText(rendered.Result.Pattern.DisplayName, labelX, rowRect.Top + 22f, _labelPaint);
        canvas.DrawText(
            $"{rendered.Result.Pattern.Strands.Count} strands | {rendered.Result.RepeatCount} cycles | {rendered.Result.Pattern.StepsPerRepeat} step cell",
            labelX,
            rowRect.Top + 42f,
            _mutedPaint);

        var stripRect = new SKRect(rowRect.Left + 186f, rowRect.Top + 8f, rowRect.Right - 14f, rowRect.Bottom - 8f);
        DrawPath(canvas, stripRect, rendered);
    }

    private void DrawPath(SKCanvas canvas, SKRect rect, RenderedStrip rendered)
    {
        var result = rendered.Result;
        if (result.Segments.Count == 0)
        {
            canvas.DrawLine(rect.Left, rect.MidY, rect.Right, rect.MidY, _stripGuidePaint);
            return;
        }

        float pad = 6f;
        float scale = Math.Min(
            rendered.Scale,
            (rect.Width - 2f * pad) / Math.Max(1f, result.HorizontalSpan));
        float heightPx = result.VerticalSpan * scale;
        float topSlack = Math.Max(0f, rect.Height - 2f * pad - heightPx);
        float originY = rect.Top + pad + result.MaxY * scale + topSlack * 0.5f;
        float originX = result.FlowsLeft
            ? rect.Right - pad - result.MaxX * scale
            : rect.Left + pad - result.MinX * scale;

        canvas.DrawLine(rect.Left, originY, rect.Right, originY, _stripGuidePaint);

        SKPoint Map(StripPoint point) =>
            new(originX + point.X * scale, originY - point.Y * scale);

        using var path = new SKPath();
        bool first = true;
        foreach (var segment in result.Segments)
        {
            var start = Map(segment.Start);
            var end = Map(segment.End);
            if (first)
            {
                path.MoveTo(start);
                first = false;
            }
            else if (path.LastPoint != start)
            {
                path.MoveTo(start);
            }

            path.LineTo(end);
        }

        canvas.DrawPath(path, _pathGlowPaint);
        canvas.DrawPath(path, _pathPaint);
    }

    private void DrawEditorPanel(SKCanvas canvas, GalleryLayout layout, RenderedStrip selected)
    {
        canvas.DrawLine(layout.EditorRect.Left, layout.EditorRect.Top - 12f, layout.EditorRect.Right, layout.EditorRect.Top - 12f, _dividerPaint);

        canvas.DrawText("Live Segment Controls", layout.EditorRect.Left, layout.EditorRect.Top + 22f, _labelPaint);
        canvas.DrawRoundRect(_resetButtonRect, 12f, 12f, _resetButtonFillPaint);
        canvas.DrawRoundRect(_resetButtonRect, 12f, 12f, _resetButtonBorderPaint);
        canvas.DrawText("Reset", _resetButtonRect.MidX, _resetButtonRect.MidY + 4f, _resetButtonTextPaint);

        float infoY = layout.EditorRect.Top + 44f;
        canvas.DrawText(
            $"{selected.Result.Pattern.DisplayName} fills the strip in {selected.Result.RepeatCount} cycle(s).",
            layout.EditorRect.Left,
            infoY,
            _detailBodyPaint);

        if (!string.IsNullOrWhiteSpace(selected.Result.Pattern.CallPattern))
        {
            float callY = infoY + 22f;
            PageChrome.DrawWrappedText(
                canvas,
                $"call cycle: {selected.Result.Pattern.CallPattern}",
                layout.EditorRect.Left,
                ref callY,
                layout.EditorRect.Width - 110f,
                _mutedPaint);
        }

        for (int index = 0; index < _editorSegments.Length; index++)
        {
            DrawEditorRow(canvas, layout.EditorRows[index], _editorSegments[index]);
        }
    }

    private void DrawEditorRow(SKCanvas canvas, SKRect rowRect, EditorSegment editor)
    {
        using var panel = new SKRoundRect(rowRect, 14f, 14f);
        canvas.DrawRoundRect(panel, _editorFillPaint);
        canvas.DrawRoundRect(panel, _editorBorderPaint);

        float labelX = rowRect.Left + 14f;
        canvas.DrawText(editor.Name, labelX, rowRect.Top + 22f, _labelPaint);
        canvas.DrawText(editor.BehaviorLabel, labelX + _labelPaint.MeasureText(editor.Name) + 10f, rowRect.Top + 22f, _mutedPaint);
        canvas.DrawText(editor.Role, rowRect.Left + 14f, rowRect.Top + 40f, _mutedPaint);

        var rulerRect = new SKRect(rowRect.Left + 238f, rowRect.Top + 6f, rowRect.Right - 14f, rowRect.Bottom - 6f);
        editor.Coords.OriginX = rulerRect.MidX;
        editor.Coords.OriginY = rulerRect.MidY - 2f;

        DrawEditorRuler(canvas, rulerRect, editor.Coords);
        editor.Renderer?.Render(canvas, editor.Display);
    }

    private void DrawEditorRuler(SKCanvas canvas, SKRect rect, CoordinateSystem coords)
    {
        float left = coords.OriginX - 4f * coords.Scale;
        float right = coords.OriginX + 4f * coords.Scale;
        canvas.DrawLine(left, coords.OriginY, right, coords.OriginY, _rulerLinePaint);

        for (int tick = -4; tick <= 4; tick++)
        {
            float x = coords.OriginX + tick * coords.Scale;
            float tickTop = coords.OriginY - 9f;
            float tickBottom = coords.OriginY + 9f;

            canvas.DrawLine(x, tickTop, x, tickBottom, tick == 0 ? _zeroPaint : _tickPaint);
            canvas.DrawText(tick.ToString("+0;-0;0"), x, rect.Top + 14f, _tickTextPaint);
        }
    }

    private sealed class EditorSegment
    {
        public EditorSegment(StripSegmentDefinition definition, string role, SegmentColorSet colors)
        {
            Definition = definition;
            DefaultAxis = definition.Segment;
            Role = role;
            Colors = colors;
            Display = new AxisDisplayMapper(definition.Segment, definition.Name, 1f);
            Coords = new CoordinateSystem(scale: EditorScale);
        }

        public StripSegmentDefinition Definition { get; }
        public Axis DefaultAxis { get; }
        public string Role { get; }
        public SegmentColorSet Colors { get; }
        public AxisDisplayMapper Display { get; }
        public CoordinateSystem Coords { get; }
        public SegmentRenderer? Renderer { get; set; }
        public string Name => Definition.Name;
        public string BehaviorLabel => Definition.Law == BoundaryContinuationLaw.ReflectiveBounce ? "Reflecting" : "Continuous";

        public StripSegmentDefinition ToDefinition() =>
            new(
                Definition.Name,
                Display.Axis,
                Definition.Law,
                Definition.AxisVector,
                Definition.StepMode,
                Definition.UseSegmentAsFrame,
                Definition.Seed);

        public void Reset() => Display.SetAxis(DefaultAxis);

        public void DisposeRenderer()
        {
            Renderer?.Dispose();
            Renderer = null;
        }
    }

    private sealed record RenderedStrip(StripOrnamentResult Result, float Scale);

    private sealed record GalleryLayout(
        SKRoundRect Card,
        IReadOnlyList<SKRect> RowRects,
        SKRect EditorRect,
        IReadOnlyList<SKRect> EditorRows,
        SKRect ResetButtonRect);
}
