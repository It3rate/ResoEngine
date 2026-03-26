namespace Core3.Engine;

/// <summary>
/// A lossy decimal read produced by folding an exact grade-0 engine element.
/// This is not a graded element and should not be fed back into the engine as
/// though it carried full provenance.
/// A decimal value carries the least possible infornation about a value, as the resolution is unknown.
/// A value of 0.5 may represent 1 sample out of 2, or 1000 samples out of 2000, showing the resolution has been discarded.
/// </summary>
public sealed class FoldedApproximation
{
    internal FoldedApproximation(long value, long unit)
    {
        if (unit == 0)
        {
            throw new InvalidOperationException("A folded approximation requires a nonzero unit.");
        }

        Value = value;
        Unit = unit;
    }

    public long Value { get; }
    public long Unit { get; }
    public decimal DecimalValue => (decimal)Value / Unit;

    public override string ToString() => $"{DecimalValue} ~= {Value}/{Unit}";
}
