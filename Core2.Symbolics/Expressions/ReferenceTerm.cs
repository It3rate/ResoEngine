namespace Core2.Symbolics.Expressions;

public sealed record ReferenceTerm : SymbolicTerm
{
    public ReferenceTerm(string name, SymbolicTermSort referencedSort)
        : base(referencedSort)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        ReferencedSort = referencedSort;
    }

    public string Name { get; }
    public SymbolicTermSort ReferencedSort { get; }
}
