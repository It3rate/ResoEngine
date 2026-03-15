namespace Core2.Geometry.Glyphs;

public readonly record struct GlyphTensionFieldSample(
    decimal Grow,
    decimal Stop,
    decimal Branch,
    GlyphVector Flow)
{
    public decimal Energy => Grow + Stop + Branch;
}
