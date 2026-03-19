namespace Core2.Elements;

public sealed record CarrierIncident(
    CarrierIncidentKind Kind,
    CarrierIdentity? Carrier,
    bool IsPresent,
    bool IsBound,
    PinSideRole? SideRole = null,
    int? CarrierRank = null,
    int TransportDirectionSign = 0)
{
    public CarrierId? CarrierId => Carrier?.Id;
    public bool IsHostIncident =>
        Kind == CarrierIncidentKind.HostNegative ||
        Kind == CarrierIncidentKind.HostPositive;

    public bool IsSideIncident => SideRole.HasValue;
}
