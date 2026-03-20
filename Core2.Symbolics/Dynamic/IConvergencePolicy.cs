namespace Core2.Symbolics.Dynamic;

public interface IConvergencePolicy<TState, TEnvironment, TEffect>
{
    bool ShouldStop(DynamicConvergenceState<TState, TEnvironment, TEffect> state);
}
