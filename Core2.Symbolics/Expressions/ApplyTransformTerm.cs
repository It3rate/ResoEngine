namespace Core2.Symbolics.Expressions;

public sealed record ApplyTransformTerm : ValueTerm
{
    public ApplyTransformTerm(ValueTerm state, TransformTerm transform)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(transform);

        State = state;
        Transform = transform;
    }

    public ValueTerm State { get; }
    public TransformTerm Transform { get; }
}
