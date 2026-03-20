using Core2.Elements;

namespace Core2.Interpretation.Analysis;

/// <summary>
/// A lightweight structural reading of a carrier pin graph.
/// It reports shared-carrier, span, and recursive-cycle facts without choosing a geometric fold.
/// </summary>
public sealed class CarrierPinGraphAnalysis
{
    private readonly IReadOnlyDictionary<CarrierId, CarrierStructuralProfile> _profilesById;
    private readonly IReadOnlyDictionary<CarrierPinSiteId, CarrierSiteStructuralProfile> _siteProfilesById;

    public CarrierPinGraphAnalysis(
        IReadOnlyList<CarrierStructuralProfile> profiles,
        IReadOnlyList<CarrierSiteStructuralProfile> siteProfiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);
        ArgumentNullException.ThrowIfNull(siteProfiles);

        Profiles = profiles.ToArray();
        SiteProfiles = siteProfiles.ToArray();
        _profilesById = Profiles.ToDictionary(profile => profile.Carrier.Id);
        _siteProfilesById = SiteProfiles.ToDictionary(profile => profile.Site.Id);
    }

    public IReadOnlyList<CarrierStructuralProfile> Profiles { get; }
    public IReadOnlyList<CarrierSiteStructuralProfile> SiteProfiles { get; }

    public CarrierStructuralProfile GetProfile(CarrierId carrierId) =>
        _profilesById.TryGetValue(carrierId, out var profile)
            ? profile
            : throw new KeyNotFoundException($"No carrier structural profile exists for id {carrierId}.");

    public bool TryGetProfile(CarrierId carrierId, out CarrierStructuralProfile? profile) =>
        _profilesById.TryGetValue(carrierId, out profile);

    public CarrierSiteStructuralProfile GetSiteProfile(CarrierPinSiteId siteId) =>
        _siteProfilesById.TryGetValue(siteId, out var profile)
            ? profile
            : throw new KeyNotFoundException($"No carrier site structural profile exists for id {siteId}.");

    public bool TryGetSiteProfile(CarrierPinSiteId siteId, out CarrierSiteStructuralProfile? profile) =>
        _siteProfilesById.TryGetValue(siteId, out profile);
}

public sealed record CarrierStructuralProfile(
    CarrierIdentity Carrier,
    IReadOnlyList<CarrierPinSite> HostedSites,
    IReadOnlyList<CarrierAttachmentOccurrence> Attachments,
    IReadOnlyList<CarrierIdentity> ReferencedCarriers,
    IReadOnlyList<CarrierIdentity> ReferencingHostCarriers,
    IReadOnlyList<CarrierSiteStructuralProfile> ParticipatingSites,
    IReadOnlyList<CarrierSiteStructuralProfile> ThroughSites,
    bool ParticipatesInRecursiveCycle)
{
    public int HostedSiteCount => HostedSites.Count;
    public int AttachmentCount => Attachments.Count;
    public int ReferencedCarrierCount => ReferencedCarriers.Count;
    public int ReferencingHostCarrierCount => ReferencingHostCarriers.Count;
    public int ParticipatingSiteCount => ParticipatingSites.Count;
    public int ThroughSiteCount => ThroughSites.Count;
    public bool IsReferenced => AttachmentCount > 0;
    public bool IsHosted => HostedSiteCount > 0;
    public bool IsSharedAcrossSites => Attachments.Select(attachment => attachment.SiteId).Distinct().Skip(1).Any();
    public bool HasAttachmentSpan => DistinctAttachmentPositions.Skip(1).Any();
    public int OpenSiteCount => ParticipatingSites.Count(site => site.Summary == CarrierJunctionSummary.Open);
    public int CuspSiteCount => ParticipatingSites.Count(site => site.Summary == CarrierJunctionSummary.Cusp);
    public int BranchSiteCount => ParticipatingSites.Count(site => site.Summary == CarrierJunctionSummary.Branch);
    public int TeeSiteCount => ParticipatingSites.Count(site => site.Summary == CarrierJunctionSummary.Tee);
    public int CrossSiteCount => ParticipatingSites.Count(site => site.Summary == CarrierJunctionSummary.Cross);
    public IReadOnlyList<Proportion> DistinctAttachmentPositions =>
        Attachments
            .Select(attachment => attachment.CarrierPosition)
            .Distinct()
            .OrderBy(position => position)
            .ToArray();
    public Proportion? FirstAttachmentPosition => DistinctAttachmentPositions.FirstOrDefault();
    public Proportion? LastAttachmentPosition => DistinctAttachmentPositions.LastOrDefault();
}

public sealed record CarrierSiteStructuralProfile(
    CarrierPinSite Site,
    CarrierSiteRouting Routing)
{
    public CarrierPinSiteId SiteId => Site.Id;
    public string? Name => Site.Name;
    public CarrierJunctionSummary Summary => Routing.Summary;
    public bool HostContinues => Routing.HostContinues;
    public bool HasTrueCross => Routing.HasTrueCross;
    public bool HasCrossShapedProposal => Routing.HasCrossShapedProposal;
    public IReadOnlyList<CarrierIdentity> ParticipatingCarriers =>
        Routing.Incidents
            .Where(incident => incident.IsPresent && incident.Carrier is not null)
            .Select(incident => incident.Carrier!)
            .DistinctBy(carrier => carrier.Id)
            .ToArray();
    public IReadOnlyList<CarrierIdentity> ThroughCarriers =>
        Routing.Routes
            .Where(route => route.Kind == CarrierRouteKind.Through)
            .Select(route => route.Carrier)
            .DistinctBy(carrier => carrier.Id)
            .ToArray();

    public bool Participates(CarrierId carrierId) =>
        ParticipatingCarriers.Any(carrier => carrier.Id == carrierId);

    public bool CarriesThrough(CarrierId carrierId) =>
        ThroughCarriers.Any(carrier => carrier.Id == carrierId);
}
