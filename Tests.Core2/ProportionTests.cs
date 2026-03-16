using Core2.Elements;
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

    [Fact]
    public void NumeratorIsDominant_AndDenominatorIsRecessive()
    {
        var proportion = new Proportion(4, 2);

        Assert.Equal(new Scalar(4m), proportion.Dominant);
        Assert.Equal(new Scalar(2m), proportion.Recessive);
    }

    [Fact]
    public void Mirror_ProducesReciprocalBySwappingRoles()
    {
        var proportion = new Proportion(4, 2);

        var mirrored = proportion.Mirror();

        Assert.Equal(new Proportion(2, 4), mirrored);
        Assert.Equal(mirrored, proportion.Reciprocal());
    }

    [Fact]
    public void Pin_PromotesTwoProportionsIntoAnAxis()
    {
        var recessive = new Proportion(3, 2);
        var dominant = new Proportion(5, 2);

        var pinned = recessive.Pin(dominant);

        Assert.Equal(new Axis(recessive, dominant), pinned);
    }

    [Fact]
    public void ZeroDenominator_IsAllowed_AndFoldsToOverflowSentinel()
    {
        var proportion = new Proportion(5, 0);

        Assert.Equal(new Scalar(decimal.MaxValue), proportion.Fold());
    }

    [Fact]
    public void ComparisonAndAbs_WorkInProportionSpace()
    {
        var negative = new Proportion(-3, 2);
        var positive = new Proportion(1, 4);

        Assert.True(negative < positive);
        Assert.Equal(new Proportion(3, 2), negative.Abs());
        Assert.Equal(new Proportion(1, 4), Proportion.Min(positive, new Proportion(2, 3)));
    }

    [Fact]
    public void ScalarPin_RaisesDegreeIntoAProportion()
    {
        var proportion = new Scalar(5).Pin(new Scalar(2));

        Assert.Equal(new Proportion(5, 2), proportion);
    }
}
