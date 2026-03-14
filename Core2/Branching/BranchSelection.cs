namespace Core2.Branching;

public readonly record struct BranchSelection(
    BranchSelectionMode Mode,
    BranchId? SelectedId,
    string? Reason = null)
{
    public static BranchSelection None => new(BranchSelectionMode.None, null);

    public static BranchSelection Principal(BranchId selectedId, string? reason = null) =>
        new(BranchSelectionMode.Principal, selectedId, reason);

    public static BranchSelection Explicit(BranchId selectedId, string? reason = null) =>
        new(BranchSelectionMode.Explicit, selectedId, reason);

    public bool HasSelection => SelectedId is not null;
}
