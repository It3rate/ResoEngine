namespace Core2.Symbolics.Expressions;

public sealed record SharedCarrierTerm : RelationTerm
{
    public SharedCarrierTerm(ReferenceTerm left, ReferenceTerm right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        Right = right;
    }

    public ReferenceTerm Left { get; }
    public ReferenceTerm Right { get; }
}
