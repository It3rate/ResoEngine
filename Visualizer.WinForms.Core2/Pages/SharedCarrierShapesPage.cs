using Core2.Elements;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

public sealed class SharedCarrierShapesPage : IVisualizerPage
{
    private readonly IReadOnlyDictionary<PresetKind, ShapePreset> _presets = BuildPresets();
    private readonly List<ButtonLayout> _buttonLayouts = [];
    private readonly List<HandleLayout> _handleLayouts = [];
    private readonly List<ToggleLayout> _toggleLayouts = [];
    private readonly Dictionary<(PresetKind Preset, string SiteName), Axis> _siteAxisOverrides = [];
    private readonly Dictionary<PresetKind, string> _selectedSiteByPreset = new()
    {
        [PresetKind.CapitalD] = "Top",
        [PresetKind.BridgeH] = "Left Join",
    };

    private CoordinateSystem? _coords;
    private SkiaCanvas? _canvasHost;
    private PresetKind _selectedPreset = PresetKind.CapitalD;
    private DragHandleKind _dragHandle;
    private float _dStemTopT;
    private float _dTopT;
    private float _dBottomT = 1f;
    private float _dStemBottomT = 1f;
    private float _dTopRecessiveLength = 68f;
    private float _dTopDominantLength = 52f;
    private float _dBottomRecessiveLength = 68f;
    private float _dBottomDominantLength = 52f;
    private float _hLeftStemTopT;
    private float _hLeftT = 0.5f;
    private float _hLeftStemBottomT = 1f;
    private float _hRightStemTopT;
    private float _hRightT = 0.5f;
    private float _hRightStemBottomT = 1f;
    private float _hLeftRecessiveLength = 52f;
    private float _hLeftDominantLength = 68f;
    private float _hRightRecessiveLength = 52f;
    private float _hRightDominantLength = 68f;

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
    private const float MaxPreviewRayLength = 140f;

    public string Title => "Shared Carrier Shapes";

    public void Init(CoordinateSystem coords, HitTestEngine hitTest, SkiaCanvas canvas)
    {
        _coords = coords;
        _canvasHost = canvas;
    }

    public void Render(SKCanvas canvas)
    {
        _buttonLayouts.Clear();
        _handleLayouts.Clear();
        _toggleLayouts.Clear();

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
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var toggle = _toggleLayouts.FirstOrDefault(layout => layout.Rect.Contains(pixelPoint));
        if (toggle is not null)
        {
            ToggleSelectedComponent(toggle.Component);
            _canvasHost?.InvalidateCanvas();
            return true;
        }

        var handle = HitHandle(pixelPoint);
        if (handle is null)
        {
            return false;
        }

        string? selectedSite = ResolveSelectedSite(handle.Target);
        if (selectedSite is not null)
        {
            _selectedSiteByPreset[_selectedPreset] = selectedSite;
        }

        _dragHandle = handle.Target;
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
            : _toggleLayouts.Any(layout => layout.Rect.Contains(pixelPoint))
                ? Cursors.Hand
            : HitHandle(pixelPoint) is not null
                ? Cursors.SizeAll
                : Cursors.Default;
    }

    public void OnPointerUp(SKPoint pixelPoint)
    {
        _dragHandle = DragHandleKind.None;
        if (_canvasHost is not null)
        {
            _canvasHost.Cursor = Cursors.Default;
        }
    }

