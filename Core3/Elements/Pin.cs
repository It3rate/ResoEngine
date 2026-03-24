namespace Core3.Elements;

/// <summary>
/// A valve-like pin located on the ambient line.
/// It does not generate carriers. Instead it reads one attached raw extent into
/// an inbound port value and an outbound port value relative to the pin origin.
/// </summary>
public readonly record struct Pin(Proportion Position, IElement Attachment)
{
    public long ResolvedPosition => Attachment.GetPositionAt(Position);

    public long InboundCarrier => Attachment.GetInboundCarrier(this);

    public long OutboundCarrier => Attachment.GetOutboundCarrier(this);

    public Proportion ToProportion() => new(new RawExtent(-InboundCarrier, OutboundCarrier));

    public override string ToString() => $"@{ResolvedPosition} : in {InboundCarrier}, out {OutboundCarrier}";
}
