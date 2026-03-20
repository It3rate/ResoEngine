using Core2.Branching;

namespace Core2.Symbolics.Branching;

public sealed class BranchGraph<T>
{
    private readonly IReadOnlyDictionary<BranchId, BranchNode<T>> _nodesById;
    private readonly IReadOnlyDictionary<BranchId, IReadOnlyList<BranchEdge>> _incomingEdgesByChild;
    private readonly IReadOnlyDictionary<BranchId, IReadOnlyList<BranchEdge>> _outgoingEdgesByParent;

    public BranchGraph(
        IReadOnlyList<BranchNode<T>> nodes,
        IReadOnlyList<BranchEdge> edges,
        IReadOnlyList<BranchEvent<T>> events)
    {
        Nodes = nodes.ToArray();
        Edges = edges.ToArray();
        Events = events.ToArray();
        _nodesById = Nodes.ToDictionary(node => node.Id);
        _incomingEdgesByChild = Edges
            .GroupBy(edge => edge.ChildId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<BranchEdge>)group.ToArray());
        _outgoingEdgesByParent = Edges
            .GroupBy(edge => edge.ParentId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<BranchEdge>)group.ToArray());
    }

    public IReadOnlyList<BranchNode<T>> Nodes { get; }
    public IReadOnlyList<BranchEdge> Edges { get; }
    public IReadOnlyList<BranchEvent<T>> Events { get; }
    public BranchFrontier CurrentFrontier => Events.Count == 0 ? BranchFrontier.Empty : Events[^1].OutgoingFrontier;
    public IReadOnlyList<BranchNode<T>> Roots => Nodes.Where(node => GetIncomingEdges(node.Id).Count == 0).ToArray();
    public IReadOnlyList<BranchNode<T>> Leaves => Nodes.Where(node => GetOutgoingEdges(node.Id).Count == 0).ToArray();

    public BranchNode<T> GetNode(BranchId id) =>
        _nodesById.TryGetValue(id, out var node)
            ? node
            : throw new KeyNotFoundException($"No branch node exists for id {id}.");

    public bool TryGetNode(BranchId id, out BranchNode<T>? node) =>
        _nodesById.TryGetValue(id, out node);

    public IReadOnlyList<BranchEdge> GetIncomingEdges(BranchId childId) =>
        _incomingEdgesByChild.TryGetValue(childId, out var edges) ? edges : [];

    public IReadOnlyList<BranchEdge> GetOutgoingEdges(BranchId parentId) =>
        _outgoingEdgesByParent.TryGetValue(parentId, out var edges) ? edges : [];

    public IReadOnlyList<BranchNode<T>> GetParents(BranchId childId) =>
        GetIncomingEdges(childId)
            .Select(edge => GetNode(edge.ParentId))
            .ToArray();

    public IReadOnlyList<BranchNode<T>> GetChildren(BranchId parentId) =>
        GetOutgoingEdges(parentId)
            .Select(edge => GetNode(edge.ChildId))
            .ToArray();

    public int GetDepth(BranchId id) => GetNode(id).Depth;

    public bool TryGetSelectedNode(out BranchNode<T>? node)
    {
        node = null;
        if (!CurrentFrontier.SelectedId.HasValue)
        {
            return false;
        }

        return TryGetNode(CurrentFrontier.SelectedId.Value, out node);
    }
}