    public void Destroy()
    {
        _buttonLayouts.Clear();
        _handleLayouts.Clear();
        _toggleLayouts.Clear();
        _coords = null;
        _canvasHost = null;
        _dragHandle = DragHandleKind.None;
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
        DrawBadge(canvas, new SKPoint(rect.Left + 20f, y), preset.Name);
        y += 44f;

        PageChrome.DrawWrappedText(
            canvas,
            preset.Description,
            rect.Left + 20f,
            ref y,
            rect.Width - 40f,
            _captionPaint);

        y += 14f;
        DrawBadge(canvas, new SKPoint(rect.Left + 20f, y), $"Carriers {preset.Graph.Carriers.Count}");
        DrawBadge(canvas, new SKPoint(rect.Left + 140f, y), $"Sites {preset.Graph.Sites.Count}");
        y += 48f;

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

        canvas.DrawText("Sites", rect.Left + 20f, y + 12f, _labelPaint);
        y += 40f;
        foreach (var site in preset.Graph.Sites.Select(ResolveSite))
        {
            string host = site.HostCarrier.Name ?? site.HostCarrier.Id.ToString();
            string bindings = string.Join(
                ", ",
                site.SideAttachments.Select(
                    attachment => $"{ShortRole(attachment.Role)}:{attachment.Carrier.Name ?? attachment.Carrier.Id.ToString()}@{FormatProportion(attachment.CarrierPosition)}"));
            string line = $"{site.Name ?? site.Id.ToString()} on {host}@{FormatProportion(site.HostPosition)}";
            canvas.DrawText(line, rect.Left + 20f, y, _monoPaint);
            y += _monoPaint.TextSize + 4f;
            PageChrome.DrawWrappedText(
                canvas,
                $"{bindings}  {FormatAxis(site.Applied)}",
                rect.Left + 28f,
                ref y,
                rect.Width - 56f,
                _captionPaint);
            y += 14f;
        }
    }

    private void DrawPresetButtons(SKCanvas canvas, float left, ref float y)
    {
        float buttonWidth = 104f;
        float buttonHeight = 34f;
        float gap = 10f;

        foreach (var preset in Enum.GetValues<PresetKind>())
        {
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
        DrawToggleLegend(canvas, inner, preset);

        switch (_selectedPreset)
        {
            case PresetKind.CapitalD:
                DrawCapitalDPreview(canvas, inner, preset);
                break;
            case PresetKind.BridgeH:
                DrawBridgeHPreview(canvas, inner, preset);
                break;
        }
    }

    private void DrawCapitalDPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        CarrierIdentity stem = preset.Graph.Carriers.First(carrier => carrier.Name == "Stem");
        CarrierIdentity bowl = preset.Graph.Carriers.First(carrier => carrier.Name == "Bowl");
        CarrierPinSite top = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Top"));
        CarrierPinSite bottom = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Bottom"));

        float leftX = rect.Left + 160f;
        float minY = rect.Top + 86f;
        float maxY = rect.Bottom - 86f;
        float stemTopY = Lerp(minY, maxY, _dStemTopT);
        float stemBottomY = Lerp(minY, maxY, _dStemBottomT);
        float topY = Lerp(stemTopY, stemBottomY, NormalizeBetween(_dTopT, _dStemTopT, _dStemBottomT));
        float bottomY = Lerp(stemTopY, stemBottomY, NormalizeBetween(_dBottomT, _dStemTopT, _dStemBottomT));
        float midY = (topY + bottomY) * 0.5f;

        SKPoint topPoint = new(leftX, topY);
        SKPoint bottomPoint = new(leftX, bottomY);
        SKPoint topEndpoint = new(leftX, stemTopY - EndpointHandleOffset);
        SKPoint bottomEndpoint = new(leftX, stemBottomY + EndpointHandleOffset);
        SKPoint hostTangent = new(0f, 1f);
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
        DrawCarrierLabel(canvas, stem.Name ?? "Stem", new SKPoint(leftX - 72f, midY), preset.CarrierColors[stem.Id]);
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
        DrawCarrierLabel(canvas, bowl.Name ?? "Bowl", new SKPoint(rect.Right - 176f, (topY + bottomY) * 0.5f), preset.CarrierColors[bowl.Id]);

        DrawStemEndpointHandle(canvas, topEndpoint, _dragHandle == DragHandleKind.DStemTop);
        DrawStemEndpointHandle(canvas, bottomEndpoint, _dragHandle == DragHandleKind.DStemBottom);
        DrawSite(canvas, top, "P1", topPoint, hostTangent, DragHandleKind.DTop);
        DrawSite(canvas, bottom, "P2", bottomPoint, hostTangent, DragHandleKind.DBottom);

        RegisterHandle(DragHandleKind.DStemTop, topEndpoint, new SKPoint(leftX, minY - EndpointHandleOffset), new SKPoint(leftX, maxY));
        RegisterHandle(DragHandleKind.DTop, topPoint, new SKPoint(leftX, stemTopY), new SKPoint(leftX, stemBottomY));
        RegisterHandle(DragHandleKind.DBottom, bottomPoint, new SKPoint(leftX, stemTopY), new SKPoint(leftX, stemBottomY));
        RegisterHandle(DragHandleKind.DStemBottom, bottomEndpoint, new SKPoint(leftX, minY), new SKPoint(leftX, maxY + EndpointHandleOffset));
    }

