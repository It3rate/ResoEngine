using Core2.Algebra;
using System.Numerics;

namespace Core2.Elements;

/// <summary>
/// Degree 0: a plain scalar with implicit unit 1.000...
/// This is the non-resolution value layer that sits below Proportion.
/// </summary>
public readonly record struct Scalar(decimal Value) : IElement, IComparable<Scalar>, IComparable
{
    public static Scalar Zero => new(0m);
    public static Scalar One => new(1m);
    public static Scalar PositiveOverflow => new(decimal.MaxValue);
    public static Scalar NegativeOverflow => new(decimal.MinValue);

    internal static IArithmetic<Scalar> Arithmetic { get; } = new ScalarArithmetic();

    public bool IsZero => Value == 0m;
    public bool IsPositive => Value > 0m;
    public bool IsNegative => Value < 0m;
    public int Sign => Math.Sign(Value);
    public int Degree => 0;

    public static implicit operator Scalar(int value) => new(value);
    public static implicit operator Scalar(long value) => new(value);
    public static implicit operator Scalar(decimal value) => new(value);
    public static explicit operator Scalar(double value) => new((decimal)value);

    public static implicit operator decimal(Scalar value) => value.Value;
    public static explicit operator double(Scalar value) => (double)value.Value;

    public static Scalar operator +(Scalar left, Scalar right) => new(left.Value + right.Value);
    public static Scalar operator -(Scalar value) => new(-value.Value);
    public static Scalar operator -(Scalar left, Scalar right) => new(left.Value - right.Value);
    public static Scalar operator *(Scalar left, Scalar right) => new(left.Value * right.Value);
    public static bool operator <(Scalar left, Scalar right) => left.Value < right.Value;
    public static bool operator <=(Scalar left, Scalar right) => left.Value <= right.Value;
    public static bool operator >(Scalar left, Scalar right) => left.Value > right.Value;
    public static bool operator >=(Scalar left, Scalar right) => left.Value >= right.Value;

    public static Scalar operator /(Scalar left, Scalar right)
    {
        if (right.IsZero)
        {
            if (left.IsZero)
            {
                return Zero;
            }

            return left.Value > 0m ? PositiveOverflow : NegativeOverflow;
        }

        return new(left.Value / right.Value);
    }

    public long ToLongExact()
    {
        if (decimal.Truncate(Value) != Value || Value < long.MinValue || Value > long.MaxValue)
        {
            throw new InvalidOperationException($"Scalar value {Value:0.###} cannot be promoted exactly into an integer-backed proportion.");
        }

        return (long)Value;
    }

    public Proportion AsProportion(long support = 1)
    {
        if (IsZero)
        {
            return new Proportion(0, support == 0 ? 0 : support);
        }

        BigInteger unscaled = GetUnscaledValue(Value);
        BigInteger exactDenominator = BigInteger.Pow(10, GetScale(Value));
        BigInteger requestedSupport = new(support);
        BigInteger sign = requestedSupport.Sign < 0 ? BigInteger.MinusOne : BigInteger.One;
        BigInteger targetDenominatorMagnitude = Lcm(
            exactDenominator,
            BigInteger.Abs(requestedSupport.IsZero ? BigInteger.One : requestedSupport));
        BigInteger denominator = sign * targetDenominatorMagnitude;
        BigInteger numerator = unscaled * sign * (targetDenominatorMagnitude / exactDenominator);

        if (numerator < long.MinValue || numerator > long.MaxValue ||
            denominator < long.MinValue || denominator > long.MaxValue)
        {
            throw new OverflowException($"Scalar value {Value:0.###} could not be promoted into a long-backed proportion.");
        }

        return new Proportion((long)numerator, (long)denominator);
    }

    public Proportion Pin(Scalar support) => new(ToLongExact(), support.ToLongExact());

    public Proportion Pin(long support) => new(ToLongExact(), support);

    public Scalar Abs() => new(decimal.Abs(Value));

    public Scalar Clamp(Scalar min, Scalar max) => new(decimal.Clamp(Value, min.Value, max.Value));

    public int CompareTo(Scalar other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj) => obj is Scalar other ? CompareTo(other) : 1;

    public static Scalar Min(Scalar left, Scalar right) => left <= right ? left : right;

    public static Scalar Max(Scalar left, Scalar right) => left >= right ? left : right;

    public override string ToString() => Value switch
    {
        decimal.MaxValue => "Infinity",
        decimal.MinValue => "-Infinity",
        _ => Value.ToString("0.###"),
    };

    private sealed class ScalarArithmetic : IArithmetic<Scalar>
    {
        public Scalar Zero => Scalar.Zero;
        public Scalar Add(Scalar left, Scalar right) => left + right;
        public Scalar Multiply(Scalar left, Scalar right) => left * right;
        public Scalar Negate(Scalar value) => -value;
    }

    private static int GetScale(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0x7F;

    private static BigInteger Lcm(BigInteger left, BigInteger right)
    {
        if (left.IsZero || right.IsZero)
        {
            return BigInteger.Zero;
        }

        return BigInteger.Abs(left / BigInteger.GreatestCommonDivisor(left, right) * right);
    }

    private static BigInteger GetUnscaledValue(decimal value)
    {
        int[] bits = decimal.GetBits(value);
        BigInteger unscaled =
            ((BigInteger)(uint)bits[2] << 64) |
            ((BigInteger)(uint)bits[1] << 32) |
            (uint)bits[0];

        return (bits[3] & unchecked((int)0x80000000)) != 0 ? -unscaled : unscaled;
    }
}
