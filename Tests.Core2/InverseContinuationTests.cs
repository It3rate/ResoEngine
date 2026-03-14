using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;
using Core2.Support;

namespace Tests.Core2;

public class InverseContinuationTests
{
    [Fact]
    public void Scalar_InverseContinuation_ReturnsBothRealRootsAndSelectsPositivePrincipal()
    {
        var result = new Scalar(4).InverseContinue(2);

        Assert.True(result.Succeeded);
        Assert.Equal(new Scalar(2), result.PrincipalCandidate);
        Assert.Contains(new Scalar(2), result.Candidates);
        Assert.Contains(new Scalar(-2), result.Candidates);
    }

    [Fact]
    public void Proportion_InverseContinuation_PreservesPositiveSupportAsPrincipalWhenPossible()
    {
        var result = new Proportion(4, 9).InverseContinue(2);

        Assert.True(result.Succeeded);
        Assert.Equal(new Proportion(2, 3), result.PrincipalCandidate);
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(2, 3));
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(-2, 3));
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(2, -3));
        Assert.Contains(result.Candidates, candidate => candidate == new Proportion(-2, -3));
    }

    [Fact]
    public void Axis_InverseContinuation_UsesBranchRuleAndCanFollowReference()
    {
        var principal = Axis.NegativeOne.InverseContinue(2);
        var nearest = Axis.NegativeOne.InverseContinue(
            2,
            InverseContinuationRule.NearestToReference,
            Axis.NegativeI);

        Assert.True(principal.Succeeded);
        Assert.Equal(Axis.I, principal.PrincipalCandidate);
        Assert.Contains(Axis.I, principal.Candidates);
        Assert.Contains(Axis.NegativeI, principal.Candidates);
        Assert.Equal(Axis.NegativeI, nearest.PrincipalCandidate);
        Assert.Equal(BranchOrigin.Preimage, principal.Branches.Origin);
        Assert.Equal(BranchSemantics.Alternative, principal.Branches.Semantics);
        Assert.Equal(BranchDirection.Reverse, principal.Branches.Direction);
        Assert.All(principal.Branches.Members, member => Assert.Empty(member.Parents));
    }

    [Fact]
    public void SplitComplexAxis_InverseContinuation_IsLimitedToPureDominantValues()
    {
        var pureDominant = new Axis(Proportion.Zero, new Proportion(4, 1), AxisBasis.SplitComplex);
        var mixed = new Axis(new Proportion(1, 1), new Proportion(4, 1), AxisBasis.SplitComplex);

        var pureResult = pureDominant.InverseContinue(2);
        var mixedResult = mixed.InverseContinue(2);

        Assert.True(pureResult.Succeeded);
        Assert.Equal(new Axis(Proportion.Zero, new Proportion(2, 1), AxisBasis.SplitComplex), pureResult.PrincipalCandidate);
        Assert.False(mixedResult.Succeeded);
        Assert.Contains(mixedResult.Tensions, tension => tension.Kind == InverseContinuationTensionKind.UnsupportedBasis);
    }

    [Fact]
    public void Area_InverseContinuation_SupportsFoldFirstAndExplainsStructurePreservingGap()
    {
        var area = new Area(Axis.I, Axis.I);

        var folded = area.InverseContinue(2);
        var structural = area.InverseContinue(2, AreaInverseContinuationMode.StructurePreserving);

        Assert.True(folded.Succeeded);
        Assert.Equal(Axis.I, folded.PrincipalCandidate);
        Assert.False(structural.Succeeded);
        Assert.Contains(structural.Tensions, tension => tension.Kind == InverseContinuationTensionKind.StructurePreservingUnavailable);
    }
}
