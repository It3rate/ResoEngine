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

        var divisor = GreatestCommonDivisor(Math.Abs(numerator), Math.Abs(denominator));
        Numerator = numerator / divisor;
        CarrierMagnitude = Math.Abs(denominator) / divisor;
        CarrierPolarity = Math.Sign(denominator);
    }

    public long Numerator { get; }
    public long Denominator => CarrierPolarity * CarrierMagnitude;
    public long CarrierMagnitude { get; }
    public int CarrierPolarity { get; }
    public bool HasAlignedCarrier => CarrierPolarity > 0;
    public bool HasOrthogonalCarrier => CarrierPolarity < 0;

    public bool CanComposeRatioWith(FoldedRatio other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return CarrierPolarity == other.CarrierPolarity;
    }

    public static FoldedProduct Multiply(FoldedRatio left, FoldedRatio right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return new FoldedProduct(left, right);
    }

    public static FoldedRatio Divide(FoldedRatio left, FoldedRatio right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (!left.CanComposeRatioWith(right))
        {
            throw new InvalidOperationException("Cannot divide folded ratios with mismatched carrier polarity.");
        }

        if (right.Numerator == 0)
        {
            throw new InvalidOperationException("Cannot divide by a zero folded ratio.");
        }

        var numerator = checked(left.Numerator * right.CarrierMagnitude);

        if (right.Numerator < 0)
        {
            numerator = checked(-numerator);
        }

        var denominator = checked(left.CarrierMagnitude * Math.Abs(right.Numerator));
        return new FoldedRatio(numerator, denominator);
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
