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

        var wrapped = frame.Continue(new Scalar(11), BoundaryContinuationLaw.PeriodicWrap);
        var reflected = frame.Continue(new Scalar(11), BoundaryContinuationLaw.ReflectiveBounce);
        var clamped = frame.Continue(new Scalar(11), BoundaryContinuationLaw.Clamp);
        var preserved = frame.Continue(new Scalar(11), BoundaryContinuationLaw.TensionPreserving);

        Assert.Equal(1m, wrapped.Value);
        Assert.Equal(9m, reflected.Value);
        Assert.Equal(10m, clamped.Value);
        Assert.Equal(11m, preserved.Value);
        Assert.Contains(preserved.Tensions, tension => tension.Kind == RepetitionTensionKind.BoundaryExceeded);
    }

    [Fact]
    public void BoundaryContinuation_WrapsAcrossMultiplePeriods()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var wrappedPositive = frame.Continue(new Scalar(15), BoundaryContinuationLaw.PeriodicWrap);
        var wrappedNegative = frame.Continue(new Scalar(-2), BoundaryContinuationLaw.PeriodicWrap);

        Assert.Equal(0m, wrappedPositive.Value);
        Assert.Equal(3m, wrappedNegative.Value);
    }

    [Fact]
    public void BoundaryContinuation_ReflectsAcrossMultipleBounces()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var reflectedPositive = frame.Continue(new Scalar(15), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedOver = frame.Continue(new Scalar(17), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedNegative = frame.Continue(new Scalar(-2), BoundaryContinuationLaw.ReflectiveBounce);

        Assert.Equal(5m, reflectedPositive.Value);
        Assert.Equal(3m, reflectedOver.Value);
        Assert.Equal(2m, reflectedNegative.Value);
    }
}
