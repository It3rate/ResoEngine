using Core2.Elements;

namespace Core2.Interpretation.Analysis;

public sealed record CarrierRoute(
    CarrierRouteKind Kind,
    CarrierIncidentKind From,
    CarrierIncidentKind To,
    CarrierIdentity Carrier)
{
    public CarrierId CarrierId => Carrier.Id;
}
