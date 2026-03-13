using Core2.Elements;

namespace Core2.Support;

internal readonly record struct RationalExponent
{
    public RationalExponent(int numerator, int denominator = 1)
    {
        if (denominator == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(denominator), "Rational exponents require a nonzero denominator.");
        }

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        int divisor = GreatestCommonDivisor(Math.Abs(numerator), denominator);
        Numerator = numerator / divisor;
        Denominator = denominator / divisor;
    }

    public int Numerator { get; }
    public int Denominator { get; }
    public bool IsZero => Numerator == 0;
    public bool IsInteger => Denominator == 1;

    public Proportion ToProportion() => new(Numerator, Denominator);

    public RationalExponent Add(RationalExponent other) =>
        new(
            checked((Numerator * other.Denominator) + (other.Numerator * Denominator)),
            checked(Denominator * other.Denominator));

    public RationalExponent Multiply(RationalExponent other) =>
        new(
            checked(Numerator * other.Numerator),
            checked(Denominator * other.Denominator));

    public RationalExponent Negate() => new(-Numerator, Denominator);

    public override string ToString() =>
        IsInteger ? Numerator.ToString() : $"{Numerator}/{Denominator}";

    public static bool TryFrom(Proportion exponent, out RationalExponent result)
    {
        if (!TryToInt(exponent.Dominant, out int numerator) ||
            !TryToInt(exponent.Recessive, out int denominator) ||
            denominator == 0)
        {
            result = default;
            return false;
        }

        result = new RationalExponent(numerator, denominator);
        return true;
    }

    private static bool TryToInt(Scalar value, out int result)
    {
        decimal raw = value.Value;
        if (decimal.Truncate(raw) != raw || raw < int.MinValue || raw > int.MaxValue)
        {
            result = default;
            return false;
        }

        result = (int)raw;
        return true;
    }

    private static int GreatestCommonDivisor(int left, int right)
    {
        if (left == 0)
        {
            return right == 0 ? 1 : right;
        }

        while (right != 0)
        {
            int remainder = left % right;
            left = right;
            right = remainder;
        }

        return left;
    }
}
