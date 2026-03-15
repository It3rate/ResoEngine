using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public static class GlyphLetterCatalog
{
    public static IReadOnlyList<GlyphLetterSpec> Specs { get; } = CreateDefaultSpecs();

    public static GlyphLetterSpec Get(string key) =>
        Specs.First(spec => string.Equals(spec.Key, key, StringComparison.OrdinalIgnoreCase));

    public static GlyphGrowthState CreateSeedState(string key) =>
        GlyphGrowthState.FromSpec(Get(key));

    public static IReadOnlyList<GlyphLetterSpec> CreateDefaultSpecs()
    {
        GlyphBox box = new(0m, 0m, 100m, 100m);

        return
        [
            CreateY(box),
            CreateV(box),
            CreateT(box),
            CreateO(box),
        ];
    }

    private static GlyphLetterSpec CreateY(GlyphBox box) =>
        new(
            "Y",
            "Y",
            "A trunk that prefers to split in the upper half and spread toward the cap corners.",
            CreateEnvironment(
                box,
                [
                    new GlyphLandmark("Y-midline", GlyphLandmarkKind.Midline, new GlyphVector(box.MidX, box.MidY), 0.25m, "Shared horizontal balance line."),
                    new GlyphLandmark("Y-centerline", GlyphLandmarkKind.Centerline, new GlyphVector(box.MidX, box.Bottom), 0.35m, "Preferred trunk axis."),
                    new GlyphLandmark("Y-branch", GlyphLandmarkKind.BranchPoint, new GlyphVector(box.MidX, 58m), 0.6m, "Preferred split site."),
                    new GlyphLandmark("Y-left-stop", GlyphLandmarkKind.StopPoint, new GlyphVector(24m, box.Top), 0.4m, "Upper left stop."),
                    new GlyphLandmark("Y-right-stop", GlyphLandmarkKind.StopPoint, new GlyphVector(76m, box.Top), 0.4m, "Upper right stop."),
                ]),
            [
                new GlyphSeed("Y-trunk", GlyphSeedKind.Tip, new GlyphVector(box.MidX, box.Bottom), 1m, new GlyphVector(0m, 1m), Note: "Initial upward trunk seed."),
                new GlyphSeed("Y-branch-zone", GlyphSeedKind.Junction, new GlyphVector(box.MidX, 58m), 0.7m, Note: "Preferred branch attractor."),
            ]);

    private static GlyphLetterSpec CreateV(GlyphBox box) =>
        new(
            "V",
            "V",
            "Two upper seeds converge toward a shared lower capture point.",
            CreateEnvironment(
                box,
                [
                    new GlyphLandmark("V-centerline", GlyphLandmarkKind.Centerline, new GlyphVector(box.MidX, box.Bottom), 0.3m, "Preferred symmetry axis."),
                    new GlyphLandmark("V-stop", GlyphLandmarkKind.StopPoint, new GlyphVector(box.MidX, box.Bottom), 0.7m, "Lower capture point."),
                ]),
            [
                new GlyphSeed("V-left", GlyphSeedKind.Tip, new GlyphVector(18m, box.Top), 1m, new GlyphVector(1m, -1m), Note: "Upper left descending arm."),
                new GlyphSeed("V-right", GlyphSeedKind.Tip, new GlyphVector(82m, box.Top), 1m, new GlyphVector(-1m, -1m), Note: "Upper right descending arm."),
            ]);

    private static GlyphLetterSpec CreateT(GlyphBox box) =>
        new(
            "T",
            "T",
            "A top bar spreads across the cap line while a centered stem is drawn downward.",
            CreateEnvironment(
                box,
                [
                    new GlyphLandmark("T-cap", GlyphLandmarkKind.Capline, new GlyphVector(box.MidX, box.Top), 0.45m, "Upper cap alignment."),
                    new GlyphLandmark("T-centerline", GlyphLandmarkKind.Centerline, new GlyphVector(box.MidX, box.MidY), 0.4m, "Centered stem alignment."),
                ]),
            [
                new GlyphSeed("T-bar", GlyphSeedKind.Tip, new GlyphVector(box.MidX, box.Top), 1m, new GlyphVector(1m, 0m), Note: "Top bar expansion."),
                new GlyphSeed("T-stem", GlyphSeedKind.Tip, new GlyphVector(box.MidX, box.Top), 1m, new GlyphVector(0m, -1m), Note: "Center stem descent."),
            ]);

    private static GlyphLetterSpec CreateO(GlyphBox box) =>
        new(
            "O",
            "O",
            "A closed orbit that balances a centerline field with edge attraction around the box.",
            CreateEnvironment(
                box,
                [
                    new GlyphLandmark("O-center", GlyphLandmarkKind.Centerline, box.Center, 0.25m, "Orbital center."),
                    new GlyphLandmark("O-midline", GlyphLandmarkKind.Midline, new GlyphVector(box.MidX, box.MidY), 0.25m, "Vertical balance."),
                ]),
            [
                new GlyphSeed("O-top", GlyphSeedKind.Tip, new GlyphVector(box.MidX, box.Top), 1m, new GlyphVector(1m, 0m), Note: "Upper orbital seed."),
                new GlyphSeed("O-bottom", GlyphSeedKind.Tip, new GlyphVector(box.MidX, box.Bottom), 1m, new GlyphVector(-1m, 0m), Note: "Lower orbital seed."),
            ]);

    private static GlyphEnvironment CreateEnvironment(
        GlyphBox box,
        IReadOnlyList<GlyphLandmark> landmarks)
    {
        var emitters = new List<GlyphFieldEmitter>
        {
            new VerticalBandGlyphFieldEmitter(
                "centerline",
                box.MidX,
                18m,
                [new CouplingRule(CouplingKind.Align, 0.45m, 18m, Channel: "centerline")],
                BaseStrength: 1m,
                Note: "Centerline alignment field."),
            new HorizontalBandGlyphFieldEmitter(
                "midline",
                box.MidY,
                14m,
                [new CouplingRule(CouplingKind.Attract, 0.2m, 14m, Channel: "midline")],
                BaseStrength: 1m,
                Note: "Shared horizontal balance field."),
            new VerticalBandGlyphFieldEmitter(
                "left-edge",
                box.Left,
                24m,
                [new CouplingRule(CouplingKind.Grow, 0.18m, 24m, Channel: "edge")],
                BaseStrength: 1m,
                Note: "Left edge growth field."),
            new VerticalBandGlyphFieldEmitter(
                "right-edge",
                box.Right,
                24m,
                [new CouplingRule(CouplingKind.Grow, 0.18m, 24m, Channel: "edge")],
                BaseStrength: 1m,
                Note: "Right edge growth field."),
            new HorizontalBandGlyphFieldEmitter(
                "top-edge",
                box.Top,
                20m,
                [new CouplingRule(CouplingKind.Stop, 0.3m, 20m, Channel: "cap")],
                BaseStrength: 1m,
                Note: "Cap stop field."),
            new HorizontalBandGlyphFieldEmitter(
                "bottom-edge",
                box.Bottom,
                16m,
                [new CouplingRule(CouplingKind.Stop, 0.2m, 16m, Channel: "baseline")],
                BaseStrength: 1m,
                Note: "Baseline stop field."),
        };

        foreach (var landmark in landmarks)
        {
            switch (landmark.Kind)
            {
                case GlyphLandmarkKind.BranchPoint:
                    emitters.Add(new PointGlyphFieldEmitter(
                        $"{landmark.Key}-field",
                        landmark.Position,
                        GlyphGrowthDefaults.BranchCaptureRadius,
                        [
                            new CouplingRule(CouplingKind.Split, landmark.Strength, GlyphGrowthDefaults.BranchCaptureRadius, Channel: "branch"),
                            new CouplingRule(CouplingKind.Attract, landmark.Strength * 0.35m, GlyphGrowthDefaults.BranchCaptureRadius, Channel: "branch"),
                        ],
                        BaseStrength: 1m,
                        Note: "Branch encouragement field."));
                    break;

                case GlyphLandmarkKind.StopPoint:
                    emitters.Add(new PointGlyphFieldEmitter(
                        $"{landmark.Key}-field",
                        landmark.Position,
                        GlyphGrowthDefaults.JoinCaptureRadius,
                        [
                            new CouplingRule(CouplingKind.Stop, landmark.Strength, GlyphGrowthDefaults.JoinCaptureRadius, Channel: "stop"),
                            new CouplingRule(CouplingKind.Join, landmark.Strength * 0.65m, GlyphGrowthDefaults.JoinCaptureRadius, Channel: "join"),
                        ],
                        BaseStrength: 1m,
                        Note: "Terminal capture field."));
                    break;
            }
        }

        return new GlyphEnvironment(box, landmarks.ToArray(), emitters, []);
    }
}
