namespace Core2.Elements;

/// <summary>
/// One occurrence of a carrier side attachment inside a carrier pin graph.
/// This keeps the local pin site together with the carrier-relative position that side claims.
/// </summary>
public sealed record CarrierAttachmentOccurrence(
    CarrierIdentity Carrier,
    CarrierPinSite Site,
    CarrierSideAttachment Attachment)
{
    public CarrierId CarrierId => Carrier.Id;
    public CarrierPinSiteId SiteId => Site.Id;
    public PinSideRole Role => Attachment.Role;
    public Proportion CarrierPosition => Attachment.CarrierPosition;
}
