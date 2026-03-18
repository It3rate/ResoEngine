namespace Core2.Elements;

/// <summary>
/// A host-relative point pinning event whose local sides may be bound onto shared carrier identities.
/// This is structural: it preserves "what is the same carrier?" without choosing one geometric fold.
/// </summary>
public sealed class CarrierPinSite
{
    private readonly IReadOnlyDictionary<PinSideRole, CarrierSideAttachment> _attachmentsByRole;

    public CarrierPinSite(
        CarrierPinSiteId id,
        CarrierIdentity hostCarrier,
        PointPinning<Axis, Axis> pinning,
        IReadOnlyList<CarrierSideAttachment> sideAttachments,
        string? name = null)
    {
        ArgumentNullException.ThrowIfNull(hostCarrier);
        ArgumentNullException.ThrowIfNull(pinning);
        ArgumentNullException.ThrowIfNull(sideAttachments);

        Id = id;
        HostCarrier = hostCarrier;
        Pinning = pinning;
        Name = name;
        SideAttachments = sideAttachments.ToArray();

        if (SideAttachments.GroupBy(attachment => attachment.Role).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Each pin-side role may be attached at most once.", nameof(sideAttachments));
        }

        _attachmentsByRole = SideAttachments.ToDictionary(attachment => attachment.Role);
    }

    public CarrierPinSiteId Id { get; }
    public CarrierIdentity HostCarrier { get; }
    public PointPinning<Axis, Axis> Pinning { get; }
    public string? Name { get; }
    public IReadOnlyList<CarrierSideAttachment> SideAttachments { get; }
    public Axis Host => Pinning.Host;
    public Axis Applied => Pinning.Applied;
    public Proportion HostPosition => Pinning.Position;

    public CarrierSideAttachment? RecessiveAttachment => GetAttachment(PinSideRole.Recessive);
    public CarrierSideAttachment? DominantAttachment => GetAttachment(PinSideRole.Dominant);

    public PositionedAxis PlaceApplied() => Applied.PlaceAt(HostPosition);

    public CarrierSideAttachment? GetAttachment(PinSideRole role) =>
        _attachmentsByRole.TryGetValue(role, out var attachment) ? attachment : null;

    public bool HasAttachment(PinSideRole role) => _attachmentsByRole.ContainsKey(role);

    public bool ReferencesCarrier(CarrierId carrierId) =>
        SideAttachments.Any(attachment => attachment.CarrierId == carrierId);

    public bool IsHostedOn(CarrierId carrierId) => HostCarrier.Id == carrierId;

    public static CarrierPinSite FromPointPinning(
        CarrierIdentity hostCarrier,
        PointPinning<Axis, Axis> pinning,
        CarrierSideAttachment? recessiveAttachment = null,
        CarrierSideAttachment? dominantAttachment = null,
        CarrierPinSiteId? id = null,
        string? name = null)
    {
        List<CarrierSideAttachment> attachments = [];

        if (recessiveAttachment is not null)
        {
            if (recessiveAttachment.Role != PinSideRole.Recessive)
            {
                throw new ArgumentException("Recessive attachment must use the recessive role.", nameof(recessiveAttachment));
            }

            attachments.Add(recessiveAttachment);
        }

        if (dominantAttachment is not null)
        {
            if (dominantAttachment.Role != PinSideRole.Dominant)
            {
                throw new ArgumentException("Dominant attachment must use the dominant role.", nameof(dominantAttachment));
            }

            attachments.Add(dominantAttachment);
        }

        return new CarrierPinSite(id ?? CarrierPinSiteId.New(), hostCarrier, pinning, attachments, name);
    }
}
