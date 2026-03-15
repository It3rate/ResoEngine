namespace Core2.Geometry.Glyphs;

public readonly record struct GlyphBox(
    decimal Left,
    decimal Bottom,
    decimal Right,
    decimal Top)
{
    public decimal Width => Right - Left;

    public decimal Height => Top - Bottom;

    public GlyphVector Center => new((Left + Right) * 0.5m, (Bottom + Top) * 0.5m);

    public decimal MidX => (Left + Right) * 0.5m;

    public decimal MidY => (Bottom + Top) * 0.5m;

    public bool Contains(GlyphVector point) =>
        point.X >= Left && point.X <= Right &&
        point.Y >= Bottom && point.Y <= Top;
}
