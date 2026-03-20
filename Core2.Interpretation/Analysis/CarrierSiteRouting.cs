using Core2.Elements;
using Core2.Interpretation.Placement;

namespace Core2.Interpretation.Analysis;

/// <summary>
/// A derived structural reading of one carrier pin site.
/// It preserves incident carrier parts and explicit through-routes
/// without requiring a geometric fold.
/// </summary>
public sealed class CarrierSiteRouting
{
    private readonly IReadOnlyDictionary<CarrierIncidentKind, CarrierIncident> _incidentsByKind;

    private CarrierSiteRouting(
        CarrierPinSite site,
        IReadOnlyList<CarrierIncident> incidents,
        IReadOnlyList<CarrierRoute> routes,
        CarrierJunctionSummary summary)
    {
        Site = site;
        Incidents = incidents.ToArray();
        Routes = routes.ToArray();
        Summary = summary;
        _incidentsByKind = Incidents.ToDictionary(incident => incident.Kind);
    }

    public CarrierPinSite Site { get; }
    public IReadOnlyList<CarrierIncident> Incidents { get; }
    public IReadOnlyList<CarrierRoute> Routes { get; }
    public CarrierJunctionSummary Summary { get; }

    public bool HostContinues =>
        HasThroughRoute(CarrierIncidentKind.HostNegative, CarrierIncidentKind.HostPositive);

    public bool HasNonHostThroughCarrier =>
        Routes.Any(route => route.CarrierId != Site.HostCarrier.Id);

    public bool HasTrueCross => Summary == CarrierJunctionSummary.Cross;

    public bool HasCrossShapedProposal =>
        HasTrueCross || HasOrthogonalUnboundSidePair;

    public bool HasOrthogonalUnboundSidePair =>
        GetIncident(CarrierIncidentKind.RecessiveSide).IsPresent &&
        GetIncident(CarrierIncidentKind.DominantSide).IsPresent &&
        !GetIncident(CarrierIncidentKind.RecessiveSide).IsBound &&
        !GetIncident(CarrierIncidentKind.DominantSide).IsBound &&
        GetIncident(CarrierIncidentKind.RecessiveSide).CarrierRank == 1 &&
        GetIncident(CarrierIncidentKind.DominantSide).CarrierRank == 1;

    public CarrierIncident GetIncident(CarrierIncidentKind kind) =>
        _incidentsByKind.TryGetValue(kind, out var incident)
            ? incident
            : throw new KeyNotFoundException($"No incident exists for kind {kind}.");

    public bool HasThroughRoute(CarrierIncidentKind from, CarrierIncidentKind to) =>
        Routes.Any(
            route =>
                route.Kind == CarrierRouteKind.Through &&
                ((route.From == from && route.To == to) ||
                 (route.From == to && route.To == from)));

    public static CarrierSiteRouting FromSite(CarrierPinSite site)
    {
        ArgumentNullException.ThrowIfNull(site);

        var placed = site.PlaceApplied();
        bool hasHostNegative = site.HostPosition > site.Host.LeftCoordinate;
        bool hasHostPositive = site.HostPosition < site.Host.RightCoordinate;

        CarrierIncident recessive = CreateSideIncident(site, placed.RecessiveSide);
        CarrierIncident dominant = CreateSideIncident(site, placed.DominantSide);
        CarrierIncident hostNegative = new(
            CarrierIncidentKind.HostNegative,
            site.HostCarrier,
            hasHostNegative,
            hasHostNegative);
        CarrierIncident hostPositive = new(
            CarrierIncidentKind.HostPositive,
            site.HostCarrier,
            hasHostPositive,
            hasHostPositive);

        List<CarrierIncident> incidents = [hostNegative, hostPositive, recessive, dominant];
        List<CarrierRoute> routes = [];

        bool hostThrough = hostNegative.IsPresent && hostPositive.IsPresent;
        if (hostThrough)
        {
            routes.Add(new CarrierRoute(CarrierRouteKind.Through, CarrierIncidentKind.HostNegative, CarrierIncidentKind.HostPositive, site.HostCarrier));
        }

        bool sharedNonHostThrough =
            recessive.IsPresent &&
            dominant.IsPresent &&
            recessive.IsBound &&
            dominant.IsBound &&
            recessive.CarrierId.HasValue &&
            dominant.CarrierId.HasValue &&
            recessive.CarrierId == dominant.CarrierId &&
            recessive.CarrierId != site.HostCarrier.Id;
        if (sharedNonHostThrough)
        {
            routes.Add(new CarrierRoute(CarrierRouteKind.Through, CarrierIncidentKind.RecessiveSide, CarrierIncidentKind.DominantSide, recessive.Carrier!));
        }

        bool hasNonHostBoundSide =
            (recessive.IsBound && recessive.CarrierId != site.HostCarrier.Id) ||
            (dominant.IsBound && dominant.CarrierId != site.HostCarrier.Id);
        bool hasHostBoundOrthogonalSide =
            HasHostBoundOrthogonalSide(site.HostCarrier.Id, recessive) ||
            HasHostBoundOrthogonalSide(site.HostCarrier.Id, dominant);

        CarrierJunctionSummary summary =
            sharedNonHostThrough && hostThrough ? CarrierJunctionSummary.Cross :
            sharedNonHostThrough ? CarrierJunctionSummary.Tee :
            hostThrough && hasHostBoundOrthogonalSide ? CarrierJunctionSummary.Cusp :
            hasNonHostBoundSide ? CarrierJunctionSummary.Branch :
            CarrierJunctionSummary.Open;

        return new CarrierSiteRouting(site, incidents, routes, summary);
    }

    private static CarrierIncident CreateSideIncident(CarrierPinSite site, PositionedAxisSide side)
    {
        CarrierSideAttachment? attachment = site.GetAttachment(side.Role);
        return new CarrierIncident(
            side.Role == PinSideRole.Recessive ? CarrierIncidentKind.RecessiveSide : CarrierIncidentKind.DominantSide,
            attachment?.Carrier,
            side.HasCarrier,
            attachment is not null,
            side.Role,
            side.CarrierRank,
            side.TransportDirectionSign);
    }

    private static bool HasHostBoundOrthogonalSide(CarrierId hostCarrierId, CarrierIncident incident) =>
        incident.IsPresent &&
        incident.IsBound &&
        incident.CarrierId == hostCarrierId &&
        incident.CarrierRank == 1;
}
