using Core2.Branching;

namespace Core2.Symbolics.Branching;

public static class BranchFamilyTransformExtensions
{
    public static BranchFamily<T> WithParents<T>(
        this BranchFamily<T> family,
        IReadOnlyList<BranchId> parents,
        bool overwriteExistingParents = true)
    {
        ArgumentNullException.ThrowIfNull(family);

        var resolvedParents = parents.Distinct().ToArray();
        var members = family.Members
            .Select(member => member with
            {
                Parents = overwriteExistingParents || member.Parents.Count == 0
                    ? resolvedParents
                    : member.Parents
            })
            .ToArray();

        return BranchFamily<T>.FromMembers(
            family.Origin,
            family.Semantics,
            family.Direction,
            members,
            family.Selection,
            family.Tensions,
            family.Annotations);
    }

    public static BranchFamily<T> WithoutParents<T>(this BranchFamily<T> family) =>
        family.WithParents([], overwriteExistingParents: true);
}
