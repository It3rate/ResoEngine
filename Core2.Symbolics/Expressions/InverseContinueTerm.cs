using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public sealed record InverseContinueTerm : ValueTerm
{
    public InverseContinueTerm(ValueTerm Source, Proportion Degree)
        : this(Source, Degree, InverseContinuationRule.Principal, null)
    {
    }

    public InverseContinueTerm(
        ValueTerm Source,
        Proportion Degree,
        InverseContinuationRule Rule,
        ValueTerm? Reference)
    {
        ArgumentNullException.ThrowIfNull(Source);

        this.Source = Source;
        this.Degree = Degree;
        this.Rule = Rule;
        this.Reference = Reference;
    }

    public ValueTerm Source { get; }
    public Proportion Degree { get; }
    public InverseContinuationRule Rule { get; }
    public ValueTerm? Reference { get; }
}
