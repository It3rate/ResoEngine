using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationStepper
{
    private const double MaxProposalStep = 0.75d;
    private const double ProposalResponsiveness = 0.35d;
    private const double MomentumBlend = 0.2d;
    private const double RandomJitterScale = 0.12d;

    public static IReadOnlyList<LetterFormationProposal> GenerateProposals(LetterFormationState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return GenerateProposalsCore(LetterFormationTensionEvaluator.Evaluate(state));
    }

    public static LetterFormationState Step(LetterFormationState state, Random? random = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        Random rng = random ?? Random.Shared;
        LetterFormationState evaluated = LetterFormationTensionEvaluator.Evaluate(state);
        IReadOnlyList<LetterFormationProposal> proposals = GenerateProposalsCore(evaluated);
        Dictionary<string, IReadOnlyList<LetterFormationProposal>> proposalsBySite = proposals
            .GroupBy(proposal => proposal.SiteId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<LetterFormationProposal>)group.ToArray(), StringComparer.Ordinal);

        IReadOnlyList<LetterFormationSiteState> advancedSites = evaluated.Sites
            .Select(site => AdvanceSite(
                site,
                proposalsBySite.TryGetValue(site.Id, out IReadOnlyList<LetterFormationProposal>? siteProposals) ? siteProposals : [],
                evaluated.Environment,
                rng))
            .ToArray();

        LetterFormationState advanced = evaluated with
        {
            StepIndex = state.StepIndex + 1,
            Sites = advancedSites,
            Tensions = [],
        };

        return LetterFormationTensionEvaluator.Evaluate(advanced);
    }

    private static IReadOnlyList<LetterFormationProposal> GenerateProposalsCore(LetterFormationState evaluatedState)
    {
        Dictionary<(string ComponentId, string Source), LetterFormationTension> activeTensions = evaluatedState.Tensions
            .ToDictionary(tension => (tension.ComponentId, tension.Source));
        List<LetterFormationProposal> proposals = [];

        foreach (LetterFormationSiteState site in evaluatedState.Sites)
        {
            foreach (LetterFormationDesire desire in site.Desires)
            {
                if (!activeTensions.TryGetValue((site.Id, desire.Label), out LetterFormationTension? tension))
                {
                    continue;
                }

                LetterFormationProposal? proposal = desire switch
                {
                    FrameProjectionDesire frame => CreateFrameProjectionProposal(evaluatedState, site, frame, tension),
                    SiteProjectionDesire relation => CreateSiteProjectionProposal(evaluatedState, site, relation, tension),
                    JoinSiteDesire join => CreateJoinSiteProposal(evaluatedState, site, join, tension),
                    _ => null,
                };

                if (proposal is not null)
                {
                    proposals.Add(proposal);
                }
            }
        }

        foreach (LetterFormationCarrierState carrier in evaluatedState.Carriers)
        {
            foreach (LetterFormationDesire desire in carrier.Desires)
            {
                if (!activeTensions.TryGetValue((carrier.Id, desire.Label), out LetterFormationTension? tension))
                {
                    continue;
                }

                IEnumerable<LetterFormationProposal> carrierProposals = desire switch
                {
                    CarrierDirectionDesire direction => CreateCarrierDirectionProposals(evaluatedState, carrier, direction, tension),
                    CarrierSpanDesire span => CreateCarrierSpanProposals(evaluatedState, carrier, span, tension),
                    _ => [],
                };

                proposals.AddRange(carrierProposals);
            }
        }

        return proposals;
    }

    private static LetterFormationProposal? CreateFrameProjectionProposal(
        LetterFormationState state,
        LetterFormationSiteState site,
        FrameProjectionDesire desire,
        LetterFormationTension tension)
    {
        double current = LetterFormationGeometry.ResolveRawProjection(site.Position, desire.Projection);
        double left = LetterFormationGeometry.ResolveRawProjection(state.Environment.TopLeft, desire.Projection);
        double right = LetterFormationGeometry.ResolveRawProjection(state.Environment.BottomRight, desire.Projection);
        double target = left + (LetterFormationGeometry.ToDouble(desire.RelativeTarget) * (right - left));
        double delta = target - current;
        (double directionX, double directionY) = LetterFormationGeometry.ResolveDirection(desire.Projection);
        return CreateSingleSiteProposal(
            site.Id,
            site.Id,
            desire.Label,
            directionX,
            directionY,
            delta,
            tension.Magnitude,
            $"Move {site.Id} toward {desire.Label}.");
    }

    private static LetterFormationProposal? CreateSiteProjectionProposal(
        LetterFormationState state,
        LetterFormationSiteState site,
        SiteProjectionDesire desire,
        LetterFormationTension tension)
    {
        if (!state.TryGetSite(desire.OtherSiteId, out LetterFormationSiteState? other) || other is null)
        {
            return null;
        }

        double current = LetterFormationGeometry.ResolveRawProjection(site.Position, desire.Projection) - LetterFormationGeometry.ResolveRawProjection(other.Position, desire.Projection);
        double target = LetterFormationGeometry.ToDouble(desire.Offset);
        double delta = target - current;
        (double directionX, double directionY) = LetterFormationGeometry.ResolveDirection(desire.Projection);
        return CreateSingleSiteProposal(
            site.Id,
            site.Id,
            desire.Label,
            directionX,
            directionY,
            delta,
            tension.Magnitude,
            $"Move {site.Id} toward projection relation with {desire.OtherSiteId}.");
    }

    private static LetterFormationProposal? CreateJoinSiteProposal(
        LetterFormationState state,
        LetterFormationSiteState site,
        JoinSiteDesire desire,
        LetterFormationTension tension)
    {
        if (!state.TryGetSite(desire.OtherSiteId, out LetterFormationSiteState? other) || other is null)
        {
            return null;
        }

        double dx = LetterFormationGeometry.ToDouble(other.Position.Horizontal - site.Position.Horizontal);
        double dy = LetterFormationGeometry.ToDouble(other.Position.Vertical - site.Position.Vertical);
        (double directionX, double directionY) = LetterFormationGeometry.Normalize(dx, dy);
        double distance = LetterFormationGeometry.Distance(site.Position, other.Position);
        double capture = LetterFormationGeometry.ToDouble(desire.CaptureDistance);
        double excess = Math.Max(0d, distance - capture);
        double step = LetterFormationGeometry.ClampMagnitude(excess * 0.25d, MaxProposalStep);
        return CreateProposal(
            site.Id,
            site.Id,
            desire.Label,
            directionX * step,
            directionY * step,
            tension.Magnitude,
            $"Move {site.Id} toward join with {desire.OtherSiteId}.");
    }

    private static IEnumerable<LetterFormationProposal> CreateCarrierDirectionProposals(
        LetterFormationState state,
        LetterFormationCarrierState carrier,
        CarrierDirectionDesire desire,
        LetterFormationTension tension)
    {
        PlanarPoint start = state.GetStartPoint(carrier.Id);
        PlanarPoint end = state.GetEndPoint(carrier.Id);
        double startX = LetterFormationGeometry.ToDouble(start.Horizontal);
        double startY = LetterFormationGeometry.ToDouble(start.Vertical);
        double endX = LetterFormationGeometry.ToDouble(end.Horizontal);
        double endY = LetterFormationGeometry.ToDouble(end.Vertical);
        double currentDx = endX - startX;
        double currentDy = endY - startY;
        double length = Math.Max(LetterFormationGeometry.Distance(start, end), 1d);

        Proportion desiredHorizontal = desire.PreferredDirection.Dominant;
        Proportion desiredVertical = desire.PreferredDirection.Recessive;
        (double desiredX, double desiredY) = LetterFormationGeometry.Normalize(
            LetterFormationGeometry.ToDirectionalComponent(desiredHorizontal),
            LetterFormationGeometry.ToDirectionalComponent(desiredVertical));

        double targetDx = ResolveCarrierDirectionTarget(currentDx, length, desiredHorizontal, desiredX);
        double targetDy = ResolveCarrierDirectionTarget(currentDy, length, desiredVertical, desiredY);
        double deltaDx = LetterFormationGeometry.ClampMagnitude((targetDx - currentDx) * ProposalResponsiveness, MaxProposalStep);
        double deltaDy = LetterFormationGeometry.ClampMagnitude((targetDy - currentDy) * ProposalResponsiveness, MaxProposalStep);
        if (Math.Abs(deltaDx) < 0.0001d && Math.Abs(deltaDy) < 0.0001d)
        {
            return [];
        }

        return
        [
            CreateProposal(
                carrier.StartSiteId,
                carrier.Id,
                desire.Label,
                -deltaDx / 2d,
                -deltaDy / 2d,
                tension.Magnitude,
                $"Move {carrier.StartSiteId} for {carrier.Id} direction."),
            CreateProposal(
                carrier.EndSiteId,
                carrier.Id,
                desire.Label,
                deltaDx / 2d,
                deltaDy / 2d,
                tension.Magnitude,
                $"Move {carrier.EndSiteId} for {carrier.Id} direction."),
        ];
    }

    private static IEnumerable<LetterFormationProposal> CreateCarrierSpanProposals(
        LetterFormationState state,
        LetterFormationCarrierState carrier,
        CarrierSpanDesire desire,
        LetterFormationTension tension)
    {
        PlanarPoint start = state.GetStartPoint(carrier.Id);
        PlanarPoint end = state.GetEndPoint(carrier.Id);
        double currentLength = LetterFormationGeometry.Distance(start, end);
        double minimum = LetterFormationGeometry.ToDouble(desire.Minimum);
        double maximum = LetterFormationGeometry.ToDouble(desire.Maximum);
        double delta = currentLength < minimum
            ? minimum - currentLength
            : currentLength > maximum
                ? maximum - currentLength
                : 0d;
        if (Math.Abs(delta) < 0.0001d)
        {
            return [];
        }

        (double directionX, double directionY) = currentLength > 0.0001d
            ? LetterFormationGeometry.Normalize(
                LetterFormationGeometry.ToDouble(end.Horizontal - start.Horizontal),
                LetterFormationGeometry.ToDouble(end.Vertical - start.Vertical))
            : ResolveFallbackCarrierDirection(carrier);
        double step = LetterFormationGeometry.ClampMagnitude(delta * 0.25d, MaxProposalStep);

        return
        [
            CreateProposal(
                carrier.StartSiteId,
                carrier.Id,
                desire.Label,
                -(directionX * step) / 2d,
                -(directionY * step) / 2d,
                tension.Magnitude,
                $"Move {carrier.StartSiteId} for {carrier.Id} span."),
            CreateProposal(
                carrier.EndSiteId,
                carrier.Id,
                desire.Label,
                (directionX * step) / 2d,
                (directionY * step) / 2d,
                tension.Magnitude,
                $"Move {carrier.EndSiteId} for {carrier.Id} span."),
        ];
    }

    private static LetterFormationSiteState AdvanceSite(
        LetterFormationSiteState site,
        IReadOnlyList<LetterFormationProposal> proposals,
        LetterFormationEnvironment environment,
        Random random)
    {
        double totalWeight = 0d;
        double weightedHorizontal = 0d;
        double weightedVertical = 0d;

        foreach (LetterFormationProposal proposal in proposals)
        {
            double weight = Math.Max(0.0001d, LetterFormationGeometry.ToDouble(proposal.Strength));
            totalWeight += weight;
            weightedHorizontal += LetterFormationGeometry.ToDouble(proposal.Offset.Horizontal) * weight;
            weightedVertical += LetterFormationGeometry.ToDouble(proposal.Offset.Vertical) * weight;
        }

        double nextHorizontal = totalWeight > 0d ? weightedHorizontal / totalWeight : 0d;
        double nextVertical = totalWeight > 0d ? weightedVertical / totalWeight : 0d;
        nextHorizontal += LetterFormationGeometry.ToDouble(site.Momentum.Horizontal) * MomentumBlend;
        nextVertical += LetterFormationGeometry.ToDouble(site.Momentum.Vertical) * MomentumBlend;

        double jitter = LetterFormationGeometry.ToDouble(environment.RandomMotionWeight) * RandomJitterScale;
        if (jitter > 0d)
        {
            nextHorizontal += RandomSigned(random) * jitter;
            nextVertical += RandomSigned(random) * jitter;
        }

        nextHorizontal = LetterFormationGeometry.ClampMagnitude(nextHorizontal, MaxProposalStep);
        nextVertical = LetterFormationGeometry.ClampMagnitude(nextVertical, MaxProposalStep);

        PlanarOffset nextMomentum = LetterFormationGeometry.Reexpress(LetterFormationGeometry.ToOffset(nextHorizontal, nextVertical));
        PlanarPoint nextPosition = LetterFormationGeometry.Reexpress(
            LetterFormationGeometry.ClampPoint(environment, site.Position + nextMomentum));
        return site with
        {
            Position = nextPosition,
            Momentum = nextMomentum,
        };
    }

    private static LetterFormationProposal? CreateSingleSiteProposal(
        string siteId,
        string componentId,
        string source,
        double directionX,
        double directionY,
        double delta,
        Proportion strength,
        string description)
    {
        double step = LetterFormationGeometry.ClampMagnitude(delta * ProposalResponsiveness, MaxProposalStep);
        if (Math.Abs(step) < 0.0001d)
        {
            return null;
        }

        return CreateProposal(siteId, componentId, source, directionX * step, directionY * step, strength, description);
    }

    private static LetterFormationProposal CreateProposal(
        string siteId,
        string componentId,
        string source,
        double horizontal,
        double vertical,
        Proportion strength,
        string description) =>
        new(
            siteId,
            componentId,
            source,
            LetterFormationGeometry.ToOffset(horizontal, vertical),
            strength,
            description);

    private static double ResolveCarrierDirectionTarget(
        double currentComponent,
        double currentLength,
        Proportion desiredComponent,
        double normalizedTarget)
    {
        if (desiredComponent.Numerator == 0 && desiredComponent.Denominator == 0)
        {
            return currentComponent;
        }

        if (desiredComponent.Denominator == 0)
        {
            double sign = Math.Sign(desiredComponent.Numerator);
            if (Math.Sign(currentComponent) == sign && Math.Abs(currentComponent) > 0.0001d)
            {
                return currentComponent;
            }

            return sign * Math.Max(Math.Abs(currentComponent), 1d);
        }

        return normalizedTarget * currentLength;
    }

    private static (double X, double Y) ResolveFallbackCarrierDirection(LetterFormationCarrierState carrier)
    {
        CarrierDirectionDesire? direction = carrier.Desires.OfType<CarrierDirectionDesire>().FirstOrDefault();
        return direction is null
            ? (1d, 0d)
            : LetterFormationGeometry.ResolveDirection(direction.PreferredDirection);
    }

    private static double RandomSigned(Random random) =>
        (random.NextDouble() * 2d) - 1d;
}
