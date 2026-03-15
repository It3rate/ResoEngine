using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphFieldInfluence(
    string EmitterKey,
    CouplingRule Rule,
    decimal Weight);
