using ResoEngine.Core2.Support;

namespace ResoEngine.Core2;

/// <summary>
/// Degree 1: an undivided proportion.
/// Numerator carries the value, denominator carries the resolution/unit.
/// </summary>
public sealed record Proportion : IElement
{
    private static readonly AlgebraTable<Scalar> Table = new(
        Scalar.Arithmetic,
        [
            new AlgebraEntry(0, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        ]);

    public static Proportion Zero => FromScalars(Scalar.Zero, Scalar.One);
    public static Proportion One => FromScalars(Scalar.One, Scalar.One);

    internal static IArithmetic<Proportion> Arithmetic { get; } = new ProportionArithmetic();

    private Proportion(Scalar numerator, Scalar denominator, bool _)
    {
        if (denominator.IsZero)
            throw new DivideByZeroException("Proportion denominator cannot be zero.");

        Numerator = numerator;
        Denominator = denominator;
    }

    public Proportion(long numerator, long denominator = 1)
        : this(new Scalar(numerator), new Scalar(denominator), true)
    {
    }

    public Scalar Numerator { get; }
    public Scalar Denominator { get; }
    public int Degree => 1;

    internal static Proportion FromScalars(Scalar numerator, Scalar denominator) =>
        new(numerator, denominator, true);

    public Scalar Fold() => Numerator / Denominator;

    public static Proportion operator +(Proportion left, Proportion right) =>
        FromScalars(
            (left.Numerator * right.Denominator) + (right.Numerator * left.Denominator),
            left.Denominator * right.Denominator);

    public static Proportion operator -(Proportion value) =>
        FromScalars(-value.Numerator, value.Denominator);

    public static Proportion operator *(Proportion left, Proportion right)
    {
        var result = Table.Multiply(
            (left.Numerator, left.Denominator),
            (right.Numerator, right.Denominator));

        return FromScalars(result.Recessive, result.Dominant);
    }

    public override string ToString() => $"{Numerator}/{Denominator}";

    private sealed class ProportionArithmetic : IArithmetic<Proportion>
    {
        public Proportion Zero => Proportion.Zero;
        public Proportion Add(Proportion left, Proportion right) => left + right;
        public Proportion Multiply(Proportion left, Proportion right) => left * right;
        public Proportion Negate(Proportion value) => -value;
    }
}
