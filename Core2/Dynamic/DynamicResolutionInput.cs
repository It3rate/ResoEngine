namespace Core2.Dynamic;

public sealed record DynamicResolutionInput<TState, TEnvironment, TEffect>(
    int StepIndex,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> Frontier,
    IReadOnlyList<DynamicProposal<TEffect>> Proposals);
