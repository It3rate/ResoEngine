namespace Core2.Symbolics.Expressions;

public sealed record EmitTerm : ProgramTerm
{
    public EmitTerm(SymbolicTerm value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
    }

    public SymbolicTerm Value { get; }
}
