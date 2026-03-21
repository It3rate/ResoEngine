using Core2.Elements;
using System.Numerics;

namespace Core2.Resolution;

/// <summary>
/// Explicit primitive support operations for degree-1 resolution behavior.
/// These are law-specific helpers rather than replacements for the existing
/// raw arithmetic operators on <see cref="Proportion" />.
/// </summary>
public static class PrimitiveProportionResolution
{
    public static Proportion Aggregate(Proportion left, Proportion right) =>
        new(
            checked(left.Numerator + right.Numerator),
            checked(left.Denominator + right.Denominator));

    public static long GetCommonSupport(Proportion left, Proportion right)
    {
        if (left.Denominator == 0)
        {
            return right.Denominator;
        }

        if (right.Denominator == 0)
        {
            return left.Denominator;
        }

        BigInteger magnitude = Lcm(BigInteger.Abs(left.Denominator), BigInteger.Abs(right.Denominator));
        int sign = Math.Sign(left.Denominator);
        if (sign == 0)
        {
            sign = Math.Sign(right.Denominator);
        }

        BigInteger support = sign * magnitude;
        if (support < long.MinValue || support > long.MaxValue)
        {
            throw new OverflowException("Common support exceeded long-backed proportion range.");
        }

        return (long)support;
    }

    public static (Proportion Left, Proportion Right, long Support) AlignToCommonSupport(
        Proportion left,
        Proportion right)
    {
        long support = GetCommonSupport(left, right);
        return (
            RefineToSupport(left, support),
            RefineToSupport(right, support),
            support);
    }

    public static Proportion CommonFrameAdd(Proportion left, Proportion right)
    {
        var aligned = AlignToCommonSupport(left, right);
        return new Proportion(
            checked(aligned.Left.Numerator + aligned.Right.Numerator),
            aligned.Support);
    }

    public static Proportion RefineToSupport(Proportion value, long targetSupport)
    {
        if (value.Denominator == 0 || targetSupport == 0)
        {
            return value;
        }

        BigInteger sourceNumerator = value.Numerator;
        BigInteger sourceDenominator = value.Denominator;
        BigInteger targetMagnitude = Lcm(BigInteger.Abs(sourceDenominator), BigInteger.Abs(targetSupport));
        BigInteger targetDenominator = Math.Sign(targetSupport) * targetMagnitude;
        BigInteger scale = targetDenominator / sourceDenominator;
        BigInteger targetNumerator = sourceNumerator * scale;

        if (targetNumerator < long.MinValue || targetNumerator > long.MaxValue ||
            targetDenominator < long.MinValue || targetDenominator > long.MaxValue)
        {
            throw new OverflowException("Refined support exceeded long-backed proportion range.");
        }

        return new Proportion((long)targetNumerator, (long)targetDenominator);
    }

    private static BigInteger Lcm(BigInteger left, BigInteger right)
    {
        if (left.IsZero || right.IsZero)
        {
            return BigInteger.Zero;
        }

        return BigInteger.Abs(left / BigInteger.GreatestCommonDivisor(left, right) * right);
    }
}
