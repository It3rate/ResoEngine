namespace Core2.Symbolics.Expressions;

public sealed record SharedCarrierTerm : RelationTerm
{
    public SharedCarrierTerm(ValueTerm left, ValueTerm right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        Right = right;
    }

    public ValueTerm Left { get; }
    public ValueTerm Right { get; }
}
