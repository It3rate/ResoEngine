using Core2.Branching;
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
        Assert.Equal(BranchOrigin.Preimage, result.Branches.Origin);
        Assert.Equal(BranchSemantics.Alternative, result.Branches.Semantics);
        Assert.Equal(BranchDirection.Forward, result.Branches.Direction);
        Assert.All(result.Branches.Members, member => Assert.Single(member.Parents));
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

    [Fact]
    public void Axis_FractionalPower_OverflowReturnsTensionInsteadOfThrowing()
    {
        var value = Axis.FromCoordinates((Scalar)(-0.5m), (Scalar)1m, Scalar.One, Scalar.One);

        var result = value.TryPow(new Proportion(1, 2));

        Assert.True(
            result.Succeeded ||
            result.Tensions.Any(tension => tension.Kind == PowerTensionKind.ComputationOverflow));
    }

    [Fact]
    public void Axis_FractionalPower_OffsetAxis_ProducesCandidates()
    {
        var value = Axis.FromCoordinates(new Proportion(-3), new Proportion(9));

        var result = value.TryPow(new Proportion(1, 2));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.PrincipalCandidate);
        Assert.NotEmpty(result.Candidates);
    }

    [Fact]
    public void Axis_FractionalPower_OffsetAxis_ThreeHalves_ProducesCandidates()
    {
        var value = Axis.FromCoordinates(new Proportion(-3), new Proportion(9));

        var result = value.TryPow(new Proportion(3, 2));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.PrincipalCandidate);
        Assert.NotEmpty(result.Candidates);
    }
}
