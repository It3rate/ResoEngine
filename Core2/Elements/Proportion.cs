using Core2.Repetition;
using ResoEngine.Core2.Support;
using System.Numerics;

namespace Core2.Elements;

/// <summary>
/// Degree 1: an undivided proportion.
/// The numerator is the dominant amount and the denominator is the recessive support/resolution.
/// </summary>
public sealed record Proportion : IElement, IComparable<Proportion>, IComparable
{
    public static Proportion Zero => new(0);
    public static Proportion One => new(1);

    internal static IArithmetic<Proportion> Arithmetic { get; } = new ProportionArithmetic();

    public Proportion(long numerator, long denominator = 1)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public long Numerator { get; }
    public long Denominator { get; }
    public long Dominant => Numerator;
    public long Recessive => Denominator;
    public int Degree => 1;

    public Scalar Fold()
    {
        if (Denominator == 0)
        {
            if (Numerator == 0)
            {
                return Scalar.Zero;
            }

            return Numerator > 0 ? Scalar.PositiveOverflow : Scalar.NegativeOverflow;
        }

        return new Scalar((decimal)Numerator / Denominator);
    }

    public bool IsZero => Dominant == 0;
    public bool IsPositive => CompareTo(Zero) > 0;
    public bool IsNegative => CompareTo(Zero) < 0;
    public int Sign => CompareTo(Zero) switch
    {
        < 0 => -1,
        > 0 => 1,
        _ => 0,
    };

    public Proportion Reciprocal() => new(Recessive, Dominant);

    public Proportion Mirror() => Reciprocal();

    public Proportion Abs() => Sign < 0 ? -this : this;

    public Axis Pin(Proportion other) => new(this, other);

    public InverseContinuationResult<Proportion> InverseContinue(
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null) =>
        InverseContinuationEngine.InverseContinue(this, degree, rule, reference);

    public PowerResult<Proportion> TryPow(
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null) =>
        PowerEngine.Pow(this, exponent, rule, reference);

    public static Proportion operator +(Proportion left, Proportion right) =>
        new(
            checked((left.Dominant * right.Recessive) + (right.Dominant * left.Recessive)),
            checked(left.Recessive * right.Recessive));

    public static Proportion operator -(Proportion left, Proportion right) => left + (-right);

    public static Proportion operator -(Proportion value) =>
        new(-value.Dominant, value.Recessive);

    public static Proportion operator *(Proportion left, Proportion right) =>
        new(
            checked(left.Dominant * right.Dominant),
            checked(left.Recessive * right.Recessive));

    public static Proportion operator /(Proportion left, Proportion right) =>
        new(
            checked(left.Dominant * right.Recessive),
            checked(left.Recessive * right.Dominant));

    public static bool operator <(Proportion left, Proportion right) => left.CompareTo(right) < 0;
    public static bool operator <=(Proportion left, Proportion right) => left.CompareTo(right) <= 0;
    public static bool operator >(Proportion left, Proportion right) => left.CompareTo(right) > 0;
    public static bool operator >=(Proportion left, Proportion right) => left.CompareTo(right) >= 0;

    public int CompareTo(Proportion? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Recessive == 0 || other.Recessive == 0)
        {
            return Fold().CompareTo(other.Fold());
        }

        BigInteger left = (BigInteger)Dominant * other.Recessive;
        BigInteger right = (BigInteger)other.Dominant * Recessive;
        return left.CompareTo(right);
    }

    public int CompareTo(object? obj) => obj is Proportion other ? CompareTo(other) : 1;

    public static Proportion Min(Proportion left, Proportion right) => left <= right ? left : right;

    public static Proportion Max(Proportion left, Proportion right) => left >= right ? left : right;

    public override string ToString() => $"{Numerator}/{Denominator}";

    private sealed class ProportionArithmetic : IArithmetic<Proportion>
    {
        public Proportion Zero => Proportion.Zero;
        public Proportion Add(Proportion left, Proportion right) => left + right;
        public Proportion Multiply(Proportion left, Proportion right) => left * right;
        public Proportion Negate(Proportion value) => -value;
    }
}
