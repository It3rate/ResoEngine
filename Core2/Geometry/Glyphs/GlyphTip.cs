namespace Core2.Geometry.Glyphs;

public sealed record GlyphTip(
    string Key,
    GlyphVector Position,
    GlyphVector PreferredDirection,
    decimal Energy = 1m,
    bool IsActive = true,
    string? CarrierKey = null,
    string? Note = null);
