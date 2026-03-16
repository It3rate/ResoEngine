using Core2.Repetition;
using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 1: an undivided proportion.
/// The numerator is the dominant amount and the denominator is the recessive support/resolution.
/// </summary>
public sealed record Proportion : IElement, IComparable<Proportion>, IComparable
{
    private static readonly AlgebraTable<Scalar> MultiplicationTable = new(
        Scalar.Arithmetic,
        [
            new AlgebraEntry(0, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        ]);

    public static Proportion Zero => new(0);
    public static Proportion One => new(1);

    internal static IArithmetic<Proportion> Arithmetic { get; } = new ProportionArithmetic();

    internal Proportion(Scalar numerator, Scalar denominator, bool _)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public Proportion(long numerator, long denominator = 1)
        : this(new Scalar(numerator), new Scalar(denominator), true)
    {
    }

    public Scalar Numerator { get; }
    public Scalar Denominator { get; }
    public Scalar Dominant => Numerator;
    public Scalar Recessive => Denominator;
    public int Degree => 1;

    public Scalar Fold() => Dominant / Recessive;

    public bool IsZero => Dominant.IsZero;
    public bool IsPositive => CompareTo(Zero) > 0;
    public bool IsNegative => CompareTo(Zero) < 0;
    public int Sign => CompareTo(Zero) switch
    {
        < 0 => -1,
        > 0 => 1,
        _ => 0,
    };

    public Proportion Reciprocal() => new(Recessive, Dominant, true);

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
            (left.Dominant * right.Recessive) + (right.Dominant * left.Recessive),
            left.Recessive * right.Recessive,
            true);

    public static Proportion operator -(Proportion left, Proportion right) => left + (-right);

    public static Proportion operator -(Proportion value) =>
        new(-value.Dominant, value.Recessive, true);

    public static Proportion operator *(Proportion left, Proportion right)
    {
        var result = MultiplicationTable.Multiply(
            (left.Recessive, left.Dominant),
            (right.Recessive, right.Dominant));

        return new(result.Dominant, result.Recessive, true);
    }

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

        return Fold().CompareTo(other.Fold());
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
