namespace Core2.Geometry.Glyphs;

public sealed record GlyphGrowthBranch(
    string Key,
    GlyphVector Direction,
    decimal Energy = 1m,
    string? Note = null);
