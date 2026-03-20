namespace Core2.Symbolics.Expressions;

public sealed record CommitTerm : ProgramTerm
{
    public CommitTerm(string name, SymbolicTerm value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
    }

    public string Name { get; }
    public SymbolicTerm Value { get; }
}
