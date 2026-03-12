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
}
