namespace Core2.Dynamic;

public sealed record DynamicConvergenceState<TState, TEnvironment, TEffect>(
    DynamicContext<TState, TEnvironment> Seed,
    IReadOnlyList<DynamicStep<TState, TEnvironment, TEffect>> Steps,
    IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> Frontier);
