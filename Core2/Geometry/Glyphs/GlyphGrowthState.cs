using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphGrowthState(
    string LetterKey,
    IReadOnlyList<GlyphTip> ActiveTips,
    IReadOnlyList<GlyphCarrier> Carriers,
    IReadOnlyList<GlyphJunction> Junctions,
    IReadOnlyList<TensionPacket> Packets,
    int MacroStep)
{
    public bool HasActiveTips => ActiveTips.Any(tip => tip.IsActive);

    public static GlyphGrowthState FromSpec(GlyphLetterSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var tips = spec.Seeds
            .Where(seed => seed.Kind == GlyphSeedKind.Tip)
            .Select(seed => new GlyphTip(
                seed.Key,
                seed.Position,
                seed.PreferredDirection ?? GlyphVector.Zero,
                seed.Energy,
                IsActive: true,
                CarrierKey: null,
                seed.Note))
            .ToArray();

        var junctions = spec.Seeds
            .Where(seed => seed.Kind == GlyphSeedKind.Junction)
            .Select(seed => new GlyphJunction(
                seed.Key,
                seed.Position,
                GlyphJunctionKind.Seed,
                [],
                AllowsSplit: true,
                AllowsJoin: true,
                seed.Note))
            .ToArray();

        string frameKey = $"{spec.Key}:box";
        var packets = spec.Seeds
            .Select(seed => new TensionPacket(
                seed.Key,
                frameKey,
                0m,
                seed.Direction,
                seed.Energy))
            .ToArray();

        return new GlyphGrowthState(spec.Key, tips, [], junctions, packets, 0);
    }
}
