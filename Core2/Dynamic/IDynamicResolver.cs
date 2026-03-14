namespace Core2.Dynamic;

public interface IDynamicResolver<TState, TEnvironment, TEffect>
{
    DynamicResolution<TState, TEnvironment, TEffect> Resolve(
        DynamicResolutionInput<TState, TEnvironment, TEffect> input);
}
