namespace Core2.Symbolics.Dynamic;

public sealed record DynamicContext<TState, TEnvironment>(
    TState State,
    TEnvironment Environment);
