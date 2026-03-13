using Core2.Elements;
using Core2.Repetition;

namespace Tests.Core2;

public class FractionalPowerTests
{
    [Fact]
    public void Scalar_FractionalPower_UsesInverseContinuationThenRepeatedMultiplication()
    {
        var result = new Scalar(9).TryPow(new Proportion(1, 2));

        Assert.True(result.Succeeded);
        Assert.Equal(new Scalar(3), result.PrincipalCandidate);
        Assert.Contains(new Scalar(3), result.Candidates);
        Assert.Contains(new Scalar(-3), result.Candidates);
    }

    [Fact]
    public void Proportion_FractionalPower_PreservesPositiveSupportAsPrincipal()
    {
        var result = new Proportion(4, 9).TryPow(new Proportion(1, 2));

        Assert.True(result.Succeeded);
        Assert.Equal(new Proportion(2, 3), result.PrincipalCandidate);
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(2, 3));
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(-2, 3));
    }

    [Fact]
    public void Axis_FractionalPower_ProducesBranchCandidates()
    {
        var result = Axis.NegativeOne.TryPow(new Proportion(1, 2));

        Assert.True(result.Succeeded);
        Assert.Equal(Axis.I, result.PrincipalCandidate);
        Assert.Contains(Axis.I, result.Candidates);
        Assert.Contains(Axis.NegativeI, result.Candidates);
    }

    [Fact]
    public void Axis_FractionalPower_CanFollowReferenceBranch()
    {
        var result = Axis.NegativeOne.TryPow(
            new Proportion(1, 2),
            InverseContinuationRule.NearestToReference,
            Axis.NegativeI);

        Assert.True(result.Succeeded);
        Assert.Equal(Axis.NegativeI, result.PrincipalCandidate);
    }

    [Fact]
    public void Area_FractionalPower_UsesFoldFirstPrincipalPath()
    {
        var area = new Area(Axis.I, Axis.I);

        var result = area.TryPow(new Proportion(1, 2));

        Assert.True(result.Succeeded);
        Assert.Equal(Axis.I, result.PrincipalCandidate);
    }

    [Fact]
    public void NegativeFractionalPowers_AreDeferred()
    {
        var result = Axis.I.TryPow(new Proportion(-1, 2));

        Assert.False(result.Succeeded);
        Assert.Contains(result.Tensions, tension => tension.Kind == PowerTensionKind.UnsupportedNegativeExponent);
    }
}
