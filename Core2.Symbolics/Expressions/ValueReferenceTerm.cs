namespace Core2.Symbolics.Expressions;

public sealed record ValueReferenceTerm : ValueTerm
{
    public ValueReferenceTerm(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}
