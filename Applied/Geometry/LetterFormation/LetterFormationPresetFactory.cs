using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationPresetFactory
{
    public static LetterFormationState CreateCapitalAAssemblySeed(
        Random? random = null,
        LetterFormationEnvironment? environment = null)
    {
        Random rng = random ?? Random.Shared;
        LetterFormationEnvironment env = environment ?? LetterFormationEnvironment.CreateLetterBox();

        Proportion midlineRatio = SafeRatio(env.MidlineY - env.Top, env.Height);

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            new LetterFormationSiteState(
                "LeftBase",
                env.ResolveRelativePoint(RandomRatio(rng, 4, 30), RandomRatio(rng, 80, 98)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(3, 20), new Proportion(3), "base near left side"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "base on baseline"),
                ]),
            new LetterFormationSiteState(
                "LeftJoin",
                env.ResolveRelativePoint(RandomRatio(rng, 4, 48), RandomRatio(rng, 18, 84)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(32, 100), new Proportion(22, 100), new Proportion(2), "left join stays left of center"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(3, 20), new Proportion(2), "left join near crossbar height"),
                    new JoinSiteDesire("CrossLeft", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left crossbar pin"),
                ]),
            new LetterFormationSiteState(
                "LeftApex",
                env.ResolveRelativePoint(RandomRatio(rng, 6, 48), RandomRatio(rng, 0, 42)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(47, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 12), new Proportion(1, 10), new Proportion(4), "apex near topline"),
                    new JoinSiteDesire("RightApex", new Proportion(1, 2), new Proportion(3), new Proportion(5), "apex join"),
                ]),
            new LetterFormationSiteState(
                "CrossLeft",
                env.ResolveRelativePoint(RandomRatio(rng, 0, 52), RandomRatio(rng, 16, 88)),
                new Axis(-2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossRight", Proportion.Zero, new Proportion(1, 8), new Proportion(4), "crossbar stays level"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(3, 20), new Proportion(3), "crossbar near midline"),
                    new JoinSiteDesire("LeftJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "left crossbar pin"),
                ]),
            new LetterFormationSiteState(
                "CrossRight",
                env.ResolveRelativePoint(RandomRatio(rng, 48, 100), RandomRatio(rng, 16, 88)),
                new Axis(2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteProjectionDesire(LetterFormationDirections.Vertical, "CrossLeft", Proportion.Zero, new Proportion(1, 8), new Proportion(4), "crossbar stays level"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(3, 20), new Proportion(3), "crossbar near midline"),
                    new JoinSiteDesire("RightJoin", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right crossbar pin"),
                ]),
            new LetterFormationSiteState(
                "RightApex",
                env.ResolveRelativePoint(RandomRatio(rng, 52, 94), RandomRatio(rng, 0, 42)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(53, 100), new Proportion(16, 100), new Proportion(3), "apex near centerline"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, new Proportion(1, 12), new Proportion(1, 10), new Proportion(4), "apex near topline"),
                    new JoinSiteDesire("LeftApex", new Proportion(1, 2), new Proportion(3), new Proportion(5), "apex join"),
                ]),
            new LetterFormationSiteState(
                "RightJoin",
                env.ResolveRelativePoint(RandomRatio(rng, 52, 96), RandomRatio(rng, 18, 84)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(68, 100), new Proportion(22, 100), new Proportion(2), "right join stays right of center"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(3, 20), new Proportion(2), "right join near crossbar height"),
                    new JoinSiteDesire("CrossRight", new Proportion(2, 5), new Proportion(2), new Proportion(4), "right crossbar pin"),
                ]),
            new LetterFormationSiteState(
                "RightBase",
                env.ResolveRelativePoint(RandomRatio(rng, 70, 96), RandomRatio(rng, 80, 98)),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(3, 20), new Proportion(3), "base near right side"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 10), new Proportion(5), "base on baseline"),
                ]),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            new LetterFormationCarrierState(
                "LeftLower",
                "LeftBase",
                "LeftJoin",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left lower rises"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left lower keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "LeftUpper",
                "LeftJoin",
                "LeftApex",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 5), new Proportion(3), "left upper rises"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "left upper keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "Crossbar",
                "CrossLeft",
                "CrossRight",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 10), new Proportion(4), "crossbar prefers horizontal"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(9), new Proportion(2), "crossbar keeps a moderate width"),
                ]),
            new LetterFormationCarrierState(
                "RightUpper",
                "RightJoin",
                "RightApex",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 5), new Proportion(3), "right upper rises"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right upper keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "RightLower",
                "RightBase",
                "RightJoin",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 5), new Proportion(3), "right lower rises"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(10), new Proportion(2), "right lower keeps a reasonable span"),
                ]),
        ];

        return new LetterFormationState(
            "CapitalAAssembly",
            0,
            env,
            sites,
            carriers,
            []);
    }

    public static LetterFormationState CreateCapitalASeed(
        Random? random = null,
        LetterFormationEnvironment? environment = null)
    {
        Random rng = random ?? Random.Shared;
        LetterFormationEnvironment env = environment ?? LetterFormationEnvironment.CreateLetterBox();

        Proportion midlineRatio = SafeRatio(env.MidlineY - env.Top, env.Height);
        Proportion leftBaseRatio = RandomRatio(rng, 8, 28);
        Proportion rightBaseRatio = RandomRatio(rng, 72, 92);
        Proportion apexRatio = RandomRatio(rng, 35, 65);
        Proportion apexHeightRatio = RandomRatio(rng, 6, 18);
        Proportion leftBarRatio = RandomRatio(rng, 22, 40);
        Proportion rightBarRatio = RandomRatio(rng, 60, 78);
        Proportion barHeightRatio = OffsetRatio(rng, midlineRatio, 6);

        IReadOnlyList<LetterFormationSiteState> sites =
        [
            new LetterFormationSiteState(
                "LeftBase",
                env.ResolveRelativePoint(leftBaseRatio, Proportion.One),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 5), new Proportion(1, 10), new Proportion(3), "base near left side"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 16), new Proportion(5), "base on baseline"),
                ]),
            new LetterFormationSiteState(
                "RightBase",
                env.ResolveRelativePoint(rightBaseRatio, Proportion.One),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(4, 5), new Proportion(1, 10), new Proportion(3), "base near right side"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.One, new Proportion(1, 16), new Proportion(5), "base on baseline"),
                ]),
            new LetterFormationSiteState(
                "Apex",
                env.ResolveRelativePoint(apexRatio, apexHeightRatio),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameProjectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 2), new Proportion(1, 10), new Proportion(4), "apex near centerline"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, Proportion.Zero, new Proportion(1, 10), new Proportion(5), "apex near topline"),
                ]),
            new LetterFormationSiteState(
                "LeftBar",
                env.ResolveRelativePoint(leftBarRatio, barHeightRatio),
                new Axis(-2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteProjectionDesire(LetterFormationDirections.Vertical, "RightBar", Proportion.Zero, new Proportion(1, 20), new Proportion(4), "crossbar stays level"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(2), "crossbar near midline"),
                ]),
            new LetterFormationSiteState(
                "RightBar",
                env.ResolveRelativePoint(rightBarRatio, barHeightRatio),
                new Axis(2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteProjectionDesire(LetterFormationDirections.Vertical, "LeftBar", Proportion.Zero, new Proportion(1, 20), new Proportion(4), "crossbar stays level"),
                    new FrameProjectionDesire(LetterFormationDirections.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(2), "crossbar near midline"),
                ]),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            new LetterFormationCarrierState(
                "LeftLeg",
                "LeftBase",
                "Apex",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpRight, new Proportion(1, 6), new Proportion(3), "left leg rises toward apex"),
                    new CarrierSpanDesire(new Proportion(5), new Proportion(16), new Proportion(2), "left leg keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "RightLeg",
                "RightBase",
                "Apex",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.UpLeft, new Proportion(1, 6), new Proportion(3), "right leg rises toward apex"),
                    new CarrierSpanDesire(new Proportion(5), new Proportion(16), new Proportion(2), "right leg keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "Crossbar",
                "LeftBar",
                "RightBar",
                [
                    new CarrierDirectionDesire(LetterFormationDirections.Horizontal, new Proportion(1, 12), new Proportion(4), "crossbar prefers horizontal"),
                    new CarrierSpanDesire(new Proportion(2), new Proportion(8), new Proportion(2), "crossbar keeps a moderate width"),
                ]),
        ];

        return new LetterFormationState(
            "CapitalA",
            0,
            env,
            sites,
            carriers,
            []);
    }

    private static Proportion RandomRatio(Random rng, int minPercent, int maxPercent) =>
        new Proportion(rng.Next(minPercent, maxPercent + 1), 100);

    private static Proportion OffsetRatio(Random rng, Proportion baseRatio, int maxPercentOffset)
    {
        int offset = rng.Next(-maxPercentOffset, maxPercentOffset + 1);
        Proportion shifted = baseRatio + new Proportion(offset, 100);
        if (shifted < Proportion.Zero)
        {
            return Proportion.Zero;
        }

        if (shifted > Proportion.One)
        {
            return Proportion.One;
        }

        return shifted;
    }

    private static Proportion SafeRatio(Proportion numerator, Proportion denominator) =>
        denominator.IsZero ? Proportion.Zero : numerator / denominator;
}
