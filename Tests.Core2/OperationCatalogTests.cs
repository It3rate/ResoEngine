using Core2.Elements;
using Core2.Support;

namespace Tests.Core2;

public class OperationCatalogTests
{
    [Fact]
    public void Scalar_KeepsComparisonDormant_ButAllowsRelationalScaling()
    {
        IElement scalar = new Scalar(5);

        var compare = scalar.DescribeOperation(ElementOperation.Compare);
        var scale = scalar.DescribeOperation(ElementOperation.Scale);

        Assert.Equal(OperationActivation.Dormant, compare.Activation);
        Assert.Equal(1, compare.MinimumGrade);
        Assert.Equal(OperationActivation.Relational, scale.Activation);
        Assert.True(scale.RequiresRelation);
    }

    [Fact]
    public void Proportion_MakesMirrorAndFoldRelational()
    {
        IElement proportion = new Proportion(4, 2);

        var mirror = proportion.DescribeOperation(ElementOperation.Mirror);
        var fold = proportion.DescribeOperation(ElementOperation.Fold);

        Assert.Equal(OperationActivation.Relational, mirror.Activation);
        Assert.Contains("reciprocal", mirror.Summary, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(OperationActivation.Relational, fold.Activation);
    }

    [Fact]
    public void Axis_UnlocksPerspectiveFlipAndConjugateAsOriented()
    {
        IElement axis = Axis.One;

        Assert.Equal(OperationActivation.Oriented, axis.DescribeOperation(ElementOperation.PerspectiveFlip).Activation);
        Assert.Equal(OperationActivation.Oriented, axis.DescribeOperation(ElementOperation.Conjugate).Activation);
        Assert.Equal(OperationActivation.Oriented, axis.DescribeOperation(ElementOperation.Compare).Activation);
    }

    [Fact]
    public void Area_UnlocksExpansionAndBooleanMergeAsExpansive()
    {
        IElement area = new Area(Axis.One, Axis.I);

        Assert.Equal(OperationActivation.Expansive, area.DescribeOperation(ElementOperation.ExpandMultiply).Activation);
        Assert.Equal(OperationActivation.Expansive, area.DescribeOperation(ElementOperation.BooleanMerge).Activation);
        Assert.Equal(OperationActivation.Expansive, area.DescribeOperation(ElementOperation.Fold).Activation);
    }

    [Fact]
    public void DescribeOperations_ExposesTheFullCatalogForAnyElement()
    {
        IElement axis = Axis.One;

        var profiles = axis.DescribeOperations();

        Assert.Equal(Enum.GetValues<ElementOperation>().Length, profiles.Count);
        Assert.Equal(ElementOperation.Pin, profiles[0].Operation);
        Assert.Contains(profiles, profile => profile.Operation == ElementOperation.PerspectiveFlip);
    }
}
