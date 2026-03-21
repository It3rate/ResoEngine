namespace Core2.Symbolics.Expressions;

public sealed record BindTerm : ProgramTerm
{
    public BindTerm(string name, SymbolicTerm value)
        : this(new SymbolicBindingTarget(name), value)
    {
    }

    public BindTerm(SymbolicBindingTarget target, SymbolicTerm value)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(value);

        Target = target;
        Value = value;
    }

    public SymbolicBindingTarget Target { get; }
    public string Name => Target.QualifiedName;
    public SymbolicTerm Value { get; }
}
