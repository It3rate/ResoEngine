namespace Core2.Dynamic;

public sealed record DynamicStrandContext<TState, TEnvironment>(
    int StepIndex,
    DynamicFrontierContext<TState, TEnvironment> Current,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> Frontier);
