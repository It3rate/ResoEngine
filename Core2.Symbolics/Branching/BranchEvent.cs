using Core2.Branching;

namespace Core2.Symbolics.Branching;

public sealed record BranchEvent<T>(
    BranchEventId Id,
    int Index,
    BranchEventKind Kind,
    BranchFrontier IncomingFrontier,
    BranchFamily<T>? Family,
    BranchFrontier OutgoingFrontier,
    IReadOnlyList<BranchId> CreatedNodeIds,
    IReadOnlyList<IBranchAnnotation> Annotations)
{
    public BranchOrigin? Origin => Family?.Origin;
    public BranchSemantics? Semantics => Family?.Semantics;
    public BranchDirection? Direction => Family?.Direction;
    public IReadOnlyList<BranchTension> Tensions => Family?.Tensions ?? [];
}
