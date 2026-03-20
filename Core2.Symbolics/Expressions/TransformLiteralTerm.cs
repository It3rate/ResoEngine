using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record TransformLiteralTerm : TransformTerm
{
    public TransformLiteralTerm(IElement code)
    {
        ArgumentNullException.ThrowIfNull(code);

        Code = code;
    }

    public IElement Code { get; }
}
