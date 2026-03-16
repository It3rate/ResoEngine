using Applied.Geometry.Utils;
using Core2.Branching;
using Core2.Dynamic;
using Core2.Elements;

namespace Applied.Geometry.Frieze;

public sealed class FriezePathResolver : IDynamicResolver<FriezePathState, FriezeEnvironment, PlanarTraversalMotion>
{
    public DynamicResolution<FriezePathState, FriezeEnvironment, PlanarTraversalMotion> Resolve(
        DynamicResolutionInput<FriezePathState, FriezeEnvironment, PlanarTraversalMotion> input)
    {
        if (input.Frontier.Count == 0)
        {
            return DynamicResolution<FriezePathState, FriezeEnvironment, PlanarTraversalMotion>.Defer(
                input.Proposals,
                [new DynamicTension("EmptyFrontier", "No active contexts remain to resolve.")],
                "Dynamic frieze execution has no active frontier.");
        }

        var incoming = input.Frontier[0].Context;
        PlanarOffset netDelta = input.Proposals
            .Select(proposal => proposal.Effect.Delta)
            .Aggregate(PlanarOffset.Zero, static (sum, delta) => sum + delta);
        bool horizontalVisible = input.Proposals.Any(proposal => !proposal.Effect.Delta.Horizontal.IsZero && proposal.Effect.IsVisible);
        bool verticalVisible = input.Proposals.Any(proposal => !proposal.Effect.Delta.Vertical.IsZero && proposal.Effect.IsVisible);

        var candidates = BuildCandidates(incoming, netDelta, horizontalVisible, verticalVisible);
        Proportion bestScore = candidates.Select(candidate => candidate.Score).Aggregate(Proportion.Min);
        var viable = candidates
            .Where(candidate => candidate.Score == bestScore)
            .OrderBy(candidate => candidate.PreferencePenalty)
            .ToArray();
        var selected = viable[0];

        var parentIds = input.Frontier.Select(frontier => frontier.NodeId).ToArray();
        var members = viable
            .Select(candidate => new BranchMember<DynamicContext<FriezePathState, FriezeEnvironment>>(
                BranchId.New(),
                candidate.Context,
                parentIds,
                []))
            .ToArray();

        var outcomes = BranchFamily<DynamicContext<FriezePathState, FriezeEnvironment>>.FromMembers(
            BranchOrigin.Continuation,
            viable.Length > 1 ? BranchSemantics.Alternative : BranchSemantics.Mixed,
            BranchDirection.Forward,
            members,
            BranchSelection.Principal(members[0].Id, selected.Note));

        DynamicResolutionKind kind = viable.Length > 1
            ? DynamicResolutionKind.Branch
            : DynamicResolutionKind.Commit;

        return DynamicResolution<FriezePathState, FriezeEnvironment, PlanarTraversalMotion>.FromFamily(
            kind,
            outcomes,
            input.Proposals,
            selected.Tensions,
            selected.Note);
    }

    private static List<FriezeCandidate> BuildCandidates(
        DynamicContext<FriezePathState, FriezeEnvironment> incoming,
        PlanarOffset netDelta,
        bool horizontalVisible,
        bool verticalVisible)
    {
        var candidates = new List<FriezeCandidate>();
        if (netDelta.IsZero)
        {
            candidates.Add(Commit(incoming, [], Proportion.Zero, "No net movement; state preserved."));
            return candidates;
        }

        if (!netDelta.Horizontal.IsZero && !netDelta.Vertical.IsZero)
        {
            candidates.Add(Commit(incoming, [new PlanarTraversalMotion(new PlanarOffset(netDelta.Horizontal, Proportion.Zero), horizontalVisible), new PlanarTraversalMotion(new PlanarOffset(Proportion.Zero, netDelta.Vertical), verticalVisible)], CornerPenalty(netDelta.Vertical, horizontalFirst: true), "Horizontal-first corner."));
            candidates.Add(Commit(incoming, [new PlanarTraversalMotion(new PlanarOffset(Proportion.Zero, netDelta.Vertical), verticalVisible), new PlanarTraversalMotion(new PlanarOffset(netDelta.Horizontal, Proportion.Zero), horizontalVisible)], CornerPenalty(netDelta.Vertical, horizontalFirst: false), "Vertical-first corner."));
            return candidates;
        }

        if (!netDelta.Horizontal.IsZero)
        {
            candidates.Add(Commit(incoming, [new PlanarTraversalMotion(new PlanarOffset(netDelta.Horizontal, Proportion.Zero), horizontalVisible)], Proportion.Zero, "Horizontal continuation."));
            return candidates;
        }

        candidates.Add(Commit(incoming, [new PlanarTraversalMotion(new PlanarOffset(Proportion.Zero, netDelta.Vertical), verticalVisible)], Proportion.Zero, "Vertical continuation."));
        return candidates;
    }

    private static Proportion CornerPenalty(Proportion vertical, bool horizontalFirst)
    {
        if (vertical > Proportion.Zero)
        {
            return horizontalFirst ? new Proportion(1, 4) : Proportion.Zero;
        }

        if (vertical < Proportion.Zero)
        {
            return horizontalFirst ? Proportion.Zero : new Proportion(1, 4);
        }

        return Proportion.Zero;
    }

    private static FriezeCandidate Commit(
        DynamicContext<FriezePathState, FriezeEnvironment> incoming,
        IReadOnlyList<PlanarTraversalMotion> sequence,
        Proportion preferencePenalty,
        string note)
    {
        var cursor = incoming.State.Cursor;
        var edges = new List<PlanarPathEdge>();
        var tensions = new List<DynamicTension>();
        Proportion score = preferencePenalty;

        foreach (var motion in sequence)
        {
            var delta = motion.Delta;
            if (delta.IsZero)
            {
                continue;
            }

            var next = cursor + delta;
            var edge = new PlanarPathEdge(cursor, next);
            if (!incoming.Environment.Contains(next))
            {
                tensions.Add(new DynamicTension(
                    "VerticalBounds",
                    $"Move to y={next.Y} exceeds frieze bounds [{incoming.Environment.MinY}, {incoming.Environment.MaxY}].",
                    10m));
                score += new Proportion(10);
            }

            if (incoming.Environment.Contains(edge))
            {
                tensions.Add(new DynamicTension(
                    "EdgeReuse",
                    $"Segment {edge.Start} -> {edge.End} was already occupied.",
                    3m));
                score += new Proportion(3);
            }

            if (motion.IsVisible)
            {
                edges.Add(edge);
            }

            cursor = next;
        }

        var nextState = new FriezePathState(
            cursor,
            incoming.State.Segments.Concat(edges).ToArray(),
            incoming.State.MacroStepValue + Proportion.One);

        var nextEnvironment = incoming.Environment.WithAddedEdges(edges);
        return new FriezeCandidate(
            new DynamicContext<FriezePathState, FriezeEnvironment>(nextState, nextEnvironment),
            tensions,
            score,
            preferencePenalty,
            note);
    }

    private sealed record FriezeCandidate(
        DynamicContext<FriezePathState, FriezeEnvironment> Context,
        IReadOnlyList<DynamicTension> Tensions,
        Proportion Score,
        Proportion PreferencePenalty,
        string Note);
}
