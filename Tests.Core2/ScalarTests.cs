using Core2.Elements;

namespace Tests.Core2;

public class ScalarTests
{
    [Fact]
    public void Scalar_SupportsComparisonAbsAndClamp()
    {
        var negative = new Scalar(-3.5m);
        var positive = new Scalar(1.25m);

        Assert.True(negative < positive);
        Assert.True(positive > negative);
        Assert.Equal(new Scalar(3.5m), negative.Abs());
        Assert.Equal(new Scalar(-1m), negative.Clamp(new Scalar(-1m), new Scalar(2m)));
        Assert.Equal(new Scalar(2m), new Scalar(4m).Clamp(new Scalar(-1m), new Scalar(2m)));
        Assert.Equal(new Scalar(1.25m), Scalar.Max(negative, positive));
        Assert.Equal(new Scalar(-3.5m), Scalar.Min(negative, positive));
    }

    [Fact]
    public void Scalar_AsProportion_UsesRequestedSupportWithoutRepeatedScaleGrowth()
    {
        var half = new Scalar(0.5m);

        Assert.Equal(new Proportion(5, 10), half.AsProportion());
        Assert.Equal(new Proportion(5, 10), half.AsProportion(10));
        Assert.Equal(new Proportion(15, 30), half.AsProportion(3));
        Assert.Equal(new Proportion(-5, -10), half.AsProportion(-10));
    }

    [Fact]
    public void ZeroScalar_AsProportion_DoesNotOverflowForHighScaleZeros()
    {
        var zero = new Scalar(0.0000000000000000000000000000m);

        Assert.Equal(new Proportion(0, 1), zero.AsProportion());
    }
}
