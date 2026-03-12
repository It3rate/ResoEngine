using Core2.Elements;
using Core2.Support;
using ResoEngine.Core2;

namespace Tests.Core2;

public class ContainmentTests
{
    [Fact]
    public void ProportionParent_InterpretsScalarChildUsingParentResolution()
    {
        var parent = new Proportion(4, 2).AsNode();

        var relation = parent.AddChild(new Scalar(3));

        Assert.Equal(new Proportion(3, 2), Assert.IsType<Proportion>(relation.ChildInParentContext));
        Assert.Empty(relation.Tensions);
    }

    [Fact]
    public void ProportionParent_RecordsRangeAndResolutionTensions()
    {
        var parent = new Proportion(4, 2).AsNode();

        var scalarRelation = parent.AddChild(new Scalar(7));
        var proportionRelation = parent.AddChild(new Proportion(7, 3));

        Assert.Equal(new Proportion(7, 2), Assert.IsType<Proportion>(scalarRelation.ChildInParentContext));
        Assert.Contains(scalarRelation.Tensions, tension => tension.Kind == ContainmentTensionKind.OutsideExpectedRange);

        Assert.Equal(new Proportion(7, 3), Assert.IsType<Proportion>(proportionRelation.ChildInParentContext));
        Assert.Contains(proportionRelation.Tensions, tension => tension.Kind == ContainmentTensionKind.OutsideExpectedRange);
        Assert.Contains(proportionRelation.Tensions, tension => tension.Kind == ContainmentTensionKind.ResolutionMismatch);
    }

    [Fact]
    public void SameElementBecomesParentOnlyThroughRelation()
    {
        var root = new Proportion(4, 2).AsNode();

        var childRelation = root.AddChild(new Proportion(3, 2));
        var grandchildRelation = childRelation.Child.AddChild(new Scalar(2));

        Assert.Same(childRelation, childRelation.Child.ParentRelation);
        Assert.Equal(new Proportion(2, 2), Assert.IsType<Proportion>(grandchildRelation.ChildInParentContext));
    }

    [Fact]
    public void AxisContainment_AnalyzesRecessiveAndDominantSlotsRecursively()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Axis(new Proportion(7, 3), new Proportion(3, 2));

        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.ResolutionMismatch && tension.Path == "recessive");
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.OutsideExpectedRange && tension.Path == "recessive");
        Assert.DoesNotContain(relation.Tensions, tension => tension.Path == "dominant");
    }

    [Fact]
    public void OppositePerspective_IsStoredOnContainmentRelation()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Axis(new Proportion(1, 2), new Proportion(2, 2));

        var relation = parent.AddChild(child, Perspective.Opposite);

        Assert.Equal(Perspective.Opposite, relation.Perspective);
        Assert.Equal(-child, Assert.IsType<Axis>(relation.ChildInParentContext));
    }

    [Fact]
    public void AreaContainment_RecursesAcrossNestedAxisStructure()
    {
        var parent = new Area(
            new Axis(new Proportion(4, 2), new Proportion(6, 2)),
            new Axis(new Proportion(5, 2), new Proportion(8, 2)))
            .AsNode();
        var child = new Area(
            new Axis(new Proportion(7, 3), new Proportion(3, 2)),
            new Axis(new Proportion(2, 2), new Proportion(3, 2)));

        var relation = parent.AddChild(child);

        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.ResolutionMismatch && tension.Path == "recessive.recessive");
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.OutsideExpectedRange && tension.Path == "recessive.recessive");
        Assert.DoesNotContain(relation.Tensions, tension => tension.Path.StartsWith("dominant"));
    }
}
