using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationPresetFactory
{
    public static LetterFormationState CreateSeed(
        LetterFormationPresetKind kind,
        Random? random = null,
        LetterFormationEnvironment? environment = null)
    {
        Random rng = random ?? Random.Shared;
        LetterFormationEnvironment env = environment ?? LetterFormationEnvironment.CreateLetterBox();
        Proportion midlineRatio = SafeRatio(env.MidlineY - env.Top, env.Height);

        return kind switch
        {
            LetterFormationPresetKind.CapitalD => BuildCapitalD(rng, env),
            LetterFormationPresetKind.BridgeH => BuildBridgeH(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterT => BuildLetterT(rng, env),
            LetterFormationPresetKind.LetterA => BuildLetterA(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterY => BuildLetterY(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterL => BuildLetterL(rng, env),
            LetterFormationPresetKind.LetterM => BuildLetterM(rng, env, midlineRatio),
            _ => BuildLetterA(rng, env, midlineRatio),
        };
    }

    public static LetterFormationState CreateCapitalAAssemblySeed(
        Random? random = null,
        LetterFormationEnvironment? environment = null) =>
        CreateSeed(LetterFormationPresetKind.LetterA, random, environment);

    public static LetterFormationState CreateCapitalASeed(
        Random? random = null,
        LetterFormationEnvironment? environment = null) =>
        CreateSeed(LetterFormationPresetKind.LetterA, random, environment);

    public static string GetShortLabel(LetterFormationPresetKind kind) =>
        kind switch
        {
            LetterFormationPresetKind.CapitalD => "D",
            LetterFormationPresetKind.BridgeH => "H",
            LetterFormationPresetKind.LetterT => "T",
            LetterFormationPresetKind.LetterA => "A",
            LetterFormationPresetKind.LetterY => "Y",
            LetterFormationPresetKind.LetterL => "L",
            LetterFormationPresetKind.LetterM => "M",
            _ => "A",
        };

    public static string GetDisplayName(LetterFormationPresetKind kind) =>
        kind switch
        {
            LetterFormationPresetKind.CapitalD => "Capital D",
            LetterFormationPresetKind.BridgeH => "Bridge H",
            LetterFormationPresetKind.LetterT => "Capital T",
            LetterFormationPresetKind.LetterA => "Capital A",
            LetterFormationPresetKind.LetterY => "Capital Y",
            LetterFormationPresetKind.LetterL => "Capital L",
            LetterFormationPresetKind.LetterM => "Capital M",
            _ => "Capital A",
        };

    private static LetterFormationState BuildLetterA(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "LeftBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(3, 20), new Proportion(3), "base near left side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "base on baseline")),
            Site(rng, env, "LeftJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(32, 100), new Proportion(22, 100), new Proportion(2), "left join stays left of center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 8), new Proportion(5), "left join near crossbar height"),
                new JoinSiteDesire("CrossLeft", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left crossbar pin")),
            Site(rng, env, "LeftApex", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(47, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.Zero, new Proportion(1, 12), new Proportion(5), "apex near topline"),
                new JoinSiteDesire("RightApex", new Proportion(3, 5), new Proportion(3), new Proportion(5), "apex join")),
            Site(rng, env, "CrossLeft", new Axis(-2, 1, 0, -1),
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossRight", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "crossbar stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "crossbar near midline"),
                new JoinSiteDesire("LeftJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left crossbar pin")),
            Site(rng, env, "CrossRight", new Axis(2, 1, 0, -1),
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossLeft", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "crossbar stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "crossbar near midline"),
                new JoinSiteDesire("RightJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right crossbar pin")),
            Site(rng, env, "RightApex", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(53, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.Zero, new Proportion(1, 12), new Proportion(5), "apex near topline"),
                new JoinSiteDesire("LeftApex", new Proportion(3, 5), new Proportion(3), new Proportion(5), "apex join")),
            Site(rng, env, "RightJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(68, 100), new Proportion(22, 100), new Proportion(2), "right join stays right of center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 8), new Proportion(5), "right join near crossbar height"),
                new JoinSiteDesire("CrossRight", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right crossbar pin")),
            Site(rng, env, "RightBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(3, 20), new Proportion(3), "base near right side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "base on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftLower", "LeftBase", "LeftJoin",
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left lower rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps a reasonable span")),
            Carrier("LeftUpper", "LeftJoin", "LeftApex",
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left upper rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps a reasonable span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight",
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "crossbar prefers horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "crossbar keeps a moderate width")),
            Carrier("RightUpper", "RightJoin", "RightApex",
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 5), new Proportion(3), "right upper rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps a reasonable span")),
            Carrier("RightLower", "RightBase", "RightJoin",
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 5), new Proportion(3), "right lower rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right lower keeps a reasonable span")),
        ];

        return CreateState("CapitalAAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildBridgeH(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "LeftBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(3, 20), new Proportion(3), "left base near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "left base on baseline")),
            Site(rng, env, "LeftJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(3, 20), new Proportion(3), "left join near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(6), "left join near midline"),
                new JoinSiteDesire("CrossLeft", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left bridge pin")),
            Site(rng, env, "LeftTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(3, 20), new Proportion(3), "left top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.Zero, new Proportion(1, 10), new Proportion(5), "left top near topline")),
            Site(rng, env, "CrossLeft", new Axis(-2, 1, 0, -1),
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossRight", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "bridge stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "bridge near midline"),
                new JoinSiteDesire("LeftJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left bridge pin")),
            Site(rng, env, "CrossRight", new Axis(2, 1, 0, -1),
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossLeft", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "bridge stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "bridge near midline"),
                new JoinSiteDesire("RightJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right bridge pin")),
            Site(rng, env, "RightTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(3, 20), new Proportion(3), "right top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.Zero, new Proportion(1, 10), new Proportion(5), "right top near topline")),
            Site(rng, env, "RightJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(3, 20), new Proportion(3), "right join near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(6), "right join near midline"),
                new JoinSiteDesire("CrossRight", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right bridge pin")),
            Site(rng, env, "RightBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(3, 20), new Proportion(3), "right base near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "right base on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftLower", "LeftBase", "LeftJoin",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "left lower vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps span")),
            Carrier("LeftUpper", "LeftJoin", "LeftTop",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "left upper vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight",
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bridge prefers horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "bridge keeps width")),
            Carrier("RightUpper", "RightJoin", "RightTop",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "right upper vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps span")),
            Carrier("RightLower", "RightBase", "RightJoin",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "right lower vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right lower keeps span")),
        ];

        return CreateState("BridgeHAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterT(Random rng, LetterFormationEnvironment env)
    {
        Proportion topRatio = new Proportion(7, 100);

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(14, 100), new Proportion(4), "stem base near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem base on baseline")),
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(5), "stem top near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, topRatio, new Proportion(1, 12), new Proportion(5), "stem top near topline"),
                new JoinSiteDesire("LeftJoin", new Proportion(1, 2), new Proportion(3), new Proportion(4), "left top junction"),
                new JoinSiteDesire("RightJoin", new Proportion(1, 2), new Proportion(3), new Proportion(4), "right top junction")),
            Site(rng, env, "LeftJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(4), "left join near stem top"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, topRatio, new Proportion(1, 12), new Proportion(5), "left join near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "left top junction")),
            Site(rng, env, "LeftTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(12, 100), new Proportion(14, 100), new Proportion(4), "left target near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, topRatio, new Proportion(1, 10), new Proportion(5), "left target near topline")),
            Site(rng, env, "RightJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(4), "right join near stem top"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, topRatio, new Proportion(1, 12), new Proportion(5), "right join near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "right top junction")),
            Site(rng, env, "RightTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(88, 100), new Proportion(14, 100), new Proportion(4), "right target near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, topRatio, new Proportion(1, 10), new Proportion(5), "right target near topline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "StemTop",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("LeftBar", "LeftJoin", "LeftTarget",
                new CarrierDirectionDesire(Axis.NegativeOne, new Proportion(1, 10), new Proportion(4), "left bar extends left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left bar keeps width")),
            Carrier("RightBar", "RightJoin", "RightTarget",
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "right bar extends right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right bar keeps width")),
        ];

        return CreateState("CapitalTAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterY(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        Proportion junctionRatio = midlineRatio - new Proportion(10, 100);
        if (junctionRatio < new Proportion(18, 100))
        {
            junctionRatio = new Proportion(18, 100);
        }

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "stem base near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 12), new Proportion(6), "stem base on baseline")),
            Site(rng, env, "Junction", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(8, 100), new Proportion(5), "junction near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, junctionRatio, new Proportion(1, 12), new Proportion(6), "junction below topline"),
                new JoinSiteDesire("LeftJoin", new Proportion(1, 2), new Proportion(3), new Proportion(4), "left branch attach"),
                new JoinSiteDesire("RightJoin", new Proportion(1, 2), new Proportion(3), new Proportion(4), "right branch attach")),
            Site(rng, env, "LeftJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(8, 100), new Proportion(5), "left branch near junction"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, junctionRatio, new Proportion(1, 12), new Proportion(6), "left branch near junction height"),
                new JoinSiteDesire("Junction", new Proportion(1, 2), new Proportion(3), new Proportion(4), "left branch attach")),
            Site(rng, env, "LeftTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(14, 100), new Proportion(14, 100), new Proportion(4), "left target near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 12), new Proportion(6), "left target near top")),
            Site(rng, env, "RightJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(8, 100), new Proportion(5), "right branch near junction"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, junctionRatio, new Proportion(1, 12), new Proportion(6), "right branch near junction height"),
                new JoinSiteDesire("Junction", new Proportion(1, 2), new Proportion(3), new Proportion(4), "right branch attach")),
            Site(rng, env, "RightTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(86, 100), new Proportion(14, 100), new Proportion(4), "right target near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 12), new Proportion(6), "right target near top")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "Junction",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 12), new Proportion(5), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "stem keeps span")),
            Carrier("LeftArm", "LeftJoin", "LeftTarget",
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 12), new Proportion(5), "left arm rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left arm keeps span")),
            Carrier("RightArm", "RightJoin", "RightTarget",
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 12), new Proportion(5), "right arm rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right arm keeps span")),
        ];

        return CreateState("CapitalYAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterL(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near top")),
            Site(rng, env, "StemBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem base near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem base on baseline"),
                new JoinSiteDesire("FootJoin", new Proportion(1, 2), new Proportion(3), new Proportion(4), "foot corner attach")),
            Site(rng, env, "FootJoin", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "foot join near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "foot join on baseline"),
                new JoinSiteDesire("StemBase", new Proportion(1, 2), new Proportion(3), new Proportion(4), "foot corner attach")),
            Site(rng, env, "FootRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(84, 100), new Proportion(16, 100), new Proportion(4), "foot right near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "foot right on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "StemTop",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("Foot", "FootJoin", "FootRight",
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "foot extends right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "foot keeps width")),
        ];

        return CreateState("CapitalLAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterM(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        Proportion valleyRatio = midlineRatio + new Proportion(16, 100);
        if (valleyRatio > new Proportion(82, 100))
        {
            valleyRatio = new Proportion(82, 100);
        }

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "LeftBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(14, 100), new Proportion(14, 100), new Proportion(4), "left base near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "left base on baseline")),
            Site(rng, env, "LeftPeak", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "left peak near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "left peak near top")),
            Site(rng, env, "CenterLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(4), "valley near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, valleyRatio, new Proportion(1, 8), new Proportion(5), "valley below midline"),
                new JoinSiteDesire("CenterRight", new Proportion(1, 2), new Proportion(3), new Proportion(4), "center valley attach")),
            Site(rng, env, "CenterRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(4), "valley near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, valleyRatio, new Proportion(1, 8), new Proportion(5), "valley below midline"),
                new JoinSiteDesire("CenterLeft", new Proportion(1, 2), new Proportion(3), new Proportion(4), "center valley attach")),
            Site(rng, env, "RightPeak", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "right peak near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "right peak near top")),
            Site(rng, env, "RightBase", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(86, 100), new Proportion(14, 100), new Proportion(4), "right base near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "right base on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStem", "LeftBase", "LeftPeak",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "left stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "left stem keeps span")),
            Carrier("LeftMiddle", "LeftPeak", "CenterLeft",
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "left middle descends"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left middle keeps span")),
            Carrier("RightMiddle", "CenterRight", "RightPeak",
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "right middle rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right middle keeps span")),
            Carrier("RightStem", "RightBase", "RightPeak",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "right stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "right stem keeps span")),
        ];

        return CreateState("CapitalMAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildCapitalD(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(92, 100), new Proportion(1, 10), new Proportion(5), "stem bottom near baseline"),
                new JoinSiteDesire("BowlBottom", new Proportion(1, 2), new Proportion(3), new Proportion(4), "bottom joins stem")),
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline"),
                new JoinSiteDesire("BowlTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "top joins stem")),
            Site(rng, env, "BowlTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(22, 100), new Proportion(18, 100), new Proportion(4), "bowl top starts near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "bowl top near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "top joins stem")),
            Site(rng, env, "BowlBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(22, 100), new Proportion(18, 100), new Proportion(4), "bowl bottom starts near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(92, 100), new Proportion(1, 10), new Proportion(5), "bowl bottom near baseline"),
                new JoinSiteDesire("StemBottom", new Proportion(1, 2), new Proportion(3), new Proportion(4), "bottom joins stem")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop",
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("Bowl", "BowlTop", "BowlBottom",
                new CarrierDirectionDesire(Axis.I, new Proportion(1, 10), new Proportion(4), "outer descends"),
                new CarrierSpanDesire(new Proportion(6), new Proportion(13), new Proportion(2), "outer keeps span")),
        ];

        return CreateState("CapitalDAssembly", env, sites, carriers);
    }

    private static LetterFormationState CreateState(
        string key,
        LetterFormationEnvironment environment,
        IReadOnlyList<LetterFormationSiteState> sites,
        IReadOnlyList<LetterFormationCarrierState> carriers) =>
        new(key, 0, environment, sites, carriers, []);

    private static LetterFormationSiteState Site(
        Random rng,
        LetterFormationEnvironment environment,
        string id,
        Axis axis,
        params LetterFormationDesire[] desires) =>
        new(
            id,
            environment.ResolveRelativePoint(RandomRatio(rng, 0, 100), RandomRatio(rng, 0, 100)),
            axis,
            PlanarOffset.Zero,
            desires);

    private static LetterFormationCarrierState Carrier(
        string id,
        string startSiteId,
        string endSiteId,
        params LetterFormationDesire[] desires) =>
        new(id, startSiteId, endSiteId, desires);

    private static Proportion RandomRatio(Random rng, int minPercent, int maxPercent) =>
        new Proportion(rng.Next(minPercent, maxPercent + 1), 100);

    private static Proportion SafeRatio(Proportion numerator, Proportion denominator) =>
        denominator.IsZero ? Proportion.Zero : numerator / denominator;
}
