using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationTensionEvaluator
{
    private const double SoftAlignmentPersistence = 0.35d;

    public static LetterFormationState Evaluate(LetterFormationState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return state with { Tensions = EvaluateTensions(state) };
    }

    public static IReadOnlyList<LetterFormationTension> EvaluateTensions(LetterFormationState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        List<LetterFormationTension> tensions = [];
        foreach (LetterFormationSiteState site in state.Sites)
        {
            foreach (LetterFormationDesire desire in site.Desires)
            {
                LetterFormationTension? tension = desire switch
                {
                    FrameProjectionDesire frame => EvaluateFrameProjection(state, site, frame),
                    SiteProjectionDesire relation => EvaluateSiteProjection(state, site, relation),
                    JoinSiteDesire join => EvaluateJoinSite(state, site, join),
                    _ => null,
                };

                if (tension is not null)
                {
                    tensions.Add(tension);
                }
            }
        }

        foreach (LetterFormationCarrierState carrier in state.Carriers)
        {
            foreach (LetterFormationDesire desire in carrier.Desires)
            {
                LetterFormationTension? tension = desire switch
                {
                    CarrierDirectionDesire direction => EvaluateCarrierDirection(state, carrier, direction),
                    CarrierSpanDesire span => EvaluateCarrierSpan(state, carrier, span),
                    _ => null,
                };

                if (tension is not null)
                {
                    tensions.Add(tension);
                }
            }
        }

        return tensions;
    }

    private static LetterFormationTension? EvaluateFrameProjection(
        LetterFormationState state,
        LetterFormationSiteState site,
        FrameProjectionDesire desire)
    {
        double current = LetterFormationGeometry.ResolveRelativeProjection(state.Environment, site.Position, desire.Projection);
        double target = LetterFormationGeometry.ToDouble(desire.RelativeTarget);
        double tolerance = LetterFormationGeometry.ToDouble(desire.Tolerance);
        double excess = ResolveAlignmentExcess(Math.Abs(current - target), tolerance);
        return CreateTension(site.Id, desire.Label, excess, desire.Weight, $"{site.Id} misses frame projection target.");
    }

    private static LetterFormationTension? EvaluateSiteProjection(
        LetterFormationState state,
        LetterFormationSiteState site,
        SiteProjectionDesire desire)
    {
        if (!state.TryGetSite(desire.OtherSiteId, out LetterFormationSiteState? other) || other is null)
        {
            return new LetterFormationTension(site.Id, desire.Label, desire.Weight, $"Missing related site '{desire.OtherSiteId}'.");
        }

        double current = LetterFormationGeometry.ResolveRawProjection(site.Position, desire.Projection) - LetterFormationGeometry.ResolveRawProjection(other.Position, desire.Projection);
        double target = LetterFormationGeometry.ToDouble(desire.Offset);
        double tolerance = LetterFormationGeometry.ToDouble(desire.Tolerance);
        double excess = ResolveAlignmentExcess(Math.Abs(current - target), tolerance);
        return CreateTension(site.Id, desire.Label, excess, desire.Weight, $"{site.Id} misses projection relation to {desire.OtherSiteId}.");
    }

    private static LetterFormationTension? EvaluateJoinSite(
        LetterFormationState state,
        LetterFormationSiteState site,
        JoinSiteDesire desire)
    {
        if (!state.TryGetSite(desire.OtherSiteId, out LetterFormationSiteState? other) || other is null)
        {
            return new LetterFormationTension(site.Id, desire.Label, desire.Weight, $"Missing join site '{desire.OtherSiteId}'.");
        }

        double distance = LetterFormationGeometry.Distance(site.Position, other.Position);
        double capture = LetterFormationGeometry.ToDouble(desire.CaptureDistance);
        double excess = Math.Max(0d, distance - capture) * Math.Max(1d, LetterFormationGeometry.ToDouble(desire.Escalation));
        return CreateTension(site.Id, desire.Label, excess, desire.Weight, $"{site.Id} remains unjoined from {desire.OtherSiteId}.");
    }

    private static LetterFormationTension? EvaluateCarrierDirection(
        LetterFormationState state,
        LetterFormationCarrierState carrier,
        CarrierDirectionDesire desire)
    {
        PlanarPoint start = state.GetStartPoint(carrier.Id);
        PlanarPoint end = state.GetEndPoint(carrier.Id);
        (double currentX, double currentY) = LetterFormationGeometry.Normalize(
            LetterFormationGeometry.ToDouble(end.Horizontal - start.Horizontal),
            LetterFormationGeometry.ToDouble(end.Vertical - start.Vertical));
        Proportion desiredHorizontal = desire.PreferredDirection.Dominant;
        Proportion desiredVertical = desire.PreferredDirection.Recessive;
        (double desiredX, double desiredY) = LetterFormationGeometry.Normalize(
            LetterFormationGeometry.ToDirectionalComponent(desiredHorizontal),
            LetterFormationGeometry.ToDirectionalComponent(desiredVertical));

        double tolerance = LetterFormationGeometry.ToDouble(desire.Tolerance);
        double mismatch = 0d;
        mismatch += EvaluateCarrierDirectionComponent(currentX, desiredHorizontal, desiredX, tolerance);
        mismatch += EvaluateCarrierDirectionComponent(currentY, desiredVertical, desiredY, tolerance);

        return CreateTension(carrier.Id, desire.Label, mismatch, desire.Weight, $"{carrier.Id} misses preferred direction.");
    }

    private static LetterFormationTension? EvaluateCarrierSpan(
        LetterFormationState state,
        LetterFormationCarrierState carrier,
        CarrierSpanDesire desire)
    {
        PlanarPoint start = state.GetStartPoint(carrier.Id);
        PlanarPoint end = state.GetEndPoint(carrier.Id);
        double length = LetterFormationGeometry.Distance(start, end);
        double minimum = LetterFormationGeometry.ToDouble(desire.Minimum);
        double maximum = LetterFormationGeometry.ToDouble(desire.Maximum);
        double excess = length < minimum
            ? minimum - length
            : Math.Max(0d, length - maximum);
        return CreateTension(carrier.Id, desire.Label, excess, desire.Weight, $"{carrier.Id} misses preferred span.");
    }

    private static LetterFormationTension? CreateTension(
        string componentId,
        string source,
        double excess,
        Proportion weight,
        string description)
    {
        if (excess <= 0d)
        {
            return null;
        }

        return new LetterFormationTension(
            componentId,
            source,
            LetterFormationGeometry.FromDouble(excess * Math.Max(0d, LetterFormationGeometry.ToDouble(weight))),
            description);
    }

    private static double EvaluateCarrierDirectionComponent(
        double current,
        Proportion desiredComponent,
        double normalizedTarget,
        double tolerance)
    {
        if (desiredComponent.Numerator == 0 && desiredComponent.Denominator == 0)
        {
            return 0d;
        }

        if (desiredComponent.Denominator == 0)
        {
            double projected = current * Math.Sign(desiredComponent.Numerator);
            return projected > tolerance
                ? 0d
                : Math.Max(0d, tolerance - projected);
        }

        return ResolveAlignmentExcess(Math.Abs(current - normalizedTarget), tolerance);
    }

    private static double ResolveAlignmentExcess(double difference, double tolerance)
    {
        if (difference <= 0d)
        {
            return 0d;
        }

        if (tolerance <= 0d)
        {
            return difference;
        }

        if (difference > tolerance)
        {
            return difference - tolerance;
        }

        double ratio = difference / tolerance;
        return tolerance * ratio * ratio * SoftAlignmentPersistence;
    }
}
