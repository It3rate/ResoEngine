using Core2.Repetition;
using Core2.Support;
using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 3: an expanded 2D structure built from two pinned axes.
/// The structure itself is two-dimensional, but when it folds it becomes a 1D Axis value.
/// </summary>
public sealed record Area(Axis Recessive, Axis Dominant) : IElement
{
    private static readonly AlgebraTable<Axis> Table = new(Axis.Arithmetic);

    public static Area Zero => new(Axis.Zero, Axis.Zero);
    public static Area One => new(Axis.One, Axis.One);
    public int Degree => 3;
    public AxisBasis Basis => Recessive.Basis;
    public AreaQuadrants Quadrants => Expand();
    public Axis Value => Fold();
    internal static IArithmetic<Area> Arithmetic { get; } = new AreaArithmetic();

    private static Area FromPair((Axis Recessive, Axis Dominant) pair) =>
        new(pair.Recessive, pair.Dominant);

    public Area ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return FromPair(result);
    }

    public Area Oppose() => ApplyOpposition();

    public InverseContinuationResult<Axis> InverseContinue(
        int degree,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null) =>
        InverseContinuationEngine.InverseContinue(this, degree, mode, rule, foldedReference);

    public Area Mirror() => FromPair(Table.Mirror(Recessive, Dominant));

    public Area SwapUnitRoles() => Mirror();

    public Area FlipPerspective() => -this;

    public Area ConjugateRecessive() => FromPair(Table.NegateRecessive(Recessive, Dominant));

    public Area ConjugateDominant() => FromPair(Table.NegateDominant(Recessive, Dominant));

    public Area ProjectRecessiveIntoDominant() =>
        FromPair(Table.ProjectRecessiveIntoDominant(Recessive, Dominant));

    public Area ProjectDominantIntoRecessive() =>
        FromPair(Table.ProjectDominantIntoRecessive(Recessive, Dominant));

    public AreaQuadrants Expand() =>
        new(
            Recessive.Recessive * Dominant.Recessive,
            Recessive.Recessive * Dominant.Dominant,
            Recessive.Dominant * Dominant.Recessive,
            Recessive.Dominant * Dominant.Dominant,
            Basis);

    public (Proportion ii, Proportion ir, Proportion ri, Proportion rr) ExpandTerms()
    {
        var quadrants = Expand();
        return (quadrants.Ii, quadrants.Ir, quadrants.Ri, quadrants.Rr);
    }

    public Area Intersect(Area other) =>
        new(Recessive.Intersect(other.Recessive), Dominant.Intersect(other.Dominant));

    public Area Union(Area other) =>
        new(Recessive.Union(other.Recessive), Dominant.Union(other.Dominant));

    public Area BooleanNot() =>
        new(Recessive.BooleanNot(), Dominant.BooleanNot());

    public Area Xor(Area other) =>
        new(Recessive.Xor(other.Recessive), Dominant.Xor(other.Dominant));

    public static Area operator +(Area left, Area right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant);

    public static Area operator -(Area value) =>
        new(-value.Recessive, -value.Dominant);

    public static Area operator *(Area state, Area transform)
    {
        var result = Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return FromPair(result);
    }

    public Axis Fold() => Expand().Fold();

    public override string ToString() => $"<{Recessive}> x <{Dominant}> => {Fold()}";

    private sealed class AreaArithmetic : IArithmetic<Area>
    {
        public Area Zero => Area.Zero;
        public Area Add(Area left, Area right) => left + right;
        public Area Multiply(Area left, Area right) => left * right;
        public Area Negate(Area value) => -value;
    }
}
