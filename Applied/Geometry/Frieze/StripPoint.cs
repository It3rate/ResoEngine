using Applied.Geometry.Utils;

namespace Applied.Geometry.Frieze;

public readonly record struct StripPoint(int X, int Y)
{
    public static StripPoint operator +(StripPoint point, Directions2D delta) =>
        new(point.X + delta.Dx, point.Y + delta.Dy);
}
