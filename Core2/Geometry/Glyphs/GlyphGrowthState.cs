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
            .Select(landmark => CreateLandmarkSignal(spec.Environment.Box, landmark))
            .Concat(
            [
                CreateFrameSignal(
                    $"{spec.Key}:frame:left",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.Left, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    "Frame growth pressure from the left wall."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:right",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.Right, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    "Frame growth pressure from the right wall."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:cap",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Top),
                    CouplingKind.Stop,
                    0.22m,
                    spec.Environment.Box.Width * 0.26m,
                    "Cap boundary signal."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:baseline",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Bottom),
                    CouplingKind.Stop,
                    0.18m,
                    spec.Environment.Box.Width * 0.24m,
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

    private static GlyphAmbientSignal CreateLandmarkSignal(
        GlyphBox box,
        GlyphLandmark landmark)
    {
        GlyphVector target = landmark.Position;
        GlyphVector origin = CreateSignalOrigin(box, target, landmark.Key);

        return new GlyphAmbientSignal(
            landmark.Key,
            origin,
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
                GlyphLandmarkKind.Centerline => box.Width * 0.22m,
                GlyphLandmarkKind.Midline => box.Width * 0.18m,
                GlyphLandmarkKind.Capline => box.Width * 0.18m,
                GlyphLandmarkKind.Baseline => box.Width * 0.16m,
                _ => GlyphGrowthDefaults.JoinCaptureRadius,
            },
            target,
            GlyphGrowthDefaults.SignalDriftStep,
            0,
            landmark.Note);
    }

    private static GlyphAmbientSignal CreateFrameSignal(
        string key,
        GlyphBox box,
        GlyphVector target,
        CouplingKind kind,
        decimal magnitude,
        decimal radius,
        string note)
    {
        return new GlyphAmbientSignal(
            key,
            CreateSignalOrigin(box, target, key),
            kind,
            magnitude,
            radius,
            target,
            GlyphGrowthDefaults.SignalDriftStep,
            0,
            note);
    }

    private static GlyphVector CreateSignalOrigin(
        GlyphBox box,
        GlyphVector target,
        string key)
    {
        GlyphVector toCenter = (box.Center - target).Normalize();
        if (toCenter == GlyphVector.Zero)
        {
            toCenter = new GlyphVector(0m, 1m);
        }

        GlyphVector tangent = new(-toCenter.Y, toCenter.X);
        decimal lateralSign = (Math.Abs(key.GetHashCode()) & 1) == 0 ? 1m : -1m;
        GlyphVector origin = target +
            (toCenter * GlyphGrowthDefaults.SignalSpawnInset) +
            (tangent * (GlyphGrowthDefaults.SignalSpawnInset * 0.35m * lateralSign));

        return new GlyphVector(
            decimal.Clamp(origin.X, box.Left, box.Right),
            decimal.Clamp(origin.Y, box.Bottom, box.Top));
    }
}
