namespace Core2.Elements;

/// <summary>
/// Declares that one side of an applied descriptor belongs to a specific carrier
/// at a specific carrier-relative position.
/// </summary>
public sealed record CarrierSideAttachment(
    PinSideRole Role,
    CarrierIdentity Carrier,
    Proportion CarrierPosition,
    string? Name = null)
{
    public CarrierId CarrierId => Carrier.Id;
}
