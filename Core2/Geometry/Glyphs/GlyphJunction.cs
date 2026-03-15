namespace Core2.Geometry.Glyphs;

public sealed record GlyphJunction(
    string Key,
    GlyphVector Position,
    GlyphJunctionKind Kind,
    IReadOnlyList<string> ConnectedKeys,
    bool AllowsSplit = true,
    bool AllowsJoin = true,
    string? Note = null);
