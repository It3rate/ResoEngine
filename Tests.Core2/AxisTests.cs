using Core2.Elements;
using Core2.Support;
using ResoEngine.Core2;

namespace Tests.Core2;

public class AxisTests
{
    [Fact]
    public void Multiplication_ReusesSamePairAlgebraAtHigherDegree()
    {
        Assert.Equal(Axis.I, Axis.One * Axis.I);
        Assert.Equal(Axis.NegativeOne, Axis.I * Axis.I);
    }

    [Fact]
    public void Fold_MultipliesAxisComponentsDownToAProportion()
    {
        var axis = new Axis(new Proportion(1, 2), new Proportion(3, 4));

        var folded = axis.Fold();

        Assert.Equal(new Proportion(3, 8), folded);
    }

    [Fact]
    public void Multiplication_DerivesComplexStyleProductFromAxisComponents()
    {
        var state = new Axis(new Proportion(2, 1), new Proportion(3, 1));
        var transform = new Axis(new Proportion(4, 1), new Proportion(5, 1));

        var result = state * transform;

        Assert.Equal(new Proportion(22, 1), result.Recessive);
        Assert.Equal(new Proportion(7, 1), result.Dominant);
    }

    [Fact]
    public void UnaryTransforms_SupportMirrorConjugatesAndProjection()
    {
        var axis = new Axis(new Proportion(3, 1), new Proportion(2, 1));

        Assert.Equal(new Axis(new Proportion(2, 1), new Proportion(3, 1)), axis.Mirror());
        Assert.Equal(axis.Mirror(), axis.SwapUnitRoles());
        Assert.Equal(new Axis(new Proportion(-3, 1), new Proportion(2, 1)), axis.ConjugateRecessive());
        Assert.Equal(new Axis(new Proportion(3, 1), new Proportion(-2, 1)), axis.ConjugateDominant());
        Assert.Equal(Axis.NegativeOne, axis.ProjectRecessiveIntoDominant());
        Assert.Equal(Axis.I, axis.ProjectDominantIntoRecessive());
    }

    [Fact]
    public void Pin_CreatesAnExpandedArea_WhoseFoldMatchesMultiplication()
    {
        var left = new Axis(new Proportion(3, 1), new Proportion(5, 1));
        var right = new Axis(new Proportion(2, 1), new Proportion(4, 1));

        var pinned = left.Pin(right);

        Assert.Equal(left, pinned.Recessive);
        Assert.Equal(right, pinned.Dominant);
        Assert.Equal(left * right, pinned.Fold());
    }

    [Fact]
    public void IntervalCoordinates_AreExposedDirectly()
    {
        var axis = new Axis(new Proportion(3, 2), new Proportion(5, 2));

        Assert.Equal(new Scalar(-1.5m), axis.Start);
        Assert.Equal(new Scalar(2.5m), axis.End);
        Assert.Equal(new Scalar(-1.5m), axis.Left);
        Assert.Equal(new Scalar(2.5m), axis.Right);
        Assert.Equal(new Scalar(4.0m), axis.Span);
        Assert.Equal(new Scalar(0.5m), axis.Midpoint);
        Assert.False(axis.IsEmptyInterval);
        Assert.True(axis.Contains(new Scalar(0.5m)));
        Assert.False(axis.Contains(new Scalar(3m)));
    }

    [Fact]
    public void Envelope_ReturnsTheConvexHullInPrimaryDirection()
    {
        var a = new Axis(new Proportion(3, 1), new Proportion(5, 1));
        var b = new Axis(new Proportion(1, 1), new Proportion(3, 1));

        Assert.Equal(new Axis(new Proportion(1, 1), new Proportion(3, 1)), a.Intersect(b));
        Assert.Equal(new Axis(new Proportion(3, 1), new Proportion(5, 1)), a.Envelope(b));
        Assert.Equal(a.Envelope(b), a.Union(b));
    }

    [Fact]
    public void FlipPerspective_IsExplicitAtTheAxisLevel()
    {
        var axis = new Axis(new Proportion(3, 1), new Proportion(2, 1));

        Assert.Equal(-axis, axis.FlipPerspective());
    }

    [Fact]
    public void SplitComplexBasis_MakesISquaredPositive()
    {
        var axis = new Axis(new Proportion(1, 1), Proportion.Zero, AxisBasis.SplitComplex);

        var result = axis * axis;

        Assert.Equal(AxisBasis.SplitComplex, result.Basis);
        Assert.Equal(Proportion.Zero, result.Recessive);
        Assert.Equal(Proportion.One, result.Dominant);
    }

    [Fact]
    public void ReversedSegmentsRetainExtentAndAreNotMarkedEmpty()
    {
        var axis = Axis.FromCoordinates((Scalar)5m, (Scalar)(-2m));

        Assert.True(axis.HasExtent);
        Assert.False(axis.IsEmptyInterval);
        Assert.False(axis.PointsRight);
        Assert.Equal(new Scalar(-2m), axis.Left);
        Assert.Equal(new Scalar(5m), axis.Right);
    }

    [Fact]
    public void Intersect_PreservesPrimaryDirection_ForReversedSegments()
    {
        var primary = Axis.FromCoordinates((Scalar)5m, (Scalar)(-5m));
        var secondary = Axis.FromCoordinates((Scalar)(-2m), (Scalar)4m);

        Assert.True(primary.TryIntersect(secondary, out var intersection));
        Assert.Equal(new Scalar(4m), intersection.Start);
        Assert.Equal(new Scalar(-2m), intersection.End);
        Assert.False(intersection.PointsRight);
    }
}
