namespace Core3.Engine;

/// <summary>
/// An exact lower-grade read produced by folding a grade-0 engine element.
/// This is not a graded element and should not be fed back into the engine as
/// though it carried full structural provenance.
/// </summary>
public sealed class FoldedRatio
{
    internal FoldedRatio(long numerator, long denominator)
    {
        if (denominator == 0)
        {
            throw new InvalidOperationException("A folded ratio requires a nonzero unit.");
        }

        Numerator = numerator;
        Denominator = denominator;
    }

    public long Numerator { get; }
    public long Denominator { get; }
    public decimal DecimalValue => (decimal)Numerator / Denominator;

    public override string ToString() => $"{Numerator}/{Denominator}";
}
