using Core2.Branching;

namespace Core2.Symbolics.Repetition;

public readonly record struct InverseContinuationTension(
    InverseContinuationTensionKind Kind,
    string Message);

public sealed record InverseContinuationResult<T>
{
    public InverseContinuationResult(
        IReadOnlyList<T> candidates,
        T? principalCandidate,
        IReadOnlyList<InverseContinuationTension> tensions)
        : this(CreateBranches(candidates, principalCandidate), tensions)
    {
    }

    public InverseContinuationResult(
        BranchFamily<T> branches,
        IReadOnlyList<InverseContinuationTension> tensions)
    {
        Branches = branches;
        Tensions = tensions.ToArray();
    }

    public BranchFamily<T> Branches { get; }
    public IReadOnlyList<T> Candidates => Branches.Values;
    public T? PrincipalCandidate => Branches.SelectedValue;
    public IReadOnlyList<InverseContinuationTension> Tensions { get; }
    public bool Succeeded => Branches.HasMembers;

    private static BranchFamily<T> CreateBranches(
        IReadOnlyList<T> candidates,
        T? principalCandidate)
    {
        int? selectedIndex = ResolveSelectedIndex(candidates, principalCandidate);
        return BranchFamily<T>.FromValues(
            BranchOrigin.Preimage,
            BranchSemantics.Alternative,
            BranchDirection.Reverse,
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
