using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record VerticalBandGlyphFieldEmitter(
    string Key,
    decimal X,
    decimal Radius,
    IReadOnlyList<CouplingRule> Couplings,
    decimal BaseStrength = 1m,
    GlyphFieldFalloff Falloff = GlyphFieldFalloff.Linear,
    string? Note = null)
    : GlyphFieldEmitter(Key, Couplings, BaseStrength, Falloff, Note)
{
    public override decimal SampleAt(GlyphVector point) =>
        ApplyFalloff(Math.Abs(point.X - X), Radius);
}
