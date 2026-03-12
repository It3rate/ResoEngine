using ResoEngine.Core2;

namespace Tests.Core2;

public class ProportionTests
{
    [Fact]
    public void Fold_DividesNumeratorByDenominator()
    {
        var proportion = new Proportion(9, 2);

        Assert.Equal(new Scalar(4.5m), proportion.Fold());
    }

    [Fact]
    public void Multiplication_MultipliesNumeratorsAndDenominators()
    {
        var left = new Proportion(2, 3);
        var right = new Proportion(5, 7);

        var result = left * right;

        Assert.Equal(new Proportion(10, 21), result);
    }

    [Fact]
    public void Addition_PreservesUndividedFractionForm()
    {
        var left = new Proportion(1, 2);
        var right = new Proportion(1, 3);

        var result = left + right;

        Assert.Equal(new Proportion(5, 6), result);
    }
}
