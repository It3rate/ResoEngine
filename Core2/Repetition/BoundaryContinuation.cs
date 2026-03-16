using Core2.Elements;
using System.Numerics;

namespace Core2.Repetition;

public static class BoundaryContinuation
{
    public static BoundaryContinuationResult Continue(Axis frame, Scalar value, BoundaryContinuationLaw law) =>
        Continue(frame, value.AsProportion(), law);

    public static BoundaryContinuationResult Continue(Axis frame, Proportion value, BoundaryContinuationLaw law) =>
        Continue(frame.LeftCoordinate, frame.RightCoordinate, value, law);

    public static BoundaryContinuationResult Continue(
        Proportion min,
        Proportion max,
        Proportion value,
        BoundaryContinuationLaw law)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (min == max)
        {
            return new BoundaryContinuationResult(min, []);
        }

        if (value >= min && value <= max)
        {
            return new BoundaryContinuationResult(value, []);
        }

        return law switch
        {
            BoundaryContinuationLaw.TensionPreserving => new BoundaryContinuationResult(
                value,
                [
                    new RepetitionTension(
                        RepetitionTensionKind.BoundaryExceeded,
                        $"Value {value} exceeded frame [{min}, {max}] and was preserved as tension.")
                ]),
            BoundaryContinuationLaw.PeriodicWrap => new BoundaryContinuationResult(Wrap(min, max, value), []),
            BoundaryContinuationLaw.ReflectiveBounce => new BoundaryContinuationResult(Reflect(min, max, value), []),
            BoundaryContinuationLaw.Clamp => new BoundaryContinuationResult(
                Proportion.Max(min, Proportion.Min(max, value)),
                []),
            _ => throw new ArgumentOutOfRangeException(nameof(law), law, null),
        };
    }

    private static Proportion Wrap(Proportion min, Proportion max, Proportion value)
    {
        Proportion span = max - min;
        Proportion shifted = value - min;
        return min + Mod(shifted, span);
    }

    private static Proportion Reflect(Proportion min, Proportion max, Proportion value)
    {
        Proportion span = max - min;
        Proportion doubled = span + span;
        Proportion shifted = value - min;
        Proportion reflected = Mod(shifted, doubled);

        return reflected <= span
            ? min + reflected
            : max - (reflected - span);
    }

    private static Proportion Mod(Proportion value, Proportion modulus)
    {
        if (modulus.IsZero)
        {
            return value;
        }

        var commonDenominator = Lcm(
            BigInteger.Abs(new BigInteger(value.Denominator)),
            BigInteger.Abs(new BigInteger(modulus.Denominator)));

        BigInteger valueScale = commonDenominator / value.Denominator;
        BigInteger modulusScale = commonDenominator / modulus.Denominator;
        BigInteger valueNumerator = new BigInteger(value.Numerator) * valueScale;
        BigInteger modulusNumerator = new BigInteger(modulus.Numerator) * modulusScale;

        if (modulusNumerator < 0)
        {
            modulusNumerator = BigInteger.Negate(modulusNumerator);
            valueNumerator = BigInteger.Negate(valueNumerator);
        }

        BigInteger remainder = valueNumerator % modulusNumerator;
        if (remainder < 0)
        {
            remainder += modulusNumerator;
        }

        return new Proportion(ToLongExact(remainder), ToLongExact(commonDenominator));
    }

    private static BigInteger Lcm(BigInteger left, BigInteger right)
    {
        if (left.IsZero || right.IsZero)
        {
            return BigInteger.Zero;
        }

        return BigInteger.Abs(left / BigInteger.GreatestCommonDivisor(left, right) * right);
    }

    private static long ToLongExact(BigInteger value)
    {
        if (value < long.MinValue || value > long.MaxValue)
        {
            throw new OverflowException("Value exceeds Int64 range.");
        }

        return (long)value;
    }
}
