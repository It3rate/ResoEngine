using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphAmbientSignal(
    string Key,
    GlyphVector Position,
    CouplingKind Kind,
    decimal Magnitude,
    decimal Radius,
    int Age = 0,
    string? Note = null);
