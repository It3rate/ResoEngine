using Core2.Branching;

namespace Core2.Dynamic;

public sealed record DynamicResolution<TState, TEnvironment, TEffect>(
    DynamicResolutionKind Kind,
    BranchFamily<DynamicContext<TState, TEnvironment>> Outcomes,
    IReadOnlyList<DynamicProposal<TEffect>> AcceptedProposals,
    IReadOnlyList<DynamicTension> Tensions,
    string? Note = null)
{
    public bool HasOutcomes => Outcomes.HasMembers;

    public static DynamicResolution<TState, TEnvironment, TEffect> Commit(
        DynamicContext<TState, TEnvironment> context,
        IReadOnlyList<DynamicProposal<TEffect>> acceptedProposals,
        IReadOnlyList<DynamicTension>? tensions = null,
        string? note = null) =>
        FromContexts(
            DynamicResolutionKind.Commit,
            [context],
            acceptedProposals,
            selectedIndex: 0,
            tensions,
            note);

    public static DynamicResolution<TState, TEnvironment, TEffect> Branch(
        IReadOnlyList<DynamicContext<TState, TEnvironment>> contexts,
        IReadOnlyList<DynamicProposal<TEffect>> acceptedProposals,
        int? selectedIndex = null,
        IReadOnlyList<DynamicTension>? tensions = null,
        string? note = null) =>
        FromContexts(
            DynamicResolutionKind.Branch,
            contexts,
            acceptedProposals,
            selectedIndex,
            tensions,
            note);

    public static DynamicResolution<TState, TEnvironment, TEffect> Defer(
        IReadOnlyList<DynamicProposal<TEffect>> acceptedProposals,
        IReadOnlyList<DynamicTension> tensions,
        string? note = null) =>
        new(
            DynamicResolutionKind.Deferred,
            BranchFamily<DynamicContext<TState, TEnvironment>>.Empty(
                BranchOrigin.Continuation,
                BranchSemantics.Alternative,
                BranchDirection.Forward),
            acceptedProposals,
            tensions,
            note);

    public static DynamicResolution<TState, TEnvironment, TEffect> FromFamily(
        DynamicResolutionKind kind,
        BranchFamily<DynamicContext<TState, TEnvironment>> outcomes,
        IReadOnlyList<DynamicProposal<TEffect>> acceptedProposals,
        IReadOnlyList<DynamicTension>? tensions = null,
        string? note = null) =>
        new(
            kind,
            outcomes,
            acceptedProposals,
            tensions ?? [],
            note);

    public static DynamicResolution<TState, TEnvironment, TEffect> FromContexts(
        DynamicResolutionKind kind,
        IReadOnlyList<DynamicContext<TState, TEnvironment>> contexts,
        IReadOnlyList<DynamicProposal<TEffect>> acceptedProposals,
        int? selectedIndex = null,
        IReadOnlyList<DynamicTension>? tensions = null,
        string? note = null)
    {
        BranchSemantics semantics = contexts.Count > 1
            ? BranchSemantics.Alternative
            : BranchSemantics.Mixed;

        var outcomes = BranchFamily<DynamicContext<TState, TEnvironment>>.FromValues(
            BranchOrigin.Continuation,
            semantics,
            BranchDirection.Forward,
            contexts,
            selectedIndex,
            selectedIndex.HasValue ? BranchSelectionMode.Principal : BranchSelectionMode.None);

        return new DynamicResolution<TState, TEnvironment, TEffect>(
            kind,
            outcomes,
            acceptedProposals,
            tensions ?? [],
            note);
    }
}
