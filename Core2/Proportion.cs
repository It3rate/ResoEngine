using ResoEngine.Core2.Support;

namespace ResoEngine.Core2;

/// <summary>
/// Degree 1: a measured interval reading ai + b.
/// Recessive is the i-slot; Dominant is the identity-slot.
/// </summary>
public sealed record Proportion(Scalar Recessive, Scalar Dominant)
{
    private static readonly AlgebraTable<Scalar> Table = new(Scalar.Arithmetic);

    public static Proportion Zero => new(Scalar.Zero, Scalar.Zero);
    public static Proportion One => new(Scalar.Zero, Scalar.One);
    public static Proportion I => new(Scalar.One, Scalar.Zero);
    public static Proportion NegativeOne => new(Scalar.Zero, -Scalar.One);
    public static Proportion NegativeI => new(-Scalar.One, Scalar.Zero);

    internal static IArithmetic<Proportion> Arithmetic { get; } = new ProportionArithmetic();

    public static Proportion FromInterval(DirectedInterval interval, MeasurementFrame frame, Perspective perspective = Perspective.Dominant) =>
        frame.Read(interval, perspective);

    public Proportion ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return new Proportion(result.Recessive, result.Dominant);
    }

    public Proportion InPerspective(Perspective perspective) =>
        perspective == Perspective.Dominant ? this : -this;

    public static Proportion operator +(Proportion left, Proportion right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant);

    public static Proportion operator -(Proportion value) =>
        new(-value.Recessive, -value.Dominant);

    /// <summary>
    /// Right action: left is the current state, right is transform data.
    /// Using a Proportion in transform position is the encoded form of applying opposition/scaling.
    /// </summary>
    public static Proportion operator *(Proportion state, Proportion transform)
    {
        var result = Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return new Proportion(result.Recessive, result.Dominant);
    }

    public override string ToString() => $"{Recessive}i + {Dominant}";

    private sealed class ProportionArithmetic : IArithmetic<Proportion>
    {
        public Proportion Zero => Proportion.Zero;
        public Proportion Add(Proportion left, Proportion right) => left + right;
        public Proportion Multiply(Proportion left, Proportion right) => left * right;
        public Proportion Negate(Proportion value) => -value;
    }
}
