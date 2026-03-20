namespace Core2.Symbolics.Expressions;

public sealed record EqualityTerm : RelationTerm
{
    public EqualityTerm(SymbolicTerm left, SymbolicTerm right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        Right = right;
    }

    public SymbolicTerm Left { get; }
    public SymbolicTerm Right { get; }
}
