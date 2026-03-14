namespace Core2.Geometry;

public readonly record struct StripPoint(int X, int Y)
{
    public static StripPoint operator +(StripPoint point, StripDelta delta) =>
        new(point.X + delta.Dx, point.Y + delta.Dy);
}
