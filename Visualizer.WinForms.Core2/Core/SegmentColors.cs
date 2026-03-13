using SkiaSharp;

namespace ResoEngine.Visualizer.Core;

/// <summary>Color configuration for a segment and its grid lines.</summary>
public record SegmentColorSet(SKColor Solid, SKColor Grid, SKColor Label);

public static class SegmentColors
{
    public static readonly SegmentColorSet Red = new(
        SKColor.Parse("#C00000"),
        SKColor.Parse("#FFB3B3"),
        SKColor.Parse("#C00000"));

    public static readonly SegmentColorSet Blue = new(
        SKColor.Parse("#0020C0"),
        SKColor.Parse("#A6C8FF"),
        SKColor.Parse("#0020C0"));

    public static readonly SegmentColorSet Green = new(
        SKColor.Parse("#008040"),
        SKColor.Parse("#B3FFD9"),
        SKColor.Parse("#008040"));

    public static readonly SegmentColorSet Orange = new(
        SKColor.Parse("#C06000"),
        SKColor.Parse("#FFD9B3"),
        SKColor.Parse("#C06000"));

    public static readonly SegmentColorSet Purple = new(
        SKColor.Parse("#8000C0"),
        SKColor.Parse("#D9B3FF"),
        SKColor.Parse("#8000C0"));
}
