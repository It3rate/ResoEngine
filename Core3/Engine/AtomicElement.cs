namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying an exact signed amount and an exact signed
/// unit number. The unit sign carries carrier relation (positive aligned,
/// negative orthogonal, zero unresolved), while the unit magnitude carries
/// exact resolution support. Opposite perspective negates the realized value
/// but preserves the unit.
/// </summary>
public sealed record AtomicElement(long Value, long Unit) : GradedElement
{
    public override int Grade => 0;
    public override bool HasResolvedUnits => Unit != 0;
    public bool IsAlignedUnit => Unit > 0;
    public bool IsOrthogonalUnit => Unit < 0;
    public bool IsUnresolvedUnit => Unit == 0;
    public bool IsClampLike => Unit == 0 && Value == 0;
    public long Resolution => Math.Abs(Unit);

    public override GradedElement Negate() => new AtomicElement(-Value, Unit);

    public override GradedElement SwapOrder() => this;

    public override GradedElement FlipPerspective() => Negate();

    public override bool TryFoldRatio(out FoldedRatio? ratio)
    {
        if (HasResolvedUnits)
        {
            ratio = new FoldedRatio(Value, Unit);
            return true;
        }

        ratio = null;
        return false;
    }

    public override bool SharesUnitSpace(GradedElement other) =>
        other is AtomicElement atomic &&
        HasResolvedUnits &&
        atomic.HasResolvedUnits &&
        Unit == atomic.Unit;

    public override bool TryAdd(GradedElement other, out GradedElement? sum)
    {
        if (other is AtomicElement atomic && SharesUnitSpace(atomic))
        {
            sum = new AtomicElement(checked(Value + atomic.Value), Unit);
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        if (other is AtomicElement atomic && SharesUnitSpace(atomic))
        {
            difference = new AtomicElement(checked(Value - atomic.Value), Unit);
            return true;
        }

        difference = null;
        return false;
    }

    public override bool TryScale(FoldedRatio ratio, out GradedElement? scaled)
    {
        ArgumentNullException.ThrowIfNull(ratio);

        if (!HasResolvedUnits)
        {
            scaled = null;
            return false;
        }

        scaled = new AtomicElement(
            checked(Value * ratio.Numerator),
            checked(Unit * ratio.Denominator));
        return true;
    }

    public override string ToString() => $"{Value}/{Unit}";
}
