using Core2.Elements;

namespace Core2.Branching;

public sealed record AxisBranchFrontierState(
    IReadOnlyList<Axis> Segments,
    Axis? SelectedSegment)
{
    public bool HasSegments => Segments.Count > 0;

    public Axis? Envelope =>
        Segments.Count == 0
            ? null
            : Segments.Skip(1).Aggregate(Segments[0], (current, next) => current.Envelope(next));
}

public static class AxisBranchGraphAdapter
{
    public static AxisBranchFrontierState GetFrontierState(BranchGraph<Axis> graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        var segments = graph.CurrentFrontier.ActiveNodeIds
            .Select(id => graph.GetNode(id).Value)
            .ToArray();

        Axis? selectedSegment = graph.TryGetSelectedNode(out var selected)
            ? selected!.Value
            : null;

        return new AxisBranchFrontierState(segments, selectedSegment);
    }

    public static IReadOnlyList<Axis> GetParentSegments(BranchGraph<Axis> graph, BranchId childId)
    {
        ArgumentNullException.ThrowIfNull(graph);

        return graph.GetParents(childId)
            .Select(node => node.Value)
            .ToArray();
    }

    public static IReadOnlyList<Axis> GetChildSegments(BranchGraph<Axis> graph, BranchId parentId)
    {
        ArgumentNullException.ThrowIfNull(graph);

        return graph.GetChildren(parentId)
            .Select(node => node.Value)
            .ToArray();
    }
}
