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

        if (denominator < 0)
        {
            numerator = checked(-numerator);
            denominator = checked(-denominator);
        }

        var divisor = GreatestCommonDivisor(Math.Abs(numerator), denominator);
        Numerator = numerator / divisor;
        Denominator = denominator / divisor;
    }

    public long Numerator { get; }
    public long Denominator { get; }

    public static FoldedRatio Multiply(FoldedRatio left, FoldedRatio right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return new FoldedRatio(
            checked(left.Numerator * right.Numerator),
            checked(left.Denominator * right.Denominator));
    }

    public static FoldedRatio Divide(FoldedRatio left, FoldedRatio right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (right.Numerator == 0)
        {
            throw new InvalidOperationException("Cannot divide by a zero folded ratio.");
        }

        return new FoldedRatio(
            checked(left.Numerator * right.Denominator),
            checked(left.Denominator * right.Numerator));
    }

    public override string ToString() => $"{Numerator}/{Denominator}";

    private static long GreatestCommonDivisor(long left, long right)
    {
        while (right != 0)
        {
            var remainder = left % right;
            left = right;
            right = remainder;
        }

        return left == 0 ? 1 : left;
    }
}
