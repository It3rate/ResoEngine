using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record HorizontalBandGlyphFieldEmitter(
    string Key,
    decimal Y,
    decimal Radius,
    IReadOnlyList<CouplingRule> Couplings,
    decimal BaseStrength = 1m,
    GlyphFieldFalloff Falloff = GlyphFieldFalloff.Linear,
    string? Note = null)
    : GlyphFieldEmitter(Key, Couplings, BaseStrength, Falloff, Note)
{
    public override decimal SampleAt(GlyphVector point) =>
        ApplyFalloff(Math.Abs(point.Y - Y), Radius);
}
 