using ResoEngine.Core2.Support;

namespace ResoEngine.Core2;

/// <summary>
/// Degree 3 stub: an Area is the next recursive space, built from two orthogonal Axis instances.
/// This is intentionally lightweight for now so the next step can flesh out area-specific semantics.
/// </summary>
public sealed record Area(Axis Recessive, Axis Dominant)
{
    private static readonly AlgebraTable<Axis> Table = new(Axis.Arithmetic);

    public static Area Zero => new(Axis.Zero, Axis.Zero);

    public Area ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return new Area(result.Recessive, result.Dominant);
    }

    public static Area operator +(Area left, Area right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant);

    public static Area operator -(Area value) =>
        new(-value.Recessive, -value.Dominant);

    public static Area operator *(Area state, Area transform)
    {
        var result = Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return new Area(result.Recessive, result.Dominant);
    }

    public Axis Fold() => Recessive * Dominant;

    public override string ToString() => $"<{Recessive}>i + <{Dominant}>";
}
