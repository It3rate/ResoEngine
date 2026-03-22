using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationPresetFactory
{
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
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Horizontal, new Proportion(1, 5), new Proportion(1, 10), new Proportion(3), "base near left side"),
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Vertical, Proportion.One, new Proportion(1, 16), new Proportion(5), "base on baseline"),
                ]),
            new LetterFormationSiteState(
                "RightBase",
                env.ResolveRelativePoint(rightBaseRatio, Proportion.One),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Horizontal, new Proportion(4, 5), new Proportion(1, 10), new Proportion(3), "base near right side"),
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Vertical, Proportion.One, new Proportion(1, 16), new Proportion(5), "base on baseline"),
                ]),
            new LetterFormationSiteState(
                "Apex",
                env.ResolveRelativePoint(apexRatio, apexHeightRatio),
                new Axis(0, 1, 0, 1),
                PlanarOffset.Zero,
                [
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Horizontal, new Proportion(1, 2), new Proportion(1, 10), new Proportion(4), "apex near centerline"),
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Vertical, Proportion.Zero, new Proportion(1, 10), new Proportion(5), "apex near topline"),
                ]),
            new LetterFormationSiteState(
                "LeftBar",
                env.ResolveRelativePoint(leftBarRatio, barHeightRatio),
                new Axis(-2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteCoordinateDesire(LetterFormationCoordinateAxis.Vertical, "RightBar", Proportion.Zero, new Proportion(1, 20), new Proportion(4), "crossbar stays level"),
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(2), "crossbar near midline"),
                ]),
            new LetterFormationSiteState(
                "RightBar",
                env.ResolveRelativePoint(rightBarRatio, barHeightRatio),
                new Axis(2, 1, 0, -1),
                PlanarOffset.Zero,
                [
                    new SiteCoordinateDesire(LetterFormationCoordinateAxis.Vertical, "LeftBar", Proportion.Zero, new Proportion(1, 20), new Proportion(4), "crossbar stays level"),
                    new FrameCoordinateDesire(LetterFormationCoordinateAxis.Vertical, midlineRatio, new Proportion(1, 10), new Proportion(2), "crossbar near midline"),
                ]),
        ];

        IReadOnlyList<LetterFormationCarrierState> carriers =
        [
            new LetterFormationCarrierState(
                "LeftLeg",
                "LeftBase",
                "Apex",
                [
                    new CarrierOrientationDesire(LetterFormationOrientationKind.UpRight, new Proportion(1, 6), new Proportion(3), "left leg rises toward apex"),
                    new CarrierSpanDesire(new Proportion(5), new Proportion(16), new Proportion(2), "left leg keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "RightLeg",
                "RightBase",
                "Apex",
                [
                    new CarrierOrientationDesire(LetterFormationOrientationKind.UpLeft, new Proportion(1, 6), new Proportion(3), "right leg rises toward apex"),
                    new CarrierSpanDesire(new Proportion(5), new Proportion(16), new Proportion(2), "right leg keeps a reasonable span"),
                ]),
            new LetterFormationCarrierState(
                "Crossbar",
                "LeftBar",
                "RightBar",
                [
                    new CarrierOrientationDesire(LetterFormationOrientationKind.Horizontal, new Proportion(1, 12), new Proportion(4), "crossbar prefers horizontal"),
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
