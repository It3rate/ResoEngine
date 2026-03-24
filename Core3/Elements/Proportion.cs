namespace Core3.Elements;

/// <summary>
/// A raw extent interpreted through a pin.
/// With no explicit pin supplied, it bootstraps itself by assuming a pin at zero.
/// </summary>
public readonly record struct Proportion(RawExtent Extent, long PinPosition)
{
    public Proportion(RawExtent extent)
        : this(extent, 0)
    {
    }

    public long InboundCarrier => PinPosition - Extent.Start;
    public long OutboundCarrier => Extent.End - PinPosition;
    public bool IsDegenerate => InboundCarrier == 0 && OutboundCarrier == 0;

    public decimal ToDecimal() => ToDecimalRatio(InboundCarrier, OutboundCarrier);

    public override string ToString() => $"{OutboundCarrier}/{InboundCarrier}";

    private static decimal ToDecimalRatio(long inboundCarrier, long outboundCarrier)
    {
        if (inboundCarrier == 0)
        {
            if (outboundCarrier == 0)
            {
                return 0m;
            }

            return outboundCarrier > 0 ? decimal.MaxValue : decimal.MinValue;
        }

        return (decimal)outboundCarrier / inboundCarrier;
    }
}
