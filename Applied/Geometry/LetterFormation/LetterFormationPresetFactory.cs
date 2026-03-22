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
            Site(rng, env, "CrossLeft", Axis.Zero,
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossRight", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "crossbar stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "crossbar near midline"),
                new JoinSiteDesire("LeftJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left crossbar pin")),
            Site(rng, env, "CrossRight", Axis.Zero,
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
            Carrier("LeftLower", "LeftBase", "LeftJoin", "LeftStroke", 1, 1, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left lower rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps a reasonable span")),
            Carrier("LeftUpper", "LeftJoin", "LeftApex", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left upper rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps a reasonable span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight", "CrossStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "crossbar prefers horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "crossbar keeps a moderate width")),
            Carrier("RightUpper", "RightJoin", "RightApex", "RightStroke", 2, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 5), new Proportion(3), "right upper rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps a reasonable span")),
            Carrier("RightLower", "RightBase", "RightJoin", "RightStroke", 2, 1, true,
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
            Site(rng, env, "CrossLeft", Axis.Zero,
                new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossRight", Proportion.Zero, new Proportion(1, 12), new Proportion(5), "bridge stays level"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 12), new Proportion(7), "bridge near midline"),
                new JoinSiteDesire("LeftJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left bridge pin")),
            Site(rng, env, "CrossRight", Axis.Zero,
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
            Carrier("LeftLower", "LeftBase", "LeftJoin", "LeftStroke", 1, 1, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "left lower vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left lower keeps span")),
            Carrier("LeftUpper", "LeftJoin", "LeftTop", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "left upper vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "left upper keeps span")),
            Carrier("Crossbar", "CrossLeft", "CrossRight", "BridgeStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bridge prefers horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "bridge keeps width")),
            Carrier("RightUpper", "RightJoin", "RightTop", "RightStroke", 2, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(3), "right upper vertical"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(12), new Proportion(2), "right upper keeps span")),
            Carrier("RightLower", "RightBase", "RightJoin", "RightStroke", 2, 1, true,
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
            Carrier("Stem", "StemBase", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("LeftBar", "LeftJoin", "LeftTarget", "TopStroke", 2, 0, true,
                new CarrierDirectionDesire(Axis.NegativeOne, new Proportion(1, 10), new Proportion(4), "left bar extends left"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left bar keeps width")),
            Carrier("RightBar", "RightJoin", "RightTarget", "TopStroke", 2, 1, false,
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
            Carrier("Stem", "StemBase", "Junction", "StemStroke", 3, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 12), new Proportion(5), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "stem keeps span")),
            Carrier("LeftArm", "LeftJoin", "LeftTarget", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 12), new Proportion(5), "left arm rises"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left arm keeps span")),
            Carrier("RightArm", "RightJoin", "RightTarget", "RightStroke", 2, 0, true,
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
            Carrier("Stem", "StemBase", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("Foot", "FootJoin", "FootRight", "FootStroke", 2, 0, false,
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
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "StemMid", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem middle near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "stem middle near midline")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "stem bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "TopEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "top bar reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top bar stays high")),
            Site(rng, env, "MidEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(64, 100), new Proportion(14, 100), new Proportion(4), "middle bar reaches center-right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "middle bar near midline")),
            Site(rng, env, "BottomEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "bottom bar reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom bar on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopBar", "StemTop", "TopEnd", "TopStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "top bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("MidBar", "StemMid", "MidEnd", "MidStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "mid bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "mid bar keeps width")),
            Carrier("BottomBar", "StemBottom", "BottomEnd", "BottomStroke", 4, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bottom bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "bottom bar keeps width")),
        ];

        return CreateState("CapitalEAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterF(Random rng, LetterFormationEnvironment env, Proportion midlineRatio)
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
            Site(rng, env, "TopEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "top bar reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top bar stays high")),
            Site(rng, env, "MidEnd", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(64, 100), new Proportion(14, 100), new Proportion(4), "middle bar reaches center-right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(6), "middle bar near midline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopBar", "StemTop", "TopEnd", "TopStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "top bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("MidBar", "StemMid", "MidEnd", "MidStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "mid bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "mid bar keeps width")),
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
            Site(rng, env, "StemTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "stem top near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "stem top near topline")),
            Site(rng, env, "StemBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(12, 100), new Proportion(5), "stem bottom near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "stem bottom on baseline")),
            Site(rng, env, "TopLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "top cap starts left"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top cap stays high")),
            Site(rng, env, "TopRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "top cap reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "top cap stays high")),
            Site(rng, env, "BottomLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "bottom cap starts left"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom cap on baseline")),
            Site(rng, env, "BottomRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "bottom cap reaches right"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom cap on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("Stem", "StemBottom", "StemTop", "StemStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "stem keeps span")),
            Carrier("TopCap", "TopLeft", "TopRight", "TopStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "top cap horizontal"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(5), new Proportion(2), "top cap keeps width")),
            Carrier("BottomCap", "BottomLeft", "BottomRight", "BottomStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bottom cap horizontal"),
                new CarrierSpanDesire(new Proportion(1), new Proportion(5), new Proportion(2), "bottom cap keeps width")),
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
            Site(rng, env, "LeftTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "left top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "left top near topline")),
            Site(rng, env, "LeftBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "left bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "left bottom on baseline")),
            Site(rng, env, "RightTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "right top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(8, 100), new Proportion(1, 10), new Proportion(5), "right top near topline")),
            Site(rng, env, "RightBottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "right bottom near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "right bottom on baseline")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStem", "LeftBottom", "LeftTop", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "left stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "left stem keeps span")),
            Carrier("Diagonal", "LeftTop", "RightBottom", "DiagonalStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "diagonal descends"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(14), new Proportion(2), "diagonal keeps span")),
            Carrier("RightStem", "RightBottom", "RightTop", "RightStroke", 3, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 10), new Proportion(4), "right stem vertical"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(13), new Proportion(2), "right stem keeps span")),
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
            Site(rng, env, "LeftTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(22, 100), new Proportion(14, 100), new Proportion(4), "left top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "left top high")),
            Site(rng, env, "Bottom", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(5), "bottom near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom on baseline")),
            Site(rng, env, "RightTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(78, 100), new Proportion(14, 100), new Proportion(4), "right top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "right top high")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStroke", "Bottom", "LeftTop", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 8), new Proportion(4), "left stroke rises"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "left stroke keeps span")),
            Carrier("RightStroke", "Bottom", "RightTop", "RightStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "right stroke rises"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "right stroke keeps span")),
        ];

        return CreateState("CapitalVAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterW(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "LeftTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(14, 100), new Proportion(14, 100), new Proportion(4), "left top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "left top high")),
            Site(rng, env, "LeftValley", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(32, 100), new Proportion(12, 100), new Proportion(4), "left valley left of center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "left valley on baseline")),
            Site(rng, env, "MiddlePeak", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(10, 100), new Proportion(5), "middle peak near center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(12, 100), new Proportion(1, 10), new Proportion(5), "middle peak high")),
            Site(rng, env, "RightValley", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(68, 100), new Proportion(12, 100), new Proportion(4), "right valley right of center"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "right valley on baseline")),
            Site(rng, env, "RightTop", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(86, 100), new Proportion(14, 100), new Proportion(4), "right top near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "right top high")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("LeftStroke", "LeftValley", "LeftTop", "LeftStroke", 1, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 8), new Proportion(4), "left stroke rises"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "left stroke keeps span")),
            Carrier("RiseStroke", "LeftValley", "MiddlePeak", "RiseStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "rise stroke climbs"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "rise stroke keeps span")),
            Carrier("FallStroke", "RightValley", "MiddlePeak", "FallStroke", 3, 0, true,
                new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 8), new Proportion(4), "fall stroke rises"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "fall stroke keeps span")),
            Carrier("RightStroke", "RightValley", "RightTop", "RightStroke", 4, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 8), new Proportion(4), "right stroke rises"),
                new CarrierSpanDesire(new Proportion(3), new Proportion(12), new Proportion(2), "right stroke keeps span")),
        ];

        return CreateState("CapitalWAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterX(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(20, 100), new Proportion(14, 100), new Proportion(4), "top left near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "top left high")),
            Site(rng, env, "BottomRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(80, 100), new Proportion(14, 100), new Proportion(4), "bottom right near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom right low")),
            Site(rng, env, "TopRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(80, 100), new Proportion(14, 100), new Proportion(4), "top right near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "top right high")),
            Site(rng, env, "BottomLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(20, 100), new Proportion(14, 100), new Proportion(4), "bottom left near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom left low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("ForwardSlash", "TopLeft", "BottomRight", "ForwardStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownRight, new Proportion(1, 8), new Proportion(4), "forward slash descends"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(14), new Proportion(2), "forward slash keeps span")),
            Carrier("BackSlash", "TopRight", "BottomLeft", "BackStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "back slash descends"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(14), new Proportion(2), "back slash keeps span")),
        ];

        return CreateState("CapitalXAssembly", env, sites, carriers);
    }

    private static LetterFormationState BuildLetterZ(Random rng, LetterFormationEnvironment env)
    {
        IReadOnlyList<LetterFormationSiteState> sites =
        [
            Site(rng, env, "TopLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "top left near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "top left high")),
            Site(rng, env, "TopRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "top right near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(10, 100), new Proportion(1, 10), new Proportion(5), "top right high")),
            Site(rng, env, "BottomLeft", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(18, 100), new Proportion(14, 100), new Proportion(4), "bottom left near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom left low")),
            Site(rng, env, "BottomRight", Axis.Zero,
                new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(82, 100), new Proportion(14, 100), new Proportion(4), "bottom right near side"),
                new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "bottom right low")),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            Carrier("TopBar", "TopLeft", "TopRight", "TopStroke", 1, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "top bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "top bar keeps width")),
            Carrier("Diagonal", "TopRight", "BottomLeft", "DiagonalStroke", 2, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.DownLeft, new Proportion(1, 8), new Proportion(4), "diagonal descends"),
                new CarrierSpanDesire(new Proportion(4), new Proportion(14), new Proportion(2), "diagonal keeps span")),
            Carrier("BottomBar", "BottomLeft", "BottomRight", "BottomStroke", 3, 0, false,
                new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "bottom bar horizontal"),
                new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "bottom bar keeps width")),
        ];

        return CreateState("CapitalZAssembly", env, sites, carriers);
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
        string? strokeId = null,
        int strokeOrder = 0,
        int strokeSegmentOrder = 0,
        bool reverseForStroke = false,
        params LetterFormationDesire[] desires) =>
        new(id, startSiteId, endSiteId, desires, strokeId, strokeOrder, strokeSegmentOrder, reverseForStroke);

    private static Proportion RandomRatio(Random rng, int minPercent, int maxPercent) =>
        new Proportion(rng.Next(minPercent, maxPercent + 1), 100);

    private static Proportion SafeRatio(Proportion numerator, Proportion denominator) =>
        denominator.IsZero ? Proportion.Zero : numerator / denominator;
}
