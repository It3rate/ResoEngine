using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record InverseContinueTerm : ValueTerm
{
    public InverseContinueTerm(ValueTerm Source, Proportion Degree)
    {
        ArgumentNullException.ThrowIfNull(Source);

        this.Source = Source;
        this.Degree = Degree;
    }

    public ValueTerm Source { get; }
    public Proportion Degree { get; }
}
