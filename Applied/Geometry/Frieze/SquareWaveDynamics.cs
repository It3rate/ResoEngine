using Applied.Geometry.Utils;
using Core2.Dynamic;
using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public static class SquareWaveDynamics
{
    public static DynamicTrace<FriezePathState, FriezeEnvironment, PlanarTraversalMotion> Run(int steps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(steps);

        var seed = new DynamicContext<FriezePathState, FriezeEnvironment>(
            FriezePathState.Origin,
            FriezeEnvironment.Create(0, 1));

        PlanarSegmentDefinition advance =
            new(
                "AdvanceRight",
                Axis.FromCoordinates(Scalar.Zero, Scalar.One),
                BoundaryContinuationLaw.TensionPreserving,
                PlanarOffset.Right,
                Scalar.One,
                UseSegmentAsFrame: false);

        PlanarSegmentDefinition verticalPulse =
            new(
                "VerticalPulse",
                Axis.FromCoordinates(Scalar.Zero, Scalar.One),
                BoundaryContinuationLaw.ReflectiveBounce,
                PlanarOffset.Up,
                Scalar.One);

        PlanarSegmentDefinition backtrack =
            new(
                "BacktrackPulse",
                Axis.FromCoordinates(Scalar.Zero, Scalar.One),
                BoundaryContinuationLaw.TensionPreserving,
                PlanarOffset.Right,
                new Scalar(-1m),
                UseSegmentAsFrame: false);

        var runner = new DynamicRunner<FriezePathState, FriezeEnvironment, PlanarTraversalMotion>(
            [
                new ScheduledPlanarSegmentStrand<FriezePathState, FriezeEnvironment>(
                    "AdvanceRight",
                    advance,
                    activePhases: Enumerable.Range(0, 6),
                    period: 6,
                    note: "Advance one unit to the right."),
                new ScheduledPlanarSegmentStrand<FriezePathState, FriezeEnvironment>(
                    "VerticalPulse",
                    verticalPulse,
                    activePhases: [0, 3],
                    period: 6,
                    note: "Pulse vertically through the reflected carrier."),
                new ScheduledPlanarSegmentStrand<FriezePathState, FriezeEnvironment>(
                    "BacktrackPulse",
                    backtrack,
                    activePhases: [0, 3],
                    period: 6,
                    note: "Cancel the first horizontal move of the cycle."),
            ],
            new FriezePathResolver(),
            new FixedStepConvergencePolicy<FriezePathState, FriezeEnvironment, PlanarTraversalMotion>(steps));

        return runner.Run(seed);
    }
}
