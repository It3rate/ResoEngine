namespace Core3.Elements;

/// <summary>
/// A valve-like pin located on the ambient line.
/// It does not generate carriers. Instead it joins two carriers at one located
/// site and reads their inbound and outbound spans relative to that position.
/// </summary>
public readonly record struct Pin(Proportion Position, ICarrier Start, ICarrier End)
{
    public Pin(Proportion position, IElement attachment)
        : this(position, attachment.Start, attachment.End)
    {
    }

    public ICarrier ResolvedPosition => Start.PositionAt(End, Position);

    public ICarrier InboundCarrier => Start.Subtract(ResolvedPosition).AsInbound();

    public ICarrier OutboundCarrier => End.Subtract(ResolvedPosition).AsOutbound();

    public Proportion ToProportion() => new(new RawExtent(InboundCarrier.RawValue, OutboundCarrier.RawValue));

    public override string ToString() => $"@{ResolvedPosition} : in {InboundCarrier}, out {OutboundCarrier}";
}
