using Core2.Repetition;
using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 1: an undivided proportion.
/// The numerator is the dominant amount and the denominator is the recessive support/resolution.
/// </summary>
public sealed record Proportion : IElement
{
    private static readonly AlgebraTable<Scalar> MultiplicationTable = new(
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

    internal static Proportion FromScalars(Scalar numerator, Scalar denominator) =>
        FromRecessiveDominant(denominator, numerator);

    internal static Proportion FromRecessiveDominant(Scalar recessive, Scalar dominant) =>
        new(dominant, recessive, true);

    public Scalar Fold() => Dominant / Recessive;

    public Proportion Reciprocal() => FromRecessiveDominant(Dominant, Recessive);

    public Proportion Mirror() => Reciprocal();

    public Axis Pin(Proportion other) => new(this, other);

    public InverseContinuationResult<Proportion> InverseContinue(
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null) =>
        InverseContinuationEngine.InverseContinue(this, degree, rule, reference);

    public static Proportion operator +(Proportion left, Proportion right) =>
        FromRecessiveDominant(
            left.Recessive * right.Recessive,
            (left.Dominant * right.Recessive) + (right.Dominant * left.Recessive));

    public static Proportion operator -(Proportion value) =>
        FromRecessiveDominant(value.Recessive, -value.Dominant);

    public static Proportion operator *(Proportion left, Proportion right)
    {
        var result = MultiplicationTable.Multiply(
            (left.Recessive, left.Dominant),
            (right.Recessive, right.Dominant));

        return FromRecessiveDominant(result.Recessive, result.Dominant);
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
