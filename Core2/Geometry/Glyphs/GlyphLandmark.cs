namespace Core2.Geometry.Glyphs;

public sealed record GlyphLandmark(
    string Key,
    GlyphLandmarkKind Kind,
    GlyphVector Position,
    decimal Strength = 1m,
    string? Note = null);
