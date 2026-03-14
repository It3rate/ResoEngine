namespace Core2.Dynamic;

public sealed record DynamicStep<TState, TEnvironment, TEffect>(
    int Index,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> IncomingFrontier,
    IReadOnlyList<DynamicProposal<TEffect>> Proposals,
    DynamicResolution<TState, TEnvironment, TEffect> Resolution,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> OutgoingFrontier);
