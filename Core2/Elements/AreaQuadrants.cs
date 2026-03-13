using Core2.Support;

namespace Core2.Elements;

/// <summary>
/// The expanded 2D product of two pinned axes before it is folded back into a 1D Axis value.
/// The four quadrants are the natural directed-segment terms:
///   ii = recessive x recessive
///   ir = recessive x dominant
///   ri = dominant x recessive
///   rr = dominant x dominant
/// </summary>
public readonly record struct AreaQuadrants(
    Proportion Ii,
    Proportion Ir,
    Proportion Ri,
    Proportion Rr,
    AxisBasis Basis = AxisBasis.Complex)
{
    public Axis Fold() => new(Ir + Ri, Rr + (-Ii), Basis);

    public override string ToString() => $"ii:{Ii}, ir:{Ir}, ri:{Ri}, rr:{Rr}";
}
