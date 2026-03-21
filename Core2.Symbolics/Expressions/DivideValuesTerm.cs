namespace Core2.Symbolics.Expressions;

public sealed record DivideValuesTerm : ValueTerm
{
    public DivideValuesTerm(ValueTerm Left, ValueTerm Right)
    {
        ArgumentNullException.ThrowIfNull(Left);
        ArgumentNullException.ThrowIfNull(Right);

        this.Left = Left;
        this.Right = Right;
    }

    public ValueTerm Left { get; }
    public ValueTerm Right { get; }
}
