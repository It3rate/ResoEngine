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
            LetterFormationPresetKind.LetterA => BuildLetterA(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterB => BuildLetterB(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterC => BuildLetterC(rng, env, midlineRatio),
            LetterFormationPresetKind.CapitalD => BuildCapitalD(rng, env),
            LetterFormationPresetKind.LetterE => BuildLetterE(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterF => BuildLetterF(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterG => BuildLetterG(rng, env, midlineRatio),
            LetterFormationPresetKind.BridgeH => BuildBridgeH(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterI => BuildLetterI(rng, env),
            LetterFormationPresetKind.LetterJ => BuildLetterJ(rng, env),
            LetterFormationPresetKind.LetterK => BuildLetterK(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterT => BuildLetterT(rng, env),
            LetterFormationPresetKind.LetterY => BuildLetterY(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterL => BuildLetterL(rng, env),
            LetterFormationPresetKind.LetterM => BuildLetterM(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterN => BuildLetterN(rng, env),
            LetterFormationPresetKind.LetterO => BuildLetterO(rng, env),
            LetterFormationPresetKind.LetterP => BuildLetterP(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterQ => BuildLetterQ(rng, env),
            LetterFormationPresetKind.LetterR => BuildLetterR(rng, env, midlineRatio),
            LetterFormationPresetKind.LetterS => BuildLetterS(rng, env),
            LetterFormationPresetKind.LetterU => BuildLetterU(rng, env),
            LetterFormationPresetKind.LetterV => BuildLetterV(rng, env),
            LetterFormationPresetKind.LetterW => BuildLetterW(rng, env),
            LetterFormationPresetKind.LetterX => BuildLetterX(rng, env),
            LetterFormationPresetKind.LetterZ => BuildLetterZ(rng, env),
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
            LetterFormationPresetKind.LetterA => "A",
            LetterFormationPresetKind.LetterB => "B",
            LetterFormationPresetKind.LetterC => "C",
            LetterFormationPresetKind.CapitalD => "D",
            LetterFormationPresetKind.LetterE => "E",
            LetterFormationPresetKind.LetterF => "F",
            LetterFormationPresetKind.LetterG => "G",
            LetterFormationPresetKind.BridgeH => "H",
            LetterFormationPresetKind.LetterI => "I",
            LetterFormationPresetKind.LetterJ => "J",
            LetterFormationPresetKind.LetterK => "K",
            LetterFormationPresetKind.LetterT => "T",
            LetterFormationPresetKind.LetterL => "L",
            LetterFormationPresetKind.LetterM => "M",
            LetterFormationPresetKind.LetterN => "N",
            LetterFormationPresetKind.LetterO => "O",
            LetterFormationPresetKind.LetterP => "P",
            LetterFormationPresetKind.LetterQ => "Q",
            LetterFormationPresetKind.LetterR => "R",
            LetterFormationPresetKind.LetterS => "S",
            LetterFormationPresetKind.LetterU => "U",
            LetterFormationPresetKind.LetterV => "V",
            LetterFormationPresetKind.LetterW => "W",
            LetterFormationPresetKind.LetterX => "X",
            LetterFormationPresetKind.LetterY => "Y",
            LetterFormationPresetKind.LetterZ => "Z",
            _ => "A",
        };

    public static string GetDisplayName(LetterFormationPresetKind kind) =>
        kind switch
        {
            LetterFormationPresetKind.LetterA => "Capital A",
            LetterFormationPresetKind.LetterB => "Capital B",
            LetterFormationPresetKind.LetterC => "Capital C",
            LetterFormationPresetKind.CapitalD => "Capital D",
            LetterFormationPresetKind.LetterE => "Capital E",
            LetterFormationPresetKind.LetterF => "Capital F",
            LetterFormationPresetKind.LetterG => "Capital G",
            LetterFormationPresetKind.BridgeH => "Bridge H",
            LetterFormationPresetKind.LetterI => "Capital I",
            LetterFormationPresetKind.LetterJ => "Capital J",
            LetterFormationPresetKind.LetterK => "Capital K",
            LetterFormationPresetKind.LetterT => "Capital T",
            LetterFormationPresetKind.LetterL => "Capital L",
            LetterFormationPresetKind.LetterM => "Capital M",
            LetterFormationPresetKind.LetterN => "Capital N",
            LetterFormationPresetKind.LetterO => "Capital O",
            LetterFormationPresetKind.LetterP => "Capital P",
            LetterFormationPresetKind.LetterQ => "Capital Q",
            LetterFormationPresetKind.LetterR => "Capital R",
            LetterFormationPresetKind.LetterS => "Capital S",
            LetterFormationPresetKind.LetterU => "Capital U",
            LetterFormationPresetKind.LetterV => "Capital V",
            LetterFormationPresetKind.LetterW => "Capital W",
            LetterFormationPresetKind.LetterX => "Capital X",
            LetterFormationPresetKind.LetterY => "Capital Y",
            LetterFormationPresetKind.LetterZ => "Capital Z",
            _ => "Capital A",
        };

    private static LetterFormationState BuildLetterA(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "LeftBase", Axis.Zero,
                NearLeftSide("base near left side", 20, 15, 3),
                OnBaseline("base on baseline")),
            AnchoredSite(rng, env, "LeftJoin", Axis.Zero,
                HorizontalAt(new Proportion(32, 100), new Proportion(22, 100), new Proportion(2), "left join stays left of center"),
                VerticalAt(midlineRatio, new Proportion(1, 8), new Proportion(5), "left join near crossbar height"),
                JoinWith("CrossLeft", "left crossbar pin")),
            AnchoredSite(rng, env, "LeftApex", Axis.Zero,
                HorizontalAt(new Proportion(47, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                NearTopline("apex near topline", 0, 8, 5),
                JoinWith("RightApex", "apex join", new Proportion(3, 5), new Proportion(3), new Proportion(5))),
            AnchoredSite(rng, env, "CrossLeft", Axis.Zero,
                HorizontalAt(new Proportion(32, 100), new Proportion(22, 100), new Proportion(2), "crossbar stays left of center"),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(7), "crossbar near midline"),
                LevelWith("CrossRight", "crossbar stays level"),
                JoinWith("LeftJoin", "left crossbar pin")),
            AnchoredSite(rng, env, "CrossRight", Axis.Zero,
                HorizontalAt(new Proportion(68, 100), new Proportion(22, 100), new Proportion(2), "crossbar stays right of center"),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(7), "crossbar near midline"),
                LevelWith("CrossLeft", "crossbar stays level"),
                JoinWith("RightJoin", "right crossbar pin")),
            AnchoredSite(rng, env, "RightApex", Axis.Zero,
                HorizontalAt(new Proportion(53, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                NearTopline("apex near topline", 0, 8, 5),
                JoinWith("LeftApex", "apex join", new Proportion(3, 5), new Proportion(3), new Proportion(5))),
            AnchoredSite(rng, env, "RightJoin", Axis.Zero,
                HorizontalAt(new Proportion(68, 100), new Proportion(22, 100), new Proportion(2), "right join stays right of center"),
                VerticalAt(midlineRatio, new Proportion(1, 8), new Proportion(5), "right join near crossbar height"),
                JoinWith("CrossRight", "right crossbar pin")),
            AnchoredSite(rng, env, "RightBase", Axis.Zero,
                NearRightSide("base near right side", 80, 15, 3),
                OnBaseline("base on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftLower", "LeftBase", "LeftJoin", "LeftStroke", 1, 1, true,
                PreferDirection(LetterFormationDirections.UpRight, "left lower rises", new Proportion(1, 5), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps a reasonable span")),
            Carrier("LeftUpper", "LeftJoin", "LeftApex", "LeftStroke", 1, 0, true,
                PreferDirection(LetterFormationDirections.UpRight, "left upper rises", new Proportion(1, 5), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps a reasonable span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight", "CrossStroke", 3, 0, false,
                PreferHorizontal("crossbar prefers horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "crossbar keeps a moderate width")),
            Carrier("RightUpper", "RightJoin", "RightApex", "RightStroke", 2, 0, true,
                PreferDirection(LetterFormationDirections.UpLeft, "right upper rises", new Proportion(1, 5), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps a reasonable span")),
            Carrier("RightLower", "RightBase", "RightJoin", "RightStroke", 2, 1, true,
                PreferDirection(LetterFormationDirections.UpLeft, "right lower rises", new Proportion(1, 5), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "right lower keeps a reasonable span")),
        ];

        return CreateState("CapitalAAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildBridgeH(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "LeftBase", Axis.Zero,
                NearLeftSide("left base near side", 20, 15, 3),
                OnBaseline("left base on baseline")),
            AnchoredSite(rng, env, "LeftJoin", Axis.Zero,
                NearLeftSide("left join near side", 20, 15, 3),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(6), "left join near midline"),
                JoinWith("CrossLeft", "left bridge pin")),
            AnchoredSite(rng, env, "LeftTop", Axis.Zero,
                NearLeftSide("left top near side", 20, 15, 3),
                NearTopline("left top near topline", 0, 10, 5)),
            AnchoredSite(rng, env, "CrossLeft", Axis.Zero,
                NearLeftSide("bridge starts on left half", 20, 15, 3),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(7), "bridge near midline"),
                LevelWith("CrossRight", "bridge stays level"),
                JoinWith("LeftJoin", "left bridge pin")),
            AnchoredSite(rng, env, "CrossRight", Axis.Zero,
                NearRightSide("bridge ends on right half", 80, 15, 3),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(7), "bridge near midline"),
                LevelWith("CrossLeft", "bridge stays level"),
                JoinWith("RightJoin", "right bridge pin")),
            AnchoredSite(rng, env, "RightTop", Axis.Zero,
                NearRightSide("right top near side", 80, 15, 3),
                NearTopline("right top near topline", 0, 10, 5)),
            AnchoredSite(rng, env, "RightJoin", Axis.Zero,
                NearRightSide("right join near side", 80, 15, 3),
                VerticalAt(midlineRatio, new Proportion(1, 12), new Proportion(6), "right join near midline"),
                JoinWith("CrossRight", "right bridge pin")),
            AnchoredSite(rng, env, "RightBase", Axis.Zero,
                NearRightSide("right base near side", 80, 15, 3),
                OnBaseline("right base on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftLower", "LeftBase", "LeftJoin", "LeftStroke", 1, 1, true,
                PreferVertical("left lower vertical", new Proportion(1, 10), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps span")),
            Carrier("LeftUpper", "LeftJoin", "LeftTop", "LeftStroke", 1, 0, true,
                PreferVertical("left upper vertical", new Proportion(1, 10), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight", "BridgeStroke", 3, 0, false,
                PreferHorizontal("bridge prefers horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "bridge keeps width")),
            Carrier("RightUpper", "RightJoin", "RightTop", "RightStroke", 2, 0, true,
                PreferVertical("right upper vertical", new Proportion(1, 10), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps span")),
            Carrier("RightLower", "RightBase", "RightJoin", "RightStroke", 2, 1, true,
                PreferVertical("right lower vertical", new Proportion(1, 10), new Proportion(3)),
                KeepSpan(new Proportion(2), new Proportion(12), new Proportion(2), "right lower keeps span")),
        ];

        return CreateState("BridgeHAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterT(Random rng, LetterFormationEnvironment env)
    {
        Proportion topRatio = new Proportion(7, 100);

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "StemBase", Axis.Zero,
                NearCenter("stem base near center", 14, 4),
                OnBaseline("stem base on baseline")),
            AnchoredSite(rng, env, "StemTop", Axis.Zero,
                NearCenter("stem top near center"),
                VerticalAt(topRatio, new Proportion(1, 12), new Proportion(5), "stem top near topline"),
                JoinWith("LeftJoin", "left top junction", new Proportion(1, 2), new Proportion(3), new Proportion(4)),
                JoinWith("RightJoin", "right top junction", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "LeftJoin", Axis.Zero,
                NearCenter("left join near stem top"),
                VerticalAt(topRatio, new Proportion(1, 12), new Proportion(5), "left join near topline"),
                JoinWith("StemTop", "left top junction", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "LeftTarget", Axis.Zero,
                NearLeftSide("left target near side", 12),
                VerticalAt(topRatio, new Proportion(1, 10), new Proportion(5), "left target near topline")),
            AnchoredSite(rng, env, "RightJoin", Axis.Zero,
                NearCenter("right join near stem top"),
                VerticalAt(topRatio, new Proportion(1, 12), new Proportion(5), "right join near topline"),
                JoinWith("StemTop", "right top junction", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "RightTarget", Axis.Zero,
                NearRightSide("right target near side", 88),
                VerticalAt(topRatio, new Proportion(1, 10), new Proportion(5), "right target near topline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "StemTop", "StemStroke", 1, 0, true,
                PreferVertical("stem vertical"),
                KeepSpan(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("LeftBar", "LeftJoin", "LeftTarget", "TopStroke", 2, 0, true,
                PreferDirection(Axis.NegativeOne, "left bar extends left"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "left bar keeps width")),
            Carrier("RightBar", "RightJoin", "RightTarget", "TopStroke", 2, 1, false,
                PreferHorizontal("right bar extends right"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "right bar keeps width")),
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
            AnchoredSite(rng, env, "StemBase", Axis.Zero,
                NearCenter("stem base near center", 12, 5),
                OnBaseline("stem base on baseline", 8, 6)),
            AnchoredSite(rng, env, "Junction", Axis.Zero,
                NearCenter("junction near center", 8, 5),
                VerticalAt(junctionRatio, new Proportion(1, 12), new Proportion(6), "junction below topline"),
                JoinWith("LeftJoin", "left branch attach", new Proportion(1, 2), new Proportion(3), new Proportion(4)),
                JoinWith("RightJoin", "right branch attach", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "LeftJoin", Axis.Zero,
                NearCenter("left branch near junction", 8, 5),
                VerticalAt(junctionRatio, new Proportion(1, 12), new Proportion(6), "left branch near junction height"),
                JoinWith("Junction", "left branch attach", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "LeftTarget", Axis.Zero,
                NearLeftSide("left target near side", 14),
                NearTopline("left target near top", 8, 8, 6)),
            AnchoredSite(rng, env, "RightJoin", Axis.Zero,
                NearCenter("right branch near junction", 8, 5),
                VerticalAt(junctionRatio, new Proportion(1, 12), new Proportion(6), "right branch near junction height"),
                JoinWith("Junction", "right branch attach", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "RightTarget", Axis.Zero,
                NearRightSide("right target near side", 86),
                NearTopline("right target near top", 8, 8, 6)),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "Junction", "StemStroke", 3, 0, true,
                PreferVertical("stem vertical", new Proportion(1, 12), new Proportion(5)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "stem keeps span")),
            Carrier("LeftArm", "LeftJoin", "LeftTarget", "LeftStroke", 1, 0, true,
                PreferDirection(LetterFormationDirections.UpLeft, "left arm rises", new Proportion(1, 12), new Proportion(5)),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "left arm keeps span")),
            Carrier("RightArm", "RightJoin", "RightTarget", "RightStroke", 2, 0, true,
                PreferDirection(LetterFormationDirections.UpRight, "right arm rises", new Proportion(1, 12), new Proportion(5)),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "right arm keeps span")),
        ];

        return CreateState("CapitalYAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterL(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "StemTop", Axis.Zero,
                NearLeftSide("stem top near side", 18),
                NearTopline("stem top near top")),
            AnchoredSite(rng, env, "StemBase", Axis.Zero,
                NearLeftSide("stem base near side", 18),
                OnBaseline("stem base on baseline"),
                JoinWith("FootJoin", "foot corner attach", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "FootJoin", Axis.Zero,
                NearLeftSide("foot join near side", 18),
                OnBaseline("foot join on baseline"),
                JoinWith("StemBase", "foot corner attach", new Proportion(1, 2), new Proportion(3), new Proportion(4))),
            AnchoredSite(rng, env, "FootRight", Axis.Zero,
                NearRightSide("foot right near side", 84, 16),
                OnBaseline("foot right on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBase", "StemTop", "StemStroke", 1, 0, true,
                PreferVertical("stem vertical"),
                KeepSpan(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("Foot", "FootJoin", "FootRight", "FootStroke", 2, 0, false,
                PreferHorizontal("foot extends right"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "foot keeps width")),
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
            Carrier("LeftStem", "LeftBase", "LeftPeak", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "left stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "left stem keeps span")),
            Carrier("LeftMiddle", "LeftPeak", "CenterLeft", "LeftMiddleStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "left middle descends"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left middle keeps span")),
            Carrier("RightMiddle", "CenterRight", "RightPeak", "RightMiddleStroke", 4, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "right middle rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right middle keeps span")),
            Carrier("RightStem", "RightBase", "RightPeak", "RightStroke", 2, 0, true,
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
            Site(rng, env, "BowlTop", new Axis(0, 1, 3, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(22, 100), new Proportion(18, 100), new Proportion(4), "bowl top starts near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "bowl top near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "top joins stem")),
            Site(rng, env, "BowlOuter", Axis.I,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(72, 100), new Proportion(12, 100), new Proportion(6), "bowl reaches outward"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(16, 100), new Proportion(4), "bowl sits around middle")),
            Site(rng, env, "BowlBottom", new Axis(0, 1, -3, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(22, 100), new Proportion(18, 100), new Proportion(4), "bowl bottom starts near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(92, 100), new Proportion(1, 10), new Proportion(5), "bowl bottom near baseline"),
                new JoinSiteDesire("StemBottom", new Proportion(1, 2), new Proportion(3), new Proportion(4), "bottom joins stem")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("UpperBowl", "BowlTop", "BowlOuter", "BowlStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 10), new Proportion(4), "upper bowl rounds outward"),
                new CarrierSpanDesire(new Proportion(5), new Proportion(10), new Proportion(2), "upper bowl keeps span")),
            Carrier("LowerBowl", "BowlOuter", "BowlBottom", "BowlStroke", 2, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 10), new Proportion(4), "lower bowl returns inward"),
                new CarrierSpanDesire(new Proportion(5), new Proportion(10), new Proportion(2), "lower bowl keeps span")),
        ];

        return CreateState("CapitalDAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterB(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "UpperStart", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper start near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "upper start near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "upper bowl stem join")),
            Site(rng, env, "UpperJoin", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper join near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "upper join near midline"),
                new JoinSiteDesire("LowerJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "shared bowl junction")),
            Site(rng, env, "LowerJoin", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "lower join near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "lower join near midline"),
                new JoinSiteDesire("UpperJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "shared bowl junction")),
            Site(rng, env, "LowerEnd", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "lower end near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "lower end on baseline"),
                new JoinSiteDesire("StemBottom", new Proportion(1, 2), new Proportion(3), new Proportion(4), "lower bowl stem join")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "UpperOuter", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(16, 100), new Proportion(4), "upper bowl reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(24, 100), new Proportion(1, 10), new Proportion(5), "upper bowl sits high")),
            Site(rng, env, "LowerOuter", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(16, 100), new Proportion(4), "lower bowl reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(68, 100), new Proportion(1, 10), new Proportion(5), "lower bowl sits low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("UpperFront", "UpperStart", "UpperOuter", "UpperStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "upper front rounds right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper front keeps span")),
            Carrier("UpperBack", "UpperOuter", "UpperJoin", "UpperStroke", 2, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper back returns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper back keeps span")),
            Carrier("LowerFront", "LowerJoin", "LowerOuter", "LowerStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "lower front rounds right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "lower front keeps span")),
            Carrier("LowerBack", "LowerOuter", "LowerEnd", "LowerStroke", 3, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "lower back returns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "lower back keeps span")),
        ];

        return CreateState("CapitalBAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterC(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopRight", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(16, 100), new Proportion(4), "top end near right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(16, 100), new Proportion(1, 10), new Proportion(5), "top end near topline")),
            Site(rng, env, "LeftMid", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(20, 100), new Proportion(14, 100), new Proportion(5), "left middle reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(5), "left middle near midline")),
            Site(rng, env, "BottomRight", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(16, 100), new Proportion(4), "bottom end near right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(84, 100), new Proportion(1, 10), new Proportion(5), "bottom end near baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("UpperArc", "TopRight", "LeftMid", "CurveStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper arc turns left"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(10), new Proportion(2), "upper arc keeps span")),
            Carrier("LowerArc", "LeftMid", "BottomRight", "CurveStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "lower arc turns right"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(10), new Proportion(2), "lower arc keeps span")),
        ];

        return CreateState("CapitalCAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterE(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "StemTop", Axis.Zero,
                NearLeftSide("stem top near side", 18),
                NearTopline("stem top near topline")),
            AnchoredSite(rng, env, "StemMid", Axis.Zero,
                NearLeftSide("stem middle near side", 18),
                NearMidline(midlineRatio, "stem middle near midline")),
            AnchoredSite(rng, env, "StemBottom", Axis.Zero,
                NearLeftSide("stem bottom near side", 18),
                OnBaseline("stem bottom on baseline")),
            AnchoredSite(rng, env, "TopEnd", Axis.Zero,
                NearRightSide("top bar reaches right", 82),
                NearTopline("top bar stays high")),
            AnchoredSite(rng, env, "MidEnd", Axis.Zero,
                NearRightSide("middle bar reaches center-right", 64),
                NearMidline(midlineRatio, "middle bar near midline")),
            AnchoredSite(rng, env, "BottomEnd", Axis.Zero,
                NearRightSide("bottom bar reaches right", 82),
                OnBaseline("bottom bar on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                PreferVertical("stem vertical"),
                KeepSpan(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopBar", "StemTop", "TopEnd", "TopStroke", 2, 0, false,
                PreferHorizontal("top bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("MidBar", "StemMid", "MidEnd", "MidStroke", 3, 0, false,
                PreferHorizontal("mid bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(8), new Proportion(2), "mid bar keeps width")),
            Carrier("BottomBar", "StemBottom", "BottomEnd", "BottomStroke", 4, 0, false,
                PreferHorizontal("bottom bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "bottom bar keeps width")),
        ];

        return CreateState("CapitalEAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterF(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "StemTop", Axis.Zero,
                NearLeftSide("stem top near side", 18),
                NearTopline("stem top near topline")),
            AnchoredSite(rng, env, "StemMid", Axis.Zero,
                NearLeftSide("stem middle near side", 18),
                NearMidline(midlineRatio, "stem middle near midline")),
            AnchoredSite(rng, env, "StemBottom", Axis.Zero,
                NearLeftSide("stem bottom near side", 18),
                OnBaseline("stem bottom on baseline")),
            AnchoredSite(rng, env, "TopEnd", Axis.Zero,
                NearRightSide("top bar reaches right", 82),
                NearTopline("top bar stays high")),
            AnchoredSite(rng, env, "MidEnd", Axis.Zero,
                NearRightSide("middle bar reaches center-right", 64),
                NearMidline(midlineRatio, "middle bar near midline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                PreferVertical("stem vertical"),
                KeepSpan(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopBar", "StemTop", "TopEnd", "TopStroke", 2, 0, false,
                PreferHorizontal("top bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("MidBar", "StemMid", "MidEnd", "MidStroke", 3, 0, false,
                PreferHorizontal("mid bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(8), new Proportion(2), "mid bar keeps width")),
        ];

        return CreateState("CapitalFAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterG(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopRight", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(16, 100), new Proportion(4), "top end near right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(16, 100), new Proportion(1, 10), new Proportion(5), "top end near topline")),
            Site(rng, env, "LeftMid", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(12, 100), new Proportion(6), "left middle reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(5), "left middle near midline")),
            Site(rng, env, "BottomRight", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(16, 100), new Proportion(4), "bottom end near right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(84, 100), new Proportion(1, 10), new Proportion(5), "bottom end near baseline")),
            Site(rng, env, "BarLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(52, 100), new Proportion(12, 100), new Proportion(4), "inner bar starts near center-right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(58, 100), new Proportion(1, 10), new Proportion(5), "inner bar below midline")),
            Site(rng, env, "BarCorner", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(12, 100), new Proportion(4), "inner corner reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(58, 100), new Proportion(1, 10), new Proportion(5), "inner corner stays below midline")),
            Site(rng, env, "BarDown", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(12, 100), new Proportion(4), "inner hook stays on the right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(88, 100), new Proportion(1, 10), new Proportion(5), "inner hook drops near the lower endpoint")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("UpperArc", "TopRight", "LeftMid", "CurveStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper arc turns left"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(10), new Proportion(2), "upper arc keeps span")),
            Carrier("LowerArc", "LeftMid", "BottomRight", "CurveStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "lower arc turns right"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(10), new Proportion(2), "lower arc keeps span")),
            Carrier("Cross", "BarLeft", "BarCorner", "BarStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bar horizontal"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(6), new Proportion(2), "bar keeps width")),
            Carrier("Hook", "BarCorner", "BarDown", "BarStroke", 2, 1, false,
                new CarrierDirectionDesire(Axis.I, new Proportion(1, 10), new Proportion(4), "hook drops into place"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(4), new Proportion(2), "hook keeps a short drop")),
        ];

        return CreateState("CapitalGAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterI(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "StemTop", Axis.Zero,
                NearCenter("stem top near center", 12, 5),
                NearTopline("stem top near topline")),
            AnchoredSite(rng, env, "StemBottom", Axis.Zero,
                NearCenter("stem bottom near center", 12, 5),
                OnBaseline("stem bottom on baseline")),
            AnchoredSite(rng, env, "TopLeft", Axis.Zero,
                HorizontalAt(new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "top cap starts left"),
                NearTopline("top cap stays high")),
            AnchoredSite(rng, env, "TopRight", Axis.Zero,
                HorizontalAt(new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "top cap reaches right"),
                NearTopline("top cap stays high")),
            AnchoredSite(rng, env, "BottomLeft", Axis.Zero,
                HorizontalAt(new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "bottom cap starts left"),
                OnBaseline("bottom cap on baseline")),
            AnchoredSite(rng, env, "BottomRight", Axis.Zero,
                HorizontalAt(new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "bottom cap reaches right"),
                OnBaseline("bottom cap on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                PreferVertical("stem vertical"),
                KeepSpan(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopCap", "TopLeft", "TopRight", "TopStroke", 2, 0, false,
                PreferHorizontal("top cap horizontal"),
                KeepSpan(new Proportion(1), new Proportion(5), new Proportion(2), "top cap keeps width")),
            Carrier("BottomCap", "BottomLeft", "BottomRight", "BottomStroke", 3, 0, false,
                PreferHorizontal("bottom cap horizontal"),
                KeepSpan(new Proportion(1), new Proportion(5), new Proportion(2), "bottom cap keeps width")),
        ];

        return CreateState("CapitalIAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterJ(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "Top", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(58, 100), new Proportion(12, 100), new Proportion(4), "top starts center-right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top near topline")),
            Site(rng, env, "Bottom", Axis.I,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(58, 100), new Proportion(12, 100), new Proportion(4), "hook pivot near center-right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(78, 100), new Proportion(1, 10), new Proportion(5), "hook pivot low")),
            Site(rng, env, "HookLeft", Axis.NegativeOne,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(28, 100), new Proportion(14, 100), new Proportion(4), "hook reaches left"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "hook lands on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "Bottom", "Top", "HookStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "stem keeps span")),
            Carrier("Hook", "Bottom", "HookLeft", "HookStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "hook bends left"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(6), new Proportion(2), "hook keeps span")),
        ];

        return CreateState("CapitalJAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterK(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "StemMid", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem middle near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "stem middle near midline")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "UpperTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "upper arm reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "upper arm reaches high")),
            Site(rng, env, "LowerTarget", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(14, 100), new Proportion(4), "lower arm reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "lower arm reaches low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("UpperArm", "StemMid", "UpperTarget", "UpperStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "upper arm rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper arm keeps span")),
            Carrier("LowerArm", "StemMid", "LowerTarget", "LowerStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "lower arm falls"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "lower arm keeps span")),
        ];

        return CreateState("CapitalKAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterN(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "LeftTop", Axis.Zero,
                NearLeftSide("left top near side", 18),
                NearTopline("left top near topline")),
            AnchoredSite(rng, env, "LeftBottom", Axis.Zero,
                NearLeftSide("left bottom near side", 18),
                OnBaseline("left bottom on baseline")),
            AnchoredSite(rng, env, "RightTop", Axis.Zero,
                NearRightSide("right top near side", 82),
                NearTopline("right top near topline")),
            AnchoredSite(rng, env, "RightBottom", Axis.Zero,
                NearRightSide("right bottom near side", 82),
                OnBaseline("right bottom on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStem", "LeftBottom", "LeftTop", "LeftStroke", 1, 0, true,
                PreferVertical("left stem vertical"),
                KeepSpan(new Proportion(4), new Proportion(13), new Proportion(2), "left stem keeps span")),
            Carrier("Diagonal", "LeftTop", "RightBottom", "DiagonalStroke", 2, 0, false,
                PreferDirection(LetterFormationDirections.DownRight, "diagonal descends", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(4), new Proportion(14), new Proportion(2), "diagonal keeps span")),
            Carrier("RightStem", "RightBottom", "RightTop", "RightStroke", 3, 0, true,
                PreferVertical("right stem vertical"),
                KeepSpan(new Proportion(4), new Proportion(13), new Proportion(2), "right stem keeps span")),
        ];

        return CreateState("CapitalNAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterO(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "Top", Axis.One,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "top near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top near topline")),
            Site(rng, env, "Right", Axis.I,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(5), "right reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(1, 10), new Proportion(5), "right near center")),
            Site(rng, env, "Bottom", Axis.NegativeOne,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "bottom near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom on baseline")),
            Site(rng, env, "Left", Axis.NegativeI,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(5), "left reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(1, 10), new Proportion(5), "left near center")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("TopRight", "Top", "Right", "LoopStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "top right descends"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "top right keeps span")),
            Carrier("BottomRight", "Right", "Bottom", "LoopStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "bottom right turns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "bottom right keeps span")),
            Carrier("BottomLeft", "Bottom", "Left", "LoopStroke", 1, 2, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 8), new Proportion(4), "bottom left rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "bottom left keeps span")),
            Carrier("TopLeft", "Left", "Top", "LoopStroke", 1, 3, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "top left returns"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "top left keeps span")),
        ];

        return CreateState("CapitalOAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterP(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "UpperStart", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper start near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "upper start near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "upper bowl stem join")),
            Site(rng, env, "UpperJoin", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper join near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "upper join near midline")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "UpperOuter", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(16, 100), new Proportion(4), "upper bowl reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(24, 100), new Proportion(1, 10), new Proportion(5), "upper bowl sits high")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("UpperFront", "UpperStart", "UpperOuter", "UpperStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "upper front rounds right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper front keeps span")),
            Carrier("UpperBack", "UpperOuter", "UpperJoin", "UpperStroke", 2, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper back returns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper back keeps span")),
        ];

        return CreateState("CapitalPAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterQ(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "Top", Axis.One,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "top near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top near topline")),
            Site(rng, env, "Right", Axis.I,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(5), "right reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(1, 10), new Proportion(5), "right near center")),
            Site(rng, env, "Bottom", Axis.NegativeOne,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "bottom near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom on baseline")),
            Site(rng, env, "Left", Axis.NegativeI,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(5), "left reaches side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(1, 10), new Proportion(5), "left near center")),
            Site(rng, env, "TailEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(12, 100), new Proportion(4), "tail reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(92, 100), new Proportion(1, 10), new Proportion(5), "tail drops low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("TopRight", "Top", "Right", "LoopStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "top right descends"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "top right keeps span")),
            Carrier("BottomRight", "Right", "Bottom", "LoopStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "bottom right turns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "bottom right keeps span")),
            Carrier("BottomLeft", "Bottom", "Left", "LoopStroke", 1, 2, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 8), new Proportion(4), "bottom left rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "bottom left keeps span")),
            Carrier("TopLeft", "Left", "Top", "LoopStroke", 1, 3, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "top left returns"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "top left keeps span")),
            Carrier("Tail", "Bottom", "TailEnd", "TailStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "tail falls right"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(5), new Proportion(2), "tail keeps span")),
        ];

        return CreateState("CapitalQAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterR(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "UpperStart", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper start near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "upper start near topline"),
                new JoinSiteDesire("StemTop", new Proportion(1, 2), new Proportion(3), new Proportion(4), "upper bowl stem join")),
            Site(rng, env, "UpperJoin", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "upper join near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "upper join near midline"),
                new JoinSiteDesire("LegJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "shared leg junction")),
            Site(rng, env, "LegJoin", LetterFormationDirections.DownRight,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "leg join near stem"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "leg join near midline"),
                new JoinSiteDesire("UpperJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "shared leg junction")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "UpperOuter", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(16, 100), new Proportion(4), "upper bowl reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(24, 100), new Proportion(1, 10), new Proportion(5), "upper bowl sits high")),
            Site(rng, env, "LegEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "leg reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "leg lands low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("UpperFront", "UpperStart", "UpperOuter", "UpperStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "upper front rounds right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper front keeps span")),
            Carrier("UpperBack", "UpperOuter", "UpperJoin", "UpperStroke", 2, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper back returns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "upper back keeps span")),
            Carrier("Leg", "LegJoin", "LegEnd", "LegStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "leg descends right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "leg keeps span")),
        ];

        return CreateState("CapitalRAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterS(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopRight", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(14, 100), new Proportion(4), "top starts right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(14, 100), new Proportion(1, 10), new Proportion(5), "top starts high")),
            Site(rng, env, "UpperLeft", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(26, 100), new Proportion(14, 100), new Proportion(4), "upper bend goes left"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(38, 100), new Proportion(1, 10), new Proportion(5), "upper bend stays above middle")),
            Site(rng, env, "Center", new Axis(0, 1, 2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(54, 100), new Proportion(10, 100), new Proportion(4), "center leans right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 2), new Proportion(1, 10), new Proportion(5), "center near midline")),
            Site(rng, env, "LowerRight", new Axis(2, 1, 0, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(74, 100), new Proportion(14, 100), new Proportion(4), "lower bend goes right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(66, 100), new Proportion(1, 10), new Proportion(5), "lower bend stays below middle")),
            Site(rng, env, "BottomLeft", new Axis(0, 1, -2, 1),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(26, 100), new Proportion(14, 100), new Proportion(4), "bottom finishes left"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(88, 100), new Proportion(1, 10), new Proportion(5), "bottom stays low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("UpperCurve", "TopRight", "UpperLeft", "CurveStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "upper curve turns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "upper curve keeps span")),
            Carrier("UpperInner", "UpperLeft", "Center", "CurveStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "upper inner turns right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "upper inner keeps span")),
            Carrier("LowerInner", "Center", "LowerRight", "CurveStroke", 1, 2, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "lower inner turns right"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "lower inner keeps span")),
            Carrier("LowerCurve", "LowerRight", "BottomLeft", "CurveStroke", 1, 3, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "lower curve turns left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "lower curve keeps span")),
        ];

        return CreateState("CapitalSAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterU(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "left top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "left top near topline")),
            Site(rng, env, "BottomLeft", new Axis(new Proportion(1, 2), Proportion.Zero),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(24, 100), new Proportion(14, 100), new Proportion(4), "left bottom tucks inward"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(94, 100), new Proportion(1, 10), new Proportion(5), "left bottom near baseline")),
            Site(rng, env, "BottomRight", new Axis(new Proportion(-1, 2), Proportion.Zero),
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(76, 100), new Proportion(14, 100), new Proportion(4), "right bottom tucks inward"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(94, 100), new Proportion(1, 10), new Proportion(5), "right bottom near baseline")),
            Site(rng, env, "TopRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "right top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "right top near topline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftSide", "BottomLeft", "TopLeft", "CurveStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "left side vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "left side keeps span")),
            Carrier("BottomCurve", "BottomLeft", "BottomRight", "CurveStroke", 1, 1, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bottom curve spans across"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "bottom curve keeps width")),
            Carrier("RightSide", "BottomRight", "TopRight", "CurveStroke", 1, 2, false,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "right side vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "right side keeps span")),
        ];

        return CreateState("CapitalUAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterV(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "LeftTop", Axis.Zero,
                NearLeftSide("left top near side", 22),
                NearTopline("left top high", 10)),
            AnchoredSite(rng, env, "Bottom", Axis.Zero,
                NearCenter("bottom near center"),
                OnBaseline("bottom on baseline")),
            AnchoredSite(rng, env, "RightTop", Axis.Zero,
                NearRightSide("right top near side", 78),
                NearTopline("right top high", 10)),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStroke", "Bottom", "LeftTop", "LeftStroke", 1, 0, true,
                PreferDirection(LetterFormationDirections.UpLeft, "left stroke rises", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "left stroke keeps span")),
            Carrier("RightStroke", "Bottom", "RightTop", "RightStroke", 2, 0, false,
                PreferDirection(LetterFormationDirections.UpRight, "right stroke rises", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "right stroke keeps span")),
        ];

        return CreateState("CapitalVAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterW(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "LeftTop", Axis.Zero,
                NearLeftSide("left top near side", 14),
                NearTopline("left top high", 10)),
            AnchoredSite(rng, env, "LeftValley", Axis.Zero,
                HorizontalAt(new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "left valley left of center"),
                OnBaseline("left valley on baseline")),
            AnchoredSite(rng, env, "MiddlePeak", Axis.Zero,
                NearCenter("middle peak near center"),
                NearTopline("middle peak high", 12)),
            AnchoredSite(rng, env, "RightValley", Axis.Zero,
                HorizontalAt(new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "right valley right of center"),
                OnBaseline("right valley on baseline")),
            AnchoredSite(rng, env, "RightTop", Axis.Zero,
                NearRightSide("right top near side", 86),
                NearTopline("right top high", 10)),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStroke", "LeftValley", "LeftTop", "LeftStroke", 1, 0, true,
                PreferDirection(LetterFormationDirections.UpLeft, "left stroke rises", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "left stroke keeps span")),
            Carrier("RiseStroke", "LeftValley", "MiddlePeak", "RiseStroke", 2, 0, false,
                PreferDirection(LetterFormationDirections.UpRight, "rise stroke climbs", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "rise stroke keeps span")),
            Carrier("FallStroke", "RightValley", "MiddlePeak", "FallStroke", 3, 0, true,
                PreferDirection(LetterFormationDirections.UpLeft, "fall stroke rises", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "fall stroke keeps span")),
            Carrier("RightStroke", "RightValley", "RightTop", "RightStroke", 4, 0, false,
                PreferDirection(LetterFormationDirections.UpRight, "right stroke rises", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(3), new Proportion(12), new Proportion(2), "right stroke keeps span")),
        ];

        return CreateState("CapitalWAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterX(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "TopLeft", Axis.Zero,
                NearLeftSide("top left near side", 20),
                NearTopline("top left high", 10)),
            AnchoredSite(rng, env, "BottomRight", Axis.Zero,
                NearRightSide("bottom right near side", 80),
                OnBaseline("bottom right low")),
            AnchoredSite(rng, env, "TopRight", Axis.Zero,
                NearRightSide("top right near side", 80),
                NearTopline("top right high", 10)),
            AnchoredSite(rng, env, "BottomLeft", Axis.Zero,
                NearLeftSide("bottom left near side", 20),
                OnBaseline("bottom left low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("ForwardSlash", "TopLeft", "BottomRight", "ForwardStroke", 1, 0, false,
                PreferDirection(LetterFormationDirections.DownRight, "forward slash descends", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(4), new Proportion(14), new Proportion(2), "forward slash keeps span")),
            Carrier("BackSlash", "TopRight", "BottomLeft", "BackStroke", 2, 0, false,
                PreferDirection(LetterFormationDirections.DownLeft, "back slash descends", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(4), new Proportion(14), new Proportion(2), "back slash keeps span")),
        ];

        return CreateState("CapitalXAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterZ(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            AnchoredSite(rng, env, "TopLeft", Axis.Zero,
                NearLeftSide("top left near side", 18),
                NearTopline("top left high", 10)),
            AnchoredSite(rng, env, "TopRight", Axis.Zero,
                NearRightSide("top right near side", 82),
                NearTopline("top right high", 10)),
            AnchoredSite(rng, env, "BottomLeft", Axis.Zero,
                NearLeftSide("bottom left near side", 18),
                OnBaseline("bottom left low")),
            AnchoredSite(rng, env, "BottomRight", Axis.Zero,
                NearRightSide("bottom right near side", 82),
                OnBaseline("bottom right low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("TopBar", "TopLeft", "TopRight", "TopStroke", 1, 0, false,
                PreferHorizontal("top bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("Diagonal", "TopRight", "BottomLeft", "DiagonalStroke", 2, 0, false,
                PreferDirection(LetterFormationDirections.DownLeft, "diagonal descends", new Proportion(1, 8), new Proportion(4)),
                KeepSpan(new Proportion(4), new Proportion(14), new Proportion(2), "diagonal keeps span")),
            Carrier("BottomBar", "BottomLeft", "BottomRight", "BottomStroke", 3, 0, false,
                PreferHorizontal("bottom bar horizontal"),
                KeepSpan(new Proportion(2), new Proportion(10), new Proportion(2), "bottom bar keeps width")),
        ];

        return CreateState("CapitalZAssembly", env, sites, carriers);
    }

    private static LetterFormationState CreateState(
        string key,
        LetterFormationEnvironment environment,
        IReadOnlyList<LetterFormationSiteState> sites,
        IReadOnlyList<LetterFormationCarrierState> carriers) =>
        new(key, 0, environment, sites, carriers, []);

    private static LetterFormationSiteState AnchoredSite(
        Random rng,
        LetterFormationEnvironment environment,
        string id,
        Axis axis,
        FrameProjectionDesire horizontal,
        FrameProjectionDesire vertical,
        params LetterFormationDesire[] extras) =>
        Site(rng, environment, id, axis, MergeDesires(horizontal, vertical, extras));

    private static FrameProjectionDesire HorizontalAt(
        Proportion target,
        Proportion tolerance,
        Proportion weight,
        string description) =>
        new(LetterFormationDirections.Horizontal, target, tolerance, weight, description);

    private static FrameProjectionDesire VerticalAt(
        Proportion target,
        Proportion tolerance,
        Proportion weight,
        string description) =>
        new(LetterFormationDirections.Vertical, target, tolerance, weight, description);

    private static FrameProjectionDesire NearLeftSide(
        string description,
        int targetPercent = 20,
        int tolerancePercent = 14,
        int weight = 4) =>
        HorizontalAt(new Proportion(targetPercent, 100), new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static FrameProjectionDesire NearRightSide(
        string description,
        int targetPercent = 80,
        int tolerancePercent = 14,
        int weight = 4) =>
        HorizontalAt(new Proportion(targetPercent, 100), new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static FrameProjectionDesire NearCenter(
        string description,
        int tolerancePercent = 10,
        int weight = 5) =>
        HorizontalAt(new Proportion(1, 2), new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static FrameProjectionDesire NearTopline(
        string description,
        int targetPercent = 8,
        int tolerancePercent = 10,
        int weight = 5) =>
        VerticalAt(new Proportion(targetPercent, 100), new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static FrameProjectionDesire OnBaseline(
        string description,
        int tolerancePercent = 10,
        int weight = 5) =>
        VerticalAt(Proportion.One, new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static FrameProjectionDesire NearMidline(
        Proportion midlineRatio,
        string description,
        int tolerancePercent = 10,
        int weight = 6) =>
        VerticalAt(midlineRatio, new Proportion(tolerancePercent, 100), new Proportion(weight), description);

    private static SiteProjectionDesire LevelWith(
        string otherId,
        string description,
        Proportion? tolerance = null,
        Proportion? weight = null) =>
        new(LetterFormationDirections.Vertical, otherId, Proportion.Zero, tolerance ?? new Proportion(1, 12), weight ?? new Proportion(5), description);

    private static JoinSiteDesire JoinWith(
        string otherId,
        string description,
        Proportion? captureRange = null,
        Proportion? attraction = null,
        Proportion? retention = null) =>
        new(otherId, captureRange ?? new Proportion(2, 5), attraction ?? new Proportion(2), retention ?? new Proportion(4), description);

    private static CarrierDirectionDesire PreferDirection(
        Axis direction,
        string description,
        Proportion? tolerance = null,
        Proportion? weight = null) =>
        new(direction, tolerance ?? new Proportion(1, 10), weight ?? new Proportion(4), description);

    private static CarrierDirectionDesire PreferVertical(
        string description,
        Proportion? tolerance = null,
        Proportion? weight = null) =>
        PreferDirection(LetterFormationDirections.Vertical, description, tolerance, weight);

    private static CarrierDirectionDesire PreferHorizontal(
        string description,
        Proportion? tolerance = null,
        Proportion? weight = null) =>
        PreferDirection(LetterFormationDirections.Horizontal, description, tolerance, weight);

    private static CarrierSpanDesire KeepSpan(
        Proportion minimum,
        Proportion preferred,
        Proportion weight,
        string description) =>
        new(minimum, preferred, weight, description);

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
        string? strokeId = null,
        int strokeOrder = 0,
        int strokeSegmentOrder = 0,
        bool reverseForStroke = false,
        params LetterFormationDesire[] desires) =>
        new(id, startSiteId, endSiteId, desires, strokeId, strokeOrder, strokeSegmentOrder, reverseForStroke);

    private static LetterFormationDesire[] MergeDesires(
        LetterFormationDesire horizontal,
        LetterFormationDesire vertical,
        params LetterFormationDesire[] extras)
    {
        LetterFormationDesire[] merged = new LetterFormationDesire[2 + extras.Length];
        merged[0] = horizontal;
        merged[1] = vertical;
        Array.Copy(extras, 0, merged, 2, extras.Length);
        return merged;
    }

    private static Proportion RandomRatio(Random rng, int minPercent, int maxPercent) =>
        new Proportion(rng.Next(minPercent, maxPercent + 1), 100);

    private static Proportion SafeRatio(Proportion numerator, Proportion denominator) =>
        denominator.IsZero ? Proportion.Zero : numerator / denominator;
}
