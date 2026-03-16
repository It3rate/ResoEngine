using Applied.Geometry.Utils;
using Core2.Dynamic;
using Applied.Geometry;

namespace Applied.Geometry.Frieze;

public static class SquareWaveDynamics
{
    public static DynamicTrace<StripPathState, StripEnvironment, Orientation2D> Run(int steps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(steps);

        var seed = new DynamicContext<StripPathState, StripEnvironment>(
            StripPathState.Origin,
            StripEnvironment.Create(0, 1));

        var runner = new DynamicRunner<StripPathState, StripEnvironment, Orientation2D>(
            [
                new AdvanceRightStrand(),
                new VerticalPulseStrand(),
                new BacktrackPulseStrand(),
            ],
            new StripPathResolver(),
            new FixedStepConvergencePolicy<StripPathState, StripEnvironment, Orientation2D>(steps));

        return runner.Run(seed);
    }
}
