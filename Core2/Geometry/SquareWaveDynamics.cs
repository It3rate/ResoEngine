using Core2.Dynamic;

namespace Core2.Geometry;

public static class SquareWaveDynamics
{
    public static DynamicTrace<StripPathState, StripEnvironment, StripEffect> Run(int steps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(steps);

        var seed = new DynamicContext<StripPathState, StripEnvironment>(
            StripPathState.Origin,
            StripEnvironment.Create(0, 1));

        var runner = new DynamicRunner<StripPathState, StripEnvironment, StripEffect>(
            [
                new AdvanceRightStrand(),
                new VerticalPulseStrand(),
                new BacktrackPulseStrand(),
            ],
            new StripPathResolver(),
            new FixedStepConvergencePolicy<StripPathState, StripEnvironment, StripEffect>(steps));

        return runner.Run(seed);
    }
}
