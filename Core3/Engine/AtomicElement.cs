namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying a signed amount and a unit number.
/// Opposite perspective negates the realized value but preserves the unit.
/// </summary>
public sealed record AtomicElement(decimal Value, decimal Unit) : GradedElement
{
    public override int Grade => 0;
    public override bool HasResolvedUnits => Unit != 0m;

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
            sum = new AtomicElement(Value + atomic.Value, Unit);
            return true;
        }

        sum = null;
        return false;
    }

    public override string ToString() => $"{Value}/{Unit}";
}