    private void DrawBridgeHPreview(SKCanvas canvas, SKRect rect, ShapePreset preset)
    {
        CarrierIdentity left = preset.Graph.Carriers.First(carrier => carrier.Name == "Left Stem");
        CarrierIdentity right = preset.Graph.Carriers.First(carrier => carrier.Name == "Right Stem");
        CarrierIdentity bridge = preset.Graph.Carriers.First(carrier => carrier.Name == "Bridge");
        CarrierPinSite leftJoin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Left Join"));
        CarrierPinSite rightJoin = ResolveSite(preset.Graph.Sites.First(site => site.Name == "Right Join"));

        float leftX = rect.Left + 154f;
        float rightX = rect.Right - 154f;
        float minY = rect.Top + 92f;
        float maxY = rect.Bottom - 92f;
        float leftStemTopY = Lerp(minY, maxY, _hLeftStemTopT);
        float leftStemBottomY = Lerp(minY, maxY, _hLeftStemBottomT);
        float rightStemTopY = Lerp(minY, maxY, _hRightStemTopT);
        float rightStemBottomY = Lerp(minY, maxY, _hRightStemBottomT);
        float leftMidY = Lerp(leftStemTopY, leftStemBottomY, NormalizeBetween(_hLeftT, _hLeftStemTopT, _hLeftStemBottomT));
        float rightMidY = Lerp(rightStemTopY, rightStemBottomY, NormalizeBetween(_hRightT, _hRightStemTopT, _hRightStemBottomT));

        DrawCarrierLine(canvas, new SKPoint(leftX, leftStemTopY), new SKPoint(leftX, leftStemBottomY), preset.CarrierColors[left.Id], 5.2f);
        DrawCarrierLine(canvas, new SKPoint(rightX, rightStemTopY), new SKPoint(rightX, rightStemBottomY), preset.CarrierColors[right.Id], 5.2f);

        SKPoint leftPoint = new(leftX, leftMidY);
        SKPoint rightPoint = new(rightX, rightMidY);
        SKPoint hostTangent = new(0f, 1f);
        DrawSharedCarrierCurve(
            canvas,
            leftPoint,
            leftJoin,
            bridge.Id,
            hostTangent,
            rightPoint,
            rightJoin,
            bridge.Id,
            hostTangent,
            preset.CarrierColors[bridge.Id],
            5.2f);

        float bridgeMidY = (leftMidY + rightMidY) * 0.5f;
        DrawCarrierLabel(canvas, left.Name ?? "Left", new SKPoint(leftX - 84f, leftMidY - 6f), preset.CarrierColors[left.Id]);
        DrawCarrierLabel(canvas, bridge.Name ?? "Bridge", new SKPoint(rect.MidX - 24f, bridgeMidY - 18f), preset.CarrierColors[bridge.Id]);
        DrawCarrierLabel(canvas, right.Name ?? "Right", new SKPoint(rightX + 18f, rightMidY - 6f), preset.CarrierColors[right.Id]);

        DrawStemEndpointHandle(canvas, new SKPoint(leftX, leftStemTopY - EndpointHandleOffset), _dragHandle == DragHandleKind.HLeftStemTop);
        DrawStemEndpointHandle(canvas, new SKPoint(leftX, leftStemBottomY + EndpointHandleOffset), _dragHandle == DragHandleKind.HLeftStemBottom);
        DrawStemEndpointHandle(canvas, new SKPoint(rightX, rightStemTopY - EndpointHandleOffset), _dragHandle == DragHandleKind.HRightStemTop);
        DrawStemEndpointHandle(canvas, new SKPoint(rightX, rightStemBottomY + EndpointHandleOffset), _dragHandle == DragHandleKind.HRightStemBottom);
        DrawSite(canvas, leftJoin, "P1", leftPoint, hostTangent, DragHandleKind.HLeft);
        DrawSite(canvas, rightJoin, "P2", rightPoint, hostTangent, DragHandleKind.HRight);

        RegisterHandle(DragHandleKind.HLeftStemTop, new SKPoint(leftX, leftStemTopY - EndpointHandleOffset), new SKPoint(leftX, minY), new SKPoint(leftX, maxY));
        RegisterHandle(DragHandleKind.HLeft, leftPoint, new SKPoint(leftX, leftStemTopY), new SKPoint(leftX, leftStemBottomY));
        RegisterHandle(DragHandleKind.HLeftStemBottom, new SKPoint(leftX, leftStemBottomY + EndpointHandleOffset), new SKPoint(leftX, minY), new SKPoint(leftX, maxY));
        RegisterHandle(DragHandleKind.HRightStemTop, new SKPoint(rightX, rightStemTopY - EndpointHandleOffset), new SKPoint(rightX, minY), new SKPoint(rightX, maxY));
        RegisterHandle(DragHandleKind.HRight, rightPoint, new SKPoint(rightX, rightStemTopY), new SKPoint(rightX, rightStemBottomY));
        RegisterHandle(DragHandleKind.HRightStemBottom, new SKPoint(rightX, rightStemBottomY + EndpointHandleOffset), new SKPoint(rightX, minY), new SKPoint(rightX, maxY));
    }

