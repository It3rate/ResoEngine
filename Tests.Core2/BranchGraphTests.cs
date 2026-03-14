using Core2.Branching;

namespace Tests.Core2;

public class BranchGraphTests
{
    [Fact]
    public void Seed_CreatesRootNodeAndSelectedFrontier()
    {
        var graph = new BranchGraphBuilder<int>()
            .Seed(10)
            .Build();

        var root = Assert.Single(graph.Roots);
        Assert.Equal(10, root.Value);
        Assert.Equal(0, root.Depth);
        Assert.Single(graph.Leaves);
        Assert.Single(graph.Events);
        Assert.Equal(BranchEventKind.Seed, graph.Events[0].Kind);
        Assert.True(graph.TryGetSelectedNode(out var selected));
        Assert.Equal(root.Id, selected!.Id);
    }

    [Fact]
    public void ApplyFamily_SplitsSingleParentIntoTwoChildren()
    {
        var builder = new BranchGraphBuilder<int>().Seed(10);
        BranchId parentId = builder.CurrentFrontier.ActiveNodeIds[0];

        var left = new BranchMember<int>(BranchId.New(), 9, [], []);
        var right = new BranchMember<int>(BranchId.New(), 11, [], []);
        var family = BranchFamily<int>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [left, right],
            BranchSelection.Principal(right.Id));

        var graph = builder
            .ApplyFamily(family)
            .Build();

        Assert.Equal(3, graph.Nodes.Count);
        Assert.Equal(2, graph.GetChildren(parentId).Count);
        Assert.All(graph.GetOutgoingEdges(parentId), edge => Assert.Equal(BranchEdgeKind.Split, edge.Kind));
        Assert.Equal(right.Id, graph.CurrentFrontier.SelectedId);
    }

    [Fact]
    public void ApplyFamily_RejoinsTwoParentsIntoOneChild()
    {
        var builder = new BranchGraphBuilder<int>().Seed(10);

        var left = new BranchMember<int>(BranchId.New(), 9, [], []);
        var right = new BranchMember<int>(BranchId.New(), 11, [], []);
        builder.ApplyFamily(BranchFamily<int>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [left, right],
            BranchSelection.Principal(right.Id)));

        var rejoined = new BranchMember<int>(BranchId.New(), 10, [left.Id, right.Id], []);
        var graph = builder
            .ApplyFamily(
                BranchFamily<int>.FromMembers(
                    BranchOrigin.Continuation,
                    BranchSemantics.Alternative,
                    BranchDirection.Forward,
                    [rejoined],
                    BranchSelection.Principal(rejoined.Id)),
                defaultParents: [])
            .Build();

        Assert.Single(graph.CurrentFrontier.ActiveNodeIds);
        Assert.Equal(rejoined.Id, graph.CurrentFrontier.ActiveNodeIds[0]);
        Assert.Equal(2, graph.GetParents(rejoined.Id).Count);
        Assert.All(graph.GetIncomingEdges(rejoined.Id), edge => Assert.Equal(BranchEdgeKind.Rejoin, edge.Kind));
    }

    [Fact]
    public void ApplyFamily_PreservesSelectedLineage_WhenOneDescendantComesFromSelectedParent()
    {
        var builder = new BranchGraphBuilder<int>().Seed(10);

        var left = new BranchMember<int>(BranchId.New(), 9, [], []);
        var right = new BranchMember<int>(BranchId.New(), 11, [], []);
        builder.ApplyFamily(BranchFamily<int>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [left, right],
            BranchSelection.Principal(right.Id)));

        var nextLeft = new BranchMember<int>(BranchId.New(), 8, [left.Id], []);
        var nextRight = new BranchMember<int>(BranchId.New(), 12, [right.Id], []);
        var graph = builder
            .ApplyFamily(
                BranchFamily<int>.FromMembers(
                    BranchOrigin.Continuation,
                    BranchSemantics.Mixed,
                    BranchDirection.Forward,
                    [nextLeft, nextRight]),
                defaultParents: [])
            .Build();

        Assert.Equal(nextRight.Id, graph.CurrentFrontier.SelectedId);
    }

    [Fact]
    public void Select_UpdatesFrontierWithoutCreatingNewNodes()
    {
        var builder = new BranchGraphBuilder<int>().Seed(10);

        var left = new BranchMember<int>(BranchId.New(), 9, [], []);
        var right = new BranchMember<int>(BranchId.New(), 11, [], []);
        builder.ApplyFamily(BranchFamily<int>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [left, right],
            BranchSelection.Principal(right.Id)));

        var graph = builder
            .Select(left.Id, "Follow the left path.")
            .Build();

        Assert.Equal(3, graph.Nodes.Count);
        Assert.Equal(3, graph.Events.Count);
        Assert.Equal(BranchEventKind.Selection, graph.Events[^1].Kind);
        Assert.Equal(left.Id, graph.CurrentFrontier.SelectedId);
    }
}
