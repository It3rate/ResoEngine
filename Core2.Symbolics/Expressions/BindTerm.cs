namespace Core2.Symbolics.Expressions;

public sealed record BindTerm : ProgramTerm
{
    public BindTerm(string name, SymbolicTerm value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
    }

    public string Name { get; }
    public SymbolicTerm Value { get; }
}
