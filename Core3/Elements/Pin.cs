namespace Core3.Elements;

/// <summary>
/// A valve-like pin located on the ambient line.
/// It does not generate carriers. Instead it reads one attached raw extent into
/// an inbound port value and an outbound port value relative to the pin origin.
/// </summary>
public readonly record struct Pin(Proportion Position, IElement Attachment)
{
    public OutboundCarrier ResolvedPosition => Position.GetPositionOn(Attachment);

    public InboundCarrier InboundCarrier => Attachment.GetInboundCarrier(this);

    public OutboundCarrier OutboundCarrier => Attachment.GetOutboundCarrier(this);

    public InboundCarrierRef InboundCarrierRef => new(this);

    public OutboundCarrierRef OutboundCarrierRef => new(this);

    public Proportion ToProportion() => new(new RawExtent(InboundCarrier.RawValue, OutboundCarrier.RawValue));

    public override string ToString() => $"@{ResolvedPosition} : in {InboundCarrier}, out {OutboundCarrier}";
}
