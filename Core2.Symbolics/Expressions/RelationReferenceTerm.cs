namespace Core2.Symbolics.Expressions;

public sealed record RelationReferenceTerm : RelationTerm
{
    public RelationReferenceTerm(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}
