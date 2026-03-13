using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 3 stub: an Area is the next recursive space, built from two orthogonal Axis instances.
/// This is intentionally lightweight for now so the next step can flesh out area-specific semantics.
/// </summary>
public sealed record Area(Axis Recessive, Axis Dominant) : IElement
{
    private static readonly AlgebraTable<Axis> Table = new(Axis.Arithmetic);

    public static Area Zero => new(Axis.Zero, Axis.Zero);
    public int Degree => 3;

    private static Area FromPair((Axis Recessive, Axis Dominant) pair) =>
        new(pair.Recessive, pair.Dominant);

    public Area ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return FromPair(result);
    }

    public Area Oppose() => ApplyOpposition();

    public Area Mirror() => FromPair(Table.Mirror(Recessive, Dominant));

    public Area SwapUnitRoles() => Mirror();

    public Area FlipPerspective() => -this;

    public Area ConjugateRecessive() => FromPair(Table.NegateRecessive(Recessive, Dominant));

    public Area ConjugateDominant() => FromPair(Table.NegateDominant(Recessive, Dominant));

    public Area ProjectRecessiveIntoDominant() =>
        FromPair(Table.ProjectRecessiveIntoDominant(Recessive, Dominant));

    public Area ProjectDominantIntoRecessive() =>
        FromPair(Table.ProjectDominantIntoRecessive(Recessive, Dominant));

    public (Proportion ii, Proportion ir, Proportion ri, Proportion rr) ExpandTerms() =>
        (
            Recessive.Recessive * Dominant.Recessive,
            Recessive.Recessive * Dominant.Dominant,
            Recessive.Dominant * Dominant.Recessive,
            Recessive.Dominant * Dominant.Dominant);

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

    public Axis Fold() => Recessive * Dominant;

    public override string ToString() => $"<{Recessive}>i + <{Dominant}>";
}
