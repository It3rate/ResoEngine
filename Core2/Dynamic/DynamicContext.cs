namespace Core2.Dynamic;

public sealed record DynamicContext<TState, TEnvironment>(
    TState State,
    TEnvironment Environment);
