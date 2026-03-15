using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record PointGlyphFieldEmitter(
    string Key,
    GlyphVector Origin,
    decimal Radius,
    IReadOnlyList<CouplingRule> Couplings,
    decimal BaseStrength = 1m,
    GlyphFieldFalloff Falloff = GlyphFieldFalloff.Linear,
    string? Note = null)
    : GlyphFieldEmitter(Key, Couplings, BaseStrength, Falloff, Note)
{
    public override decimal SampleAt(GlyphVector point) =>
        ApplyFalloff(Origin.DistanceTo(point), Radius);
}
