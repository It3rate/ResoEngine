namespace Core2.Geometry.Glyphs;

public readonly record struct GlyphVector(decimal X, decimal Y)
{
    public static GlyphVector Zero { get; } = new(0m, 0m);

    public decimal Length => DistanceTo(Zero);

    public decimal LengthSquared => X * X + Y * Y;

    public decimal DistanceTo(GlyphVector other)
    {
        decimal dx = other.X - X;
        decimal dy = other.Y - Y;
        return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
    }

    public GlyphVector Normalize()
    {
        decimal length = Length;
        if (length == 0m)
        {
            return Zero;
        }

        return new GlyphVector(X / length, Y / length);
    }

    public decimal Dot(GlyphVector other) => X * other.X + Y * other.Y;

    public static GlyphVector operator +(GlyphVector left, GlyphVector right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static GlyphVector operator -(GlyphVector left, GlyphVector right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static GlyphVector operator *(GlyphVector value, decimal scale) =>
        new(value.X * scale, value.Y * scale);
}
