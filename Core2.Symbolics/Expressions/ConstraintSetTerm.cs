namespace Core2.Symbolics.Expressions;

public sealed record ConstraintSetTerm : ConstraintTerm
{
    public ConstraintSetTerm(IReadOnlyList<ConstraintTerm> constraints)
    {
        ArgumentNullException.ThrowIfNull(constraints);

        Constraints = constraints.ToArray();
    }

    public IReadOnlyList<ConstraintTerm> Constraints { get; }
}
