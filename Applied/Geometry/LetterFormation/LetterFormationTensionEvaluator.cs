using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationTensionEvaluator
{
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
        double current = ResolveRelativeProjection(state.Environment, site.Position, desire.Projection);
        double target = ToDouble(desire.RelativeTarget);
        double tolerance = ToDouble(desire.Tolerance);
        double excess = Math.Max(0d, Math.Abs(current - target) - tolerance);
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

        double current = ResolveRawProjection(site.Position, desire.Projection) - ResolveRawProjection(other.Position, desire.Projection);
        double target = ToDouble(desire.Offset);
        double tolerance = ToDouble(desire.Tolerance);
        double excess = Math.Max(0d, Math.Abs(current - target) - tolerance);
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

        double distance = Distance(site.Position, other.Position);
        double capture = ToDouble(desire.CaptureDistance);
        double excess = Math.Max(0d, distance - capture) * Math.Max(1d, ToDouble(desire.Escalation));
        return CreateTension(site.Id, desire.Label, excess, desire.Weight, $"{site.Id} remains unjoined from {desire.OtherSiteId}.");
    }

    private static LetterFormationTension? EvaluateCarrierDirection(
        LetterFormationState state,
        LetterFormationCarrierState carrier,
        CarrierDirectionDesire desire)
    {
        PlanarPoint start = state.GetStartPoint(carrier.Id);
        PlanarPoint end = state.GetEndPoint(carrier.Id);
        (double currentX, double currentY) = Normalize(
            ToDouble(end.Horizontal - start.Horizontal),
            ToDouble(end.Vertical - start.Vertical));
        Proportion desiredHorizontal = desire.PreferredDirection.Dominant;
        Proportion desiredVertical = desire.PreferredDirection.Recessive;
        (double desiredX, double desiredY) = Normalize(
            ToDirectionalComponent(desiredHorizontal),
            ToDirectionalComponent(desiredVertical));

        double tolerance = ToDouble(desire.Tolerance);
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
        double length = Distance(start, end);
        double minimum = ToDouble(desire.Minimum);
        double maximum = ToDouble(desire.Maximum);
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
            FromDouble(excess * Math.Max(0d, ToDouble(weight))),
            description);
    }

    private static double ResolveRelativeProjection(LetterFormationEnvironment environment, PlanarPoint point, Axis projection)
    {
        double current = ResolveRawProjection(point, projection);
        double left = ResolveRawProjection(environment.TopLeft, projection);
        double right = ResolveRawProjection(environment.BottomRight, projection);
        double span = right - left;
        if (Math.Abs(span) < 0.0001d)
        {
            return 0d;
        }

        return (current - left) / span;
    }

    private static double ResolveRawProjection(PlanarPoint point, Axis projection)
    {
        double projectionX = ToDirectionalComponent(projection.Dominant);
        double projectionY = ToDirectionalComponent(projection.Recessive);
        return (ToDouble(point.Horizontal) * projectionX) + (ToDouble(point.Vertical) * projectionY);
    }

    private static (double X, double Y) Normalize(double x, double y)
    {
        double length = Math.Sqrt((x * x) + (y * y));
        if (length < 0.0001d)
        {
            return (0d, 0d);
        }

        return (x / length, y / length);
    }

    private static double Distance(PlanarPoint left, PlanarPoint right)
    {
        double dx = ToDouble(right.Horizontal - left.Horizontal);
        double dy = ToDouble(right.Vertical - left.Vertical);
        return Math.Sqrt((dx * dx) + (dy * dy));
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

        return Math.Max(0d, Math.Abs(current - normalizedTarget) - tolerance);
    }

    private static double ToDouble(Proportion value) => (double)value.Numerator / value.Denominator;

    private static double ToDirectionalComponent(Proportion value)
    {
        if (value.Numerator == 0)
        {
            return 0d;
        }

        return value.Denominator == 0
            ? value.Numerator
            : ToDouble(value);
    }

    private static Proportion FromDouble(double value) =>
        new Proportion((long)Math.Round(value * 1000d), 1000);
}
