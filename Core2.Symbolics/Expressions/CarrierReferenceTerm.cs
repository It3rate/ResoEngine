namespace Core2.Symbolics.Expressions;

public sealed record CarrierReferenceTerm : ValueTerm
{
    public CarrierReferenceTerm(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}
