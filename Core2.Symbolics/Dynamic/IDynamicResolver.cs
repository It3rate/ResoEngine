namespace Core2.Symbolics.Dynamic;

public interface IDynamicResolver<TState, TEnvironment, TEffect>
{
    DynamicResolution<TState, TEnvironment, TEffect> Resolve(
        DynamicResolutionInput<TState, TEnvironment, TEffect> input);
}
