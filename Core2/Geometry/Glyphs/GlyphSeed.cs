using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphSeed(
    string Key,
    GlyphSeedKind Kind,
    GlyphVector Position,
    decimal Energy = 1m,
    GlyphVector? PreferredDirection = null,
    PacketFlowDirection Direction = PacketFlowDirection.Forward,
    string? Note = null);
