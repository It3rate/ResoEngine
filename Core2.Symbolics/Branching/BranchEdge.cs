using Core2.Branching;

namespace Core2.Symbolics.Branching;

public readonly record struct BranchEdge(
    BranchId ParentId,
    BranchId ChildId,
    BranchEdgeKind Kind);
