namespace Core2.Symbolics.Dynamic;

public sealed record DynamicStrandContext<TState, TEnvironment>(
    int StepIndex,
    DynamicFrontierContext<TState, TEnvironment> Current,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> Frontier);
