using System.Numerics;

using Core2.Elements;

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
                [CreateBoundaryTension(value, min, max)]),
            BoundaryContinuationLaw.Clamp => new BoundaryContinuationResult(value < min ? min : max, []),
            BoundaryContinuationLaw.PeriodicWrap => new BoundaryContinuationResult(Wrap(min, max, value), []),
            BoundaryContinuationLaw.ReflectiveBounce => new BoundaryContinuationResult(Reflect(min, max, value), []),
            _ => throw new ArgumentOutOfRangeException(nameof(law), law, null),
        };
    }

    private static RepetitionTension CreateBoundaryTension(Proportion value, Proportion min, Proportion max) =>
        new(
            RepetitionTensionKind.BoundaryExceeded,
            $"Value {value} exceeded frame [{min}, {max}] and was preserved as tension.");

    private static Proportion Wrap(Proportion min, Proportion max, Proportion value)
    {
        Proportion span = max - min;
        Proportion offset = value - min;
        return min + PositiveModulo(offset, span);
    }

    private static Proportion Reflect(Proportion min, Proportion max, Proportion value)
    {
        Proportion span = max - min;
        Proportion doubledSpan = span + span;
        Proportion offset = value - min;
        Proportion phase = PositiveModulo(offset, doubledSpan);
        return phase <= span
            ? min + phase
            : max - (phase - span);
    }

    private static Proportion PositiveModulo(Proportion value, Proportion modulus)
    {
        if (modulus <= Proportion.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(modulus), "Modulo requires a positive modulus.");
        }

        BigInteger leftDenominator = BigInteger.Abs(value.Recessive);
        BigInteger rightDenominator = BigInteger.Abs(modulus.Recessive);
        BigInteger commonDenominator = LeastCommonMultiple(leftDenominator, rightDenominator);

        BigInteger scaledValue = (BigInteger)value.Dominant * (commonDenominator / value.Recessive);
        BigInteger scaledModulus = (BigInteger)modulus.Dominant * (commonDenominator / modulus.Recessive);

        BigInteger remainder = scaledValue % scaledModulus;
        if (remainder < 0)
        {
            remainder += scaledModulus;
        }

        return FromBigIntegers(remainder, commonDenominator);
    }

    private static BigInteger LeastCommonMultiple(BigInteger left, BigInteger right) =>
        left.IsZero || right.IsZero
            ? BigInteger.Zero
            : BigInteger.Abs(left / BigInteger.GreatestCommonDivisor(left, right) * right);

    private static Proportion FromBigIntegers(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
        {
            throw new DivideByZeroException("Cannot create a proportion with zero denominator.");
        }

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        BigInteger gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
        if (gcd > BigInteger.One)
        {
            numerator /= gcd;
            denominator /= gcd;
        }

        return new Proportion(
            checked((long)numerator),
            checked((long)denominator));
    }
}
