using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ElementLiteralTerm : ValueTerm
{
    public ElementLiteralTerm(IElement value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
    }

    public IElement Value { get; }
}
