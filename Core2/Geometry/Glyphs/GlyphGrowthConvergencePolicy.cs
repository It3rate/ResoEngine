using Core2.Dynamic;

namespace Core2.Geometry.Glyphs;

public sealed class GlyphGrowthConvergencePolicy : IConvergencePolicy<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>
{
    public GlyphGrowthConvergencePolicy(int maxSteps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxSteps);
        MaxSteps = maxSteps;
    }

    public int MaxSteps { get; }

    public bool ShouldStop(DynamicConvergenceState<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> state)
    {
        if (state.Steps.Count >= MaxSteps)
        {
            return true;
        }

        return state.Frontier.Count > 0 &&
            state.Frontier.All(frontier => !frontier.Context.State.HasActiveTips);
    }
}
