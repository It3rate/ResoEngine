namespace Core2.Symbolics.Expressions;

public sealed record SymbolicBindingTarget
{
    public SymbolicBindingTarget(string qualifiedName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(qualifiedName);

        QualifiedName = qualifiedName;
        Segments = qualifiedName.Split('.', StringSplitOptions.RemoveEmptyEntries);
    }

    public string QualifiedName { get; }
    public IReadOnlyList<string> Segments { get; }

    public bool IsScoped => Segments.Count > 1;

    public override string ToString() => QualifiedName;
}
