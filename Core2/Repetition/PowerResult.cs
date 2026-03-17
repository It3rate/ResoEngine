using Core2.Branching;

namespace Core2.Repetition;

public enum PowerTensionKind
{
    InvalidExponent,
    UnsupportedNegativeExponent,
    InverseContinuationFailed,
    ShapeChangingPower,
    ComputationOverflow,
}

public readonly record struct PowerTension(
    PowerTensionKind Kind,
    string Message);

public sealed record PowerResult<T>
{
    public PowerResult(
        IReadOnlyList<T> candidates,
        T? principalCandidate,
        IReadOnlyList<PowerTension> tensions)
        : this(CreateBranches(candidates, principalCandidate), tensions)
    {
    }

    public PowerResult(
        BranchFamily<T> branches,
        IReadOnlyList<PowerTension> tensions)
    {
        Branches = branches;
        Tensions = tensions.ToArray();
    }

    public BranchFamily<T> Branches { get; }
    public IReadOnlyList<T> Candidates => Branches.Values;
    public T? PrincipalCandidate => Branches.SelectedValue;
    public IReadOnlyList<PowerTension> Tensions { get; }
    public bool Succeeded => Branches.HasMembers;

    private static BranchFamily<T> CreateBranches(
        IReadOnlyList<T> candidates,
        T? principalCandidate)
    {
        int? selectedIndex = ResolveSelectedIndex(candidates, principalCandidate);
        return BranchFamily<T>.FromValues(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            candidates,
            selectedIndex,
            selectedIndex.HasValue ? BranchSelectionMode.Principal : BranchSelectionMode.None);
    }

    private static int? ResolveSelectedIndex(
        IReadOnlyList<T> candidates,
        T? selectedValue)
    {
        if (selectedValue is null)
        {
            return null;
        }

        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < candidates.Count; i++)
        {
            if (comparer.Equals(candidates[i], selectedValue))
            {
                return i;
            }
        }

        return null;
    }
}
