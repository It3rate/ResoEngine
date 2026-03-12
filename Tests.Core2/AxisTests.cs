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

        Assert.Equal(new Proportion(10, 5), folded);
    }
}
