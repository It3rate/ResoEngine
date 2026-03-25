namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying a signed amount and a signed unit signature.
/// Mirroring flips both the amount and the unit orientation.
/// </summary>
public sealed record AtomicElement(decimal Value, EngineUnit Unit) : GradedElement
{
    public override int Grade => 0;
    public override EngineSignature Signature => new AtomicSignature(Unit);

    public override GradedElement Mirror() => new AtomicElement(-Value, Unit.Mirror());

    public override string ToString() => $"{Value}/{Unit}";
}
