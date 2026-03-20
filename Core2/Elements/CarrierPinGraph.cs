namespace Core2.Elements;

/// <summary>
/// A structural graph of carrier identities and host-relative pin sites.
/// It preserves shared-carrier and recursive pinning relations before any later geometric fold.
/// </summary>
public sealed class CarrierPinGraph
{
    private readonly IReadOnlyDictionary<CarrierId, CarrierIdentity> _carriersById;
    private readonly IReadOnlyDictionary<CarrierPinSiteId, CarrierPinSite> _sitesById;
    private readonly IReadOnlyDictionary<CarrierId, IReadOnlyList<CarrierPinSite>> _hostedSitesByCarrier;
    private readonly IReadOnlyDictionary<CarrierId, IReadOnlyList<CarrierPinSite>> _referencingSitesByCarrier;

    public CarrierPinGraph(
        IReadOnlyList<CarrierIdentity> carriers,
        IReadOnlyList<CarrierPinSite> sites)
    {
        ArgumentNullException.ThrowIfNull(carriers);
        ArgumentNullException.ThrowIfNull(sites);

        var carrierMap = carriers.ToDictionary(carrier => carrier.Id);
        foreach (var site in sites)
        {
            carrierMap[site.HostCarrier.Id] = site.HostCarrier;
            foreach (var attachment in site.SideAttachments)
            {
                carrierMap[attachment.Carrier.Id] = attachment.Carrier;
            }
        }

        Carriers = carrierMap.Values.ToArray();
        Sites = sites.ToArray();
        _carriersById = Carriers.ToDictionary(carrier => carrier.Id);
        _sitesById = Sites.ToDictionary(site => site.Id);
        _hostedSitesByCarrier = Sites
            .GroupBy(site => site.HostCarrier.Id)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<CarrierPinSite>)group.ToArray());
        _referencingSitesByCarrier = Sites
            .SelectMany(
                site => site.SideAttachments.Select(attachment => (attachment.Carrier.Id, Site: site)))
            .GroupBy(pair => pair.Id, pair => pair.Site)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<CarrierPinSite>)group.Distinct().ToArray());
    }

    public IReadOnlyList<CarrierIdentity> Carriers { get; }
    public IReadOnlyList<CarrierPinSite> Sites { get; }

    public CarrierIdentity GetCarrier(CarrierId id) =>
        _carriersById.TryGetValue(id, out var carrier)
            ? carrier
            : throw new KeyNotFoundException($"No carrier exists for id {id}.");

    public bool TryGetCarrier(CarrierId id, out CarrierIdentity? carrier) =>
        _carriersById.TryGetValue(id, out carrier);

    public CarrierPinSite GetSite(CarrierPinSiteId id) =>
        _sitesById.TryGetValue(id, out var site)
            ? site
            : throw new KeyNotFoundException($"No carrier pin site exists for id {id}.");

    public bool TryGetSite(CarrierPinSiteId id, out CarrierPinSite? site) =>
        _sitesById.TryGetValue(id, out site);

    public IReadOnlyList<CarrierPinSite> GetHostedSites(CarrierId hostCarrierId) =>
        _hostedSitesByCarrier.TryGetValue(hostCarrierId, out var sites) ? sites : [];

    public IReadOnlyList<CarrierPinSite> GetReferencingSites(CarrierId carrierId) =>
        _referencingSitesByCarrier.TryGetValue(carrierId, out var sites) ? sites : [];

    public IReadOnlyList<CarrierIdentity> GetReferencedCarriers(CarrierId hostCarrierId, bool includeSelf = true)
    {
        var ids = GetHostedSites(hostCarrierId)
            .SelectMany(site => site.SideAttachments)
            .Select(attachment => attachment.CarrierId)
            .Where(id => includeSelf || id != hostCarrierId)
            .Distinct()
            .ToArray();

        return ids.Select(GetCarrier).ToArray();
    }

    public bool HasRecursiveCarrierCycle(bool includeSelf = false)
    {
        HashSet<CarrierId> visiting = [];
        HashSet<CarrierId> visited = [];

        foreach (var carrier in Carriers)
        {
            if (HasCycleFrom(carrier.Id, includeSelf, visiting, visited))
            {
                return true;
            }
        }

        return false;
    }

    public bool ParticipatesInRecursiveCycle(CarrierId carrierId, bool includeSelf = false)
    {
        HashSet<CarrierId> visited = [];
        return HasCycleBackToStart(carrierId, carrierId, includeSelf, isInitial: true, visited);
    }

    private bool HasCycleFrom(
        CarrierId current,
        bool includeSelf,
        HashSet<CarrierId> visiting,
        HashSet<CarrierId> visited)
    {
        if (visited.Contains(current))
        {
            return false;
        }

        if (!visiting.Add(current))
        {
            return true;
        }

        foreach (var dependency in GetReferencedCarriers(current, includeSelf))
        {
            if (!includeSelf && dependency.Id == current)
            {
                continue;
            }

            if (HasCycleFrom(dependency.Id, includeSelf, visiting, visited))
            {
                return true;
            }
        }

        visiting.Remove(current);
        visited.Add(current);
        return false;
    }

    private bool HasCycleBackToStart(
        CarrierId start,
        CarrierId current,
        bool includeSelf,
        bool isInitial,
        HashSet<CarrierId> visited)
    {
        if (!isInitial && current == start)
        {
            return true;
        }

        if (!visited.Add(current))
        {
            return false;
        }

        foreach (var dependency in GetReferencedCarriers(current, includeSelf))
        {
            if (!includeSelf && dependency.Id == current)
            {
                continue;
            }

            if (HasCycleBackToStart(start, dependency.Id, includeSelf, isInitial: false, visited))
            {
                return true;
            }
        }

        visited.Remove(current);
        return false;
    }
}
