namespace Core3.Elements;

/// <summary>
/// A valve-like pin located on the ambient line.
/// It does not generate carriers. Instead it reads one attached raw extent into
/// an inbound port value and an outbound port value relative to the pin origin.
/// </summary>
public readonly record struct Pin(IScalar Position, IElement Attachment) : IScalar
{
    public long ResolvedPosition => Attachment.GetPositionAt(Position);

    public long InboundCarrier => Attachment.GetInboundCarrier(this);

    public long OutboundCarrier => Attachment.GetOutboundCarrier(this);

    public decimal ToScalar() => ToScalarRatio(InboundCarrier, OutboundCarrier);

    public override string ToString() => $"@{ResolvedPosition} : in {InboundCarrier}, out {OutboundCarrier}";

    private static decimal ToScalarRatio(long inboundCarrier, long outboundCarrier)
    {
        decimal inbound = inboundCarrier;
        decimal outbound = outboundCarrier;

        if (inbound == 0m)
        {
            if (outbound == 0m)
            {
                return 0m;
            }

            return outbound > 0m ? decimal.MaxValue : decimal.MinValue;
        }

        return outbound / inbound;
    }
}
