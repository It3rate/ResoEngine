using SkiaSharp;

namespace ResoEngine.Visualizer.Core;

/// <summary>
/// Math ↔ pixel coordinate system. Y-axis is flipped (math up = pixel down).
/// </summary>
public class CoordinateSystem
{
    public float Width { get; }
    public float Height { get; }
    public float OriginX { get; }
    public float OriginY { get; }
    public float Scale { get; }

    public CoordinateSystem(
        float width = 720f, float height = 620f,
        float originX = 360f, float originY = 350f,
        float scale = 30f)
    {
        Width = width;
        Height = height;
        OriginX = originX;
        OriginY = originY;
        Scale = scale;
    }

    /// <summary>Convert math coordinates to pixel coordinates.</summary>
    public SKPoint MathToPixel(float mx, float my) =>
        new(OriginX + mx * Scale, OriginY - my * Scale);

    /// <summary>Convert pixel coordinates to math coordinates.</summary>
    public (float mx, float my) PixelToMath(float px, float py) =>
        ((px - OriginX) / Scale, (OriginY - py) / Scale);

    /// <summary>Convert a math distance to pixel distance.</summary>
    public float MathToPixelDist(float d) => d * Scale;
}
