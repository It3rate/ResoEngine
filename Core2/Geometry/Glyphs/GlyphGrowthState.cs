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
    decimal LastAdjustment = 0m,
    GlyphTensionField? TensionField = null,
    int RandomSeed = 0)
{
    public bool HasActiveTips => ActiveTips.Any(tip => tip.IsActive);

    public static GlyphGrowthState FromSpec(GlyphLetterSpec spec, int randomSeed = 0)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var tips = spec.Seeds
            .Where(seed => seed.Kind == GlyphSeedKind.Tip)
            .Select(seed => new GlyphTip(
                seed.Key,
                Jitter(seed.Position, spec.Environment.Box, seed.Key, randomSeed, 4m),
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
                Jitter(seed.Position, spec.Environment.Box, seed.Key, randomSeed, 3m),
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
            .Select(landmark => CreateLandmarkSignal(spec.Environment.Box, landmark, randomSeed))
            .Concat(
            [
                CreateFrameSignal(
                    $"{spec.Key}:frame:left",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.Left, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    randomSeed,
                    "Frame growth pressure from the left wall."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:right",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.Right, spec.Environment.Box.MidY),
                    CouplingKind.Grow,
                    0.28m,
                    spec.Environment.Box.Width * 0.32m,
                    randomSeed,
                    "Frame growth pressure from the right wall."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:cap",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Top),
                    CouplingKind.Stop,
                    0.22m,
                    spec.Environment.Box.Width * 0.26m,
                    randomSeed,
                    "Cap boundary signal."),
                CreateFrameSignal(
                    $"{spec.Key}:frame:baseline",
                    spec.Environment.Box,
                    new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Bottom),
                    CouplingKind.Stop,
                    0.18m,
                    spec.Environment.Box.Width * 0.24m,
                    randomSeed,
                    "Baseline boundary signal."),
            ])
            .ToArray();

        var tensionField = GlyphTensionField.CreateSeeded(spec.Environment.Box, randomSeed, ambientSignals);

        return new GlyphGrowthState(
            spec.Key,
            tips,
            [],
            junctions,
            packets,
            0,
            ambientSignals,
            ambientSignals.Sum(signal => signal.Magnitude),
            0m,
            tensionField,
            randomSeed);
    }

    private static GlyphAmbientSignal CreateLandmarkSignal(
        GlyphBox box,
        GlyphLandmark landmark,
        int randomSeed)
    {
        GlyphVector target = landmark.Position;
        GlyphVector origin = CreateSignalOrigin(box, target, landmark.Key, randomSeed);

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
        int randomSeed,
        string note)
    {
        return new GlyphAmbientSignal(
            key,
            CreateSignalOrigin(box, target, key, randomSeed),
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
        string key,
        int randomSeed)
    {
        GlyphVector toCenter = (box.Center - target).Normalize();
        if (toCenter == GlyphVector.Zero)
        {
            toCenter = new GlyphVector(0m, 1m);
        }

        GlyphVector tangent = new(-toCenter.Y, toCenter.X);
        decimal lateralSign = (Math.Abs(Hash(key, randomSeed)) & 1) == 0 ? 1m : -1m;
        decimal inwardOffset = GlyphGrowthDefaults.SignalSpawnInset + Noise(key, randomSeed, 1) * 2.4m;
        GlyphVector origin = target +
            (toCenter * inwardOffset) +
            (tangent * ((GlyphGrowthDefaults.SignalSpawnInset * 0.35m + Noise(key, randomSeed, 2)) * lateralSign));

        return new GlyphVector(
            decimal.Clamp(origin.X, box.Left, box.Right),
            decimal.Clamp(origin.Y, box.Bottom, box.Top));
    }

    private static GlyphVector Jitter(
        GlyphVector point,
        GlyphBox box,
        string key,
        int randomSeed,
        decimal amount)
    {
        if (randomSeed == 0 || amount <= 0m)
        {
            return point;
        }

        decimal dx = Noise(key, randomSeed, 3) * amount;
        decimal dy = Noise(key, randomSeed, 4) * amount;
        var jittered = new GlyphVector(point.X + dx, point.Y + dy);
        return new GlyphVector(
            decimal.Clamp(jittered.X, box.Left, box.Right),
            decimal.Clamp(jittered.Y, box.Bottom, box.Top));
    }

    private static decimal Noise(string key, int randomSeed, int salt)
    {
        if (randomSeed == 0)
        {
            return 0m;
        }

        uint hash = (uint)Hash(key, randomSeed ^ (salt * 7919));
        hash ^= hash >> 13;
        hash *= 1274126177u;
        hash ^= hash >> 16;
        decimal normalized = (hash & 0x00FFFFFF) / 16777215m;
        return normalized * 2m - 1m;
    }

    private static int Hash(string key, int randomSeed)
    {
        unchecked
        {
            uint hash = 2166136261u;
            foreach (char character in key)
            {
                hash ^= character;
                hash *= 16777619u;
            }

            hash ^= (uint)randomSeed;
            hash *= 16777619u;
            return (int)hash;
        }
    }
}
