namespace Core2.Branching;

public readonly record struct BranchEdge(
    BranchId ParentId,
    BranchId ChildId,
    BranchEdgeKind Kind);
