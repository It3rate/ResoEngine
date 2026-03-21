using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record PowerTerm : ValueTerm
{
    public PowerTerm(ValueTerm Base, Proportion Exponent)
    {
        ArgumentNullException.ThrowIfNull(Base);

        this.Base = Base;
        this.Exponent = Exponent;
    }

    public ValueTerm Base { get; }
    public Proportion Exponent { get; }
}
