using Applied.Geometry.Frieze;
using Applied.Geometry.Utils;
using Core2.Elements;
using Core2.Repetition;
using ResoEngine.Visualizer.Adapt;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Rendering;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public class FriezePatternEditorPage : IVisualizerPage
{
    private const int MaxColumns = 10;
    private const int MaxPatternSteps = 200;
    private const float StepScale = 24f;
    private const float SpanScale = 44f;

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

    private readonly SKPaint _previewFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _previewBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(220, 220, 220),
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

    private readonly SKPaint _guidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(225, 225, 225),
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

    private readonly SKPaint _sectionLabelPaint = new()
    {
        Color = new SKColor(104, 104, 104),
        TextSize = 11f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
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

    private readonly SKPaint _lawButtonFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };

    private readonly SKPaint _lawButtonSelectedPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(222, 244, 248),
        IsAntialias = true,
    };

    private readonly SKPaint _lawButtonBorderPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(205, 205, 205),
        IsAntialias = true,
    };

    private readonly SKPaint _lawIconPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.8f,
        Color = new SKColor(86, 86, 86),
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round,
        IsAntialias = true,
    };

    private readonly IReadOnlyList<FriezePattern> _basePatterns = FriezeCatalog.GalleryPatterns;
    private readonly EditorSegment[] _editorSegments;

    private SkiaCanvas? _canvasHost;
    private Panel? _selectorPanel;
    private Panel? _palettePanel;
    private Panel? _gridHostPanel;
    private Button? _trashButton;
    private Bitmap? _trashIcon;
    private readonly List<Button> _patternButtons = [];
    private readonly List<Button> _paletteButtons = [];
    private readonly List<Control> _gridColumns = [];
    private readonly List<LawButtonHit> _lawHitTargets = [];

    private int _selectedPatternIndex;
    private List<List<string>> _workingColumns = [];

    public FriezePatternEditorPage()
    {
        var defaults = FriezeCatalog.CreateDefaultSegments().ToDictionary(
            definition => definition.Name,
            StringComparer.OrdinalIgnoreCase);

        _editorSegments =
        [
            new EditorSegment(defaults["X0"], "Short horizontal carrier.", SegmentColors.Red),
            new EditorSegment(defaults["Y0"], "Vertical pulse carrier shown horizontally here.", SegmentColors.Blue),
            new EditorSegment(defaults["X1"], "Long horizontal carrier.", SegmentColors.Green),
        ];
    }

    public string Title => "Frieze Pattern Editor";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _canvasHost = canvas;
        EnsureSegmentEditors(hitTest);
        EnsureControls();
        LoadPattern(_selectedPatternIndex);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        foreach (var hit in _lawHitTargets)
        {
            if (!hit.Rect.Contains(pixelPoint.X, pixelPoint.Y))
            {
                continue;
            }

            hit.Editor.SetLaw(hit.Law);
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        return false;
    }

    public void Render(SKCanvas canvas)
    {
        if (_canvasHost is null)
        {
            return;
        }

        var layout = ComputeLayout();
        LayoutControls(layout);

        canvas.DrawText("Frieze Pattern Editor", 34f, 42f, _headingPaint);
        float subtitleY = 68f;
        PageChrome.DrawWrappedText(
            canvas,
            "Choose one ornament, edit its tick columns by dragging equation tiles, and tune the shared segment carriers below. " +
            "The preview recomputes the selected pattern live from the edited sequence and the current Core 2 segment definitions.",
            34f,
            ref subtitleY,
            780f,
            _bodyPaint);

        canvas.DrawRoundRect(layout.Card, _cardFillPaint);
        canvas.DrawRoundRect(layout.Card, _cardBorderPaint);

        canvas.DrawRoundRect(layout.PreviewRect, 18f, 18f, _previewFillPaint);
        canvas.DrawRoundRect(layout.PreviewRect, 18f, 18f, _previewBorderPaint);
        DrawPreview(canvas, layout.PreviewRect);

        DrawSegmentEditor(canvas, layout.EditorRect);
    }

    public void Destroy()
    {
        if (_canvasHost is not null)
        {
            RemoveControl(_selectorPanel);
            RemoveControl(_palettePanel);
            RemoveControl(_gridHostPanel);
            RemoveControl(_trashButton);
        }

        _selectorPanel = null;
        _palettePanel = null;
        _gridHostPanel = null;
        _trashButton = null;
        _patternButtons.Clear();
        _paletteButtons.Clear();
        _gridColumns.Clear();
        _lawHitTargets.Clear();

        foreach (var editor in _editorSegments)
        {
            editor.DisposeRenderer();
        }

        _trashIcon?.Dispose();
        _trashIcon = null;
        _canvasHost = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardBorderPaint.Dispose();
        _previewFillPaint.Dispose();
        _previewBorderPaint.Dispose();
        _pathGlowPaint.Dispose();
        _pathPaint.Dispose();
        _guidePaint.Dispose();
        _labelPaint.Dispose();
        _mutedPaint.Dispose();
        _sectionLabelPaint.Dispose();
        _editorFillPaint.Dispose();
        _editorBorderPaint.Dispose();
        _rulerLinePaint.Dispose();
        _zeroPaint.Dispose();
        _tickPaint.Dispose();
        _tickTextPaint.Dispose();
        _lawButtonFillPaint.Dispose();
        _lawButtonSelectedPaint.Dispose();
        _lawButtonBorderPaint.Dispose();
        _lawIconPaint.Dispose();
    }

    private void EnsureSegmentEditors(HitTestEngine hitTest)
    {
        foreach (var editor in _editorSegments)
        {
            editor.SpanRenderer ??= new SegmentRenderer(editor.SpanCoords, SegmentOrientation.Horizontal, editor.Colors);
            editor.StepRenderer ??= new SegmentRenderer(editor.StepCoords, SegmentOrientation.Horizontal, editor.Colors);
            hitTest.Register(editor.SpanRenderer, editor.SpanDisplay);
            hitTest.Register(editor.StepRenderer, editor.StepDisplay);
        }
    }

    private void EnsureControls()
    {
        if (_canvasHost is null)
        {
            return;
        }

        if (_selectorPanel is null)
        {
            _selectorPanel = new Panel
            {
                BackColor = Color.Transparent,
            };

            for (int index = 0; index < _basePatterns.Count; index++)
            {
                int captured = index;
                var button = CreateUiButton(_basePatterns[index].DisplayName, bold: false, fontSize: 9f);
                button.Click += (_, _) =>
                {
                    LoadPattern(captured);
                    _canvasHost?.InvalidateCanvas();
                };

                _selectorPanel.Controls.Add(button);
                _patternButtons.Add(button);
            }

            _canvasHost.Controls.Add(_selectorPanel);
        }

        if (_palettePanel is null)
        {
            _palettePanel = new Panel
            {
                BackColor = Color.Transparent,
            };

            foreach (var editor in _editorSegments)
            {
                var button = CreateTokenButton(editor.Name, editor.Colors, isPalette: true, null, -1);
                _palettePanel.Controls.Add(button);
                _paletteButtons.Add(button);
            }

            _canvasHost.Controls.Add(_palettePanel);
        }

        if (_gridHostPanel is null)
        {
            _gridHostPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = false,
            };
            _canvasHost.Controls.Add(_gridHostPanel);
        }

        if (_trashButton is null)
        {
            _trashIcon ??= CreateTrashIcon();
            _trashButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(90, 90, 90),
                Text = string.Empty,
                Image = _trashIcon,
                ImageAlign = ContentAlignment.MiddleCenter,
                AllowDrop = true,
                TabStop = false,
            };
            _trashButton.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            _trashButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(240, 240, 240);
            _trashButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 248, 248);
            _trashButton.DragEnter += (_, eventArgs) =>
            {
                eventArgs.Effect = ReadDragData(eventArgs.Data)?.SourceColumnIndex is int ? DragDropEffects.Move : DragDropEffects.None;
            };
            _trashButton.DragDrop += (_, eventArgs) =>
            {
                var drag = ReadDragData(eventArgs.Data);
                if (drag?.SourceColumnIndex is int sourceColumn)
                {
                    RemoveToken(sourceColumn, drag.SourceTokenIndex);
                }
            };

            _canvasHost.Controls.Add(_trashButton);
        }
    }

    private void LoadPattern(int patternIndex)
    {
        _selectedPatternIndex = Math.Clamp(patternIndex, 0, _basePatterns.Count - 1);
        var pattern = _basePatterns[_selectedPatternIndex];
        var equations = pattern.Program?.Equations ?? FriezeCatalog.CreateDefaultSegments();
        var byName = equations.ToDictionary(equation => equation.Name, StringComparer.OrdinalIgnoreCase);
        var defaults = FriezeCatalog.CreateDefaultSegments().ToDictionary(
            definition => definition.Name,
            StringComparer.OrdinalIgnoreCase);

        foreach (var editor in _editorSegments)
        {
            var definition = byName.TryGetValue(editor.Name, out var current)
                ? current
                : defaults[editor.Name];
            editor.SetDefinition(definition);
        }

        _workingColumns = ExtractColumns(pattern.Program);
        if (_workingColumns.Count > MaxColumns)
        {
            _workingColumns = _workingColumns.Take(MaxColumns).ToList();
        }

        if (_workingColumns.Count == 0)
        {
            _workingColumns.Add([]);
        }

        while (_workingColumns.Count < MaxColumns)
        {
            _workingColumns.Add([]);
        }

        RefreshSelectorButtons();
        RefreshPaletteButtons();
        RefreshGrid();
    }

    private void RefreshSelectorButtons()
    {
        for (int index = 0; index < _patternButtons.Count; index++)
        {
            var button = _patternButtons[index];
            bool isSelected = index == _selectedPatternIndex;
            button.BackColor = isSelected ? Color.FromArgb(215, 241, 246) : Color.White;
            button.FlatAppearance.BorderColor = isSelected ? Color.FromArgb(122, 190, 201) : Color.FromArgb(210, 210, 210);
        }
    }

    private void RefreshPaletteButtons()
    {
        for (int index = 0; index < _paletteButtons.Count; index++)
        {
            var button = _paletteButtons[index];
            var editor = _editorSegments[index];
            button.Text = editor.Name;
            button.Tag = editor.Name;
        }
    }

    private void RefreshGrid()
    {
        if (_gridHostPanel is null)
        {
            return;
        }

        _gridHostPanel.SuspendLayout();
        _gridHostPanel.Controls.Clear();
        _gridColumns.Clear();

        int columnGap = 4;
        int usableWidth = Math.Max(560, _gridHostPanel.ClientSize.Width - 24);
        int columnWidth = Math.Max(92, (usableWidth - columnGap * (MaxColumns - 1)) / MaxColumns);
        int x = 12;
        int hostHeight = Math.Max(180, _gridHostPanel.ClientSize.Height - 28);

        for (int index = 0; index < _workingColumns.Count; index++)
        {
            var container = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Location = new Point(x, 12),
                Size = new Size(columnWidth, hostHeight),
                AllowDrop = true,
                Tag = index,
            };

            var title = new Label
            {
                Text = $"Tick {index + 1}",
                Font = new Font(VisualStyle.UiFontFamily, 7.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(86, 86, 86),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 8),
                Size = new Size(columnWidth - 2, 18),
                BackColor = Color.Transparent,
            };

            var flow = new FlowLayoutPanel
            {
                Location = new Point(8, 34),
                Size = new Size(columnWidth - 18, hostHeight - 44),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                AllowDrop = true,
                AutoScroll = true,
                Tag = index,
            };

            AttachColumnDropHandlers(container, index);
            AttachColumnDropHandlers(flow, index);

            for (int tokenIndex = 0; tokenIndex < _workingColumns[index].Count; tokenIndex++)
            {
                flow.Controls.Add(CreateTokenButton(_workingColumns[index][tokenIndex], FindColors(_workingColumns[index][tokenIndex]), false, index, tokenIndex));
            }

            container.Controls.Add(title);
            container.Controls.Add(flow);
            _gridHostPanel.Controls.Add(container);
            _gridColumns.Add(container);
            x += columnWidth + columnGap;
        }

        _gridHostPanel.AutoScrollMinSize = Size.Empty;
        _gridHostPanel.ResumeLayout();
    }

    private void AttachColumnDropHandlers(Control control, int columnIndex)
    {
        control.DragEnter += (_, eventArgs) =>
        {
            var drag = ReadDragData(eventArgs.Data);
            eventArgs.Effect = drag is null ? DragDropEffects.None : drag.SourceColumnIndex is null ? DragDropEffects.Copy : DragDropEffects.Move;
            HighlightColumn(columnIndex, true);
        };
        control.DragLeave += (_, _) => HighlightColumn(columnIndex, false);
        control.DragDrop += (_, eventArgs) =>
        {
            HighlightColumn(columnIndex, false);
            var drag = ReadDragData(eventArgs.Data);
            if (drag is null)
            {
                return;
            }

            if (drag.SourceColumnIndex is int sourceColumn)
            {
                string? token = RemoveToken(sourceColumn, drag.SourceTokenIndex, refresh: false);
                if (token is not null)
                {
                    _workingColumns[columnIndex].Add(token);
                }
            }
            else
            {
                _workingColumns[columnIndex].Add(drag.EquationName);
            }

            RefreshGrid();
            _canvasHost?.InvalidateCanvas();
        };
    }

    private void HighlightColumn(int columnIndex, bool highlighted)
    {
        if (columnIndex < 0 || columnIndex >= _gridColumns.Count)
        {
            return;
        }

        _gridColumns[columnIndex].BackColor = highlighted
            ? Color.FromArgb(240, 249, 252)
            : Color.White;
    }

    private string? RemoveToken(int sourceColumn, int sourceTokenIndex, bool refresh = true)
    {
        if (sourceColumn < 0 || sourceColumn >= _workingColumns.Count)
        {
            return null;
        }

        if (sourceTokenIndex < 0 || sourceTokenIndex >= _workingColumns[sourceColumn].Count)
        {
            return null;
        }

        string token = _workingColumns[sourceColumn][sourceTokenIndex];
        _workingColumns[sourceColumn].RemoveAt(sourceTokenIndex);
        if (refresh)
        {
            RefreshGrid();
            _canvasHost?.InvalidateCanvas();
        }

        return token;
    }

    private FriezePattern BuildCurrentPattern()
    {
        var basePattern = _basePatterns[_selectedPatternIndex];
        var equations = _editorSegments.Select(editor => editor.ToDefinition()).ToArray();
        var loop = new List<EquationCommand>();
        int lastActiveColumn = _workingColumns.FindLastIndex(column => column.Count > 0);
        if (lastActiveColumn < 0)
        {
            lastActiveColumn = 0;
        }

        for (int columnIndex = 0; columnIndex <= lastActiveColumn; columnIndex++)
        {
            var column = _workingColumns[columnIndex];
            foreach (var equationName in column)
            {
                loop.Add(EquationCommand.Fire(equationName));
            }

            loop.Add(EquationCommand.Commit());
        }

        var program = new EquationProgram(
            equations,
            loop,
            basePattern.Program?.Prelude);

        return basePattern with
        {
            StepsPerRepeat = Math.Max(1, lastActiveColumn + 1),
            Program = program,
        };
    }

    private void DrawPreview(SKCanvas canvas, SKRect rect)
    {
        var pattern = BuildCurrentPattern();
        int minimumWidth = Math.Max(8, (int)MathF.Floor((rect.Width - 36f) / 16f));
        var result = FriezeComposer.ComposeToWidth(pattern, minimumWidth, MaxPatternSteps);
        int activeTickCount = Math.Max(1, _workingColumns.FindLastIndex(column => column.Count > 0) + 1);

        canvas.DrawText(pattern.DisplayName, rect.Left + 18f, rect.Top + 26f, _headingPaint);
        canvas.DrawText(
            $"{activeTickCount} active ticks | {MaxColumns} editable columns | {result.RepeatCount} cycles | end ({result.Cursor.X}, {result.Cursor.Y})",
            rect.Left + 18f,
            rect.Top + 46f,
            _mutedPaint);

        var pathRect = new SKRect(rect.Left + 18f, rect.Top + 58f, rect.Right - 18f, rect.Bottom - 20f);
        DrawPath(canvas, pathRect, result);
    }

    private void DrawPath(SKCanvas canvas, SKRect rect, FriezeResult result)
    {
        if (result.Segments.Count == 0)
        {
            canvas.DrawLine(rect.Left, rect.MidY, rect.Right, rect.MidY, _guidePaint);
            return;
        }

        float scale = Math.Min(
            (rect.Width - 12f) / Math.Max(1f, result.HorizontalSpan),
            Math.Max(1f, Math.Min(20f, (rect.Height - 12f) / Math.Max(2f, result.VerticalSpan + 2f))));
        float heightPx = result.VerticalSpan * scale;
        float topSlack = Math.Max(0f, rect.Height - 12f - heightPx);
        float originY = rect.Top + 6f + result.MaxY * scale + topSlack * 0.5f;
        float originX = result.FlowsLeft
            ? rect.Right - 6f - result.MaxX * scale
            : rect.Left + 6f - result.MinX * scale;

        canvas.DrawLine(rect.Left, originY, rect.Right, originY, _guidePaint);

        SKPoint Map(PlanarPoint point) =>
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

    private void DrawSegmentEditor(SKCanvas canvas, SKRect editorRect)
    {
        _lawHitTargets.Clear();

        canvas.DrawRoundRect(editorRect, 18f, 18f, _previewFillPaint);
        canvas.DrawRoundRect(editorRect, 18f, 18f, _previewBorderPaint);
        //canvas.DrawText("Shared Segment Controls", editorRect.Left + 18f, editorRect.Top + 24f, _labelPaint);
        //canvas.DrawText("Choose the continuation law, drag the left step carrier, and drag the right span carrier.", editorRect.Left + 18f, editorRect.Top + 44f, _mutedPaint);

        float rowTop = editorRect.Top + 12f;
        float gap = 20f;
        float rowHeight = (editorRect.Height - 40f - gap * 2f) / 3f;
        for (int index = 0; index < _editorSegments.Length; index++)
        {
            float top = rowTop + index * (rowHeight + gap);
            DrawEditorRow(canvas, new SKRect(editorRect.Left + 14f, top, editorRect.Right - 14f, top + rowHeight), _editorSegments[index]);
        }
    }

    private void DrawEditorRow(SKCanvas canvas, SKRect rowRect, EditorSegment editor)
    {
        using var panel = new SKRoundRect(rowRect, 14f, 14f);
        canvas.DrawRoundRect(panel, _editorFillPaint);
        canvas.DrawRoundRect(panel, _editorBorderPaint);

        float labelX = rowRect.Left + 14f;
        canvas.DrawText(editor.Name, labelX, rowRect.Top + 26f, _labelPaint);

        float buttonLeft = rowRect.Left + 40f;
        float buttonTop = rowRect.Top + 5f;
        float buttonSize = 32f;
        float buttonGap = 8f;
        DrawLawToggleButton(canvas, new SKRect(buttonLeft, buttonTop, buttonLeft + buttonSize, buttonTop + buttonSize), editor, BoundaryContinuationLaw.ReflectiveBounce);
        DrawLawToggleButton(canvas, new SKRect(buttonLeft + buttonSize + buttonGap, buttonTop, buttonLeft + buttonSize * 2f + buttonGap, buttonTop + buttonSize), editor, BoundaryContinuationLaw.PeriodicWrap);
        DrawLawToggleButton(canvas, new SKRect(buttonLeft + (buttonSize + buttonGap) * 2f, buttonTop, buttonLeft + buttonSize * 3f + buttonGap * 2f, buttonTop + buttonSize), editor, BoundaryContinuationLaw.TensionPreserving);

        var stepRect = new SKRect(rowRect.Left + 240f, rowRect.Top + 8f, rowRect.Left + 320f, rowRect.Bottom - 8f);
        var spanRect = new SKRect(rowRect.Left + 560f, rowRect.Top + 8f, rowRect.Left + 640f, rowRect.Bottom - 8f);

        editor.StepCoords.OriginX = stepRect.MidX;
        editor.StepCoords.OriginY = stepRect.MidY + 6f;
        editor.SpanCoords.OriginX = spanRect.MidX;
        editor.SpanCoords.OriginY = spanRect.MidY + 6f;

        DrawEditorRuler(canvas, stepRect, editor.StepCoords);
        DrawEditorRuler(canvas, spanRect, editor.SpanCoords);
        editor.StepRenderer?.Render(canvas, editor.StepDisplay);
        editor.SpanRenderer?.Render(canvas, editor.SpanDisplay);
    }

    private void DrawEditorRuler(SKCanvas canvas, SKRect rect, CoordinateSystem coords)
    {
        float left = coords.OriginX - 4f * coords.Scale;
        float right = coords.OriginX + 4f * coords.Scale;
        canvas.DrawLine(left, coords.OriginY, right, coords.OriginY, _rulerLinePaint);

        for (int tick = -4; tick <= 4; tick++)
        {
            float x = coords.OriginX + tick * coords.Scale;
            canvas.DrawLine(x, coords.OriginY - 9f, x, coords.OriginY + 9f, tick == 0 ? _zeroPaint : _tickPaint);
            canvas.DrawText(tick.ToString("+0;-0;0"), x, rect.Top + 14f, _tickTextPaint);
        }
    }

    private void DrawLawToggleButton(SKCanvas canvas, SKRect rect, EditorSegment editor, BoundaryContinuationLaw law)
    {
        bool selected = editor.Law == law;
        using var roundRect = new SKRoundRect(rect, 8f, 8f);
        canvas.DrawRoundRect(roundRect, selected ? _lawButtonSelectedPaint : _lawButtonFillPaint);
        canvas.DrawRoundRect(roundRect, _lawButtonBorderPaint);
        _lawHitTargets.Add(new LawButtonHit(editor, law, rect));

        switch (law)
        {
            case BoundaryContinuationLaw.ReflectiveBounce:
                DrawArrowIcon(canvas, rect, stacked: true, reverseBottom: true);
                break;
            case BoundaryContinuationLaw.PeriodicWrap:
                DrawArrowIcon(canvas, rect, stacked: true, reverseBottom: false);
                break;
            case BoundaryContinuationLaw.TensionPreserving:
                DrawContinuousIcon(canvas, rect);
                break;
        }
    }

    private void DrawArrowIcon(SKCanvas canvas, SKRect rect, bool stacked, bool reverseBottom)
    {
        float left = rect.Left + 6f;
        float right = rect.Right - 6f;
        if (stacked)
        {
            float topY = rect.Top + 10f;
            float bottomY = rect.Bottom - 10f;
            DrawArrowLine(canvas, left, right, topY, forward: true);
            DrawArrowLine(canvas, left, right, bottomY, forward: !reverseBottom);
            return;
        }

        float y = rect.MidY;
        DrawArrowLine(canvas, left, right, y, forward: true);
    }

    private void DrawContinuousIcon(SKCanvas canvas, SKRect rect)
    {
        float left = rect.Left + 5f;
        float right = rect.Right - 5f;
        float middle = (left + right) * 0.5f;
        float y = rect.MidY;

        canvas.DrawLine(left, y, right, y, _lawIconPaint);
        DrawArrowHead(canvas, middle, y, forward: true, scale: 4f);
        DrawArrowHead(canvas, right, y, forward: true, scale: 4f);
    }

    private void DrawArrowLine(SKCanvas canvas, float left, float right, float y, bool forward)
    {
        if (forward)
        {
            canvas.DrawLine(left, y, right, y, _lawIconPaint);
            DrawArrowHead(canvas, right, y, forward: true);
        }
        else
        {
            canvas.DrawLine(right, y, left, y, _lawIconPaint);
            DrawArrowHead(canvas, left, y, forward: false);
        }
    }

    private void DrawArrowHead(SKCanvas canvas, float x, float y, bool forward, float scale = 3.5f)
    {
        float signed = forward ? 1f : -1f;
        canvas.DrawLine(x, y, x - 2.2f * scale * signed, y - scale, _lawIconPaint);
        canvas.DrawLine(x, y, x - 2.2f * scale * signed, y + scale, _lawIconPaint);
    }

    private void LayoutControls(EditorLayout layout)
    {
        if (_canvasHost is null || _selectorPanel is null || _palettePanel is null || _gridHostPanel is null || _trashButton is null)
        {
            return;
        }

        _selectorPanel.Bounds = ToControlRect(layout.SelectorRect);
        int buttonHeight = 36;
        int buttonGap = 8;
        for (int index = 0; index < _patternButtons.Count; index++)
        {
            _patternButtons[index].Bounds = new Rectangle(0, index * (buttonHeight + buttonGap), _selectorPanel.Width, buttonHeight);
        }

        _palettePanel.Bounds = ToControlRect(layout.PaletteRect);
        int tokenWidth = 58;
        int tokenGap = 8;
        for (int index = 0; index < _paletteButtons.Count; index++)
        {
            _paletteButtons[index].Bounds = new Rectangle(index * (tokenWidth + tokenGap), 0, tokenWidth, _palettePanel.Height);
        }

        _gridHostPanel.Bounds = ToControlRect(layout.GridRect);
        foreach (var column in _gridColumns)
        {
            column.Height = Math.Max(180, _gridHostPanel.ClientSize.Height - 28);
            if (column.Controls.Count > 1 && column.Controls[1] is FlowLayoutPanel flow)
            {
                flow.Height = column.Height - 44;
            }
        }

        _trashButton.Bounds = ToControlRect(layout.TrashRect);
        _selectorPanel.BringToFront();
        _palettePanel.BringToFront();
        _gridHostPanel.BringToFront();
        _trashButton.BringToFront();
    }

    private EditorLayout ComputeLayout()
    {
        float cardLeft = 20f;
        float cardTop = 100f;
        float cardRight = 880f;
        float cardBottom = 780f;
        var card = new SKRoundRect(new SKRect(cardLeft, cardTop, cardRight, cardBottom), 24f, 24f);

        var selectorRect = new SKRect(cardLeft + 18f, cardTop + 18f, cardLeft + 160f, cardTop + 414f);
        var previewRect = new SKRect(cardLeft + 174f, cardTop + 18f, cardRight - 18f, cardTop + 170f);
        var paletteRect = new SKRect(cardLeft + 188f, previewRect.Bottom + 18f, cardLeft + 396f, previewRect.Bottom + 56f);
        var trashRect = new SKRect(cardRight - 76f, previewRect.Bottom + 12f, cardRight - 20f, previewRect.Bottom + 50f);
        var gridRect = new SKRect(cardLeft + 174f, previewRect.Bottom + 64f, cardRight - 18f, cardTop + 430f);
        var editorRect = new SKRect(cardLeft + 18f, cardTop + 452f, cardRight - 18f, cardBottom - 18f);
        return new EditorLayout(card, selectorRect, previewRect, paletteRect, gridRect, trashRect, editorRect);
    }

    private Rectangle ToControlRect(SKRect logicalRect)
    {
        if (_canvasHost is null)
        {
            return Rectangle.Empty;
        }

        float scaleX = _canvasHost.ClientSize.Width / _canvasHost.Coords.Width;
        float scaleY = _canvasHost.ClientSize.Height / _canvasHost.Coords.Height;
        return new Rectangle(
            (int)Math.Round(logicalRect.Left * scaleX),
            (int)Math.Round(logicalRect.Top * scaleY),
            Math.Max(1, (int)Math.Round(logicalRect.Width * scaleX)),
            Math.Max(1, (int)Math.Round(logicalRect.Height * scaleY)));
    }

    private static List<List<string>> ExtractColumns(EquationProgram? program)
    {
        var columns = new List<List<string>>();
        if (program is null)
        {
            return columns;
        }

        var current = new List<string>();
        foreach (var command in program.Loop)
        {
            switch (command.Kind)
            {
                case CommandKind.Fire when command.EquationName is not null:
                    current.Add(command.EquationName);
                    break;
                case CommandKind.Commit:
                    columns.Add(current);
                    current = [];
                    break;
            }
        }

        if (current.Count > 0)
        {
            columns.Add(current);
        }

        return columns;
    }

    private Button CreateUiButton(string text, bool bold, float? fontSize = null)
    {
        var button = new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(58, 58, 58),
            Font = new Font(VisualStyle.UiFontFamily, fontSize ?? (bold ? 10.5f : 9.25f), bold ? FontStyle.Bold : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleCenter,
            TabStop = false,
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(236, 244, 246);
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 249, 250);
        return button;
    }

    private Button CreateTokenButton(string name, SegmentColorSet colors, bool isPalette, int? sourceColumnIndex, int sourceTokenIndex)
    {
        var button = CreateUiButton(name, bold: true);
        button.BackColor = Color.FromArgb(250, 250, 250);
        button.ForeColor = Color.FromArgb(colors.Label.Red, colors.Label.Green, colors.Label.Blue);
        button.FlatAppearance.BorderColor = Color.FromArgb(198, 198, 198);
        button.Font = new Font(VisualStyle.UiFontFamily, isPalette ? 8.5f : 7.5f, FontStyle.Bold);
        button.Width = isPalette ? 52 : 38;
        button.Height = 28;
        if (!isPalette)
        {
            button.Margin = new Padding(0, 0, 0, 6);
        }

        button.MouseDown += (_, eventArgs) =>
        {
            if (eventArgs.Button != MouseButtons.Left)
            {
                return;
            }

            button.DoDragDrop(new EquationTokenDragData(name, sourceColumnIndex, sourceTokenIndex), isPalette ? DragDropEffects.Copy : DragDropEffects.Move);
        };

        return button;
    }

    private EquationTokenDragData? ReadDragData(IDataObject? dataObject) =>
        dataObject?.GetData(typeof(EquationTokenDragData)) as EquationTokenDragData;

    private Bitmap CreateTrashIcon()
    {
        var bitmap = new Bitmap(40, 40);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        graphics.ScaleTransform(2.2f,2.2f);
        using var pen = new Pen(Color.FromArgb(140, 90, 90), 1.5f);
        graphics.DrawRectangle(pen, 4, 5, 10, 10);
        graphics.DrawLine(pen, 3, 5, 15, 5);
        graphics.DrawLine(pen, 6, 3, 12, 3);
        graphics.DrawLine(pen, 7, 7, 7, 13);
        graphics.DrawLine(pen, 9, 7, 9, 13);
        graphics.DrawLine(pen, 11, 7, 11, 13);
        return bitmap;
    }

    private void RemoveControl(Control? control)
    {
        if (control is null || _canvasHost is null)
        {
            return;
        }

        _canvasHost.Controls.Remove(control);
        control.Dispose();
    }

    private SegmentColorSet FindColors(string equationName) =>
        _editorSegments.FirstOrDefault(segment => string.Equals(segment.Name, equationName, StringComparison.OrdinalIgnoreCase))?.Colors
        ?? SegmentColors.Green;

    private sealed class EditorSegment
    {
        public EditorSegment(PlanarSegmentDefinition definition, string role, SegmentColorSet colors)
        {
            Definition = definition;
            Role = role;
            Colors = colors;
            SpanDisplay = new AxisDisplayMapper(definition.Segment, string.Empty, 1f);
            StepDisplay = new StepDisplayMapper(definition.Step, 1f);
            StepCoords = new CoordinateSystem(scale: StepScale);
            SpanCoords = new CoordinateSystem(scale: SpanScale);
            SetDefinition(definition);
        }

        public PlanarSegmentDefinition Definition { get; private set; }
        public string Role { get; }
        public SegmentColorSet Colors { get; }
        public AxisDisplayMapper SpanDisplay { get; }
        public StepDisplayMapper StepDisplay { get; }
        public CoordinateSystem StepCoords { get; }
        public CoordinateSystem SpanCoords { get; }
        public SegmentRenderer? StepRenderer { get; set; }
        public SegmentRenderer? SpanRenderer { get; set; }
        public BoundaryContinuationLaw Law { get; private set; }
        public string Name => Definition.Name;

        public void SetDefinition(PlanarSegmentDefinition definition)
        {
            Definition = definition;
            Law = definition.Law;
            SpanDisplay.SetAxis(definition.Segment);
            StepDisplay.SetStep(definition.Step);
        }

        public void SetLaw(BoundaryContinuationLaw law)
        {
            Law = law;
        }

        public PlanarSegmentDefinition ToDefinition() =>
            new(
                Definition.Name,
                SpanDisplay.Axis,
                Law,
                Definition.AxisVector,
                StepDisplay.ToScalar(),
                Definition.UseSegmentAsFrame,
                Definition.Seed);

        public void DisposeRenderer()
        {
            StepRenderer?.Dispose();
            StepRenderer = null;
            SpanRenderer?.Dispose();
            SpanRenderer = null;
        }
    }

    private sealed record EquationTokenDragData(string EquationName, int? SourceColumnIndex, int SourceTokenIndex);

    private sealed class StepDisplayMapper : ISegmentValue, ISegmentDragConfig
    {
        public StepDisplayMapper(Scalar step, float snapIncrement)
        {
            Label = string.Empty;
            SnapIncrement = snapIncrement;
            Real = (float)step.Value;
        }

        public float Imaginary
        {
            get => 0f;
            set { }
        }

        public float Real { get; set; }

        public string Label { get; set; }

        public float SnapIncrement { get; }

        public void SetStep(Scalar step) => Real = (float)step.Value;

        public Scalar ToScalar() => new((decimal)Real);
    }

    private sealed record LawButtonHit(EditorSegment Editor, BoundaryContinuationLaw Law, SKRect Rect);

    private sealed record EditorLayout(
        SKRoundRect Card,
        SKRect SelectorRect,
        SKRect PreviewRect,
        SKRect PaletteRect,
        SKRect GridRect,
        SKRect TrashRect,
        SKRect EditorRect);
}
