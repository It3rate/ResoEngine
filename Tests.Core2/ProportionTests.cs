using ResoEngine.Core2;

namespace Tests.Core2;

public class ProportionTests
{
    [Fact]
    public void Multiplication_UsesRightActionOppositionCycle()
    {
        Assert.Equal(Proportion.I, Proportion.One * Proportion.I);
        Assert.Equal(Proportion.NegativeOne, Proportion.I * Proportion.I);
        Assert.Equal(Proportion.NegativeI, Proportion.NegativeOne * Proportion.I);
        Assert.Equal(Proportion.One, Proportion.NegativeI * Proportion.I);
    }

    [Fact]
    public void Multiplication_DerivesComplexStyleProductFromOppositionBasis()
    {
        var state = new Proportion(2, 3);
        var transform = new Proportion(4, 5);

        var result = state * transform;

        Assert.Equal(new Scalar(22), result.Recessive);
        Assert.Equal(new Scalar(7), result.Dominant);
    }

    [Fact]
    public void ApplyOpposition_MatchesRightMultiplicationByI()
    {
        var value = new Proportion(2, 3);

        Assert.Equal(value.ApplyOpposition(), value * Proportion.I);
    }
}
