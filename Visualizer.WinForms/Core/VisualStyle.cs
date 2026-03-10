using SkiaSharp;

namespace ResoEngine.Visualizer.Core;

/// <summary>Shared style constants matching the reference SVG.</summary>
public static class VisualStyle
{
    public const float StrokeWidth = 4f;
    public const float GridStrokeWidth = 2f;
    public static readonly float[] DashPattern = [8f, 6f];
    public const float CircleRadius = 7f;
    public const float DotRadius = 4.9f;       // CircleRadius * 0.7
    public const float ArrowSize = 12f;
    public const float FontSize = 22f;
    public const string FontFamily = "Arial";
    public static readonly SKColor FillColor = SKColor.Parse("#FFFACD");
    public const float GridSpacing = 1f;
    public const float HitPadding = 14f;
    public const float SnapIncrement = 0.5f;
}