    private void DrawSite(SKCanvas canvas, CarrierPinSite site, string label, SKPoint point, SKPoint hostTangent, DragHandleKind handleKind)
    {
        bool selected = IsSelectedSite(site);
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

        DrawSiteRay(canvas, point, hostTangent, site, PinSideRole.Recessive, RecessiveRayColor);
        DrawSiteRay(canvas, point, hostTangent, site, PinSideRole.Dominant, DominantRayColor);
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

    private void DrawSiteRay(SKCanvas canvas, SKPoint origin, SKPoint hostTangent, CarrierPinSite site, PinSideRole role, SKColor color)
    {
        if (!TryResolveSideDirection(site, role, hostTangent, out SKPoint direction, out float magnitude))
        {
            return;
        }
        float length = GetPreviewRayLength(ResolveRayHandle(site.Name, role), Math.Clamp(20f + magnitude * 16f, 24f, 104f));
        SKPoint end = new(origin.X + direction.X * length, origin.Y + direction.Y * length);

        if (role == PinSideRole.Recessive)
        {
            DrawDotSegment(canvas, origin, end, color, 4.4f);
        }
        else
        {
            DrawArrowSegment(canvas, origin, end, color, 4.4f);
        }

        DragHandleKind handle = ResolveRayHandle(site.Name, role);
        if (handle != DragHandleKind.None)
        {
            if (_dragHandle == handle)
            {
                canvas.DrawCircle(end, 16f, _handleHaloFillPaint);
                canvas.DrawCircle(end, 16f, _handleHaloStrokePaint);
            }
            else
            {
                canvas.DrawCircle(end, 14f, _handleHaloFillPaint);
            }

            RegisterHandle(handle, end, origin, new SKPoint(origin.X + direction.X * MaxPreviewRayLength, origin.Y + direction.Y * MaxPreviewRayLength));
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
        if (!TryResolveAttachmentDirection(startSite, carrierId, startHostTangent, out SKPoint startDirection, out float startMagnitude, out PinSideRole startRole) ||
            !TryResolveAttachmentDirection(endSite, endCarrierId, endHostTangent, out SKPoint endDirection, out float endMagnitude, out PinSideRole endRole))
        {
            DrawCarrierLine(canvas, startPoint, endPoint, color, strokeWidth);
            return;
        }

        float startHandle = GetPreviewRayLength(ResolveRayHandle(startSite.Name, startRole), Math.Clamp(20f + startMagnitude * 16f, 24f, 104f));
        float endHandle = GetPreviewRayLength(ResolveRayHandle(endSite.Name, endRole), Math.Clamp(20f + endMagnitude * 16f, 24f, 104f));
        // The preview treats both local rays as outward construction directions,
        // so the shared carrier should sit on the same side of each endpoint
        // rather than using a single start->end path orientation.
        SKPoint control1 = new(startPoint.X + startDirection.X * startHandle, startPoint.Y + startDirection.Y * startHandle);
        SKPoint control2 = new(endPoint.X + endDirection.X * endHandle, endPoint.Y + endDirection.Y * endHandle);

        using var path = new SKPath();
        path.MoveTo(startPoint);
        path.CubicTo(control1, control2, endPoint);
        using var paint = CreateStrokePaint(WithAlpha(color, CarrierPreviewAlpha), strokeWidth);
        canvas.DrawPath(path, paint);
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

    private static bool TryResolveAttachmentDirection(
        CarrierPinSite site,
        CarrierId carrierId,
        SKPoint hostTangent,
        out SKPoint direction,
        out float magnitude,
        out PinSideRole role)
    {
        var attachment = site.SideAttachments.FirstOrDefault(candidate => candidate.CarrierId == carrierId);
        if (attachment is null)
        {
            direction = SKPoint.Empty;
            magnitude = 0f;
            role = PinSideRole.Dominant;
            return false;
        }

        role = attachment.Role;
        return TryResolveSideDirection(site, role, hostTangent, out direction, out magnitude);
    }

    private void DrawCarrierLine(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = CreateStrokePaint(WithAlpha(color, CarrierPreviewAlpha), strokeWidth);
        canvas.DrawLine(start, end, paint);
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

    private CarrierPinSite ResolveSite(CarrierPinSite site)
    {
        if (site.Name is null ||
            !_siteAxisOverrides.TryGetValue((_selectedPreset, site.Name), out var applied))
        {
            return site;
        }

        return new CarrierPinSite(site.Id, site.HostCarrier, new PointPinning<Axis, Axis>(site.Host, applied, site.HostPosition), site.SideAttachments, site.Name);
    }

    private CarrierPinSite GetSelectedSite(ShapePreset preset)
    {
        string selectedName = _selectedSiteByPreset.TryGetValue(_selectedPreset, out var name)
            ? name
            : preset.Graph.Sites.First().Name ?? preset.Graph.Sites.First().Id.ToString();
        CarrierPinSite site = preset.Graph.Sites.FirstOrDefault(candidate => candidate.Name == selectedName) ?? preset.Graph.Sites.First();
        return ResolveSite(site);
    }

    private bool IsSelectedSite(CarrierPinSite site) =>
        site.Name is not null &&
        _selectedSiteByPreset.TryGetValue(_selectedPreset, out var selectedName) &&
        string.Equals(selectedName, site.Name, StringComparison.Ordinal);

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

        _siteAxisOverrides[(_selectedPreset, selectedSite.Name)] = updated;
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
        _ => null,
    };

    private void UpdateDragHandle(SKPoint pixelPoint)
    {
        var handle = _handleLayouts.FirstOrDefault(layout => layout.Target == _dragHandle);
        if (handle is null || !TryProjectToAxis(pixelPoint, handle.AxisStart, handle.AxisEnd, out float t))
        {
            return;
        }

        switch (_dragHandle)
        {
            case DragHandleKind.DStemTop:
                _dStemTopT = ClampOrdered(t, 0f, Math.Max(0f, Math.Min(_dTopT, _dBottomT) - 0.08f));
                break;
            case DragHandleKind.DTop:
                _dTopT = ClampOrdered(t, _dStemTopT + 0.08f, _dBottomT - 0.08f);
                break;
            case DragHandleKind.DBottom:
                _dBottomT = ClampOrdered(t, _dTopT + 0.08f, _dStemBottomT - 0.08f);
                break;
            case DragHandleKind.DStemBottom:
                _dStemBottomT = ClampOrdered(t, Math.Min(1f, Math.Max(_dTopT, _dBottomT) + 0.08f), 1f);
                break;
            case DragHandleKind.HLeftStemTop:
                _hLeftStemTopT = ClampOrdered(t, 0f, Math.Max(0f, _hLeftT - 0.08f));
                break;
            case DragHandleKind.HLeft:
                _hLeftT = ClampOrdered(t, _hLeftStemTopT + 0.08f, _hLeftStemBottomT - 0.08f);
                break;
            case DragHandleKind.HLeftStemBottom:
                _hLeftStemBottomT = ClampOrdered(t, Math.Min(1f, _hLeftT + 0.08f), 1f);
                break;
            case DragHandleKind.HRightStemTop:
                _hRightStemTopT = ClampOrdered(t, 0f, Math.Max(0f, _hRightT - 0.08f));
                break;
            case DragHandleKind.HRight:
                _hRightT = ClampOrdered(t, _hRightStemTopT + 0.08f, _hRightStemBottomT - 0.08f);
                break;
            case DragHandleKind.HRightStemBottom:
                _hRightStemBottomT = ClampOrdered(t, Math.Min(1f, _hRightT + 0.08f), 1f);
                break;
            case DragHandleKind.DTopRecessive:
                _dTopRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.DTopDominant:
                _dTopDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.DBottomRecessive:
                _dBottomRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.DBottomDominant:
                _dBottomDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.HLeftRecessive:
                _hLeftRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.HLeftDominant:
                _hLeftDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.HRightRecessive:
                _hRightRecessiveLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
            case DragHandleKind.HRightDominant:
                _hRightDominantLength = Math.Clamp(t * MaxPreviewRayLength, 8f, MaxPreviewRayLength);
                break;
        }
    }

    private HandleLayout? HitHandle(SKPoint point)
    {
        const float threshold = 18f;
        return _handleLayouts.FirstOrDefault(layout => Distance(point, layout.Center) <= threshold);
    }

    private void RegisterHandle(DragHandleKind target, SKPoint center, SKPoint axisStart, SKPoint axisEnd) =>
        _handleLayouts.Add(new HandleLayout(target, center, axisStart, axisEnd));

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

    private static SKColor WithAlpha(SKColor color, byte alpha) => new(color.Red, color.Green, color.Blue, alpha);

    private float GetPreviewRayLength(DragHandleKind handleKind, float fallback) => handleKind switch
    {
        DragHandleKind.DTopRecessive => _dTopRecessiveLength,
        DragHandleKind.DTopDominant => _dTopDominantLength,
        DragHandleKind.DBottomRecessive => _dBottomRecessiveLength,
        DragHandleKind.DBottomDominant => _dBottomDominantLength,
        DragHandleKind.HLeftRecessive => _hLeftRecessiveLength,
        DragHandleKind.HLeftDominant => _hLeftDominantLength,
        DragHandleKind.HRightRecessive => _hRightRecessiveLength,
        DragHandleKind.HRightDominant => _hRightDominantLength,
        _ => fallback,
    };

    private DragHandleKind ResolveRayHandle(string? siteName, PinSideRole role) => (siteName, role) switch
    {
        ("Top", PinSideRole.Recessive) => DragHandleKind.DTopRecessive,
        ("Top", PinSideRole.Dominant) => DragHandleKind.DTopDominant,
        ("Bottom", PinSideRole.Recessive) => DragHandleKind.DBottomRecessive,
        ("Bottom", PinSideRole.Dominant) => DragHandleKind.DBottomDominant,
        ("Left Join", PinSideRole.Recessive) => DragHandleKind.HLeftRecessive,
        ("Left Join", PinSideRole.Dominant) => DragHandleKind.HLeftDominant,
        ("Right Join", PinSideRole.Recessive) => DragHandleKind.HRightRecessive,
        ("Right Join", PinSideRole.Dominant) => DragHandleKind.HRightDominant,
        _ => DragHandleKind.None,
    };

    private static string GetPresetButtonText(PresetKind preset) => preset switch
    {
        PresetKind.CapitalD => "D Bowl",
        PresetKind.BridgeH => "H Bridge",
        _ => preset.ToString(),
    };

    private static IReadOnlyDictionary<PresetKind, ShapePreset> BuildPresets()
    {
        var d = BuildCapitalDPreset();
        var h = BuildBridgeHPreset();
        return new Dictionary<PresetKind, ShapePreset>
        {
            [PresetKind.CapitalD] = d,
            [PresetKind.BridgeH] = h,
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

    private sealed record ShapePreset(
        string Name,
        string Description,
        CarrierPinGraph Graph,
        CarrierPinGraphAnalysis Analysis,
        IReadOnlyDictionary<CarrierId, SKColor> CarrierColors);

    private sealed record ButtonLayout(SKRect Rect, PresetKind Preset);
    private sealed record HandleLayout(DragHandleKind Target, SKPoint Center, SKPoint AxisStart, SKPoint AxisEnd);
    private sealed record ToggleLayout(SKRect Rect, SignToggleComponent Component);

    private enum PresetKind
    {
        CapitalD,
        BridgeH,
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
    }

    private enum SignToggleComponent
    {
        RecessiveUnit,
        DominantValue,
        RecessiveValue,
        DominantUnit,
    }
}
