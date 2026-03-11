using SkiaSharp;

namespace ResoEngine.Visualizer.Core;

/// <summary>Shared style constants.</summary>
public static class VisualStyle
{
    public const float StrokeWidth = 4f;
    public const float GridStrokeWidth = 0.8f;
    public const float HalfTickStrokeWidth = 0.4f;
    public static readonly float[] DashPattern = [8f, 6f];
    public const float CircleRadius = 7f;
    public const float DotRadius = 5f;
    public const float OriginDotRadius = 8f;
    public const float ArrowSize = 12f;
    public const float FontSize = 20f;
    public const string FontFamily = "Arial";
    public static readonly SKColor FillColor = SKColor.Parse("#FFFACD");
    public const float GridSpacing = 1f;
    public const float GridExtension = 1f;     // how far grid lines extend past segment ends
    public const float HitPadding = 16f;
    public const float SnapIncrement = 0.5f;
}
