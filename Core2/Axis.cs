using ResoEngine.Core2.Support;

namespace ResoEngine.Core2;

/// <summary>
/// Degree 2: a pair of Proportions that obey the same opposition algebra as degree 1.
/// This preserves the Scalar -> Proportion -> Axis ladder while moving to the new semantics.
/// </summary>
public sealed record Axis(Proportion Recessive, Proportion Dominant)
{
    private static readonly AlgebraTable<Proportion> Table = new(Proportion.Arithmetic);

    public static Axis Zero => new(Proportion.Zero, Proportion.Zero);
    public static Axis One => new(Proportion.Zero, Proportion.One);
    public static Axis I => new(Proportion.One, Proportion.Zero);
    public static Axis NegativeOne => new(Proportion.Zero, -Proportion.One);
    public static Axis NegativeI => new(-Proportion.One, Proportion.Zero);

    internal static IArithmetic<Axis> Arithmetic { get; } = new AxisArithmetic();

    public Axis ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return new Axis(result.Recessive, result.Dominant);
    }

    public static Axis operator +(Axis left, Axis right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant);

    public static Axis operator -(Axis value) =>
        new(-value.Recessive, -value.Dominant);

    public static Axis operator *(Axis state, Axis transform)
    {
        var result = Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return new Axis(result.Recessive, result.Dominant);
    }

    public Proportion Fold() => Recessive * Dominant;

    public override string ToString() => $"[{Recessive}]i + [{Dominant}]";

    private sealed class AxisArithmetic : IArithmetic<Axis>
    {
        public Axis Zero => Axis.Zero;
        public Axis Add(Axis left, Axis right) => left + right;
        public Axis Multiply(Axis left, Axis right) => left * right;
        public Axis Negate(Axis value) => -value;
    }
}
