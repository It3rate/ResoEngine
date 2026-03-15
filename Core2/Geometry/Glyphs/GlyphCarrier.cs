namespace Core2.Geometry.Glyphs;

public sealed record GlyphCarrier(
    string Key,
    GlyphVector Start,
    GlyphVector End,
    GlyphCarrierKind Kind = GlyphCarrierKind.Segment,
    bool IsCommitted = false,
    decimal Tension = 0m);
