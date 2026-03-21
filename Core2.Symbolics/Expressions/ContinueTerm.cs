using Core2.Repetition;

namespace Core2.Symbolics.Expressions;

public sealed record ContinueTerm : ValueTerm
{
    public ContinueTerm(ValueTerm frame, ValueTerm value, BoundaryContinuationLaw law)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(value);

        Frame = frame;
        Value = value;
        Law = law;
    }

    public ValueTerm Frame { get; }
    public ValueTerm Value { get; }
    public BoundaryContinuationLaw Law { get; }
}
