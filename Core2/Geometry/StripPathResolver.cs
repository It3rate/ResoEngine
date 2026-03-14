using Core2.Branching;
using Core2.Dynamic;

namespace Core2.Geometry;

public sealed class StripPathResolver : IDynamicResolver<StripPathState, StripEnvironment, StripEffect>
{
    public DynamicResolution<StripPathState, StripEnvironment, StripEffect> Resolve(
        DynamicResolutionInput<StripPathState, StripEnvironment, StripEffect> input)
    {
        if (input.Frontier.Count == 0)
        {
            return DynamicResolution<StripPathState, StripEnvironment, StripEffect>.Defer(
                input.Proposals,
                [new DynamicTension("EmptyFrontier", "No active contexts remain to resolve.")],
                "Dynamic strip execution has no active frontier.");
        }

        var incoming = input.Frontier[0].Context;
        int horizontal = input.Proposals.Sum(proposal => proposal.Effect.HorizontalDelta);
        int vertical = input.Proposals.Sum(proposal => proposal.Effect.VerticalDelta);

        var candidates = BuildCandidates(incoming, horizontal, vertical);
        decimal bestScore = candidates.Min(candidate => candidate.Score);
        var viable = candidates
            .Where(candidate => candidate.Score == bestScore)
            .OrderBy(candidate => candidate.PreferencePenalty)
            .ToArray();
        var selected = viable[0];

        var parentIds = input.Frontier.Select(frontier => frontier.NodeId).ToArray();
        var members = viable
            .Select(candidate => new BranchMember<DynamicContext<StripPathState, StripEnvironment>>(
                BranchId.New(),
                candidate.Context,
                parentIds,
                []))
            .ToArray();

        var outcomes = BranchFamily<DynamicContext<StripPathState, StripEnvironment>>.FromMembers(
            BranchOrigin.Continuation,
            viable.Length > 1 ? BranchSemantics.Alternative : BranchSemantics.Mixed,
            BranchDirection.Forward,
            members,
            BranchSelection.Principal(members[0].Id, selected.Note));

        DynamicResolutionKind kind = viable.Length > 1
            ? DynamicResolutionKind.Branch
            : DynamicResolutionKind.Commit;

        return DynamicResolution<StripPathState, StripEnvironment, StripEffect>.FromFamily(
            kind,
            outcomes,
            input.Proposals,
            selected.Tensions,
            selected.Note);
    }

    private static List<StripCandidate> BuildCandidates(
        DynamicContext<StripPathState, StripEnvironment> incoming,
        int horizontal,
        int vertical)
    {
        var candidates = new List<StripCandidate>();
        if (horizontal == 0 && vertical == 0)
        {
            candidates.Add(Commit(incoming, [], 0m, "No net movement; state preserved."));
            return candidates;
        }

        if (horizontal != 0 && vertical != 0)
        {
            candidates.Add(Commit(incoming, [new StripDelta(horizontal, 0), new StripDelta(0, vertical)], CornerPenalty(vertical, horizontalFirst: true), "Horizontal-first corner."));
            candidates.Add(Commit(incoming, [new StripDelta(0, vertical), new StripDelta(horizontal, 0)], CornerPenalty(vertical, horizontalFirst: false), "Vertical-first corner."));
            return candidates;
        }

        if (horizontal != 0)
        {
            candidates.Add(Commit(incoming, [new StripDelta(horizontal, 0)], 0m, "Horizontal continuation."));
            return candidates;
        }

        candidates.Add(Commit(incoming, [new StripDelta(0, vertical)], 0m, "Vertical continuation."));
        return candidates;
    }

    private static decimal CornerPenalty(int vertical, bool horizontalFirst)
    {
        if (vertical > 0)
        {
            return horizontalFirst ? 0.25m : 0m;
        }

        if (vertical < 0)
        {
            return horizontalFirst ? 0m : 0.25m;
        }

        return 0m;
    }

    private static StripCandidate Commit(
        DynamicContext<StripPathState, StripEnvironment> incoming,
        IReadOnlyList<StripDelta> sequence,
        decimal preferencePenalty,
        string note)
    {
        var cursor = incoming.State.Cursor;
        var edges = new List<StripPathEdge>();
        var tensions = new List<DynamicTension>();
        decimal score = preferencePenalty;

        foreach (var delta in sequence)
        {
            if (delta.Dx == 0 && delta.Dy == 0)
            {
                continue;
            }

            var next = cursor + delta;
            var edge = new StripPathEdge(cursor, next);
            if (!incoming.Environment.ContainsY(next.Y))
            {
                tensions.Add(new DynamicTension(
                    "VerticalBounds",
                    $"Move to y={next.Y} exceeds strip bounds [{incoming.Environment.MinY}, {incoming.Environment.MaxY}].",
                    10m));
                score += 10m;
            }

            if (incoming.Environment.Contains(edge))
            {
                tensions.Add(new DynamicTension(
                    "EdgeReuse",
                    $"Segment {edge.Start} -> {edge.End} was already occupied.",
                    3m));
                score += 3m;
            }

            edges.Add(edge);
            cursor = next;
        }

        var nextState = new StripPathState(
            cursor,
            incoming.State.Segments.Concat(edges).ToArray(),
            incoming.State.MacroStep + 1);

        var nextEnvironment = incoming.Environment.WithAddedEdges(edges);
        return new StripCandidate(
            new DynamicContext<StripPathState, StripEnvironment>(nextState, nextEnvironment),
            tensions,
            score,
            preferencePenalty,
            note);
    }

    private sealed record StripCandidate(
        DynamicContext<StripPathState, StripEnvironment> Context,
        IReadOnlyList<DynamicTension> Tensions,
        decimal Score,
        decimal PreferencePenalty,
        string Note);
}
