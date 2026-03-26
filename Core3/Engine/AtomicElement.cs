namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying an exact signed amount and an exact unit
/// number. Opposite perspective negates the realized value but preserves the
/// unit.
/// </summary>
public sealed record AtomicElement(long Value, long Unit) : GradedElement
{
    public override int Grade => 0;
    public override bool HasResolvedUnits => Unit != 0;

    public override GradedElement InvertPerspective() => new AtomicElement(-Value, Unit);

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

    public FoldedApproximation Fold() => new(Value, Unit);

    public override string ToString() => $"{Value}/{Unit}";
}
