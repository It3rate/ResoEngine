using Core2.Geometry;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class StripOrnamentGalleryPage : IVisualizerPage
{
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

    private SkiaCanvas? _canvasHost;
    private Panel? _controlsPanel;
    private NumericUpDown? _repeatInput;
    private IReadOnlyList<StripOrnamentResult>? _results;
    private int _cachedRepeats = -1;
    private int? _activeIndex;

    public string Title => "Strip Ornament Gallery";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureControls();
        RefreshResults();
    }

    public void Render(SKCanvas canvas)
    {
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);

        canvas.DrawText("Strip Ornament Gallery", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Each strip is generated from a compact instruction set. The lower patterns now use the same three stateful equations " +
            "with different call orders, so the visible shape comes from how the equations fire, bounce, and commit.",
            34f,
            ref subtitleY,
            700f,
            _bodyPaint);

        RefreshResults();
        if (_results is null || _results.Count == 0)
        {
            return;
        }

        var layout = ComputeLayout(_results.Count);
        canvas.DrawRoundRect(layout.Card, _cardFillPaint);
        canvas.DrawRoundRect(layout.Card, _cardBorderPaint);

        for (int index = 0; index < _results.Count; index++)
        {
            DrawStripRow(canvas, layout.RowRects[index], _results[index], index == _activeIndex);
        }

        DrawDetails(canvas, layout.DetailRect, SelectedResult());
    }

    public void Destroy()
    {
        if (_controlsPanel is not null)
        {
            _canvasHost?.Controls.Remove(_controlsPanel);
            _controlsPanel.Dispose();
            _controlsPanel = null;
        }

        _activeIndex = null;
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
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost is null || _results is null || _results.Count == 0)
        {
            return;
        }

        var layout = ComputeLayout(_results.Count);
        for (int index = 0; index < layout.RowRects.Count; index++)
        {
            if (layout.RowRects[index].Contains(pixelPoint.X, pixelPoint.Y))
            {
                if (_activeIndex != index)
                {
                    _activeIndex = index;
                    _canvasHost.InvalidateCanvas();
                }

                return;
            }
        }
    }

    private void EnsureControls()
    {
        if (_canvasHost is null || _controlsPanel is not null)
        {
            return;
        }

        _controlsPanel = new Panel
        {
            Size = new Size(218, 86),
            BackColor = Color.FromArgb(248, 248, 248),
        };

        var title = new Label
        {
            Text = "Frieze Gallery",
            Location = new Point(14, 10),
            Size = new Size(190, 18),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(62, 62, 62),
        };

        var repeatsLabel = new Label
        {
            Text = "Repeats",
            Location = new Point(14, 40),
            Size = new Size(96, 18),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(92, 92, 92),
        };

        _repeatInput = new NumericUpDown
        {
            Minimum = 4,
            Maximum = 14,
            Value = 8,
            Increment = 1,
            DecimalPlaces = 0,
            Location = new Point(116, 37),
            Size = new Size(74, 24),
            Font = new Font(VisualStyle.UiFontFamily, 9f, FontStyle.Regular),
        };
        _repeatInput.ValueChanged += (_, _) =>
        {
            _cachedRepeats = -1;
            _canvasHost?.InvalidateCanvas();
        };

        _controlsPanel.Controls.Add(title);
        _controlsPanel.Controls.Add(repeatsLabel);
        _controlsPanel.Controls.Add(_repeatInput);
        _canvasHost.Controls.Add(_controlsPanel);
        PageChrome.PositionTopRightPanel(_canvasHost, _controlsPanel);
    }

    private void RefreshResults()
    {
        int repeats = (int)(_repeatInput?.Value ?? 8m);
        if (_results is not null && _cachedRepeats == repeats)
        {
            return;
        }

        _results = StripOrnamentCatalog.GalleryPatterns
            .Select(pattern => StripOrnamentComposer.Compose(pattern, repeats))
            .ToArray();

        _cachedRepeats = repeats;
        if (_activeIndex is null && _results.Count > 0)
        {
            _activeIndex = 0;
        }
        else if (_activeIndex is not null && _activeIndex >= _results.Count)
        {
            _activeIndex = _results.Count - 1;
        }
    }

    private StripOrnamentResult SelectedResult()
    {
        if (_results is null || _results.Count == 0)
        {
            throw new InvalidOperationException("No ornament results are available.");
        }

        int index = Math.Clamp(_activeIndex ?? 0, 0, _results.Count - 1);
        return _results[index];
    }

    private GalleryLayout ComputeLayout(int patternCount)
    {
        float cardLeft = 20f;
        float cardTop = 100f;
        float cardRight = cardLeft + 860f;
        float cardBottom = cardTop + 660f;

        var card = new SKRoundRect(new SKRect(cardLeft, cardTop, cardRight, cardBottom), 24f, 24f);
        var rowRects = new List<SKRect>(patternCount);

        float rowsTop = cardTop + 10f;
        float detailHeight = Math.Clamp((cardBottom - cardTop) * 0.28f, 160f, 210f);
        float rowsBottom = cardBottom - detailHeight - 48f;
        float rowGap = 10f;
        float availableHeight = rowsBottom - rowsTop - rowGap * (patternCount - 1);
        float rowHeight = availableHeight / patternCount;

        for (int index = 0; index < patternCount; index++)
        {
            float top = rowsTop + index * (rowHeight + rowGap);
            rowRects.Add(new SKRect(cardLeft + 20f, top, cardRight - 20f, top + rowHeight));
        }

        var detailRect = new SKRect(cardLeft + 20f, rowsBottom + 26f, cardRight - 20f, cardBottom - 24f);
        return new GalleryLayout(card, rowRects, detailRect);
    }

    private void DrawStripRow(SKCanvas canvas, SKRect rowRect, StripOrnamentResult result, bool isActive)
    {
        if (isActive)
        {
            using var highlight = new SKRoundRect(rowRect, 14f, 14f);
            canvas.DrawRoundRect(highlight, _rowHighlightPaint);
        }

        canvas.DrawLine(rowRect.Left, rowRect.Bottom + 5f, rowRect.Right, rowRect.Bottom + 5f, _dividerPaint);

        float labelX = rowRect.Left + 12f;
        canvas.DrawText(result.Pattern.DisplayName, labelX, rowRect.Top + 22f, _labelPaint);
        canvas.DrawText($"{result.Pattern.Strands.Count} strands · {result.Pattern.StepsPerRepeat} steps", labelX, rowRect.Top + 42f, _mutedPaint);

        var stripRect = new SKRect(rowRect.Left + 176f, rowRect.Top + 8f, rowRect.Right - 14f, rowRect.Bottom - 8f);
        canvas.DrawLine(stripRect.Left, stripRect.MidY, stripRect.Right, stripRect.MidY, _stripGuidePaint);
        DrawPath(canvas, stripRect, result);
    }

    private void DrawPath(SKCanvas canvas, SKRect rect, StripOrnamentResult result)
    {
        if (result.Segments.Count == 0)
        {
            return;
        }

        float width = Math.Max(1, result.MaxX - result.MinX);
        float height = Math.Max(1, result.MaxY - result.MinY);
        float scaleX = (rect.Width - 10f) / width;
        float scaleY = (rect.Height - 10f) / Math.Max(1f, height);
        float scale = Math.Min(scaleX, scaleY);
        float xPad = (rect.Width - width * scale) * 0.5f;
        float yPad = (rect.Height - Math.Max(1f, height) * scale) * 0.5f;

        SKPoint Map(StripPoint point)
        {
            float x = rect.Left + xPad + (point.X - result.MinX) * scale;
            float y = rect.Bottom - yPad - (point.Y - result.MinY) * scale;
            return new SKPoint(x, y);
        }

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

    private void DrawDetails(SKCanvas canvas, SKRect detailRect, StripOrnamentResult selected)
    {
        canvas.DrawLine(detailRect.Left, detailRect.Top - 12f, detailRect.Right, detailRect.Top - 12f, _dividerPaint);

        float y = detailRect.Top + 24f;
        PageChrome.DrawWrappedText(canvas, selected.Pattern.Description, detailRect.Left, ref y, detailRect.Width, _detailBodyPaint);

        if (!string.IsNullOrWhiteSpace(selected.Pattern.CallPattern))
        {
            PageChrome.DrawWrappedText(
                canvas,
                $"call cycle: {selected.Pattern.CallPattern}",
                detailRect.Left,
                ref y,
                detailRect.Width,
                _mutedPaint);
        }

        canvas.DrawText(
            $"cycle {selected.Pattern.StepsPerRepeat} steps   ·   repeats {selected.RepeatCount}   ·   end ({selected.Cursor.X}, {selected.Cursor.Y})",
            detailRect.Left,
            y + 6f,
            _mutedPaint);

        float strandY = y + 32f;
        foreach (var strand in selected.Pattern.Strands)
        {
            canvas.DrawText(strand.Name, detailRect.Left, strandY, _labelPaint);
            canvas.DrawText(strand.Rhythm, detailRect.Left + 118f, strandY, _mutedPaint);

            float detailLineY = strandY + 18f;
            PageChrome.DrawWrappedText(
                canvas,
                strand.Description,
                detailRect.Left + 118f,
                ref detailLineY,
                detailRect.Width - 118f,
                _detailBodyPaint);

            strandY = detailLineY + 8f;
        }
    }

    private sealed record GalleryLayout(
        SKRoundRect Card,
        IReadOnlyList<SKRect> RowRects,
        SKRect DetailRect);
}
