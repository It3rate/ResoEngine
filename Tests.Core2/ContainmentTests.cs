using Core2.Elements;
using Core2.Interpretation.Support;

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

        decimal proportionRangeMetric = proportionRelation.TensionMetrics.GetAmount(ContainmentTensionKind.OutsideExpectedRange, string.Empty)!.Fold();
        Assert.True(Math.Abs(proportionRangeMetric - (1m / 6m)) < 0.000000000000000000000000001m);
        Assert.Equal(1m / 2m, (decimal)proportionRelation.TensionMetrics.GetAmount(ContainmentTensionKind.ResolutionMismatch, string.Empty)!.Fold());
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
    public void AxisContainment_UsesWholeAxisEnvelope_AndSupportTensions()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Axis(new Proportion(7, 3), new Proportion(3, 2));

        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.ResolutionMismatch && tension.Path == "recessive.support");
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.OutsideExpectedRange && tension.Path == "recessive.boundary");
        Assert.DoesNotContain(relation.Tensions, tension => tension.Path.StartsWith("dominant"));
        Assert.Equal(1m / 15m, (decimal)relation.TensionMetrics.StartRange!.Value.Amount.Fold());
        Assert.Equal(1m / 2m, (decimal)relation.TensionMetrics.RecessiveSupport!.Value.Amount.Fold());
        Assert.True(relation.HasTension);
        Assert.True(relation.HasTensionOf(ContainmentTensionKind.OutsideExpectedRange));
    }

    [Fact]
    public void AxisContainment_UsesEnvelopeBounds_WhenParentSegmentIsReversed()
    {
        var parent = Axis.FromCoordinates((Scalar)7m, (Scalar)(-6.5m)).AsNode();
        var child = Axis.FromCoordinates((Scalar)2m, (Scalar)(-3.5m));

        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));
        Assert.Empty(relation.Tensions);
        Assert.False(relation.TensionMetrics.HasAny);
    }

    [Fact]
    public void AxisContainment_RecordsEndSpecificMetrics_OnBothSides()
    {
        var parent = Axis.FromCoordinates((Scalar)(-2m), (Scalar)3m).AsNode();
        var child = new Axis(new Proportion(8, 2), new Proportion(20, 4));

        var relation = parent.AddChild(child);

        Assert.Equal(2m / 5m, (decimal)relation.TensionMetrics.StartRange!.Value.Amount.Fold());
        Assert.Equal(2m / 5m, (decimal)relation.TensionMetrics.EndRange!.Value.Amount.Fold());
        Assert.Equal(1m, (decimal)relation.TensionMetrics.RecessiveSupport!.Value.Amount.Fold());
        Assert.Equal(3m, (decimal)relation.TensionMetrics.DominantSupport!.Value.Amount.Fold());
    }

    [Fact]
    public void OppositePerspective_IsStoredOnContainmentRelation()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Axis(new Proportion(1, 2), new Proportion(2, 2));

        parent.Perspective = Perspective.Opposite;
        var relation = parent.AddChild(child);

        Assert.Equal(Perspective.Opposite, relation.Perspective);
        Assert.Equal(-child, Assert.IsType<Axis>(relation.ChildInParentContext));
    }

    [Fact]
    public void ChangingParentPerspective_ReinterpretsExistingChildLive()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Axis(new Proportion(1, 2), new Proportion(2, 2));
        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));

        parent.OpposePerspective();

        Assert.Equal(-child, Assert.IsType<Axis>(relation.ChildInParentContext));
    }

    [Fact]
    public void ChildContainerPerspective_IsItsOwnAndDoesNotOverrideParentReading()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode(Perspective.Dominant);
        var child = new Axis(new Proportion(1, 2), new Proportion(2, 2));

        var relation = parent.AddChild(child, Perspective.Opposite);

        Assert.Equal(Perspective.Dominant, relation.Perspective);
        Assert.Equal(Perspective.Opposite, relation.Child.Perspective);
        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));
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
            tension.Kind == ContainmentTensionKind.ResolutionMismatch && tension.Path == "recessive-axis.recessive.support");
        Assert.Contains(relation.Tensions, tension =>
            tension.Kind == ContainmentTensionKind.OutsideExpectedRange && tension.Path == "recessive-axis.recessive.boundary");
        Assert.DoesNotContain(relation.Tensions, tension => tension.Path.StartsWith("dominant-axis"));
    }

    [Fact]
    public void AreaParent_CanHostAxisChild_WithPlacementUncertaintyInsteadOfUnsupported()
    {
        var parent = new Area(
            new Axis(new Proportion(4, 2), new Proportion(6, 2)),
            new Axis(new Proportion(5, 2), new Proportion(8, 2)))
            .AsNode();
        var child = new Axis(new Proportion(1, 2), new Proportion(2, 2));

        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Axis>(relation.ChildInParentContext));
        Assert.Contains(relation.Tensions, tension => tension.Kind == ContainmentTensionKind.PlacementUnderspecified);
        Assert.DoesNotContain(relation.Tensions, tension => tension.Kind == ContainmentTensionKind.UnsupportedInterpretation);
    }

    [Fact]
    public void AxisParent_CanHostAreaChild_WithPlacementUncertaintyInsteadOfUnsupported()
    {
        var parent = new Axis(new Proportion(4, 2), new Proportion(6, 2)).AsNode();
        var child = new Area(
            new Axis(new Proportion(1, 2), new Proportion(2, 2)),
            new Axis(new Proportion(1, 2), new Proportion(3, 2)));

        var relation = parent.AddChild(child);

        Assert.Equal(child, Assert.IsType<Area>(relation.ChildInParentContext));
        Assert.Contains(relation.Tensions, tension => tension.Kind == ContainmentTensionKind.PlacementUnderspecified);
        Assert.DoesNotContain(relation.Tensions, tension => tension.Kind == ContainmentTensionKind.UnsupportedInterpretation);
    }
}
