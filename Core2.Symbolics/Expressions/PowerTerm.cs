using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public sealed record PowerTerm : ValueTerm
{
    public PowerTerm(ValueTerm Base, Proportion Exponent)
        : this(Base, Exponent, InverseContinuationRule.Principal, null)
    {
    }

    public PowerTerm(
        ValueTerm Base,
        Proportion Exponent,
        InverseContinuationRule Rule,
        ValueTerm? Reference)
    {
        ArgumentNullException.ThrowIfNull(Base);

        this.Base = Base;
        this.Exponent = Exponent;
        this.Rule = Rule;
        this.Reference = Reference;
    }

    public ValueTerm Base { get; }
    public Proportion Exponent { get; }
    public InverseContinuationRule Rule { get; }
    public ValueTerm? Reference { get; }
}
