using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public sealed record BranchFamilyTerm : ValueTerm
{
    public BranchFamilyTerm(BranchFamily<ValueTerm> family)
    {
        ArgumentNullException.ThrowIfNull(family);

        Family = family;
    }

    public BranchFamily<ValueTerm> Family { get; }
}
