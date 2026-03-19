namespace Core2.Elements;

public sealed record CarrierRoute(
    CarrierRouteKind Kind,
    CarrierIncidentKind From,
    CarrierIncidentKind To,
    CarrierIdentity Carrier)
{
    public CarrierId CarrierId => Carrier.Id;
}
