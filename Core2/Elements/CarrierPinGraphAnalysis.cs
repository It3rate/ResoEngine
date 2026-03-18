namespace Core2.Elements;

/// <summary>
/// A lightweight structural reading of a carrier pin graph.
/// It reports shared-carrier, span, and recursive-cycle facts without choosing a geometric fold.
/// </summary>
public sealed class CarrierPinGraphAnalysis
{
    private readonly IReadOnlyDictionary<CarrierId, CarrierStructuralProfile> _profilesById;

    public CarrierPinGraphAnalysis(IReadOnlyList<CarrierStructuralProfile> profiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);

        Profiles = profiles.ToArray();
        _profilesById = Profiles.ToDictionary(profile => profile.Carrier.Id);
    }

    public IReadOnlyList<CarrierStructuralProfile> Profiles { get; }

    public CarrierStructuralProfile GetProfile(CarrierId carrierId) =>
        _profilesById.TryGetValue(carrierId, out var profile)
            ? profile
            : throw new KeyNotFoundException($"No carrier structural profile exists for id {carrierId}.");

    public bool TryGetProfile(CarrierId carrierId, out CarrierStructuralProfile? profile) =>
        _profilesById.TryGetValue(carrierId, out profile);
}

public sealed record CarrierStructuralProfile(
    CarrierIdentity Carrier,
    IReadOnlyList<CarrierPinSite> HostedSites,
    IReadOnlyList<CarrierAttachmentOccurrence> Attachments,
    IReadOnlyList<CarrierIdentity> ReferencedCarriers,
    IReadOnlyList<CarrierIdentity> ReferencingHostCarriers,
    bool ParticipatesInRecursiveCycle)
{
    public int HostedSiteCount => HostedSites.Count;
    public int AttachmentCount => Attachments.Count;
    public int ReferencedCarrierCount => ReferencedCarriers.Count;
    public int ReferencingHostCarrierCount => ReferencingHostCarriers.Count;
    public bool IsReferenced => AttachmentCount > 0;
    public bool IsHosted => HostedSiteCount > 0;
    public bool IsSharedAcrossSites => Attachments.Select(attachment => attachment.SiteId).Distinct().Skip(1).Any();
    public bool HasAttachmentSpan => DistinctAttachmentPositions.Skip(1).Any();
    public IReadOnlyList<Proportion> DistinctAttachmentPositions =>
        Attachments
            .Select(attachment => attachment.CarrierPosition)
            .Distinct()
            .OrderBy(position => position)
            .ToArray();
    public Proportion? FirstAttachmentPosition => DistinctAttachmentPositions.FirstOrDefault();
    public Proportion? LastAttachmentPosition => DistinctAttachmentPositions.LastOrDefault();
}
