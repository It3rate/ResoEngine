using Core2.Branching;

namespace Core2.Symbolics.Branching;

public sealed record BranchFrontier
{
    public BranchFrontier(
        IReadOnlyList<BranchId> activeNodeIds,
        BranchSelection selection)
    {
        ActiveNodeIds = activeNodeIds.Distinct().ToArray();
        Selection = selection;
    }

    public static BranchFrontier Empty { get; } = new([], BranchSelection.None);

    public IReadOnlyList<BranchId> ActiveNodeIds { get; }
    public BranchSelection Selection { get; }
    public bool IsEmpty => ActiveNodeIds.Count == 0;
    public BranchId? SelectedId => Selection.SelectedId;

    public bool Contains(BranchId id) => ActiveNodeIds.Contains(id);
}
