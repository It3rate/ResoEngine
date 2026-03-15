using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphGrowthState(
    string LetterKey,
    IReadOnlyList<GlyphTip> ActiveTips,
    IReadOnlyList<GlyphCarrier> Carriers,
    IReadOnlyList<GlyphJunction> Junctions,
    IReadOnlyList<TensionPacket> Packets,
    int MacroStep,
    IReadOnlyList<GlyphAmbientSignal>? AmbientSignals = null,
    decimal ResidualTension = 0m,
    decimal LastAdjustment = 0m)
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

        var ambientSignals = spec.Environment.Landmarks
            .Select(landmark => new GlyphAmbientSignal(
                landmark.Key,
                landmark.Position,
                landmark.Kind switch
                {
                    GlyphLandmarkKind.BranchPoint => CouplingKind.Split,
                    GlyphLandmarkKind.StopPoint => CouplingKind.Stop,
                    GlyphLandmarkKind.Centerline => CouplingKind.Align,
                    _ => CouplingKind.Attract,
                },
                landmark.Strength * 0.55m,
                landmark.Kind switch
                {
                    GlyphLandmarkKind.Centerline => spec.Environment.Box.Width * 0.22m,
                    GlyphLandmarkKind.Midline => spec.Environment.Box.Width * 0.18m,
                    GlyphLandmarkKind.Capline => spec.Environment.Box.Width * 0.18m,
                    GlyphLandmarkKind.Baseline => spec.Environment.Box.Width * 0.16m,
                    _ => GlyphGrowthDefaults.JoinCaptureRadius,
                },
                0,
                landmark.Note))
            .Concat(
            [
                new GlyphAmbientSignal(
                    $"{spec.Key}:frame:left",
                    new GlyphVector(spec.Environment.Box.Left, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    0,
                    "Frame growth pressure from the left wall."),
                new GlyphAmbientSignal(
                    $"{spec.Key}:frame:right",
                    new GlyphVector(spec.Environment.Box.Right, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    0,
                    "Frame growth pressure from the right wall."),
                new GlyphAmbientSignal(
                    $"{spec.Key}:frame:cap",
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Top),
                    CouplingKind.Stop,
                    0.22m,
                    spec.Environment.Box.Width * 0.26m,
                    0,
                    "Cap boundary signal."),
                new GlyphAmbientSignal(
                    $"{spec.Key}:frame:baseline",
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Bottom),
                    CouplingKind.Stop,
                    0.18m,
                    spec.Environment.Box.Width * 0.24m,
                    0,
                    "Baseline boundary signal."),
            ])
            .ToArray();

        return new GlyphGrowthState(
            spec.Key,
            tips,
            [],
            junctions,
            packets,
            0,
            ambientSignals,
            ambientSignals.Sum(signal => signal.Magnitude),
            0m);
    }
}
