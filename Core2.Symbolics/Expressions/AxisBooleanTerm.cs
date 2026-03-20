using Core2.Boolean;

namespace Core2.Symbolics.Expressions;

public sealed record AxisBooleanTerm : ValueTerm
{
    public AxisBooleanTerm(
        ValueTerm primary,
        ValueTerm secondary,
        AxisBooleanOperation operation,
        ValueTerm? frame = null)
    {
        ArgumentNullException.ThrowIfNull(primary);
        ArgumentNullException.ThrowIfNull(secondary);

        Primary = primary;
        Secondary = secondary;
        Operation = operation;
        Frame = frame;
    }

    public ValueTerm Primary { get; }
    public ValueTerm Secondary { get; }
    public AxisBooleanOperation Operation { get; }
    public ValueTerm? Frame { get; }
}
