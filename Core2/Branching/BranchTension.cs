namespace Core2.Branching;

public enum BranchTensionKind
{
    UnresolvedMultiplicity,
    LiftRequired,
    ConstraintConflict,
    RejoinCollapsed,
}

public readonly record struct BranchTension(
    BranchTensionKind Kind,
    string Message);
