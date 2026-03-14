namespace Core2.Dynamic;

public sealed class FixedStepConvergencePolicy<TState, TEnvironment, TEffect> : IConvergencePolicy<TState, TEnvironment, TEffect>
{
    public FixedStepConvergencePolicy(int maxSteps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxSteps);
        MaxSteps = maxSteps;
    }

    public int MaxSteps { get; }

    public bool ShouldStop(DynamicConvergenceState<TState, TEnvironment, TEffect> state) =>
        state.Steps.Count >= MaxSteps;
}
