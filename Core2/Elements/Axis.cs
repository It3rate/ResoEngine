using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 2: a 1D directed reading with four scalar degrees of freedom.
/// Recessive and dominant are each Proportions, where each Proportion is
/// dominant amount over recessive support. Together that gives four scalar degrees:
/// recessive amount/support and dominant amount/support.
/// </summary>
public sealed record Axis(Proportion Recessive, Proportion Dominant, AxisBasis Basis = AxisBasis.Complex) : IElement
{
    private static readonly AlgebraTable<Proportion> ComplexTable = new(Proportion.Arithmetic);
    private static readonly AlgebraTable<Proportion> SplitComplexTable = new(
        Proportion.Arithmetic,
        [
            new AlgebraEntry(0, 0, 1, +1),
            new AlgebraEntry(0, 1, 0, +1),
            new AlgebraEntry(1, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        ]);

    public static Axis Zero => new(Proportion.Zero, Proportion.Zero);
    public static Axis One => new(Proportion.Zero, Proportion.One);
    public static Axis I => new(Proportion.One, Proportion.Zero);
    public static Axis NegativeOne => new(Proportion.Zero, -Proportion.One);
    public static Axis NegativeI => new(-Proportion.One, Proportion.Zero);
    public int Degree => 2;

    internal static IArithmetic<Axis> Arithmetic { get; } = new AxisArithmetic();

    public Axis(long recessiveValue, long recessiveUnit, long dominantValue, long dominantUnit)
        : this(new Proportion(recessiveValue, recessiveUnit), new Proportion(dominantValue, dominantUnit))
    {
    }

    private static Axis FromPair((Proportion Recessive, Proportion Dominant) pair) =>
        new(pair.Recessive, pair.Dominant);

    private static Axis FromPair((Proportion Recessive, Proportion Dominant) pair, AxisBasis basis) =>
        new(pair.Recessive, pair.Dominant, basis);

    private AlgebraTable<Proportion> Table => Basis == AxisBasis.SplitComplex ? SplitComplexTable : ComplexTable;

    public Scalar Start => -Recessive.Fold();
    public Scalar End => Dominant.Fold();
    public Scalar Span => End - Start;
    public bool IsEmptyInterval => Start >= End;

    public static Axis FromCoordinates(
        Scalar start,
        Scalar end,
        Scalar? recessiveSupport = null,
        Scalar? dominantSupport = null,
        AxisBasis basis = AxisBasis.Complex) =>
        new(
            Proportion.FromRecessiveDominant(recessiveSupport ?? Scalar.One, -start),
            Proportion.FromRecessiveDominant(dominantSupport ?? Scalar.One, end),
            basis);

    public Axis ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return FromPair(result, Basis);
    }

    public Axis Oppose() => ApplyOpposition();

    public Axis Mirror() => FromPair(Table.Mirror(Recessive, Dominant), Basis);

    public Axis SwapUnitRoles() => Mirror();

    public Axis FlipPerspective() => -this;

    public Axis ConjugateRecessive() => FromPair(Table.NegateRecessive(Recessive, Dominant), Basis);

    public Axis ConjugateDominant() => FromPair(Table.NegateDominant(Recessive, Dominant), Basis);

    public Axis ProjectRecessiveIntoDominant() =>
        FromPair(Table.ProjectRecessiveIntoDominant(Recessive, Dominant), Basis);

    public Axis ProjectDominantIntoRecessive() =>
        FromPair(Table.ProjectDominantIntoRecessive(Recessive, Dominant), Basis);

    public Area Pin(Axis other) => new(this, other);

    public Axis Intersect(Axis other)
    {
        var start = SelectByStart(this, other, selectGreater: true);
        var end = SelectByEnd(this, other, selectGreater: false);
        return FromBounds(start, end, Basis);
    }

    public Axis Union(Axis other)
    {
        var start = SelectByStart(this, other, selectGreater: false);
        var end = SelectByEnd(this, other, selectGreater: true);
        return FromBounds(start, end, Basis);
    }

    /// <summary>
    /// The current boolean visualizer's NOT is the interval mirrored across the origin:
    /// start becomes the negated end and end becomes the negated start.
    /// In Axis encoding, that is the recessive/dominant role swap.
    /// </summary>
    public Axis BooleanNot() => Mirror();

    public Axis Xor(Axis other)
    {
        var leftOnly = Intersect(other.BooleanNot());
        var rightOnly = BooleanNot().Intersect(other);

        if (leftOnly.IsEmptyInterval)
        {
            return rightOnly;
        }

        if (rightOnly.IsEmptyInterval)
        {
            return leftOnly;
        }

        return leftOnly.Union(rightOnly);
    }

    public static Axis operator +(Axis left, Axis right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant, left.Basis);

    public static Axis operator -(Axis value) =>
        new(-value.Recessive, -value.Dominant, value.Basis);

    /// <summary>
    /// Right action: the left operand is the current line-state, the right operand is transform data.
    /// </summary>
    public static Axis operator *(Axis state, Axis transform)
    {
        var result = state.Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return FromPair(result, state.Basis);
    }

    public Proportion Fold() => Recessive * Dominant;

    public override string ToString() => $"[{Recessive}]i + [{Dominant}]";

    private static (Proportion Value, Scalar Boundary) SelectByStart(Axis left, Axis right, bool selectGreater)
    {
        var leftBoundary = left.Start;
        var rightBoundary = right.Start;
        bool takeLeft = selectGreater ? leftBoundary >= rightBoundary : leftBoundary <= rightBoundary;
        return takeLeft ? (left.Recessive, leftBoundary) : (right.Recessive, rightBoundary);
    }

    private static (Proportion Value, Scalar Boundary) SelectByEnd(Axis left, Axis right, bool selectGreater)
    {
        var leftBoundary = left.End;
        var rightBoundary = right.End;
        bool takeLeft = selectGreater ? leftBoundary >= rightBoundary : leftBoundary <= rightBoundary;
        return takeLeft ? (left.Dominant, leftBoundary) : (right.Dominant, rightBoundary);
    }

    private static Axis FromBounds(
        (Proportion Value, Scalar Boundary) start,
        (Proportion Value, Scalar Boundary) end,
        AxisBasis basis)
    {
        if (start.Boundary >= end.Boundary)
        {
            return Zero;
        }

        return new(start.Value, end.Value, basis);
    }

    private sealed class AxisArithmetic : IArithmetic<Axis>
    {
        public Axis Zero => Axis.Zero;
        public Axis Add(Axis left, Axis right) => left + right;
        public Axis Multiply(Axis left, Axis right) => left * right;
        public Axis Negate(Axis value) => -value;
    }
}
