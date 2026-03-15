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

        if (state.Frontier.Count == 0)
        {
            return true;
        }

        if (state.Steps.Count < GlyphGrowthDefaults.MinimumSettleSteps)
        {
            return false;
        }

        return state.Frontier.All(frontier =>
            !frontier.Context.State.HasActiveTips &&
            frontier.Context.State.ResidualTension <= GlyphGrowthDefaults.ResidualTensionThreshold &&
            frontier.Context.State.LastAdjustment <= GlyphGrowthDefaults.RelaxationThreshold &&
            (frontier.Context.State.AmbientSignals?.Count ?? 0) == 0);
    }
}
