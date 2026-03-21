namespace Core2.Symbolics.Expressions;

public sealed record SymbolicEnvironmentScope(
    string Name,
    string QualifiedName,
    IReadOnlyList<KeyValuePair<string, SymbolicTerm>> DirectBindings,
    IReadOnlyList<SymbolicEnvironmentScope> Children)
{
    public bool IsRoot => string.IsNullOrEmpty(QualifiedName);
}
