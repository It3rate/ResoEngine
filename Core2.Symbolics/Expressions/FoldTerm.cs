namespace Core2.Symbolics.Expressions;

public sealed record FoldTerm : ValueTerm
{
    public FoldTerm(ValueTerm source, SymbolicFoldKind kind = SymbolicFoldKind.Canonical)
    {
        ArgumentNullException.ThrowIfNull(source);

        Source = source;
        Kind = kind;
    }

    public ValueTerm Source { get; }
    public SymbolicFoldKind Kind { get; }
}
