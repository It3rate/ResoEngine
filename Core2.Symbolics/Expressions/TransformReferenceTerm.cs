namespace Core2.Symbolics.Expressions;

public sealed record TransformReferenceTerm : TransformTerm
{
    public TransformReferenceTerm(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}
