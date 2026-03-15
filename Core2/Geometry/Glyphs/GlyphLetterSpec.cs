namespace Core2.Geometry.Glyphs;

public sealed record GlyphLetterSpec(
    string Key,
    string DisplayName,
    string Description,
    GlyphEnvironment Environment,
    IReadOnlyList<GlyphSeed> Seeds);
