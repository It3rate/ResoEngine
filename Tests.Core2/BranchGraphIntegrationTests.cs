using Core2.Branching;
using Core2.Elements;
using Core2.Symbolics.Branching;
using Core2.Support;

namespace Tests.Core2;

public class BranchGraphIntegrationTests
{
    [Fact]
    public void InverseContinuation_ToBranchGraph_SeedsSourceAndCandidates()
    {
        var result = new Scalar(4).InverseContinue(2);

        var graph = result.ToBranchGraph(new Scalar(4));

        Assert.Equal(3, graph.Nodes.Count);
        Assert.Single(graph.Roots);
        var root = graph.Roots[0];
        Assert.Equal(new Scalar(4), root.Value);
        Assert.Equal(2, graph.GetChildren(root.Id).Count);
        Assert.All(graph.GetOutgoingEdges(root.Id), edge => Assert.Equal(BranchEdgeKind.Split, edge.Kind));
        Assert.Equal(new Scalar(2), Assert.IsType<Scalar>(graph.GetNode(graph.CurrentFrontier.SelectedId!.Value).Value));
    }

    [Fact]
    public void PowerResult_ToBranchGraph_RewiresToSourceFrontierForStandaloneGraph()
    {
        var result = new Scalar(9).TryPow(new Proportion(1, 2));

        var graph = result.ToBranchGraph(new Scalar(9));

        Assert.Equal(3, graph.Nodes.Count);
        var root = Assert.Single(graph.Roots);
        Assert.Equal(new Scalar(9), root.Value);
        Assert.Equal(2, graph.GetChildren(root.Id).Count);
        Assert.All(graph.GetOutgoingEdges(root.Id), edge => Assert.Equal(BranchEdgeKind.Split, edge.Kind));
        Assert.Equal(new Scalar(3), graph.GetNode(graph.CurrentFrontier.SelectedId!.Value).Value);
    }

    [Fact]
    public void AxisBooleanResult_ToBranchGraph_ProducesCoPresentAxisFrontierState()
    {
        var middle = Axis.FromCoordinates((Scalar)3m, (Scalar)5m);
        var carrier = Axis.FromCoordinates((Scalar)0m, (Scalar)10m);
        var result = AxisBooleanProjection.Resolve(middle, carrier, AxisBooleanOperation.Xor);

        var graph = result.ToBranchGraph();
        var frontier = AxisBranchGraphAdapter.GetFrontierState(graph);

        Assert.Equal(2, frontier.Segments.Count);
        Assert.Null(frontier.SelectedSegment);
        Assert.Equal(0m, frontier.Segments[0].Start.Value);
        Assert.Equal(3m, frontier.Segments[0].End.Value);
        Assert.Equal(5m, frontier.Segments[1].Start.Value);
        Assert.Equal(10m, frontier.Segments[1].End.Value);
        Assert.NotNull(frontier.Envelope);
        Assert.Equal(0m, frontier.Envelope!.Left.Value);
        Assert.Equal(10m, frontier.Envelope.Right.Value);
    }
}
