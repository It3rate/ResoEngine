using Core2.Elements;
using Core2.Repetition;

namespace Tests.Core2;

public class RepetitionTests
{
    [Fact]
    public void AdditiveAndMultiplicativeRepetitionUseDifferentIdentityStates()
    {
        var additive = RepetitionEngine.RepeatAdditive(new Scalar(2), 4);
        var multiplicative = RepetitionEngine.RepeatMultiplicative(new Scalar(2), 4);

        Assert.Equal(RepetitionKind.Additive, additive.Kind);
        Assert.Equal(new Scalar(0), additive.States[0]);
        Assert.Equal(new Scalar(8), additive.Result);

        Assert.Equal(RepetitionKind.Multiplicative, multiplicative.Kind);
        Assert.Equal(new Scalar(1), multiplicative.States[0]);
        Assert.Equal(new Scalar(16), multiplicative.Result);
    }

    [Fact]
    public void TransformRepetition_TracesTheAxisOppositionCycle()
    {
        var trace = RepetitionEngine.RepeatTransform(Axis.One, Axis.I, 4);

        Assert.Equal(
            [Axis.One, Axis.I, Axis.NegativeOne, Axis.NegativeI, Axis.One],
            trace.States);
    }

    [Fact]
    public void RecursiveRepetition_CanExpressFibonacciStyleGeneration()
    {
        var trace = RepetitionEngine.RepeatRecursive(
            [new Scalar(0), new Scalar(1)],
            5,
            states => states[^1] + states[^2]);

        Assert.Equal(
            [new Scalar(0), new Scalar(1), new Scalar(1), new Scalar(2), new Scalar(3), new Scalar(5), new Scalar(8)],
            trace.States);
    }

    [Fact]
    public void BoundaryContinuation_CanWrapReflectOrPreserveTension()
    {
        var frame = Axis.FromCoordinates(0, 10);

        var wrapped = frame.Continue(new Proportion(11), BoundaryContinuationLaw.PeriodicWrap);
        var reflected = frame.Continue(new Proportion(11), BoundaryContinuationLaw.ReflectiveBounce);
        var clamped = frame.Continue(new Proportion(11), BoundaryContinuationLaw.Clamp);
        var preserved = frame.Continue(new Proportion(11), BoundaryContinuationLaw.TensionPreserving);

        Assert.Equal(new Proportion(1), wrapped.Value);
        Assert.Equal(new Proportion(9), reflected.Value);
        Assert.Equal(new Proportion(10), clamped.Value);
        Assert.Equal(new Proportion(11), preserved.Value);
        Assert.Contains(preserved.Tensions, tension => tension.Kind == RepetitionTensionKind.BoundaryExceeded);
    }

    [Fact]
    public void BoundaryContinuation_WrapsAcrossMultiplePeriods()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var wrappedPositive = frame.Continue(new Proportion(15), BoundaryContinuationLaw.PeriodicWrap);
        var wrappedNegative = frame.Continue(new Proportion(-2), BoundaryContinuationLaw.PeriodicWrap);

        Assert.Equal(Proportion.Zero, wrappedPositive.Value);
        Assert.Equal(new Proportion(3), wrappedNegative.Value);
    }

    [Fact]
    public void BoundaryContinuation_ReflectsAcrossMultipleBounces()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var reflectedPositive = frame.Continue(new Proportion(15), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedOver = frame.Continue(new Proportion(17), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedNegative = frame.Continue(new Proportion(-2), BoundaryContinuationLaw.ReflectiveBounce);

        Assert.Equal(new Proportion(5), reflectedPositive.Value);
        Assert.Equal(new Proportion(3), reflectedOver.Value);
        Assert.Equal(new Proportion(2), reflectedNegative.Value);
    }

    [Fact]
    public void AxisTraversal_EnumeratesReflectionAndWrapAsExactStepSequences()
    {
        var reflective = new AxisTraversalDefinition(
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            new Proportion(3),
            BoundaryContinuationLaw.ReflectiveBounce,
            Proportion.Zero).CreateState();

        var reflectiveParts = reflective.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(new Proportion(2), Proportion.One)],
            reflectiveParts.Select(part => part.Segment).ToArray());
        Assert.True(reflectiveParts[0].BreakAfter);
        Assert.Equal(new Proportion(1), reflective.Value);

        var wrapped = new AxisTraversalDefinition(
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            new Proportion(5),
            BoundaryContinuationLaw.PeriodicWrap,
            Proportion.Zero).CreateState();

        var wrappedParts = wrapped.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, Proportion.One)],
            wrappedParts.Select(part => part.Segment).ToArray());
        Assert.Equal(new Proportion(1), wrapped.Value);
    }
}
