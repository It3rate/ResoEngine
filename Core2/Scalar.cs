using System.Numerics;
using ResoEngine.Core2.Support;

namespace ResoEngine.Core2;

/// <summary>
/// Degree 0: a rational scalar. This is the atomic measurable element used by the higher degrees.
/// </summary>
public readonly record struct Scalar
{
    public static Scalar Zero => new(0, 1);
    public static Scalar One => new(1, 1);

    internal static IArithmetic<Scalar> Arithmetic { get; } = new ScalarArithmetic();

    public Scalar(long numerator, long denominator = 1)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Scalar denominator cannot be zero.");

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        long gcd = (long)BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), BigInteger.Abs(denominator));
        Numerator = gcd == 0 ? 0 : numerator / gcd;
        Denominator = gcd == 0 ? 1 : denominator / gcd;
    }

    public long Numerator { get; }
    public long Denominator { get; }
    public double Value => Numerator / (double)Denominator;
    public bool IsZero => Numerator == 0;

    public static implicit operator Scalar(long value) => new(value);
    public static implicit operator double(Scalar value) => value.Value;

    public static Scalar operator +(Scalar left, Scalar right) =>
        new(
            (left.Numerator * right.Denominator) + (right.Numerator * left.Denominator),
            left.Denominator * right.Denominator);

    public static Scalar operator -(Scalar value) => new(-value.Numerator, value.Denominator);

    public static Scalar operator *(Scalar left, Scalar right) =>
        new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";

    private sealed class ScalarArithmetic : IArithmetic<Scalar>
    {
        public Scalar Zero => Scalar.Zero;
        public Scalar Add(Scalar left, Scalar right) => left + right;
        public Scalar Multiply(Scalar left, Scalar right) => left * right;
        public Scalar Negate(Scalar value) => -value;
    }
}
