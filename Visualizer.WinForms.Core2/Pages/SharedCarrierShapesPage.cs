using Core2.Elements;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace ResoEngine.Visualizer.Pages;

public sealed class SharedCarrierShapesPage : IVisualizerPage
{
    private readonly IReadOnlyDictionary<PresetKind, ShapePreset> _presets = BuildPresets();
    private readonly List<ButtonLayout> _buttonLayouts = [];
    private readonly List<HandleLayout> _handleLayouts = [];
    private readonly List<ToggleLayout> _toggleLayouts = [];
    private readonly List<CopyLayout> _copyLayouts = [];
    private readonly List<ValueLayout> _valueLayouts = [];
    private readonly List<ActionLayout> _actionLayouts = [];
    private readonly List<PreviewCarrierLayout> _previewCarriers = [];
    private readonly Dictionary<(PresetKind Preset, string SiteName), Axis> _siteAxisOverrides = [];
    private readonly Dictionary<PresetKind, List<CustomPreviewPin>> _customPins = [];
    private readonly Dictionary<PresetKind, string> _selectedSiteByPreset = new()
    {
        [PresetKind.CapitalD] = "Top",
        [PresetKind.BridgeH] = "Left Join",
        [PresetKind.LetterT] = "Base",
        [PresetKind.LetterA] = "Left Bar",
        [PresetKind.LetterY] = "Junction",
        [PresetKind.LetterL] = "Corner",
        [PresetKind.LetterM] = "Left Peak",
    };

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private TextBox? _inlineValueEditor;
    private (PresetKind Preset, string SiteName)? _inlineValueTarget;
    private bool _inlineValueSyncing;
    private HandleLayout? _activeHandleLayout;
    private SKRect _letterboxToggleRect;
    private PresetKind _selectedPreset = PresetKind.CapitalD;
    private DragHandleKind _dragHandle;
    private bool _showLetterbox = true;
    private bool _addPinArmed;
    private SKPoint _dStemStart = new(0.22f, 0.12f);
    private SKPoint _dStemEnd = new(0.22f, 0.88f);
    private float _dTopT;
    private float _dBottomT = 1f;
    private float _dTopRecessiveLength = 68f;
    private float _dTopDominantLength = 52f;
    private float _dBottomRecessiveLength = 68f;
    private float _dBottomDominantLength = 52f;
    private SKPoint _hLeftStemStart = new(0.24f, 0.14f);
    private SKPoint _hLeftStemEnd = new(0.24f, 0.86f);
    private float _hLeftT = 0.5f;
    private SKPoint _hRightStemStart = new(0.76f, 0.14f);
    private SKPoint _hRightStemEnd = new(0.76f, 0.86f);
    private float _hRightT = 0.5f;
    private float _hLeftRecessiveLength = 52f;
    private float _hLeftDominantLength = 68f;
    private float _hRightRecessiveLength = 52f;
    private float _hRightDominantLength = 68f;
    private float _tBaseDominantLength = 84f;
    private float _tCrossbarRecessiveLength = 68f;
    private float _tCrossbarDominantLength = 68f;
    private float _yJunctionRecessiveLength = 52f;
    private float _yJunctionDominantLength = 68f;
    private float _lCornerRecessiveLength = 68f;
    private float _lCornerDominantLength = 68f;
    private float _aLeftRecessiveLength = 52f;
    private float _aLeftDominantLength = 52f;
    private float _aLeftT = 0.45f;
    private float _aRightRecessiveLength = 52f;
    private float _aRightDominantLength = 52f;
    private float _aRightT = 0.45f;
    private float _mLeftRecessiveLength = 52f;
    private float _mLeftDominantLength = 68f;
    private float _mRightRecessiveLength = 52f;
    private float _mRightDominantLength = 68f;

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
    private readonly SKPaint _panelFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(250, 250, 250),
        IsAntialias = true,
    };
    private readonly SKPaint _panelStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(224, 224, 224),
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
    private readonly SKPaint _monoPaint = new()
    {
        Color = new SKColor(68, 68, 68),
        TextSize = 14f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Normal),
        IsAntialias = true,
    };
    private readonly SKPaint _valuePaint = new()
    {
        Color = new SKColor(48, 48, 48),
        TextSize = 16f,
        Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(255, 255, 255),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonSelectedFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(239, 246, 255),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(214, 214, 214),
        IsAntialias = true,
    };
    private readonly SKPaint _buttonSelectedStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(110, 158, 220),
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
    private readonly SKPaint _sceneGuidePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.2f,
        Color = new SKColor(188, 188, 188, 120),
        IsAntialias = true,
        PathEffect = SKPathEffect.CreateDash([6f, 8f], 0f),
    };
    private readonly SKPaint _sceneCaptionPaint = new()
    {
        Color = new SKColor(100, 100, 100),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _pinFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
        IsAntialias = true,
    };
    private readonly SKPaint _pinStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2.2f,
        Color = new SKColor(74, 74, 74),
        IsAntialias = true,
    };
    private readonly SKPaint _pinLabelPaint = new()
    {
        Color = new SKColor(60, 60, 60),
        TextSize = 12f,
        Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
        IsAntialias = true,
    };
    private readonly SKPaint _handleHaloFillPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = new SKColor(110, 158, 220, 36),
        IsAntialias = true,
    };
    private readonly SKPaint _handleHaloStrokePaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1.4f,
        Color = new SKColor(110, 158, 220, 124),
        IsAntialias = true,
    };

    private static readonly SKColor RecessiveRayColor = SegmentColors.Blue.Solid;
    private static readonly SKColor DominantRayColor = SegmentColors.Orange.Solid;
    private const byte CarrierPreviewAlpha = 128;
    private const float EndpointHandleOffset = 18f;
    private const float MaxPreviewRayLength = 4000f;
    private const float ZeroRayHandleOffset = 18f;
    private const float CarrierHitThreshold = 18f;
    private static readonly Axis PreviewHostAxis = Axis.FromCoordinates(Proportion.Zero, new Proportion(100));

    public string Title => "Shared Carrier Shapes";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
        EnsureInlineValueEditor();
    }

    public void Render(SKCanvas canvas)
    {
        _buttonLayouts.Clear();
        _handleLayouts.Clear();
        _toggleLayouts.Clear();
        _copyLayouts.Clear();
        _valueLayouts.Clear();
        _actionLayouts.Clear();
        _previewCarriers.Clear();

        float width = _coords?.Width ?? 1220f;
        float height = _coords?.Height ?? 920f;
        ShapePreset preset = _presets[_selectedPreset];

        canvas.DrawText("Shared Carrier Shapes", 32f, 44f, _headingPaint);
        float textY = 70f;
        PageChrome.DrawWrappedText(
            canvas,
            "Each preset builds a real carrier graph first. The scene on the right is one planar fold preview of that graph, while the panel on the left reports the shared-carrier and recursive facts directly from Core 2.",
            32f,
            ref textY,
            width - 80f,
            _bodyPaint);

        var leftPanel = new SKRect(24f, 106f, 392f, height - 24f);
        var scenePanel = new SKRect(424f, 106f, width - 20f, height - 24f);

        DrawLeftPanel(canvas, leftPanel, preset);
        DrawScenePanel(canvas, scenePanel, preset);
    }

    public bool OnPointerDown(SKPoint pixelPoint)
    {
        var button = _buttonLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint));
        if (button is not null)
        {
            _selectedPreset = button.Preset;
            _addPinArmed = false;
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var action = _actionLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint));
        if (action is not null)
        {
            switch (action.Action)
            {
                case SceneAction.AddPin:
                    _addPinArmed = !_addPinArmed;
                    break;
                case SceneAction.Trash:
                    RemoveSelectedCustomPin();
                    break;
                case SceneAction.CopyStructure:
                    CopyTextToClipboard(SerializeCurrentStructure());
                    break;
            }

            _canvasHost?.InvalidateCanvas();
            return true;
        }

        if (_letterboxToggleRect.Contains(pixelPoint))
        {
            ToggleLetterboxMode();
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var copy = _copyLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint));
        if (copy is not null)
        {
            CopyTextToClipboard(copy.Text);

            return true;
        }

        var toggle = _toggleLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint));
        if (toggle is not null)
        {
            ToggleSelectedComponent(toggle.Component);
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        if (_addPinArmed && TryAddCustomPin(pixelPoint))
        {
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var handle = HitHandle(pixelPoint);
        if (handle is null)
        {
            return false;
        }

        string? selectedSite = handle.SiteName ?? ResolveSelectedSite(handle.Target);
        if (selectedSite is not null)
        {
            _selectedSiteByPreset[_selectedPreset] = selectedSite;
        }

        _dragHandle = handle.Target;
        _activeHandleLayout = handle;
        _canvasHost?.InvalidateCanvas();
        return true;
    }

    public void OnPointerMove(SKPoint pixelPoint)
    {
        if (_canvasHost is null)
        {
            return;
        }

        if (_dragHandle != DragHandleKind.None)
        {
            UpdateDragHandle(pixelPoint);
            _canvasHost.Cursor = Cursors.SizeAll;
            _canvasHost.InvalidateCanvas();
            return;
        }

        _canvasHost.Cursor = _buttonLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
            ? Cursors.Hand
            : _actionLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
                ? Cursors.Hand
            : _letterboxToggleRect.Contains(pixelPoint)
                ? Cursors.Hand
            : _copyLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
                ? Cursors.Hand
            : _valueLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
                ? Cursors.IBeam
            : _toggleLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
                ? Cursors.Hand
            : _addPinArmed && TryFindNearestPreviewCarrier(pixelPoint, out _, out _, out _, out _, out float carrierDistance) && carrierDistance <= CarrierHitThreshold
                ? Cursors.Cross
            : HitHandle(pixelPoint) is not null
                ? Cursors.SizeAll
                : Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        _dragHandle = DragHandleKind.None;
        _activeHandleLayout = null;
        SyncInlineValueText();
        if (_canvasHost is not null)
        {
            _canvasHost.Cursor = Cursors.Default;
        }
    }

    public void Destroy()
    {
        RemoveInlineValueEditor();
        _buttonLayouts.Clear();
        _handleLayouts.Clear();
        _toggleLayouts.Clear();
        _copyLayouts.Clear();
        _valueLayouts.Clear();
        _actionLayouts.Clear();
        _previewCarriers.Clear();
        _coords = null;
        _canvasHost = null;
        _dragHandle = DragHandleKind.None;
        _activeHandleLayout = null;
    }

    public void Dispose()
    {
        Destroy();
        _headingPaint.Dispose();
        _bodyPaint.Dispose();
        _cardFillPaint.Dispose();
        _cardStrokePaint.Dispose();
        _panelFillPaint.Dispose();
        _panelStrokePaint.Dispose();
        _labelPaint.Dispose();
        _captionPaint.Dispose();
        _monoPaint.Dispose();
        _valuePaint.Dispose();
        _buttonFillPaint.Dispose();
        _buttonSelectedFillPaint.Dispose();
        _buttonStrokePaint.Dispose();
        _buttonSelectedStrokePaint.Dispose();
        _buttonTextPaint.Dispose();
        _badgeFillPaint.Dispose();
        _badgeStrokePaint.Dispose();
        _badgeTextPaint.Dispose();
        _sceneGuidePaint.Dispose();
        _sceneCaptionPaint.Dispose();
        _pinFillPaint.Dispose();
        _pinStrokePaint.Dispose();
        _pinLabelPaint.Dispose();
        _handleHaloFillPaint.Dispose();
        _handleHaloStrokePaint.Dispose();
    }

    private void DrawLeftPanel(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        canvas.DrawRoundRect(rect, 22f, 22f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 22f, 22f, _cardStrokePaint);

        canvas.DrawText("Preset + Analysis", rect.Left + 20f, rect.Top + 28f, _labelPaint);
        float y = rect.Top + 54f;

        DrawPresetButtons(canvas, rect.Left + 20f, ref y);

        y += 18f;
        DrawBadge(canvas, new SKPoint(rect.Left + 20f, y), $"Carriers {preset.Graph.Carriers.Count}");
        DrawBadge(canvas, new SKPoint(rect.Left + 140f, y), $"Sites {preset.Graph.Sites.Count}");
        y += 48f;

        CarrierPinSite selectedSite = GetSelectedSite(preset);
        CarrierSiteStructuralProfile selectedProfile = ResolveSiteProfile(preset, selectedSite);
        string selectedTitle = ResolveSelectedCarrierTitle(selectedSite);
        SKColor selectedAccent = ResolveSelectedCarrierColor(preset, selectedSite);
        DrawBadge(canvas, new SKPoint(rect.Left + 20f, y), selectedTitle, selectedAccent);
        DrawBadge(canvas, new SKPoint(rect.Left + 144f, y), FormatJunctionSummary(selectedProfile.Summary));
        y += 40f;

        string host = selectedSite.HostCarrier.Name ?? selectedSite.HostCarrier.Id.ToString();
        string line = $"{selectedSite.Name ?? selectedSite.Id.ToString()} on {host}@{FormatProportion(selectedSite.HostPosition)}";
        canvas.DrawText(line, rect.Left + 20f, y, _monoPaint);
        y += _monoPaint.TextSize + 4f;

        string routingLine =
            $"through host {(selectedProfile.HostContinues ? "yes" : "no")}, " +
            $"cross proposal {(selectedProfile.HasCrossShapedProposal ? "yes" : "no")}";
        PageChrome.DrawWrappedText(
            canvas,
            routingLine,
            rect.Left + 28f,
            ref y,
            rect.Width - 56f,
            _captionPaint);

        if (selectedProfile.ThroughCarriers.Count > 0)
        {
            string throughCarriers = string.Join(", ", selectedProfile.ThroughCarriers.Select(carrier => carrier.Name ?? carrier.Id.ToString()));
            PageChrome.DrawWrappedText(
                canvas,
                $"through: {throughCarriers}",
                rect.Left + 28f,
                ref y,
                rect.Width - 56f,
                _captionPaint);
        }

        if (selectedProfile.ParticipatingCarriers.Count > 0)
        {
            string participants = string.Join(", ", selectedProfile.ParticipatingCarriers.Select(carrier => carrier.Name ?? carrier.Id.ToString()));
            PageChrome.DrawWrappedText(
                canvas,
                $"carriers: {participants}",
                rect.Left + 28f,
                ref y,
                rect.Width - 56f,
                _captionPaint);
        }

        string bindings = selectedSite.SideAttachments.Count == 0
            ? "unbound side attachments"
            : string.Join(
                ", ",
                selectedSite.SideAttachments.Select(
                    attachment => $"{ShortRole(attachment.Role)}:{attachment.Carrier.Name ?? attachment.Carrier.Id.ToString()}@{FormatProportion(attachment.CarrierPosition)}"));
        PageChrome.DrawWrappedText(
            canvas,
            bindings,
            rect.Left + 28f,
            ref y,
            rect.Width - 82f,
            _captionPaint);

        string axisText = FormatAxis(selectedSite.Applied);
        float valueY = y + _valuePaint.TextSize;
        float editorLeft = rect.Left + 20f;
        float copyWidth = 30f;
        float gap = 10f;
        float editorRight = rect.Right - 20f - copyWidth - gap;
        var valueRect = new SKRect(editorLeft, valueY - 20f, editorRight, valueY + 10f);
        using var editorFramePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.1f,
            Color = new SKColor(214, 214, 214),
            IsAntialias = true,
        };
        canvas.DrawRoundRect(valueRect, 8f, 8f, editorFramePaint);
        _valueLayouts.Add(new ValueLayout(valueRect, _selectedPreset, selectedSite.Name ?? string.Empty, axisText));
        UpdateInlineValueEditor(valueRect, _selectedPreset, selectedSite.Name ?? string.Empty, axisText);

        var copyRect = new SKRect(editorRight + gap, valueY - 16f, editorRight + gap + copyWidth, valueY + 8f);
        DrawCopyButton(canvas, copyRect);
        _copyLayouts.Add(new CopyLayout(copyRect, axisText));
        y = valueY + 26f;

        y += 8f;
        canvas.DrawText("Carriers", rect.Left + 20f, y + 12f, _labelPaint);
        y += 38f;
        foreach (var carrier in preset.Graph.Carriers)
        {
            CarrierStructuralProfile profile = preset.Analysis.GetProfile(carrier.Id);
            string colorName = $"{carrier.Name ?? "Carrier"}";
            DrawBadge(canvas, new SKPoint(rect.Left + 20f, y), colorName, preset.CarrierColors[carrier.Id]);
            y += 40f;

            string summary =
                $"hosted {profile.HostedSiteCount}, attached {profile.AttachmentCount}, " +
                $"shared {(profile.IsSharedAcrossSites ? "yes" : "no")}, " +
                $"recursive {(profile.ParticipatesInRecursiveCycle ? "yes" : "no")}";
            PageChrome.DrawWrappedText(
                canvas,
                summary,
                rect.Left + 28f,
                ref y,
                    rect.Width - 56f,
                    _captionPaint);

            string junctionSummary =
                $"through {profile.ThroughSiteCount}, cusp {profile.CuspSiteCount}, branch {profile.BranchSiteCount}, " +
                $"tee {profile.TeeSiteCount}, cross {profile.CrossSiteCount}";
            PageChrome.DrawWrappedText(
                canvas,
                junctionSummary,
                rect.Left + 28f,
                ref y,
                rect.Width - 56f,
                _captionPaint);

            if (profile.HasAttachmentSpan)
            {
                string span = $"attachment span {FormatProportion(profile.FirstAttachmentPosition)} -> {FormatProportion(profile.LastAttachmentPosition)}";
                canvas.DrawText(span, rect.Left + 28f, y, _monoPaint);
                y += _monoPaint.TextSize + 12f;
            }

            if (profile.ReferencedCarrierCount > 0)
            {
                string references = string.Join(", ", profile.ReferencedCarriers.Select(reference => reference.Name ?? reference.Id.ToString()));
                PageChrome.DrawWrappedText(
                    canvas,
                    $"references: {references}",
                    rect.Left + 28f,
                    ref y,
                    rect.Width - 56f,
                    _captionPaint);
            }

            y += 14f;
        }
    }

    private void DrawPresetButtons(SKCanvas canvas, float left, ref float y)
    {
        float buttonWidth = 42f;
        float buttonHeight = 34f;
        float gap = 8f;
        float rowStart = left;
        float maxRight = rowStart + 320f;

        foreach (var preset in Enum.GetValues<PresetKind>())
        {
            if (left + buttonWidth > maxRight)
            {
                left = rowStart;
                y += buttonHeight + 8f;
            }

            var rect = new SKRect(left, y, left + buttonWidth, y + buttonHeight);
            bool selected = preset == _selectedPreset;
            canvas.DrawRoundRect(rect, 16f, 16f, selected ? _buttonSelectedFillPaint : _buttonFillPaint);
            canvas.DrawRoundRect(rect, 16f, 16f, selected ? _buttonSelectedStrokePaint : _buttonStrokePaint);
            canvas.DrawText(GetPresetButtonText(preset), rect.MidX, rect.MidY + 4f, _buttonTextPaint);
            _buttonLayouts.Add(new ButtonLayout(rect, preset));
            left += buttonWidth + gap;
        }

        y += buttonHeight;
    }

    private void DrawScenePanel(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        canvas.DrawRoundRect(rect, 22f, 22f, _cardFillPaint);
        canvas.DrawRoundRect(rect, 22f, 22f, _cardStrokePaint);

        canvas.DrawText("Planar Fold Preview", rect.Left + 20f, rect.Top + 28f, _labelPaint);
        canvas.DrawText("One display interpretation of the shared carrier graph", rect.Left + 20f, rect.Top + 50f, _captionPaint);

        var inner = new SKRect(rect.Left + 18f, rect.Top + 72f, rect.Right - 18f, rect.Bottom - 18f);
        canvas.DrawRoundRect(inner, 20f, 20f, _panelFillPaint);
        canvas.DrawRoundRect(inner, 20f, 20f, _panelStrokePaint);
        canvas.DrawLine(inner.MidX, inner.Top + 16f, inner.MidX, inner.Bottom - 16f, _sceneGuidePaint);
        canvas.DrawText("Preview only", inner.Right - 98f, inner.Top + 24f, _sceneCaptionPaint);
        DrawLetterboxToggle(canvas, inner);
        if (_showLetterbox)
        {
            DrawLetterboxGuides(canvas, inner);
        }
        DrawToggleLegend(canvas, inner, preset);

        switch (_selectedPreset)
        {
            case PresetKind.CapitalD:
                DrawCapitalDPreview(canvas, inner, preset);
                break;
            case PresetKind.BridgeH:
                DrawBridgeHPreview(canvas, inner, preset);
                break;
            case PresetKind.LetterT:
                DrawTPreview(canvas, inner, preset);
                break;
            case PresetKind.LetterA:
                DrawAPreview(canvas, inner, preset);
                break;
            case PresetKind.LetterY:
                DrawYPreview(canvas, inner, preset);
                break;
            case PresetKind.LetterL:
                DrawLPreview(canvas, inner, preset);
                break;
            case PresetKind.LetterM:
                DrawMPreview(canvas, inner, preset);
                break;
        }

        DrawCustomPins(canvas, preset);
        DrawSceneActions(canvas, inner);
    }

    private void DrawLetterboxToggle(SKCanvas canvas, SKRect sceneRect)
    {
        string text = _showLetterbox ? "Guides On" : "Guides Off";
        float width = Math.Max(92f, _buttonTextPaint.MeasureText(text) + 28f);
        _letterboxToggleRect = new SKRect(sceneRect.Right - width - 18f, sceneRect.Top + 14f, sceneRect.Right - 18f, sceneRect.Top + 44f);
        canvas.DrawRoundRect(_letterboxToggleRect, 14f, 14f, _buttonFillPaint);
        canvas.DrawRoundRect(_letterboxToggleRect, 14f, 14f, _showLetterbox ? _buttonSelectedStrokePaint : _buttonStrokePaint);
        canvas.DrawText(text, _letterboxToggleRect.MidX, _letterboxToggleRect.MidY + 4f, _buttonTextPaint);
    }

    private void DrawLetterboxGuides(SKCanvas canvas, SKRect sceneRect)
    {
        var guide = GetLetterboxRect(sceneRect);
        canvas.DrawRoundRect(guide, 4f, 4f, _sceneGuidePaint);
        canvas.DrawLine(guide.MidX, guide.Top, guide.MidX, guide.Bottom, _sceneGuidePaint);
        float crossbarY = guide.Top + (guide.Height * 0.42f);
        canvas.DrawLine(guide.Left, crossbarY, guide.Right, crossbarY, _sceneGuidePaint);
    }

    private void DrawSceneActions(SKCanvas canvas, SKRect sceneRect)
    {
        float buttonWidth = 108f;
        float buttonHeight = 36f;
        float gap = 10f;
        float top = sceneRect.Bottom - buttonHeight - 12f;
        float left = sceneRect.Left + 16f;

        DrawSceneActionButton(canvas, new SKRect(left, top, left + buttonWidth, top + buttonHeight), SceneAction.AddPin, "Pin", _addPinArmed);
        left += buttonWidth + gap;
        DrawSceneActionButton(canvas, new SKRect(left, top, left + buttonWidth, top + buttonHeight), SceneAction.Trash, "Trash", false, HasSelectedCustomPin());
        left += buttonWidth + gap;
        DrawSceneActionButton(canvas, new SKRect(left, top, left + buttonWidth, top + buttonHeight), SceneAction.CopyStructure, "Copy", false);
    }

    private void DrawSceneActionButton(SKCanvas canvas, SKRect rect, SceneAction action, string text, bool selected, bool enabled = true)
    {
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = enabled
                ? (selected ? new SKColor(239, 246, 255) : new SKColor(255, 255, 255, 242))
                : new SKColor(248, 248, 248, 180),
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = selected ? 1.4f : 1.2f,
            Color = enabled
                ? (selected ? new SKColor(110, 158, 220) : new SKColor(214, 214, 214))
                : new SKColor(224, 224, 224, 180),
            IsAntialias = true,
        };
        using var textPaint = new SKPaint
        {
            Color = enabled ? new SKColor(68, 68, 68) : new SKColor(156, 156, 156),
            TextSize = 12f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 16f, 16f, fill);
        canvas.DrawRoundRect(rect, 16f, 16f, stroke);
        DrawSceneActionIcon(canvas, action, new SKPoint(rect.Left + 20f, rect.MidY), enabled);
        canvas.DrawText(text, rect.MidX + 10f, rect.MidY + 4f, textPaint);
        _actionLayouts.Add(new ActionLayout(rect, action));
    }

    private void DrawSceneActionIcon(SKCanvas canvas, SceneAction action, SKPoint center, bool enabled)
    {
        SKColor color = enabled ? new SKColor(112, 112, 112) : new SKColor(166, 166, 166);
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.8f,
            Color = color,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };

        switch (action)
        {
            case SceneAction.AddPin:
                canvas.DrawCircle(center, 9f, stroke);
                canvas.DrawLine(center.X - 4f, center.Y, center.X + 4f, center.Y, stroke);
                canvas.DrawLine(center.X, center.Y - 4f, center.X, center.Y + 4f, stroke);
                break;
            case SceneAction.Trash:
                canvas.DrawRect(new SKRect(center.X - 6f, center.Y - 5f, center.X + 6f, center.Y + 6f), stroke);
                canvas.DrawLine(center.X - 8f, center.Y - 7f, center.X + 8f, center.Y - 7f, stroke);
                canvas.DrawLine(center.X - 3f, center.Y - 10f, center.X + 3f, center.Y - 10f, stroke);
                canvas.DrawLine(center.X - 2f, center.Y - 2f, center.X - 2f, center.Y + 4f, stroke);
                canvas.DrawLine(center.X + 2f, center.Y - 2f, center.X + 2f, center.Y + 4f, stroke);
                break;
            case SceneAction.CopyStructure:
                canvas.DrawRoundRect(new SKRect(center.X - 5f, center.Y - 4f, center.X + 3f, center.Y + 6f), 2f, 2f, stroke);
                canvas.DrawRoundRect(new SKRect(center.X - 1f, center.Y - 8f, center.X + 7f, center.Y + 2f), 2f, 2f, stroke);
                break;
        }
    }

    private void DrawCustomPins(SKCanvas canvas, ShapePreset preset)
    {
        foreach (var customPin in GetCustomPins(_selectedPreset))
        {
            if (!TryResolvePreviewCarrierPoint(customPin.CarrierKey, customPin.T, out SKPoint point, out SKPoint tangent))
            {
                continue;
            }

            CarrierPinSite site = BuildCustomSite(customPin);
            DrawSite(
                canvas,
                site,
                customPin.Name,
                point,
                tangent,
                DragHandleKind.CustomSite,
                DragHandleKind.CustomRecessive,
                DragHandleKind.CustomDominant,
                customPin.Name,
                customPin.CarrierKey);
            RegisterHandle(DragHandleKind.CustomSite, point, point, point, siteName: customPin.Name, carrierKey: customPin.CarrierKey);
        }
    }

    private void DrawCapitalDPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        CarrierIdentity stem = preset.Graph.Carriers.First(carrier => carrier.Name == "Stem");
        CarrierIdentity bowl = preset.Graph.Carriers.First(carrier => carrier.Name == "Bowl");
        CarrierPinSite top = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Top"));
        CarrierPinSite bottom = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Bottom"));

        SKPoint topEndpoint = ResolveStemEndpoint(rect, _dStemStart, LetterboxEdge.Left);
        SKPoint bottomEndpoint = ResolveStemEndpoint(rect, _dStemEnd, LetterboxEdge.Left);
        SKPoint stemVector = new(bottomEndpoint.X - topEndpoint.X, bottomEndpoint.Y - topEndpoint.Y);
        SKPoint hostTangent = Normalize(stemVector);
        if (hostTangent == SKPoint.Empty)
        {
            hostTangent = new SKPoint(0f, 1f);
        }

        SKPoint topPoint = new(
            Lerp(topEndpoint.X, bottomEndpoint.X, _dTopT),
            Lerp(topEndpoint.Y, bottomEndpoint.Y, _dTopT));
        SKPoint bottomPoint = new(
            Lerp(topEndpoint.X, bottomEndpoint.X, _dBottomT),
            Lerp(topEndpoint.Y, bottomEndpoint.Y, _dBottomT));
        float midX = (topPoint.X + bottomPoint.X) * 0.5f;
        float midY = (topPoint.Y + bottomPoint.Y) * 0.5f;

        DrawCarrierLine(canvas, topEndpoint, topPoint, preset.CarrierColors[stem.Id], 5.6f);
        DrawSharedCarrierCurve(
            canvas,
            topPoint,
            top,
            stem.Id,
            hostTangent,
            bottomPoint,
            bottom,
            stem.Id,
            hostTangent,
            preset.CarrierColors[stem.Id],
            5.6f);
        DrawCarrierLine(canvas, bottomPoint, bottomEndpoint, preset.CarrierColors[stem.Id], 5.6f);
        DrawCarrierLabel(canvas, stem.Name ?? "Stem", new SKPoint(midX - 72f, midY), preset.CarrierColors[stem.Id]);
        DrawSharedCarrierCurve(
            canvas,
            topPoint,
            top,
            bowl.Id,
            hostTangent,
            bottomPoint,
            bottom,
            bowl.Id,
            hostTangent,
            preset.CarrierColors[bowl.Id],
            5.6f);
        DrawCarrierLabel(canvas, bowl.Name ?? "Bowl", new SKPoint(rect.Right - 176f, midY), preset.CarrierColors[bowl.Id]);

        RegisterPreviewCarrier(
            $"{_selectedPreset}.Stem",
            stem,
            stem.Name ?? "Stem",
            CombineSamples(
                [topEndpoint, topPoint],
                BuildSharedCarrierSamples(topPoint, top, stem.Id, hostTangent, bottomPoint, bottom, stem.Id, hostTangent),
                [bottomPoint, bottomEndpoint]));
        RegisterPreviewCarrier(
            $"{_selectedPreset}.Bowl",
            bowl,
            bowl.Name ?? "Bowl",
            BuildSharedCarrierSamples(topPoint, top, bowl.Id, hostTangent, bottomPoint, bottom, bowl.Id, hostTangent));

        DrawStemEndpointHandle(canvas, topEndpoint, _dragHandle == DragHandleKind.DStemTop);
        DrawStemEndpointHandle(canvas, bottomEndpoint, _dragHandle == DragHandleKind.DStemBottom);
        DrawSite(canvas, top, "P1", topPoint, hostTangent, DragHandleKind.DTop);
        DrawSite(canvas, bottom, "P2", bottomPoint, hostTangent, DragHandleKind.DBottom);

        RegisterStemHandle(DragHandleKind.DStemTop, topEndpoint, rect, LetterboxEdge.Left);
        RegisterHandle(DragHandleKind.DTop, topPoint, topEndpoint, bottomEndpoint);
        RegisterHandle(DragHandleKind.DBottom, bottomPoint, topEndpoint, bottomEndpoint);
        RegisterStemHandle(DragHandleKind.DStemBottom, bottomEndpoint, rect, LetterboxEdge.Left);
    }

    private void DrawBridgeHPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        CarrierIdentity left = preset.Graph.Carriers.First(carrier => carrier.Name == "Left Stem");
        CarrierIdentity right = preset.Graph.Carriers.First(carrier => carrier.Name == "Right Stem");
        CarrierIdentity bridge = preset.Graph.Carriers.First(carrier => carrier.Name == "Bridge");
        CarrierPinSite leftJoin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Left Join"));
        CarrierPinSite rightJoin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Right Join"));

        SKPoint leftTopEndpoint = ResolveStemEndpoint(rect, _hLeftStemStart, LetterboxEdge.Left);
        SKPoint leftBottomEndpoint = ResolveStemEndpoint(rect, _hLeftStemEnd, LetterboxEdge.Left);
        SKPoint rightTopEndpoint = ResolveStemEndpoint(rect, _hRightStemStart, LetterboxEdge.Right);
        SKPoint rightBottomEndpoint = ResolveStemEndpoint(rect, _hRightStemEnd, LetterboxEdge.Right);
        SKPoint leftTangent = Normalize(new SKPoint(leftBottomEndpoint.X - leftTopEndpoint.X, leftBottomEndpoint.Y - leftTopEndpoint.Y));
        SKPoint rightTangent = Normalize(new SKPoint(rightBottomEndpoint.X - rightTopEndpoint.X, rightBottomEndpoint.Y - rightTopEndpoint.Y));
        if (leftTangent == SKPoint.Empty)
        {
            leftTangent = new SKPoint(0f, 1f);
        }

        if (rightTangent == SKPoint.Empty)
        {
            rightTangent = new SKPoint(0f, 1f);
        }

        SKPoint leftPoint = new(
            Lerp(leftTopEndpoint.X, leftBottomEndpoint.X, _hLeftT),
            Lerp(leftTopEndpoint.Y, leftBottomEndpoint.Y, _hLeftT));
        SKPoint rightPoint = new(
            Lerp(rightTopEndpoint.X, rightBottomEndpoint.X, _hRightT),
            Lerp(rightTopEndpoint.Y, rightBottomEndpoint.Y, _hRightT));
        float bridgeMidY = (leftPoint.Y + rightPoint.Y) * 0.5f;

        DrawCarrierLine(canvas, leftTopEndpoint, leftPoint, preset.CarrierColors[left.Id], 5.2f);
        DrawCarrierLine(canvas, leftPoint, leftBottomEndpoint, preset.CarrierColors[left.Id], 5.2f);
        DrawCarrierLine(canvas, rightTopEndpoint, rightPoint, preset.CarrierColors[right.Id], 5.2f);
        DrawCarrierLine(canvas, rightPoint, rightBottomEndpoint, preset.CarrierColors[right.Id], 5.2f);

        DrawSharedCarrierCurve(
            canvas,
            leftPoint,
            leftJoin,
            bridge.Id,
            leftTangent,
            rightPoint,
            rightJoin,
            bridge.Id,
            rightTangent,
            preset.CarrierColors[bridge.Id],
            5.2f);

        DrawCarrierLabel(canvas, left.Name ?? "Left", new SKPoint(leftPoint.X - 84f, leftPoint.Y - 6f), preset.CarrierColors[left.Id]);
        DrawCarrierLabel(canvas, bridge.Name ?? "Bridge", new SKPoint(rect.MidX - 24f, bridgeMidY - 18f), preset.CarrierColors[bridge.Id]);
        DrawCarrierLabel(canvas, right.Name ?? "Right", new SKPoint(rightPoint.X + 18f, rightPoint.Y - 6f), preset.CarrierColors[right.Id]);

        RegisterPreviewCarrier($"{_selectedPreset}.LeftStem", left, left.Name ?? "Left Stem", [leftTopEndpoint, leftPoint, leftBottomEndpoint]);
        RegisterPreviewCarrier($"{_selectedPreset}.RightStem", right, right.Name ?? "Right Stem", [rightTopEndpoint, rightPoint, rightBottomEndpoint]);
        RegisterPreviewCarrier(
            $"{_selectedPreset}.Bridge",
            bridge,
            bridge.Name ?? "Bridge",
            BuildSharedCarrierSamples(leftPoint, leftJoin, bridge.Id, leftTangent, rightPoint, rightJoin, bridge.Id, rightTangent));

        DrawStemEndpointHandle(canvas, leftTopEndpoint, _dragHandle == DragHandleKind.HLeftStemTop);
        DrawStemEndpointHandle(canvas, leftBottomEndpoint, _dragHandle == DragHandleKind.HLeftStemBottom);
        DrawStemEndpointHandle(canvas, rightTopEndpoint, _dragHandle == DragHandleKind.HRightStemTop);
        DrawStemEndpointHandle(canvas, rightBottomEndpoint, _dragHandle == DragHandleKind.HRightStemBottom);
        DrawSite(canvas, leftJoin, "P1", leftPoint, leftTangent, DragHandleKind.HLeft);
        DrawSite(canvas, rightJoin, "P2", rightPoint, rightTangent, DragHandleKind.HRight);

        RegisterStemHandle(DragHandleKind.HLeftStemTop, leftTopEndpoint, rect, LetterboxEdge.Left);
        RegisterHandle(DragHandleKind.HLeft, leftPoint, leftTopEndpoint, leftBottomEndpoint);
        RegisterStemHandle(DragHandleKind.HLeftStemBottom, leftBottomEndpoint, rect, LetterboxEdge.Left);
        RegisterStemHandle(DragHandleKind.HRightStemTop, rightTopEndpoint, rect, LetterboxEdge.Right);
        RegisterHandle(DragHandleKind.HRight, rightPoint, rightTopEndpoint, rightBottomEndpoint);
        RegisterStemHandle(DragHandleKind.HRightStemBottom, rightBottomEndpoint, rect, LetterboxEdge.Right);
    }

    private void DrawTPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        var guide = GetLetterboxRect(rect);
        CarrierIdentity stem = preset.Graph.Carriers.First(carrier => carrier.Name == "Stem");
        CarrierIdentity bar = preset.Graph.Carriers.First(carrier => carrier.Name == "Bar");
        CarrierPinSite basePin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Base"));
        CarrierPinSite topPin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Crossbar"));
        SKPoint bottomPoint = new(guide.MidX, guide.Bottom);
        SKPoint guideTangent = new(1f, 0f);
        SKPoint stemTangent = new(0f, 1f);
        SKPoint topPoint = new(guide.MidX, guide.Top);
        if (_showLetterbox)
        {
            topPoint = new(guide.MidX, guide.Top);
            _tBaseDominantLength = Distance(bottomPoint, topPoint);
            _tCrossbarRecessiveLength = Distance(topPoint, new SKPoint(guide.Left, topPoint.Y));
            _tCrossbarDominantLength = Distance(topPoint, new SKPoint(guide.Right, topPoint.Y));
        }
        else if (TryResolveSideDirection(basePin, PinSideRole.Dominant, guideTangent, out SKPoint stemDirection, out float stemMagnitude))
        {
            float stemLength = GetPreviewRayLength(ResolveRayHandle(basePin.Name, PinSideRole.Dominant), Math.Clamp(20f + stemMagnitude * 16f, 24f, 104f));
            topPoint = new(bottomPoint.X + (stemDirection.X * stemLength), bottomPoint.Y + (stemDirection.Y * stemLength));
        }

        SKPoint leftPoint = new(guide.Left, topPoint.Y);
        SKPoint rightPoint = new(guide.Right, topPoint.Y);
        if (TryResolveSideDirection(topPin, PinSideRole.Recessive, stemTangent, out SKPoint leftDirection, out float leftMagnitude))
        {
            float length = GetPreviewRayLength(ResolveRayHandle(topPin.Name, PinSideRole.Recessive), Math.Clamp(20f + leftMagnitude * 16f, 24f, 104f));
            leftPoint = new(topPoint.X + (leftDirection.X * length), topPoint.Y + (leftDirection.Y * length));
        }

        if (TryResolveSideDirection(topPin, PinSideRole.Dominant, stemTangent, out SKPoint rightDirection, out float rightMagnitude))
        {
            float length = GetPreviewRayLength(ResolveRayHandle(topPin.Name, PinSideRole.Dominant), Math.Clamp(20f + rightMagnitude * 16f, 24f, 104f));
            rightPoint = new(topPoint.X + (rightDirection.X * length), topPoint.Y + (rightDirection.Y * length));
        }

        DrawCarrierLine(canvas, leftPoint, rightPoint, WithAlpha(preset.CarrierColors[bar.Id], CarrierPreviewAlpha), 5.2f);
        DrawCarrierLabel(canvas, stem.Name ?? "Stem", new SKPoint(bottomPoint.X + 14f, (bottomPoint.Y + topPoint.Y) * 0.5f), preset.CarrierColors[stem.Id]);
        DrawCarrierLabel(canvas, bar.Name ?? "Bar", new SKPoint(guide.MidX - 10f, topPoint.Y - 12f), preset.CarrierColors[bar.Id]);
        RegisterPreviewCarrier($"{_selectedPreset}.Stem", stem, stem.Name ?? "Stem", [bottomPoint, topPoint]);
        RegisterPreviewCarrier($"{_selectedPreset}.Bar", bar, bar.Name ?? "Bar", [leftPoint, topPoint, rightPoint]);
        DrawSite(canvas, basePin, "P1", bottomPoint, guideTangent, DragHandleKind.TBase);
        DrawSite(canvas, topPin, "P2", topPoint, stemTangent, DragHandleKind.TCrossbar);
        RegisterHandle(DragHandleKind.TBase, bottomPoint, bottomPoint, bottomPoint);
        RegisterHandle(DragHandleKind.TCrossbar, topPoint, topPoint, topPoint);
    }

    private void DrawLPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        var guide = GetLetterboxRect(rect);
        CarrierIdentity stem = preset.Graph.Carriers.First(carrier => carrier.Name == "Stem");
        CarrierIdentity foot = preset.Graph.Carriers.First(carrier => carrier.Name == "Foot");
        CarrierPinSite corner = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Corner"));
        SKPoint cornerPoint = new(guide.Left, guide.Bottom);
        SKPoint topPoint = new(guide.Left, guide.Top);
        SKPoint rightPoint = new(guide.Right, guide.Bottom);
        SKPoint hostTangent = new(0f, 1f);

        DrawCarrierLine(canvas, topPoint, cornerPoint, preset.CarrierColors[stem.Id], 5.4f);
        DrawCarrierLine(canvas, cornerPoint, rightPoint, WithAlpha(preset.CarrierColors[foot.Id], CarrierPreviewAlpha), 5.2f);
        DrawCarrierLabel(canvas, stem.Name ?? "Stem", new SKPoint(topPoint.X - 68f, guide.MidY), preset.CarrierColors[stem.Id]);
        DrawCarrierLabel(canvas, foot.Name ?? "Foot", new SKPoint(guide.MidX - 16f, guide.Bottom - 10f), preset.CarrierColors[foot.Id]);
        RegisterPreviewCarrier($"{_selectedPreset}.Stem", stem, stem.Name ?? "Stem", [topPoint, cornerPoint]);
        RegisterPreviewCarrier($"{_selectedPreset}.Foot", foot, foot.Name ?? "Foot", [cornerPoint, rightPoint]);
        DrawSite(canvas, corner, "P1", cornerPoint, hostTangent, DragHandleKind.LCorner);
        RegisterHandle(DragHandleKind.LCorner, cornerPoint, cornerPoint, cornerPoint);
    }

    private void DrawYPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        var guide = GetLetterboxRect(rect);
        CarrierIdentity stem = preset.Graph.Carriers.First(carrier => carrier.Name == "Stem");
        CarrierIdentity fork = preset.Graph.Carriers.First(carrier => carrier.Name == "Fork");
        CarrierPinSite junction = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Junction"));
        SKPoint junctionPoint = new(guide.MidX, guide.Top + (guide.Height * 0.46f));
        SKPoint bottomPoint = new(guide.MidX, guide.Bottom);
        SKPoint leftArm = new(guide.Left, guide.Top);
        SKPoint rightArm = new(guide.Right, guide.Top);
        SKPoint hostTangent = new(0f, 1f);

        DrawCarrierLine(canvas, junctionPoint, bottomPoint, preset.CarrierColors[stem.Id], 5.4f);
        DrawCarrierLine(canvas, junctionPoint, leftArm, WithAlpha(preset.CarrierColors[fork.Id], CarrierPreviewAlpha), 5.2f);
        DrawCarrierLine(canvas, junctionPoint, rightArm, WithAlpha(preset.CarrierColors[fork.Id], CarrierPreviewAlpha), 5.2f);
        DrawCarrierLabel(canvas, stem.Name ?? "Stem", new SKPoint(junctionPoint.X + 14f, guide.Bottom - 10f), preset.CarrierColors[stem.Id]);
        DrawCarrierLabel(canvas, fork.Name ?? "Fork", new SKPoint(guide.MidX - 20f, guide.Top + 18f), preset.CarrierColors[fork.Id]);
        RegisterPreviewCarrier($"{_selectedPreset}.Stem", stem, stem.Name ?? "Stem", [junctionPoint, bottomPoint]);
        RegisterPreviewCarrier($"{_selectedPreset}.ForkLeft", fork, fork.Name ?? "Fork", [junctionPoint, leftArm]);
        RegisterPreviewCarrier($"{_selectedPreset}.ForkRight", fork, fork.Name ?? "Fork", [junctionPoint, rightArm]);
        DrawSite(canvas, junction, "P1", junctionPoint, hostTangent, DragHandleKind.YJunction);
        RegisterHandle(DragHandleKind.YJunction, junctionPoint, junctionPoint, junctionPoint);
    }

    private void DrawAPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        var guide = GetLetterboxRect(rect);
        CarrierIdentity leftLeg = preset.Graph.Carriers.First(carrier => carrier.Name == "Left Leg");
        CarrierIdentity rightLeg = preset.Graph.Carriers.First(carrier => carrier.Name == "Right Leg");
        CarrierIdentity crossbar = preset.Graph.Carriers.First(carrier => carrier.Name == "Crossbar");
        CarrierPinSite leftBar = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Left Bar"));
        CarrierPinSite rightBar = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Right Bar"));

        SKPoint apex = new(guide.MidX, guide.Top);
        SKPoint leftBase = new(guide.Left, guide.Bottom);
        SKPoint rightBase = new(guide.Right, guide.Bottom);
        SKPoint leftBarPoint = new(
            Lerp(leftBase.X, apex.X, _aLeftT),
            Lerp(leftBase.Y, apex.Y, _aLeftT));
        SKPoint rightBarPoint = new(
            Lerp(rightBase.X, apex.X, _aRightT),
            Lerp(rightBase.Y, apex.Y, _aRightT));
        SKPoint leftTangent = Normalize(new SKPoint(apex.X - leftBase.X, apex.Y - leftBase.Y));
        SKPoint rightTangent = Normalize(new SKPoint(apex.X - rightBase.X, apex.Y - rightBase.Y));

        DrawCarrierLine(canvas, leftBase, apex, preset.CarrierColors[leftLeg.Id], 5.2f);
        DrawCarrierLine(canvas, rightBase, apex, preset.CarrierColors[rightLeg.Id], 5.2f);
        DrawSharedCarrierCurve(
            canvas,
            leftBarPoint,
            leftBar,
            crossbar.Id,
            leftTangent,
            rightBarPoint,
            rightBar,
            crossbar.Id,
            rightTangent,
            preset.CarrierColors[crossbar.Id],
            5.0f);
        DrawCarrierLabel(canvas, leftLeg.Name ?? "Left", new SKPoint(leftBase.X - 44f, guide.MidY), preset.CarrierColors[leftLeg.Id]);
        DrawCarrierLabel(canvas, rightLeg.Name ?? "Right", new SKPoint(rightBase.X + 12f, guide.MidY), preset.CarrierColors[rightLeg.Id]);
        DrawCarrierLabel(canvas, crossbar.Name ?? "Crossbar", new SKPoint(guide.MidX - 28f, leftBarPoint.Y - 16f), preset.CarrierColors[crossbar.Id]);
        RegisterPreviewCarrier($"{_selectedPreset}.LeftLeg", leftLeg, leftLeg.Name ?? "Left Leg", [leftBase, apex]);
        RegisterPreviewCarrier($"{_selectedPreset}.RightLeg", rightLeg, rightLeg.Name ?? "Right Leg", [rightBase, apex]);
        RegisterPreviewCarrier(
            $"{_selectedPreset}.Crossbar",
            crossbar,
            crossbar.Name ?? "Crossbar",
            BuildSharedCarrierSamples(leftBarPoint, leftBar, crossbar.Id, leftTangent, rightBarPoint, rightBar, crossbar.Id, rightTangent));
        DrawSite(canvas, leftBar, "P1", leftBarPoint, leftTangent, DragHandleKind.ALeft);
        DrawSite(canvas, rightBar, "P2", rightBarPoint, rightTangent, DragHandleKind.ARight);
        RegisterHandle(DragHandleKind.ALeft, leftBarPoint, leftBase, apex);
        RegisterHandle(DragHandleKind.ARight, rightBarPoint, rightBase, apex);
    }

    private void DrawMPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        var guide = GetLetterboxRect(rect);
        CarrierIdentity leftStem = preset.Graph.Carriers.First(carrier => carrier.Name == "Left Stem");
        CarrierIdentity rightStem = preset.Graph.Carriers.First(carrier => carrier.Name == "Right Stem");
        CarrierIdentity middle = preset.Graph.Carriers.First(carrier => carrier.Name == "Middle");
        CarrierPinSite leftPeak = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Left Peak"));
        CarrierPinSite rightPeak = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Right Peak"));

        SKPoint leftTop = new(guide.Left, guide.Top);
        SKPoint rightTop = new(guide.Right, guide.Top);
        SKPoint leftBottom = new(guide.Left, guide.Bottom);
        SKPoint rightBottom = new(guide.Right, guide.Bottom);
        SKPoint leftPeakPoint = new(guide.Left, guide.Top + (guide.Height * 0.12f));
        SKPoint rightPeakPoint = new(guide.Right, guide.Top + (guide.Height * 0.12f));
        SKPoint hostTangent = new(0f, 1f);

        DrawCarrierLine(canvas, leftTop, leftBottom, preset.CarrierColors[leftStem.Id], 5.2f);
        DrawCarrierLine(canvas, rightTop, rightBottom, preset.CarrierColors[rightStem.Id], 5.2f);
        DrawSharedCarrierCurve(
            canvas,
            leftPeakPoint,
            leftPeak,
            middle.Id,
            hostTangent,
            rightPeakPoint,
            rightPeak,
            middle.Id,
            hostTangent,
            preset.CarrierColors[middle.Id],
            5.0f);
        DrawCarrierLabel(canvas, leftStem.Name ?? "Left", new SKPoint(leftTop.X - 44f, guide.MidY), preset.CarrierColors[leftStem.Id]);
        DrawCarrierLabel(canvas, rightStem.Name ?? "Right", new SKPoint(rightTop.X + 12f, guide.MidY), preset.CarrierColors[rightStem.Id]);
        DrawCarrierLabel(canvas, middle.Name ?? "Middle", new SKPoint(guide.MidX - 20f, guide.Top + (guide.Height * 0.35f)), preset.CarrierColors[middle.Id]);
        RegisterPreviewCarrier($"{_selectedPreset}.LeftStem", leftStem, leftStem.Name ?? "Left Stem", [leftTop, leftBottom]);
        RegisterPreviewCarrier($"{_selectedPreset}.RightStem", rightStem, rightStem.Name ?? "Right Stem", [rightTop, rightBottom]);
        RegisterPreviewCarrier(
            $"{_selectedPreset}.Middle",
            middle,
            middle.Name ?? "Middle",
            BuildSharedCarrierSamples(leftPeakPoint, leftPeak, middle.Id, hostTangent, rightPeakPoint, rightPeak, middle.Id, hostTangent));
        DrawSite(canvas, leftPeak, "P1", leftPeakPoint, hostTangent, DragHandleKind.MLeft);
        DrawSite(canvas, rightPeak, "P2", rightPeakPoint, hostTangent, DragHandleKind.MRight);
        RegisterHandle(DragHandleKind.MLeft, leftPeakPoint, leftPeakPoint, leftPeakPoint);
        RegisterHandle(DragHandleKind.MRight, rightPeakPoint, rightPeakPoint, rightPeakPoint);
    }

    private void DrawSite(
        SKCanvas canvas,
        CarrierPinSite site,
        string label,
        SKPoint point,
        SKPoint hostTangent,
        DragHandleKind handleKind,
        DragHandleKind? recessiveHandle = null,
        DragHandleKind? dominantHandle = null,
        string? siteName = null,
        string? carrierKey = null)
    {
        bool selected = IsSelectedSite(siteName ?? site.Name);
        if (_dragHandle == handleKind)
        {
            canvas.DrawCircle(point, 18f, _handleHaloFillPaint);
            canvas.DrawCircle(point, 18f, _handleHaloStrokePaint);
        }
        else if (selected)
        {
            canvas.DrawCircle(point, 18f, _handleHaloFillPaint);
            canvas.DrawCircle(point, 18f, _handleHaloStrokePaint);
        }
        else
        {
            canvas.DrawCircle(point, 16f, _handleHaloFillPaint);
        }

        DrawSiteRay(
            canvas,
            point,
            hostTangent,
            site,
            PinSideRole.Recessive,
            RecessiveRayColor,
            recessiveHandle ?? ResolveRayHandle(site.Name, PinSideRole.Recessive),
            siteName ?? site.Name,
            carrierKey);
        DrawSiteRay(
            canvas,
            point,
            hostTangent,
            site,
            PinSideRole.Dominant,
            DominantRayColor,
            dominantHandle ?? ResolveRayHandle(site.Name, PinSideRole.Dominant),
            siteName ?? site.Name,
            carrierKey);
        canvas.DrawCircle(point, 11f, _pinFillPaint);
        canvas.DrawCircle(point, 11f, _pinStrokePaint);
        canvas.DrawText(label, point.X + 14f, point.Y - 14f, _pinLabelPaint);
    }

    private void DrawToggleLegend(SKCanvas canvas, SKRect sceneRect, ShapePreset preset)
    {
        CarrierPinSite site = GetSelectedSite(preset);
        float cellWidth = 44f;
        float cellHeight = 32f;
        float gap = 6f;

        var items = new (SignToggleComponent Component, string Label, string Sign, SKColor Accent)[]
        {
            (SignToggleComponent.RecessiveUnit, "i", FormatSign(site.Applied.Recessive.Recessive), RecessiveRayColor),
            (SignToggleComponent.DominantValue, "uV", FormatSign(site.Applied.Dominant.Dominant), DominantRayColor),
            (SignToggleComponent.RecessiveValue, "iV", FormatSign(site.Applied.Recessive.Dominant), RecessiveRayColor),
            (SignToggleComponent.DominantUnit, "u", FormatSign(site.Applied.Dominant.Recessive), DominantRayColor),
        };

        float startX = sceneRect.Left + 10f;
        float startY = sceneRect.Top + 10f;
        for (int index = 0; index < items.Length; index++)
        {
            int row = index / 2;
            int col = index % 2;
            var cellRect = new SKRect(
                startX + (col * (cellWidth + gap)),
                startY + (row * (cellHeight + gap)),
                startX + (col * (cellWidth + gap)) + cellWidth,
                startY + (row * (cellHeight + gap)) + cellHeight);
            DrawToggleCell(canvas, cellRect, items[index].Label, items[index].Sign, items[index].Accent);
            _toggleLayouts.Add(new ToggleLayout(cellRect, items[index].Component));
        }
    }

    private void DrawToggleCell(SKCanvas canvas, SKRect rect, string label, string sign, SKColor accent)
    {
        canvas.DrawRoundRect(rect, 10f, 10f, _buttonFillPaint);
        canvas.DrawRoundRect(rect, 10f, 10f, _buttonStrokePaint);
        using var accentPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accent,
            IsAntialias = true,
        };
        canvas.DrawRoundRect(new SKRect(rect.Left + 6f, rect.Top + 6f, rect.Left + 10f, rect.Bottom - 6f), 2f, 2f, accentPaint);

        using var labelPaint = new SKPaint
        {
            Color = accent,
            TextSize = 11f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };
        using var signPaint = new SKPaint
        {
            Color = new SKColor(66, 66, 66),
            TextSize = 16f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };

        canvas.DrawText(label, rect.Left + 15f, rect.Top + 14f, labelPaint);
        float signWidth = signPaint.MeasureText(sign);
        canvas.DrawText(sign, rect.Right - signWidth - 10f, rect.MidY + 6f, signPaint);
    }

    private void DrawStemEndpointHandle(SKCanvas canvas, SKPoint center, bool selected)
    {
        float radius = selected ? 9f : 7f;
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 240),
            IsAntialias = true,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = selected ? 2f : 1.5f,
            Color = selected ? new SKColor(110, 158, 220) : new SKColor(148, 148, 148),
            IsAntialias = true,
        };
        canvas.DrawCircle(center, radius, fill);
        canvas.DrawCircle(center, radius, stroke);
    }

    private void DrawSiteRay(
        SKCanvas canvas,
        SKPoint origin,
        SKPoint hostTangent,
        CarrierPinSite site,
        PinSideRole role,
        SKColor color,
        DragHandleKind handle,
        string? siteName,
        string? carrierKey = null)
    {
        bool resolved = TryResolveSideDirection(site, role, hostTangent, out SKPoint direction, out float magnitude);
        if (!resolved && !TryResolveCollapsedSideDirection(site, role, hostTangent, out direction))
        {
            return;
        }

        float length = resolved
            ? GetPreviewRayLength(handle, Math.Clamp(20f + magnitude * 16f, 24f, 104f), siteName)
            : 0f;
        SKPoint handlePoint = resolved
            ? new(origin.X + direction.X * length, origin.Y + direction.Y * length)
            : new(origin.X + direction.X * ZeroRayHandleOffset, origin.Y + direction.Y * ZeroRayHandleOffset);

        if (resolved && role == PinSideRole.Recessive)
        {
            DrawDotSegment(canvas, origin, handlePoint, color, 4.4f);
        }
        else if (resolved)
        {
            DrawArrowSegment(canvas, origin, handlePoint, color, 4.4f);
        }

        if (handle != DragHandleKind.None)
        {
            bool active = _dragHandle == handle && (_activeHandleLayout?.SiteName is null || _activeHandleLayout.SiteName == siteName);
            if (active)
            {
                canvas.DrawCircle(handlePoint, 16f, _handleHaloFillPaint);
                canvas.DrawCircle(handlePoint, 16f, _handleHaloStrokePaint);
            }
            else
            {
                canvas.DrawCircle(handlePoint, 14f, _handleHaloFillPaint);
            }

            RegisterRayHandle(
                handle,
                handlePoint,
                origin,
                new SKPoint(origin.X + direction.X * MaxPreviewRayLength, origin.Y + direction.Y * MaxPreviewRayLength),
                siteName,
                role,
                carrierKey);
        }
    }

    private void DrawSharedCarrierCurve(
        SKCanvas canvas,
        SKPoint startPoint,
        CarrierPinSite startSite,
        CarrierId carrierId,
        SKPoint startHostTangent,
        SKPoint endPoint,
        CarrierPinSite endSite,
        CarrierId endCarrierId,
        SKPoint endHostTangent,
        SKColor color,
        float strokeWidth)
    {
        DrawCarrierPath(
            canvas,
            BuildSharedCarrierSamples(startPoint, startSite, carrierId, startHostTangent, endPoint, endSite, endCarrierId, endHostTangent),
            color,
            strokeWidth);
    }

    private static bool TryResolveSideDirection(
        CarrierPinSite site,
        PinSideRole role,
        SKPoint hostTangent,
        out SKPoint direction,
        out float magnitude)
    {
        PositionedAxisSide side = role == PinSideRole.Recessive
            ? site.PlaceApplied().RecessiveSide
            : site.PlaceApplied().DominantSide;
        if (!side.HasCarrier || side.DisplayDirectionSign == 0)
        {
            direction = SKPoint.Empty;
            magnitude = 0f;
            return false;
        }

        SKPoint tangent = Normalize(hostTangent);
        SKPoint orthogonal = new(tangent.Y, -tangent.X);
        SKPoint basis = side.CarrierRank switch
        {
            0 => tangent,
            1 => orthogonal,
            _ => SKPoint.Empty,
        };
        if (basis == SKPoint.Empty)
        {
            direction = SKPoint.Empty;
            magnitude = 0f;
            return false;
        }

        direction = new(basis.X * side.DisplayDirectionSign, basis.Y * side.DisplayDirectionSign);
        magnitude = SafeMagnitude(side.Magnitude);
        return true;
    }

    private static bool TryResolveCollapsedSideDirection(
        CarrierPinSite site,
        PinSideRole role,
        SKPoint hostTangent,
        out SKPoint direction)
    {
        PositionedAxisSide side = role == PinSideRole.Recessive
            ? site.PlaceApplied().RecessiveSide
            : site.PlaceApplied().DominantSide;
        if (!side.HasCarrier)
        {
            direction = SKPoint.Empty;
            return false;
        }

        SKPoint tangent = Normalize(hostTangent);
        SKPoint orthogonal = new(tangent.Y, -tangent.X);
        SKPoint basis = side.CarrierRank switch
        {
            0 => tangent,
            1 => orthogonal,
            _ => SKPoint.Empty,
        };
        if (basis == SKPoint.Empty)
        {
            direction = SKPoint.Empty;
            return false;
        }

        int nativePositiveDirection = role == PinSideRole.Recessive ? -1 : 1;
        direction = new(basis.X * nativePositiveDirection, basis.Y * nativePositiveDirection);
        return true;
    }

    private static bool TryResolveAttachmentBehavior(
        CarrierPinSite site,
        CarrierId carrierId,
        SKPoint hostTangent,
        out SKPoint direction,
        out float magnitude,
        out PinSideRole role,
        out bool hasInfluence)
    {
        var attachment = site.SideAttachments.FirstOrDefault(candidate => candidate.CarrierId == carrierId);
        if (attachment is null)
        {
            direction = SKPoint.Empty;
            magnitude = 0f;
            role = PinSideRole.Dominant;
            hasInfluence = false;
            return false;
        }

        role = attachment.Role;
        if (TryResolveSideDirection(site, role, hostTangent, out direction, out magnitude))
        {
            hasInfluence = true;
            return true;
        }

        hasInfluence = false;
        magnitude = 0f;
        return TryResolveCollapsedSideDirection(site, role, hostTangent, out direction);
    }

    private void DrawCarrierLine(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = CreateStrokePaint(WithAlpha(color, CarrierPreviewAlpha), strokeWidth);
        canvas.DrawLine(start, end, paint);
    }

    private void DrawCarrierPath(SKCanvas canvas, IReadOnlyList<SKPoint> samples, SKColor color, float strokeWidth)
    {
        if (samples.Count < 2)
        {
            return;
        }

        using var paint = CreateStrokePaint(WithAlpha(color, CarrierPreviewAlpha), strokeWidth);
        using var path = new SKPath();
        path.MoveTo(samples[0]);
        for (int index = 1; index < samples.Count; index++)
        {
            path.LineTo(samples[index]);
        }

        canvas.DrawPath(path, paint);
    }

    private IReadOnlyList<SKPoint> BuildSharedCarrierSamples(
        SKPoint startPoint,
        CarrierPinSite startSite,
        CarrierId carrierId,
        SKPoint startHostTangent,
        SKPoint endPoint,
        CarrierPinSite endSite,
        CarrierId endCarrierId,
        SKPoint endHostTangent)
    {
        if (!TryResolveAttachmentBehavior(startSite, carrierId, startHostTangent, out SKPoint startDirection, out float startMagnitude, out PinSideRole startRole, out bool startHasInfluence) ||
            !TryResolveAttachmentBehavior(endSite, endCarrierId, endHostTangent, out SKPoint endDirection, out float endMagnitude, out PinSideRole endRole, out bool endHasInfluence))
        {
            return [startPoint, endPoint];
        }

        float startHandle = GetPreviewRayLength(
            ResolveRayHandle(startSite.Name, startRole),
            Math.Clamp(20f + startMagnitude * 16f, 24f, 104f),
            startSite.Name);
        float endHandle = GetPreviewRayLength(
            ResolveRayHandle(endSite.Name, endRole),
            Math.Clamp(20f + endMagnitude * 16f, 24f, 104f),
            endSite.Name);

        if (startHasInfluence && endHasInfluence)
        {
            SKPoint control1 = new(startPoint.X + startDirection.X * startHandle, startPoint.Y + startDirection.Y * startHandle);
            SKPoint control2 = new(endPoint.X + endDirection.X * endHandle, endPoint.Y + endDirection.Y * endHandle);
            return SampleCubic(startPoint, control1, control2, endPoint, 24);
        }

        if (startHasInfluence)
        {
            SKPoint control = new(startPoint.X + startDirection.X * startHandle, startPoint.Y + startDirection.Y * startHandle);
            return SampleQuadratic(startPoint, control, endPoint, 18);
        }

        if (endHasInfluence)
        {
            SKPoint control = new(endPoint.X + endDirection.X * endHandle, endPoint.Y + endDirection.Y * endHandle);
            return SampleQuadratic(startPoint, control, endPoint, 18);
        }

        return [startPoint, endPoint];
    }

    private void RegisterPreviewCarrier(string key, CarrierIdentity carrier, string label, IReadOnlyList<SKPoint> samples)
    {
        List<SKPoint> filtered = [];
        foreach (SKPoint sample in samples)
        {
            if (filtered.Count == 0 || Distance(filtered[^1], sample) > 0.5f)
            {
                filtered.Add(sample);
            }
        }

        if (filtered.Count >= 2)
        {
            _previewCarriers.Add(new PreviewCarrierLayout(key, carrier, label, filtered));
        }
    }

    private bool TryResolvePreviewCarrierPoint(string carrierKey, float t, out SKPoint point, out SKPoint tangent)
    {
        PreviewCarrierLayout? carrier = _previewCarriers.FirstOrDefault(candidate => candidate.Key == carrierKey);
        if (carrier is null)
        {
            point = SKPoint.Empty;
            tangent = SKPoint.Empty;
            return false;
        }

        return TryEvaluateCarrierAt(carrier, t, out point, out tangent);
    }

    private bool TryFindNearestPreviewCarrier(
        SKPoint point,
        out PreviewCarrierLayout carrier,
        out float t,
        out SKPoint projectedPoint,
        out SKPoint tangent,
        out float distance)
    {
        carrier = null!;
        t = 0f;
        projectedPoint = SKPoint.Empty;
        tangent = SKPoint.Empty;
        distance = float.MaxValue;

        bool found = false;
        foreach (var candidate in _previewCarriers)
        {
            if (!TryProjectToCarrier(candidate, point, out float candidateT, out SKPoint candidatePoint, out SKPoint candidateTangent, out float candidateDistance))
            {
                continue;
            }

            if (candidateDistance >= distance)
            {
                continue;
            }

            found = true;
            carrier = candidate;
            t = candidateT;
            projectedPoint = candidatePoint;
            tangent = candidateTangent;
            distance = candidateDistance;
        }

        return found;
    }

    private static bool TryProjectToCarrier(
        PreviewCarrierLayout carrier,
        SKPoint point,
        out float t,
        out SKPoint projectedPoint,
        out SKPoint tangent,
        out float distance)
    {
        t = 0f;
        projectedPoint = SKPoint.Empty;
        tangent = SKPoint.Empty;
        distance = float.MaxValue;

        if (carrier.Samples.Count < 2)
        {
            return false;
        }

        float bestAlong = 0f;
        float totalLength = 0f;
        float bestDistance = float.MaxValue;
        SKPoint bestPoint = SKPoint.Empty;
        SKPoint bestTangent = SKPoint.Empty;

        for (int index = 0; index < carrier.Samples.Count - 1; index++)
        {
            SKPoint start = carrier.Samples[index];
            SKPoint end = carrier.Samples[index + 1];
            float segmentLength = Distance(start, end);
            if (segmentLength < 0.001f)
            {
                continue;
            }

            if (TryProjectToAxis(point, start, end, out float localT))
            {
                SKPoint candidate = new(
                    Lerp(start.X, end.X, localT),
                    Lerp(start.Y, end.Y, localT));
                float candidateDistance = Distance(point, candidate);
                if (candidateDistance < bestDistance)
                {
                    bestDistance = candidateDistance;
                    bestAlong = totalLength + (segmentLength * localT);
                    bestPoint = candidate;
                    bestTangent = Normalize(new SKPoint(end.X - start.X, end.Y - start.Y));
                }
            }

            totalLength += segmentLength;
        }

        if (bestDistance == float.MaxValue)
        {
            return false;
        }

        t = totalLength <= 0.001f ? 0f : bestAlong / totalLength;
        projectedPoint = bestPoint;
        tangent = bestTangent;
        distance = bestDistance;
        return true;
    }

    private static bool TryEvaluateCarrierAt(PreviewCarrierLayout carrier, float t, out SKPoint point, out SKPoint tangent)
    {
        point = SKPoint.Empty;
        tangent = SKPoint.Empty;
        if (carrier.Samples.Count < 2)
        {
            return false;
        }

        float clampedT = Math.Clamp(t, 0f, 1f);
        float totalLength = 0f;
        for (int index = 0; index < carrier.Samples.Count - 1; index++)
        {
            totalLength += Distance(carrier.Samples[index], carrier.Samples[index + 1]);
        }

        if (totalLength < 0.001f)
        {
            point = carrier.Samples[0];
            return false;
        }

        float target = totalLength * clampedT;
        float travelled = 0f;
        for (int index = 0; index < carrier.Samples.Count - 1; index++)
        {
            SKPoint start = carrier.Samples[index];
            SKPoint end = carrier.Samples[index + 1];
            float segmentLength = Distance(start, end);
            if (segmentLength < 0.001f)
            {
                continue;
            }

            if (travelled + segmentLength >= target)
            {
                float localT = (target - travelled) / segmentLength;
                point = new(
                    Lerp(start.X, end.X, localT),
                    Lerp(start.Y, end.Y, localT));
                tangent = Normalize(new SKPoint(end.X - start.X, end.Y - start.Y));
                return true;
            }

            travelled += segmentLength;
        }

        point = carrier.Samples[^1];
        tangent = Normalize(new SKPoint(
            carrier.Samples[^1].X - carrier.Samples[^2].X,
            carrier.Samples[^1].Y - carrier.Samples[^2].Y));
        return true;
    }

    private static IReadOnlyList<SKPoint> CombineSamples(params IReadOnlyList<SKPoint>[] parts)
    {
        List<SKPoint> combined = [];
        foreach (var part in parts)
        {
            foreach (SKPoint point in part)
            {
                if (combined.Count == 0 || Distance(combined[^1], point) > 0.5f)
                {
                    combined.Add(point);
                }
            }
        }

        return combined;
    }

    private static IReadOnlyList<SKPoint> SampleQuadratic(SKPoint start, SKPoint control, SKPoint end, int segments)
    {
        List<SKPoint> samples = [];
        for (int index = 0; index <= segments; index++)
        {
            float t = index / (float)segments;
            float mt = 1f - t;
            samples.Add(new SKPoint(
                (mt * mt * start.X) + (2f * mt * t * control.X) + (t * t * end.X),
                (mt * mt * start.Y) + (2f * mt * t * control.Y) + (t * t * end.Y)));
        }

        return samples;
    }

    private static IReadOnlyList<SKPoint> SampleCubic(SKPoint start, SKPoint control1, SKPoint control2, SKPoint end, int segments)
    {
        List<SKPoint> samples = [];
        for (int index = 0; index <= segments; index++)
        {
            float t = index / (float)segments;
            float mt = 1f - t;
            samples.Add(new SKPoint(
                (mt * mt * mt * start.X) + (3f * mt * mt * t * control1.X) + (3f * mt * t * t * control2.X) + (t * t * t * end.X),
                (mt * mt * mt * start.Y) + (3f * mt * mt * t * control1.Y) + (3f * mt * t * t * control2.Y) + (t * t * t * end.Y)));
        }

        return samples;
    }

    private void DrawCarrierLabel(SKCanvas canvas, string label, SKPoint point, SKColor color)
    {
        using var paint = new SKPaint
        {
            Color = color,
            TextSize = 14f,
            Typeface = SKTypeface.FromFamilyName(VisualStyle.FontFamily, SKFontStyle.Bold),
            IsAntialias = true,
        };
        canvas.DrawText(label, point.X, point.Y, paint);
    }

    private void DrawArrowSegment(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        var vector = new SKPoint(end.X - start.X, end.Y - start.Y);
        float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        if (length < 0.001f)
        {
            return;
        }

        float ux = vector.X / length;
        float uy = vector.Y / length;
        float headLength = 16f;
        float headWidth = 9f;
        var back = new SKPoint(end.X - ux * headLength, end.Y - uy * headLength);
        using var stroke = CreateStrokePaint(color, strokeWidth);
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

    private void DrawDotSegment(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var stroke = CreateStrokePaint(color, strokeWidth);
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true,
        };
        canvas.DrawLine(start, end, stroke);
        canvas.DrawCircle(end, 8.5f, fill);
    }

    private void DrawBadge(SKCanvas canvas, SKPoint topLeft, string text, SKColor? accentColor = null)
    {
        float width = Math.Max(86f, _badgeTextPaint.MeasureText(text) + 26f);
        var rect = new SKRect(topLeft.X, topLeft.Y, topLeft.X + width, topLeft.Y + 28f);
        canvas.DrawRoundRect(rect, 14f, 14f, _badgeFillPaint);
        canvas.DrawRoundRect(rect, 14f, 14f, _badgeStrokePaint);
        if (accentColor is SKColor accent)
        {
            using var stripe = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = accent,
                IsAntialias = true,
            };
            canvas.DrawRoundRect(new SKRect(rect.Left + 6f, rect.Top + 6f, rect.Left + 14f, rect.Bottom - 6f), 4f, 4f, stripe);
            canvas.DrawText(text, rect.Left + 22f, rect.MidY + 5f, _badgeTextPaint);
            return;
        }

        canvas.DrawText(text, rect.Left + 12f, rect.MidY + 5f, _badgeTextPaint);
    }

    private void DrawCopyButton(SKCanvas canvas, SKRect rect)
    {
        canvas.DrawRoundRect(rect, 8f, 8f, _buttonFillPaint);
        canvas.DrawRoundRect(rect, 8f, 8f, _buttonStrokePaint);
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.4f,
            Color = new SKColor(112, 112, 112),
            IsAntialias = true,
        };
        var back = new SKRect(rect.Left + 9f, rect.Top + 7f, rect.Left + 17f, rect.Top + 15f);
        var front = new SKRect(rect.Left + 12f, rect.Top + 10f, rect.Left + 20f, rect.Top + 18f);
        canvas.DrawRoundRect(back, 2f, 2f, stroke);
        canvas.DrawRoundRect(front, 2f, 2f, stroke);
    }

    private string ResolveSelectedCarrierTitle(CarrierPinSite site)
    {
        CarrierIdentity? primaryAttachment =
            site.DominantAttachment?.Carrier ??
            site.RecessiveAttachment?.Carrier;
        if (primaryAttachment is not null && primaryAttachment.Id != site.HostCarrier.Id)
        {
            return primaryAttachment.Name ?? primaryAttachment.Id.ToString();
        }

        return site.HostCarrier.Name ?? site.HostCarrier.Id.ToString();
    }

    private SKColor ResolveSelectedCarrierColor(ShapePreset preset, CarrierPinSite site)
    {
        CarrierId carrierId =
            site.DominantAttachment?.CarrierId ??
            site.RecessiveAttachment?.CarrierId ??
            site.HostCarrier.Id;
        return preset.CarrierColors.TryGetValue(carrierId, out var color)
            ? color
            : new SKColor(96, 96, 96);
    }

    private List<CustomPreviewPin> GetCustomPins(PresetKind preset)
    {
        if (!_customPins.TryGetValue(preset, out var pins))
        {
            pins = [];
            _customPins[preset] = pins;
        }

        return pins;
    }

    private bool TryGetCustomPin(PresetKind preset, string siteName, out CustomPreviewPin? pin)
    {
        pin = GetCustomPins(preset).FirstOrDefault(candidate => string.Equals(candidate.Name, siteName, StringComparison.Ordinal));
        return pin is not null;
    }

    private CarrierPinSite BuildCustomSite(CustomPreviewPin pin) =>
        CarrierPinSite.FromPointPinning(
            pin.HostCarrier,
            PreviewHostAxis.PinAt(pin.Axis, ToPreviewProportion(pin.T)),
            name: pin.Name);

    private CarrierPinSite ResolveSite(CarrierPinSite site)
    {
        if (site.Name is null ||
            !_siteAxisOverrides.TryGetValue((_selectedPreset, site.Name), out var applied))
        {
            return site;
        }

        return new CarrierPinSite(site.Id, site.HostCarrier, new PointPinning<Axis, Axis>(site.Host, applied, site.HostPosition), site.SideAttachments, site.Name);
    }

    private CarrierPinSite FindSite(ShapePreset preset, string siteName)
    {
        CarrierPinSite? presetSite = preset.Graph.Sites.FirstOrDefault(candidate => candidate.Name == siteName);
        if (presetSite is not null)
        {
            return ResolveSite(presetSite);
        }

        if (TryGetCustomPin(_selectedPreset, siteName, out var customPin) && customPin is not null)
        {
            return BuildCustomSite(customPin);
        }

        return ResolveSite(preset.Graph.Sites.First());
    }

    private CarrierPinSite GetSelectedSite(ShapePreset preset)
    {
        string selectedName = _selectedSiteByPreset.TryGetValue(_selectedPreset, out var name)
            ? name
            : preset.Graph.Sites.First().Name ?? preset.Graph.Sites.First().Id.ToString();
        return FindSite(preset, selectedName);
    }

    private CarrierSiteStructuralProfile ResolveSiteProfile(ShapePreset preset, CarrierPinSite site)
    {
        if (preset.Analysis.TryGetSiteProfile(site.Id, out var profile) && profile is not null)
        {
            return profile;
        }

        return new CarrierSiteStructuralProfile(site, site.ResolveRouting());
    }

    private bool UpdateSiteAxis(PresetKind preset, string siteName, Axis axis)
    {
        if (TryGetCustomPin(preset, siteName, out var customPin) && customPin is not null)
        {
            customPin.Axis = axis;
            SyncInlineValueText();
            return true;
        }

        _siteAxisOverrides[(preset, siteName)] = axis;
        SyncInlineValueText();
        return true;
    }

    private bool HasSelectedCustomPin() =>
        _selectedSiteByPreset.TryGetValue(_selectedPreset, out var selectedName) &&
        TryGetCustomPin(_selectedPreset, selectedName, out _);

    private bool RemoveSelectedCustomPin()
    {
        if (!_selectedSiteByPreset.TryGetValue(_selectedPreset, out var selectedName))
        {
            return false;
        }

        List<CustomPreviewPin> pins = GetCustomPins(_selectedPreset);
        int index = pins.FindIndex(candidate => candidate.Name == selectedName);
        if (index < 0)
        {
            return false;
        }

        pins.RemoveAt(index);
        _selectedSiteByPreset[_selectedPreset] = _presets[_selectedPreset].Graph.Sites.First().Name ?? string.Empty;
        return true;
    }

    private bool TryAddCustomPin(SKPoint point)
    {
        if (!TryFindNearestPreviewCarrier(point, out var carrier, out float t, out _, out _, out float distance) ||
            distance > CarrierHitThreshold)
        {
            return false;
        }

        ShapePreset preset = _presets[_selectedPreset];
        Axis axis = GetSelectedSite(preset).Applied;
        string name = NextCustomPinName(_selectedPreset);
        GetCustomPins(_selectedPreset).Add(
            new CustomPreviewPin
            {
                Name = name,
                CarrierKey = carrier.Key,
                HostCarrier = carrier.Carrier,
                T = t,
                Axis = axis,
            });
        _selectedSiteByPreset[_selectedPreset] = name;
        _addPinArmed = false;
        return true;
    }

    private static bool TryParseAxisText(string text, out Axis axis)
    {
        axis = Axis.Zero;
        var match = System.Text.RegularExpressions.Regex.Match(
            text,
            @"^\s*\[\s*(?<rv>-?\d+)\s*/\s*(?<ru>-?\d+)\s*\]\s*i\s*\+\s*\[\s*(?<dv>-?\d+)\s*/\s*(?<du>-?\d+)\s*\]\s*$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return false;
        }

        bool parsedRecessiveValue = long.TryParse(match.Groups["rv"].Value, out long recessiveValue);
        bool parsedRecessiveUnit = long.TryParse(match.Groups["ru"].Value, out long recessiveUnit);
        bool parsedDominantValue = long.TryParse(match.Groups["dv"].Value, out long dominantValue);
        bool parsedDominantUnit = long.TryParse(match.Groups["du"].Value, out long dominantUnit);
        if (!(parsedRecessiveValue && parsedRecessiveUnit && parsedDominantValue && parsedDominantUnit))
        {
            return false;
        }

        axis = new Axis(recessiveValue, recessiveUnit, dominantValue, dominantUnit);
        return true;
    }

    private void EnsureInlineValueEditor()
    {
        if (_canvasHost is null || _inlineValueEditor is not null)
        {
            return;
        }

        _inlineValueEditor = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 11f, FontStyle.Bold),
            Visible = false,
            Multiline = false,
        };
        _inlineValueEditor.KeyDown += OnInlineValueEditorKeyDown;
        _inlineValueEditor.Leave += OnInlineValueEditorLeave;
        _canvasHost.Controls.Add(_inlineValueEditor);
        _inlineValueEditor.BringToFront();
    }

    private void RemoveInlineValueEditor()
    {
        if (_inlineValueEditor is null)
        {
            return;
        }

        _inlineValueEditor.KeyDown -= OnInlineValueEditorKeyDown;
        _inlineValueEditor.Leave -= OnInlineValueEditorLeave;
        if (_canvasHost is not null)
        {
            _canvasHost.Controls.Remove(_inlineValueEditor);
        }

        _inlineValueEditor.Dispose();
        _inlineValueEditor = null;
        _inlineValueTarget = null;
    }

    private void UpdateInlineValueEditor(SKRect valueRect, PresetKind preset, string siteName, string axisText)
    {
        EnsureInlineValueEditor();
        if (_inlineValueEditor is null || _canvasHost is null || _coords is null || string.IsNullOrWhiteSpace(siteName))
        {
            return;
        }

        Rectangle pixelRect = ToControlRect(valueRect);
        _inlineValueEditor.SetBounds(pixelRect.Left + 6, pixelRect.Top + 5, Math.Max(80, pixelRect.Width - 12), Math.Max(20, pixelRect.Height - 10));
        _inlineValueEditor.Visible = true;
        _inlineValueEditor.BringToFront();

        var target = (preset, siteName);
        if (_inlineValueTarget != target || (!_inlineValueEditor.Focused && _inlineValueEditor.Text != axisText))
        {
            _inlineValueSyncing = true;
            _inlineValueEditor.Text = axisText;
            _inlineValueTarget = target;
            _inlineValueSyncing = false;
        }
    }

    private void SyncInlineValueText()
    {
        if (_inlineValueEditor is null || _inlineValueEditor.Focused || _inlineValueTarget is not { } target)
        {
            return;
        }

        ShapePreset preset = _presets[target.Preset];
        CarrierPinSite site = FindSite(preset, target.SiteName);
        string axisText = FormatAxis(site.Applied);
        if (_inlineValueEditor.Text == axisText)
        {
            return;
        }

        _inlineValueSyncing = true;
        _inlineValueEditor.Text = axisText;
        _inlineValueSyncing = false;
    }

    private void OnInlineValueEditorKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        CommitInlineValueEditor();
        e.Handled = true;
        e.SuppressKeyPress = true;
        _canvasHost?.Focus();
    }

    private void OnInlineValueEditorLeave(object? sender, EventArgs e)
    {
        if (_inlineValueSyncing)
        {
            return;
        }

        CommitInlineValueEditor();
    }

    private void CommitInlineValueEditor()
    {
        if (_inlineValueEditor is null || _inlineValueTarget is not { } target)
        {
            return;
        }

        string text = _inlineValueEditor.Text.Trim();
        ShapePreset preset = _presets[target.Preset];
        CarrierPinSite currentSite = FindSite(preset, target.SiteName);
        string fallback = FormatAxis(currentSite.Applied);

        if (!TryParseAxisText(text, out var axis))
        {
            _inlineValueSyncing = true;
            _inlineValueEditor.Text = fallback;
            _inlineValueSyncing = false;
            _canvasHost?.InvalidateCanvas();
            return;
        }

        UpdateSiteAxis(target.Preset, target.SiteName, axis);
        string committed = FormatAxis(axis);
        if (_inlineValueEditor.Text != committed)
        {
            _inlineValueSyncing = true;
            _inlineValueEditor.Text = committed;
            _inlineValueSyncing = false;
        }

        _canvasHost?.InvalidateCanvas();
    }

    private void ToggleLetterboxMode()
    {
        PreserveCurrentStemPositions();
        _showLetterbox = !_showLetterbox;
    }

    private void PreserveCurrentStemPositions()
    {
        if (_coords is null)
        {
            return;
        }

        SKRect sceneRect = GetSceneInnerRect();
        _dStemStart = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _dStemStart, LetterboxEdge.Left));
        _dStemEnd = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _dStemEnd, LetterboxEdge.Left));
        _hLeftStemStart = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _hLeftStemStart, LetterboxEdge.Left));
        _hLeftStemEnd = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _hLeftStemEnd, LetterboxEdge.Left));
        _hRightStemStart = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _hRightStemStart, LetterboxEdge.Right));
        _hRightStemEnd = ToNormalized(sceneRect, ResolveStemEndpoint(sceneRect, _hRightStemEnd, LetterboxEdge.Right));
    }

    private SKRect GetSceneInnerRect()
    {
        float width = _coords?.Width ?? 1220f;
        float height = _coords?.Height ?? 920f;
        var scenePanel = new SKRect(424f, 106f, width - 20f, height - 24f);
        return new SKRect(scenePanel.Left + 18f, scenePanel.Top + 72f, scenePanel.Right - 18f, scenePanel.Bottom - 18f);
    }

    private Rectangle ToControlRect(SKRect rect)
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

    private bool IsSelectedSite(CarrierPinSite site) =>
        IsSelectedSite(site.Name);

    private bool IsSelectedSite(string? siteName) =>
        siteName is not null &&
        _selectedSiteByPreset.TryGetValue(_selectedPreset, out var selectedName) &&
        string.Equals(selectedName, siteName, StringComparison.Ordinal);

    private void ToggleSelectedComponent(SignToggleComponent component)
    {
        ShapePreset preset = _presets[_selectedPreset];
        CarrierPinSite selectedSite = GetSelectedSite(preset);
        if (selectedSite.Name is null)
        {
            return;
        }

        Axis axis = selectedSite.Applied;
        Axis updated = component switch
        {
            SignToggleComponent.RecessiveUnit => new Axis(
                axis.Recessive.Dominant,
                FlipSign(axis.Recessive.Recessive),
                axis.Dominant.Dominant,
                axis.Dominant.Recessive),
            SignToggleComponent.DominantValue => new Axis(
                axis.Recessive.Dominant,
                axis.Recessive.Recessive,
                FlipSign(axis.Dominant.Dominant),
                axis.Dominant.Recessive),
            SignToggleComponent.RecessiveValue => new Axis(
                FlipSign(axis.Recessive.Dominant),
                axis.Recessive.Recessive,
                axis.Dominant.Dominant,
                axis.Dominant.Recessive),
            SignToggleComponent.DominantUnit => new Axis(
                axis.Recessive.Dominant,
                axis.Recessive.Recessive,
                axis.Dominant.Dominant,
                FlipSign(axis.Dominant.Recessive)),
            _ => axis,
        };

        UpdateSiteAxis(_selectedPreset, selectedSite.Name, updated);
    }

    private static long FlipSign(long value) => value == 0 ? 0 : -value;

    private static string FormatSign(long value) => value switch
    {
        < 0 => "-",
        > 0 => "+",
        _ => "0",
    };

    private static string GetSelectedSiteLabel(CarrierPinSite site) => site.Name switch
    {
        "Top" => "P1",
        "Bottom" => "P2",
        "Left Join" => "P1",
        "Right Join" => "P2",
        "Base" => "P1",
        "Crossbar" => "P2",
        "Corner" => "P1",
        "Junction" => "P1",
        "Left Bar" => "P1",
        "Right Bar" => "P2",
        "Left Peak" => "P1",
        "Right Peak" => "P2",
        _ => site.Name ?? "Pin",
    };

    private static float SafeMagnitude(Proportion magnitude)
    {
        decimal folded = magnitude.Fold();
        if (folded == decimal.MaxValue || folded == decimal.MinValue)
        {
            return Math.Abs(magnitude.Dominant);
        }

        return Math.Abs((float)folded);
    }

    private static SKRect GetLetterboxRect(SKRect sceneRect)
    {
        float size = MathF.Min(sceneRect.Width - 220f, sceneRect.Height - 132f);
        size = MathF.Max(size, 220f);
        float left = sceneRect.MidX - (size * 0.5f);
        float top = sceneRect.MidY - (size * 0.5f);
        return new SKRect(left, top, left + size, top + size);
    }

    private static SKPoint FromNormalized(SKRect rect, SKPoint normalized) =>
        new(rect.Left + (normalized.X * rect.Width), rect.Top + (normalized.Y * rect.Height));

    private static SKPoint ToNormalized(SKRect rect, SKPoint point) =>
        new(
            rect.Width <= 0f ? 0f : (point.X - rect.Left) / rect.Width,
            rect.Height <= 0f ? 0f : (point.Y - rect.Top) / rect.Height);

    private SKPoint ResolveStemEndpoint(SKRect sceneRect, SKPoint normalizedPoint, LetterboxEdge edge)
    {
        SKPoint point = FromNormalized(sceneRect, normalizedPoint);
        if (!_showLetterbox)
        {
            return point;
        }

        var guide = GetLetterboxRect(sceneRect);
        return edge switch
        {
            LetterboxEdge.Left => new SKPoint(guide.Left, Math.Clamp(point.Y, guide.Top, guide.Bottom)),
            LetterboxEdge.Right => new SKPoint(guide.Right, Math.Clamp(point.Y, guide.Top, guide.Bottom)),
            LetterboxEdge.Top => new SKPoint(Math.Clamp(point.X, guide.Left, guide.Right), guide.Top),
            LetterboxEdge.Bottom => new SKPoint(Math.Clamp(point.X, guide.Left, guide.Right), guide.Bottom),
            _ => point,
        };
    }

    private static SKPoint Normalize(SKPoint point)
    {
        float length = MathF.Sqrt(point.X * point.X + point.Y * point.Y);
        return length < 0.0001f ? SKPoint.Empty : new SKPoint(point.X / length, point.Y / length);
    }

    private static SKPaint CreateStrokePaint(SKColor color, float strokeWidth) =>
        new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeWidth,
            Color = color,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
        };

    private static Proportion ToPreviewProportion(float t) =>
        new((long)Math.Round(Math.Clamp(t, 0f, 1f) * 1000f), 1000);

    private static string FormatFloat(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private string NextCustomPinName(PresetKind preset)
    {
        int next = _presets[preset].Graph.Sites.Count + 1;
        foreach (var customPin in GetCustomPins(preset))
        {
            if (customPin.Name.StartsWith("P", StringComparison.Ordinal) &&
                int.TryParse(customPin.Name[1..], out int parsed))
            {
                next = Math.Max(next, parsed + 1);
            }
        }

        return $"P{next}";
    }

    private void CopyTextToClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text);
        }
        catch
        {
            // Ignore clipboard failures in the visualizer.
        }
    }

    private string SerializeCurrentStructure()
    {
        ShapePreset preset = _presets[_selectedPreset];
        var builder = new StringBuilder();
        builder.AppendLine("SharedCarrierScene");
        builder.AppendLine($"preset: {GetPresetButtonText(_selectedPreset)}");
        builder.AppendLine($"guides: {(_showLetterbox ? "on" : "off")}");
        if (_selectedSiteByPreset.TryGetValue(_selectedPreset, out var selectedName))
        {
            builder.AppendLine($"selected: {selectedName}");
        }

        builder.AppendLine("geometry:");
        foreach (string line in SerializePresetGeometry(_selectedPreset))
        {
            builder.AppendLine($"  {line}");
        }

        builder.AppendLine("sites:");
        foreach (CarrierPinSite site in preset.Graph.Sites.Select(ResolveSite))
        {
            builder.AppendLine($"  - kind: preset");
            builder.AppendLine($"    name: {site.Name}");
            builder.AppendLine($"    hostCarrier: {site.HostCarrier.Name ?? site.HostCarrier.Id.ToString()}");
            builder.AppendLine($"    hostPosition: {FormatProportion(site.HostPosition)}");
            builder.AppendLine($"    axis: {FormatAxis(site.Applied)}");
        }

        foreach (CustomPreviewPin pin in GetCustomPins(_selectedPreset))
        {
            builder.AppendLine($"  - kind: custom");
            builder.AppendLine($"    name: {pin.Name}");
            builder.AppendLine($"    hostCarrier: {pin.HostCarrier.Name ?? pin.HostCarrier.Id.ToString()}");
            builder.AppendLine($"    previewCarrier: {pin.CarrierKey}");
            builder.AppendLine($"    t: {FormatFloat(pin.T)}");
            builder.AppendLine($"    axis: {FormatAxis(pin.Axis)}");
        }

        return builder.ToString();
    }

    private IEnumerable<string> SerializePresetGeometry(PresetKind preset) => preset switch
    {
        PresetKind.CapitalD => new[]
        {
            $"dStemStart: [{FormatFloat(_dStemStart.X)}, {FormatFloat(_dStemStart.Y)}]",
            $"dStemEnd: [{FormatFloat(_dStemEnd.X)}, {FormatFloat(_dStemEnd.Y)}]",
            $"dTopT: {FormatFloat(_dTopT)}",
            $"dBottomT: {FormatFloat(_dBottomT)}",
        },
        PresetKind.BridgeH => new[]
        {
            $"hLeftStemStart: [{FormatFloat(_hLeftStemStart.X)}, {FormatFloat(_hLeftStemStart.Y)}]",
            $"hLeftStemEnd: [{FormatFloat(_hLeftStemEnd.X)}, {FormatFloat(_hLeftStemEnd.Y)}]",
            $"hLeftT: {FormatFloat(_hLeftT)}",
            $"hRightStemStart: [{FormatFloat(_hRightStemStart.X)}, {FormatFloat(_hRightStemStart.Y)}]",
            $"hRightStemEnd: [{FormatFloat(_hRightStemEnd.X)}, {FormatFloat(_hRightStemEnd.Y)}]",
            $"hRightT: {FormatFloat(_hRightT)}",
        },
        PresetKind.LetterA => new[]
        {
            $"aLeftT: {FormatFloat(_aLeftT)}",
            $"aRightT: {FormatFloat(_aRightT)}",
        },
        _ => ["(no extra geometry state)"],
    };

    private static string FormatAxis(Axis axis) => $"[{axis.Recessive}]i + [{axis.Dominant}]";

    private static string FormatProportion(Proportion? value)
    {
        if (value is null)
        {
            return "?";
        }

        return value.Recessive == 1
            ? value.Dominant.ToString()
            : $"{value.Dominant}/{value.Recessive}";
    }

    private static string? ResolveSelectedSite(DragHandleKind handleKind) => handleKind switch
    {
        DragHandleKind.DTop or DragHandleKind.DTopRecessive or DragHandleKind.DTopDominant => "Top",
        DragHandleKind.DBottom or DragHandleKind.DBottomRecessive or DragHandleKind.DBottomDominant => "Bottom",
        DragHandleKind.HLeft or DragHandleKind.HLeftRecessive or DragHandleKind.HLeftDominant => "Left Join",
        DragHandleKind.HRight or DragHandleKind.HRightRecessive or DragHandleKind.HRightDominant => "Right Join",
        DragHandleKind.TBase or DragHandleKind.TBaseDominant => "Base",
        DragHandleKind.TCrossbar or DragHandleKind.TCrossbarRecessive or DragHandleKind.TCrossbarDominant => "Crossbar",
        DragHandleKind.YJunction or DragHandleKind.YJunctionRecessive or DragHandleKind.YJunctionDominant => "Junction",
        DragHandleKind.LCorner or DragHandleKind.LCornerRecessive or DragHandleKind.LCornerDominant => "Corner",
        DragHandleKind.ALeft or DragHandleKind.ALeftRecessive or DragHandleKind.ALeftDominant => "Left Bar",
        DragHandleKind.ARight or DragHandleKind.ARightRecessive or DragHandleKind.ARightDominant => "Right Bar",
        DragHandleKind.MLeft or DragHandleKind.MLeftRecessive or DragHandleKind.MLeftDominant => "Left Peak",
        DragHandleKind.MRight or DragHandleKind.MRightRecessive or DragHandleKind.MRightDominant => "Right Peak",
        _ => null,
    };

    private void UpdateDragHandle(SKPoint pixelPoint)
    {
        var handle = _activeHandleLayout ?? _handleLayouts.FirstOrDefault(layout => layout.Target == _dragHandle);
        if (handle is null)
        {
            return;
        }

        if (_dragHandle == DragHandleKind.CustomSite)
        {
            UpdateCustomSiteHandle(pixelPoint, handle);
            return;
        }

        if (handle.FreeMove)
        {
            UpdateFreeStemHandle(pixelPoint, handle);
            return;
        }

        if (!TryProjectToAxis(pixelPoint, handle.AxisStart, handle.AxisEnd, out float t))
        {
            return;
        }

        if ((_dragHandle == DragHandleKind.CustomRecessive || _dragHandle == DragHandleKind.CustomDominant) &&
            handle.SiteName is not null)
        {
            float length = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
            UpdateSiteAxisFromPreviewLength(
                handle.SiteName,
                _dragHandle == DragHandleKind.CustomRecessive ? PinSideRole.Recessive : PinSideRole.Dominant,
                length);
            return;
        }

        switch (_dragHandle)
        {
            case DragHandleKind.DStemTop:
                _dStemStart = UpdateConstrainedStemPoint(_dStemStart, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.DTop:
                _dTopT = ClampOrdered(t, 0f, _dBottomT - 0.08f);
                break;
            case DragHandleKind.DBottom:
                _dBottomT = ClampOrdered(t, _dTopT + 0.08f, 1f);
                break;
            case DragHandleKind.DStemBottom:
                _dStemEnd = UpdateConstrainedStemPoint(_dStemEnd, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.HLeftStemTop:
                _hLeftStemStart = UpdateConstrainedStemPoint(_hLeftStemStart, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.HLeft:
                _hLeftT = ClampOrdered(t, 0f, 1f);
                break;
            case DragHandleKind.HLeftStemBottom:
                _hLeftStemEnd = UpdateConstrainedStemPoint(_hLeftStemEnd, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.HRightStemTop:
                _hRightStemStart = UpdateConstrainedStemPoint(_hRightStemStart, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.HRight:
                _hRightT = ClampOrdered(t, 0f, 1f);
                break;
            case DragHandleKind.HRightStemBottom:
                _hRightStemEnd = UpdateConstrainedStemPoint(_hRightStemEnd, pixelPoint, handle.AxisStart, handle.AxisEnd, handle.BoundsRect);
                break;
            case DragHandleKind.ALeft:
                _aLeftT = ClampOrdered(t, 0f, 1f);
                break;
            case DragHandleKind.ARight:
                _aRightT = ClampOrdered(t, 0f, 1f);
                break;
            case DragHandleKind.DTopRecessive:
                _dTopRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Top", PinSideRole.Recessive, _dTopRecessiveLength);
                break;
            case DragHandleKind.DTopDominant:
                _dTopDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Top", PinSideRole.Dominant, _dTopDominantLength);
                break;
            case DragHandleKind.DBottomRecessive:
                _dBottomRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Bottom", PinSideRole.Recessive, _dBottomRecessiveLength);
                break;
            case DragHandleKind.DBottomDominant:
                _dBottomDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Bottom", PinSideRole.Dominant, _dBottomDominantLength);
                break;
            case DragHandleKind.HLeftRecessive:
                _hLeftRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Join", PinSideRole.Recessive, _hLeftRecessiveLength);
                break;
            case DragHandleKind.HLeftDominant:
                _hLeftDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Join", PinSideRole.Dominant, _hLeftDominantLength);
                break;
            case DragHandleKind.HRightRecessive:
                _hRightRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Join", PinSideRole.Recessive, _hRightRecessiveLength);
                break;
            case DragHandleKind.HRightDominant:
                _hRightDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Join", PinSideRole.Dominant, _hRightDominantLength);
                break;
            case DragHandleKind.TBaseDominant:
                _tBaseDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Base", PinSideRole.Dominant, _tBaseDominantLength);
                break;
            case DragHandleKind.TCrossbarRecessive:
                _tCrossbarRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Crossbar", PinSideRole.Recessive, _tCrossbarRecessiveLength);
                break;
            case DragHandleKind.TCrossbarDominant:
                _tCrossbarDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Crossbar", PinSideRole.Dominant, _tCrossbarDominantLength);
                break;
            case DragHandleKind.YJunctionRecessive:
                _yJunctionRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Junction", PinSideRole.Recessive, _yJunctionRecessiveLength);
                break;
            case DragHandleKind.YJunctionDominant:
                _yJunctionDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Junction", PinSideRole.Dominant, _yJunctionDominantLength);
                break;
            case DragHandleKind.LCornerRecessive:
                _lCornerRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Corner", PinSideRole.Recessive, _lCornerRecessiveLength);
                break;
            case DragHandleKind.LCornerDominant:
                _lCornerDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Corner", PinSideRole.Dominant, _lCornerDominantLength);
                break;
            case DragHandleKind.ALeftRecessive:
                _aLeftRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Bar", PinSideRole.Recessive, _aLeftRecessiveLength);
                break;
            case DragHandleKind.ALeftDominant:
                _aLeftDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Bar", PinSideRole.Dominant, _aLeftDominantLength);
                break;
            case DragHandleKind.ARightRecessive:
                _aRightRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Bar", PinSideRole.Recessive, _aRightRecessiveLength);
                break;
            case DragHandleKind.ARightDominant:
                _aRightDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Bar", PinSideRole.Dominant, _aRightDominantLength);
                break;
            case DragHandleKind.MLeftRecessive:
                _mLeftRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Peak", PinSideRole.Recessive, _mLeftRecessiveLength);
                break;
            case DragHandleKind.MLeftDominant:
                _mLeftDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Left Peak", PinSideRole.Dominant, _mLeftDominantLength);
                break;
            case DragHandleKind.MRightRecessive:
                _mRightRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Peak", PinSideRole.Recessive, _mRightRecessiveLength);
                break;
            case DragHandleKind.MRightDominant:
                _mRightDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                UpdateSiteAxisFromPreviewLength("Right Peak", PinSideRole.Dominant, _mRightDominantLength);
                break;
        }
    }

    private void UpdateCustomSiteHandle(SKPoint pixelPoint, HandleLayout handle)
    {
        if (handle.SiteName is null ||
            handle.CarrierKey is null ||
            !TryGetCustomPin(_selectedPreset, handle.SiteName, out var customPin) ||
            customPin is null)
        {
            return;
        }

        PreviewCarrierLayout? carrier = _previewCarriers.FirstOrDefault(candidate => candidate.Key == handle.CarrierKey);
        if (carrier is null ||
            !TryProjectToCarrier(carrier, pixelPoint, out float t, out _, out _, out _))
        {
            return;
        }

        customPin.T = t;
    }

    private void UpdateFreeStemHandle(SKPoint pixelPoint, HandleLayout handle)
    {
        if (handle.BoundsRect is not SKRect bounds)
        {
            return;
        }

        SKPoint normalized = ToNormalized(bounds, pixelPoint);
        switch (_dragHandle)
        {
            case DragHandleKind.DStemTop:
                _dStemStart = normalized;
                break;
            case DragHandleKind.DStemBottom:
                _dStemEnd = normalized;
                break;
            case DragHandleKind.HLeftStemTop:
                _hLeftStemStart = normalized;
                break;
            case DragHandleKind.HLeftStemBottom:
                _hLeftStemEnd = normalized;
                break;
            case DragHandleKind.HRightStemTop:
                _hRightStemStart = normalized;
                break;
            case DragHandleKind.HRightStemBottom:
                _hRightStemEnd = normalized;
                break;
        }
    }

    private void UpdateSiteAxisFromPreviewLength(string siteName, PinSideRole role, float length)
    {
        ShapePreset preset = _presets[_selectedPreset];
        CarrierPinSite site = FindSite(preset, siteName);
        Axis current = site.Applied;
        long magnitude = Math.Max(0L, (long)Math.Round((length - 20f) / 16f));
        long currentSignedValue = role == PinSideRole.Recessive
            ? current.Recessive.Dominant
            : current.Dominant.Dominant;
        int sign = Math.Sign(currentSignedValue);
        if (sign == 0 && magnitude > 0)
        {
            // When a side is exactly zero, dragging its handle outward should be able to
            // re-establish a positive local value instead of getting trapped at zero.
            sign = 1;
        }

        long signedMagnitude = magnitude * sign;

        Axis updated = role == PinSideRole.Recessive
            ? new Axis(signedMagnitude, current.Recessive.Recessive, current.Dominant.Dominant, current.Dominant.Recessive)
            : new Axis(current.Recessive.Dominant, current.Recessive.Recessive, signedMagnitude, current.Dominant.Recessive);

        UpdateSiteAxis(_selectedPreset, siteName, updated);
    }

    private static SKPoint UpdateConstrainedStemPoint(SKPoint current, SKPoint pixelPoint, SKPoint axisStart, SKPoint axisEnd, SKRect? boundsRect)
    {
        if (boundsRect is not SKRect bounds)
        {
            return current;
        }

        if (!TryProjectToAxis(pixelPoint, axisStart, axisEnd, out float t))
        {
            return current;
        }

        SKPoint point = new(
            Lerp(axisStart.X, axisEnd.X, t),
            Lerp(axisStart.Y, axisEnd.Y, t));
        return ToNormalized(bounds, point);
    }

    private HandleLayout? HitHandle(SKPoint point)
    {
        const float threshold = 18f;
        return _handleLayouts
            .Where(layout => Distance(point, layout.Center) <= threshold)
            .OrderBy(layout => layout.Kind switch
            {
                HandleKind.Site => 0,
                HandleKind.Ray => 1,
                _ => 2,
            })
            .ThenBy(layout => Distance(point, layout.Center))
            .FirstOrDefault();
    }

    private void RegisterHandle(
        DragHandleKind target,
        SKPoint center,
        SKPoint axisStart,
        SKPoint axisEnd,
        string? siteName = null,
        string? carrierKey = null) =>
        _handleLayouts.Add(new HandleLayout(target, center, axisStart, axisEnd, null, false, HandleKind.Site, siteName, null, carrierKey));

    private void RegisterRayHandle(
        DragHandleKind target,
        SKPoint center,
        SKPoint axisStart,
        SKPoint axisEnd,
        string? siteName = null,
        PinSideRole? sideRole = null,
        string? carrierKey = null) =>
        _handleLayouts.Add(new HandleLayout(target, center, axisStart, axisEnd, null, false, HandleKind.Ray, siteName, sideRole, carrierKey));

    private void RegisterStemHandle(DragHandleKind target, SKPoint center, SKRect sceneRect, LetterboxEdge edge)
    {
        if (_showLetterbox)
        {
            var guide = GetLetterboxRect(sceneRect);
            (SKPoint axisStart, SKPoint axisEnd) = edge switch
            {
                LetterboxEdge.Left => (new SKPoint(guide.Left, guide.Top), new SKPoint(guide.Left, guide.Bottom)),
                LetterboxEdge.Right => (new SKPoint(guide.Right, guide.Top), new SKPoint(guide.Right, guide.Bottom)),
                LetterboxEdge.Top => (new SKPoint(guide.Left, guide.Top), new SKPoint(guide.Right, guide.Top)),
                LetterboxEdge.Bottom => (new SKPoint(guide.Left, guide.Bottom), new SKPoint(guide.Right, guide.Bottom)),
                _ => (center, center),
            };
            _handleLayouts.Add(new HandleLayout(target, center, axisStart, axisEnd, sceneRect, false, HandleKind.Endpoint));
            return;
        }

        _handleLayouts.Add(new HandleLayout(target, center, SKPoint.Empty, SKPoint.Empty, sceneRect, true, HandleKind.Endpoint));
    }

    private static bool TryProjectToAxis(SKPoint point, SKPoint start, SKPoint end, out float t)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float lengthSquared = dx * dx + dy * dy;
        if (lengthSquared < 0.001f)
        {
            t = 0f;
            return false;
        }

        t = ((point.X - start.X) * dx + (point.Y - start.Y) * dy) / lengthSquared;
        t = Math.Clamp(t, 0f, 1f);
        return true;
    }

    private static float Distance(SKPoint left, SKPoint right)
    {
        float dx = right.X - left.X;
        float dy = right.Y - left.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    private static float ClampOrdered(float value, float min, float max)
    {
        if (max < min)
        {
            max = min;
        }

        return Math.Clamp(value, min, max);
    }

    private static float Lerp(float start, float end, float t) => start + ((end - start) * t);

    private static float NormalizeBetween(float value, float start, float end)
    {
        float span = end - start;
        if (Math.Abs(span) < 0.0001f)
        {
            return 0f;
        }

        return Math.Clamp((value - start) / span, 0f, 1f);
    }

    private static string ShortRole(PinSideRole role) => role == PinSideRole.Recessive ? "r" : "d";

    private static string FormatJunctionSummary(CarrierJunctionSummary summary) => summary switch
    {
        CarrierJunctionSummary.Open => "Open",
        CarrierJunctionSummary.Cusp => "Cusp",
        CarrierJunctionSummary.Branch => "Branch",
        CarrierJunctionSummary.Tee => "T",
        CarrierJunctionSummary.Cross => "Cross",
        _ => summary.ToString(),
    };

    private static SKColor WithAlpha(SKColor color, byte alpha) => new(color.Red, color.Green, color.Blue, alpha);

    private float GetPreviewRayLength(DragHandleKind handleKind, float fallback, string? siteName = null)
    {
        if ((handleKind == DragHandleKind.CustomRecessive || handleKind == DragHandleKind.CustomDominant) &&
            siteName is not null &&
            TryGetCustomPin(_selectedPreset, siteName, out var customPin) &&
            customPin is not null)
        {
            Axis axis = customPin.Axis;
            long magnitude = handleKind == DragHandleKind.CustomRecessive
                ? Math.Abs(axis.Recessive.Dominant)
                : Math.Abs(axis.Dominant.Dominant);
            return Math.Clamp(20f + (magnitude * 16f), 8f, MaxPreviewRayLength);
        }

        return handleKind switch
        {
            DragHandleKind.DTopRecessive => _dTopRecessiveLength,
            DragHandleKind.DTopDominant => _dTopDominantLength,
            DragHandleKind.DBottomRecessive => _dBottomRecessiveLength,
            DragHandleKind.DBottomDominant => _dBottomDominantLength,
            DragHandleKind.HLeftRecessive => _hLeftRecessiveLength,
            DragHandleKind.HLeftDominant => _hLeftDominantLength,
            DragHandleKind.HRightRecessive => _hRightRecessiveLength,
            DragHandleKind.HRightDominant => _hRightDominantLength,
            DragHandleKind.TBaseDominant => _tBaseDominantLength,
            DragHandleKind.TCrossbarRecessive => _tCrossbarRecessiveLength,
            DragHandleKind.TCrossbarDominant => _tCrossbarDominantLength,
            DragHandleKind.YJunctionRecessive => _yJunctionRecessiveLength,
            DragHandleKind.YJunctionDominant => _yJunctionDominantLength,
            DragHandleKind.LCornerRecessive => _lCornerRecessiveLength,
            DragHandleKind.LCornerDominant => _lCornerDominantLength,
            DragHandleKind.ALeftRecessive => _aLeftRecessiveLength,
            DragHandleKind.ALeftDominant => _aLeftDominantLength,
            DragHandleKind.ARightRecessive => _aRightRecessiveLength,
            DragHandleKind.ARightDominant => _aRightDominantLength,
            DragHandleKind.MLeftRecessive => _mLeftRecessiveLength,
            DragHandleKind.MLeftDominant => _mLeftDominantLength,
            DragHandleKind.MRightRecessive => _mRightRecessiveLength,
            DragHandleKind.MRightDominant => _mRightDominantLength,
            _ => fallback,
        };
    }

    private DragHandleKind ResolveRayHandle(string? siteName, PinSideRole role) => (siteName, role) switch
    {
        ("Base", PinSideRole.Dominant) => DragHandleKind.TBaseDominant,
        ("Crossbar", PinSideRole.Recessive) => DragHandleKind.TCrossbarRecessive,
        ("Crossbar", PinSideRole.Dominant) => DragHandleKind.TCrossbarDominant,
        ("Top", PinSideRole.Recessive) => DragHandleKind.DTopRecessive,
        ("Top", PinSideRole.Dominant) => DragHandleKind.DTopDominant,
        ("Bottom", PinSideRole.Recessive) => DragHandleKind.DBottomRecessive,
        ("Bottom", PinSideRole.Dominant) => DragHandleKind.DBottomDominant,
        ("Left Join", PinSideRole.Recessive) => DragHandleKind.HLeftRecessive,
        ("Left Join", PinSideRole.Dominant) => DragHandleKind.HLeftDominant,
        ("Right Join", PinSideRole.Recessive) => DragHandleKind.HRightRecessive,
        ("Right Join", PinSideRole.Dominant) => DragHandleKind.HRightDominant,
        ("Junction", PinSideRole.Recessive) => DragHandleKind.YJunctionRecessive,
        ("Junction", PinSideRole.Dominant) => DragHandleKind.YJunctionDominant,
        ("Corner", PinSideRole.Recessive) => DragHandleKind.LCornerRecessive,
        ("Corner", PinSideRole.Dominant) => DragHandleKind.LCornerDominant,
        ("Left Bar", PinSideRole.Recessive) => DragHandleKind.ALeftRecessive,
        ("Left Bar", PinSideRole.Dominant) => DragHandleKind.ALeftDominant,
        ("Right Bar", PinSideRole.Recessive) => DragHandleKind.ARightRecessive,
        ("Right Bar", PinSideRole.Dominant) => DragHandleKind.ARightDominant,
        ("Left Peak", PinSideRole.Recessive) => DragHandleKind.MLeftRecessive,
        ("Left Peak", PinSideRole.Dominant) => DragHandleKind.MLeftDominant,
        ("Right Peak", PinSideRole.Recessive) => DragHandleKind.MRightRecessive,
        ("Right Peak", PinSideRole.Dominant) => DragHandleKind.MRightDominant,
        _ => DragHandleKind.None,
    };

    private static string GetPresetButtonText(PresetKind preset) => preset switch
    {
        PresetKind.CapitalD => "D",
        PresetKind.BridgeH => "H",
        PresetKind.LetterT => "T",
        PresetKind.LetterA => "A",
        PresetKind.LetterY => "Y",
        PresetKind.LetterL => "L",
        PresetKind.LetterM => "M",
        _ => preset.ToString(),
    };

    private static IReadOnlyDictionary<PresetKind, ShapePreset> BuildPresets()
    {
        var d = BuildCapitalDPreset();
        var h = BuildBridgeHPreset();
        var t = BuildTPreset();
        var a = BuildAPreset();
        var y = BuildYPreset();
        var l = BuildLPreset();
        var m = BuildMPreset();
        return new Dictionary<PresetKind, ShapePreset>
        {
            [PresetKind.CapitalD] = d,
            [PresetKind.BridgeH] = h,
            [PresetKind.LetterT] = t,
            [PresetKind.LetterA] = a,
            [PresetKind.LetterY] = y,
            [PresetKind.LetterL] = l,
            [PresetKind.LetterM] = m,
        };
    }

    private static ShapePreset BuildCapitalDPreset()
    {
        CarrierIdentity stem = CarrierIdentity.Create("Stem");
        CarrierIdentity bowl = CarrierIdentity.Create("Bowl");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        CarrierPinSite top = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(-3, 1, 2, -1), Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.Zero),
            name: "Top");
        CarrierPinSite bottom = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, 1, 2, -1), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.One),
            name: "Bottom");

        CarrierPinGraph graph = new([stem, bowl], [top, bottom]);
        return new ShapePreset(
            "Capital D",
            "Two pin sites live on one stem carrier. Their dominant sides both bind onto one shared bowl carrier, so the bowl is one carrier with two attachment positions rather than two separate emitted segments.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [stem.Id] = SegmentColors.Blue.Solid,
                [bowl.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildBridgeHPreset()
    {
        CarrierIdentity leftStem = CarrierIdentity.Create("Left Stem");
        CarrierIdentity rightStem = CarrierIdentity.Create("Right Stem");
        CarrierIdentity bridge = CarrierIdentity.Create("Bridge");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        Proportion midpoint = new Proportion(5);

        CarrierPinSite leftJoin = CarrierPinSite.FromPointPinning(
            leftStem,
            host.PinAt(new Axis(-2, 1, 3, -1), midpoint),
            new CarrierSideAttachment(PinSideRole.Recessive, leftStem, new Proportion(1, 2)),
            new CarrierSideAttachment(PinSideRole.Dominant, bridge, Proportion.Zero),
            name: "Left Join");
        CarrierPinSite rightJoin = CarrierPinSite.FromPointPinning(
            rightStem,
            host.PinAt(new Axis(2, 1, -3, -1), midpoint),
            new CarrierSideAttachment(PinSideRole.Recessive, rightStem, new Proportion(1, 2)),
            new CarrierSideAttachment(PinSideRole.Dominant, bridge, Proportion.One),
            name: "Right Join");

        CarrierPinGraph graph = new([leftStem, rightStem, bridge], [leftJoin, rightJoin]);
        return new ShapePreset(
            "Bridge H",
            "A bridge carrier is shared across two midpoint pin sites living on different host stems. This is still one carrier identity with two attachments, even though the planar fold preview draws it as a horizontal bar.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [leftStem.Id] = SegmentColors.Blue.Solid,
                [rightStem.Id] = SegmentColors.Purple.Solid,
                [bridge.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildTPreset()
    {
        CarrierIdentity guide = CarrierIdentity.Create("Guide");
        CarrierIdentity stem = CarrierIdentity.Create("Stem");
        CarrierIdentity bar = CarrierIdentity.Create("Bar");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        CarrierPinSite bottom = CarrierPinSite.FromPointPinning(
            guide,
            host.PinAt(new Axis(0, 0, 4, -2), new Proportion(5)),
            dominantAttachment: new CarrierSideAttachment(PinSideRole.Dominant, stem, Proportion.Zero),
            name: "Base");
        CarrierPinSite top = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), Proportion.One),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "Crossbar");

        CarrierPinGraph graph = new([guide, stem, bar], [bottom, top]);
        return new ShapePreset(
            "Capital T",
            "A base pin on the bottom guide emits the stem upward. A second pin at the stem endpoint emits the shared crossbar carrier in both directions.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [guide.Id] = new SKColor(160, 160, 160),
                [stem.Id] = SegmentColors.Blue.Solid,
                [bar.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildLPreset()
    {
        CarrierIdentity stem = CarrierIdentity.Create("Stem");
        CarrierIdentity foot = CarrierIdentity.Create("Foot");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        CarrierPinSite corner = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, 1, 3, -1), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, foot, Proportion.Zero),
            name: "Corner");

        CarrierPinGraph graph = new([stem, foot], [corner]);
        return new ShapePreset(
            "Capital L",
            "A single corner pin on the stem emits one foot carrier. It is the simplest one-join letter fold on the page.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [stem.Id] = SegmentColors.Blue.Solid,
                [foot.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildYPreset()
    {
        CarrierIdentity stem = CarrierIdentity.Create("Stem");
        CarrierIdentity fork = CarrierIdentity.Create("Fork");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        CarrierPinSite junction = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(2, 1, 3, -1), new Proportion(6)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, fork, new Proportion(1, 2)),
            name: "Junction");

        CarrierPinGraph graph = new([stem, fork], [junction]);
        return new ShapePreset(
            "Capital Y",
            "One junction pin on a stem emits a shared upper fork carrier. The preview folds that carrier into the two upper arms of a Y.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [stem.Id] = SegmentColors.Blue.Solid,
                [fork.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildAPreset()
    {
        CarrierIdentity leftLeg = CarrierIdentity.Create("Left Leg");
        CarrierIdentity rightLeg = CarrierIdentity.Create("Right Leg");
        CarrierIdentity crossbar = CarrierIdentity.Create("Crossbar");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        Proportion mid = new Proportion(9, 2);

        CarrierPinSite leftBar = CarrierPinSite.FromPointPinning(
            leftLeg,
            host.PinAt(new Axis(-2, 1, -2, -1), mid),
            new CarrierSideAttachment(PinSideRole.Recessive, leftLeg, mid),
            new CarrierSideAttachment(PinSideRole.Dominant, crossbar, Proportion.Zero),
            name: "Left Bar");
        CarrierPinSite rightBar = CarrierPinSite.FromPointPinning(
            rightLeg,
            host.PinAt(new Axis(2, 1, 2, -1), mid),
            new CarrierSideAttachment(PinSideRole.Recessive, rightLeg, mid),
            new CarrierSideAttachment(PinSideRole.Dominant, crossbar, Proportion.One),
            name: "Right Bar");

        CarrierPinGraph graph = new([leftLeg, rightLeg, crossbar], [leftBar, rightBar]);
        return new ShapePreset(
            "Capital A",
            "Two slanted leg carriers host a shared crossbar carrier. The apex is folded as a geometric display choice rather than a separate carrier here.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [leftLeg.Id] = SegmentColors.Blue.Solid,
                [rightLeg.Id] = SegmentColors.Purple.Solid,
                [crossbar.Id] = SegmentColors.Orange.Solid,
            });
    }

    private static ShapePreset BuildMPreset()
    {
        CarrierIdentity leftStem = CarrierIdentity.Create("Left Stem");
        CarrierIdentity rightStem = CarrierIdentity.Create("Right Stem");
        CarrierIdentity middle = CarrierIdentity.Create("Middle");
        Axis host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        Proportion top = new Proportion(1);

        CarrierPinSite leftPeak = CarrierPinSite.FromPointPinning(
            leftStem,
            host.PinAt(new Axis(3, 1, 3, -1), top),
            new CarrierSideAttachment(PinSideRole.Recessive, leftStem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, middle, Proportion.Zero),
            name: "Left Peak");
        CarrierPinSite rightPeak = CarrierPinSite.FromPointPinning(
            rightStem,
            host.PinAt(new Axis(-3, 1, -3, -1), top),
            new CarrierSideAttachment(PinSideRole.Recessive, rightStem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, middle, Proportion.One),
            name: "Right Peak");

        CarrierPinGraph graph = new([leftStem, rightStem, middle], [leftPeak, rightPeak]);
        return new ShapePreset(
            "Capital M",
            "Two outer stems host one shared middle carrier whose planar fold becomes the inner valley of the M.",
            graph,
            graph.Analyze(),
            new Dictionary<CarrierId, SKColor>
            {
                [leftStem.Id] = SegmentColors.Blue.Solid,
                [rightStem.Id] = SegmentColors.Purple.Solid,
                [middle.Id] = SegmentColors.Orange.Solid,
            });
    }

    private sealed record ShapePreset(
        string Name,
        string Description,
        CarrierPinGraph Graph,
        CarrierPinGraphAnalysis Analysis,
        IReadOnlyDictionary<CarrierId, SKColor> CarrierColors);

    private sealed record ButtonLayout(SKRect Rect, PresetKind Preset);
    private sealed record HandleLayout(
        DragHandleKind Target,
        SKPoint Center,
        SKPoint AxisStart,
        SKPoint AxisEnd,
        SKRect? BoundsRect = null,
        bool FreeMove = false,
        HandleKind Kind = HandleKind.Site,
        string? SiteName = null,
        PinSideRole? SideRole = null,
        string? CarrierKey = null);
    private sealed record ToggleLayout(SKRect Rect, SignToggleComponent Component);
    private sealed record CopyLayout(SKRect Rect, string Text);
    private sealed record ValueLayout(SKRect Rect, PresetKind Preset, string SiteName, string Text);
    private sealed record ActionLayout(SKRect Rect, SceneAction Action);
    private sealed record PreviewCarrierLayout(string Key, CarrierIdentity Carrier, string Label, IReadOnlyList<SKPoint> Samples);

    private sealed class CustomPreviewPin
    {
        public required string Name { get; init; }
        public required string CarrierKey { get; set; }
        public required CarrierIdentity HostCarrier { get; set; }
        public required float T { get; set; }
        public required Axis Axis { get; set; }
    }

    private enum PresetKind
    {
        CapitalD,
        BridgeH,
        LetterT,
        LetterA,
        LetterY,
        LetterL,
        LetterM,
    }

    private enum DragHandleKind
    {
        None,
        DStemTop,
        DTop,
        DBottom,
        DStemBottom,
        DTopRecessive,
        DTopDominant,
        DBottomRecessive,
        DBottomDominant,
        HLeftStemTop,
        HLeft,
        HLeftStemBottom,
        HLeftRecessive,
        HLeftDominant,
        HRightStemTop,
        HRight,
        HRightStemBottom,
        HRightRecessive,
        HRightDominant,
        TBase,
        TBaseDominant,
        TCrossbar,
        TCrossbarRecessive,
        TCrossbarDominant,
        YJunction,
        YJunctionRecessive,
        YJunctionDominant,
        LCorner,
        LCornerRecessive,
        LCornerDominant,
        ALeft,
        ALeftRecessive,
        ALeftDominant,
        ARight,
        ARightRecessive,
        ARightDominant,
        MLeft,
        MLeftRecessive,
        MLeftDominant,
        MRight,
        MRightRecessive,
        MRightDominant,
        CustomSite,
        CustomRecessive,
        CustomDominant,
    }

    private enum SignToggleComponent
    {
        RecessiveUnit,
        DominantValue,
        RecessiveValue,
        DominantUnit,
    }

    private enum LetterboxEdge
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    private enum HandleKind
    {
        Site,
        Ray,
        Endpoint,
    }

    private enum SceneAction
    {
        AddPin,
        Trash,
        CopyStructure,
    }
}
