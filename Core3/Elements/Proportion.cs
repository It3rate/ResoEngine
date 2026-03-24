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

    public InboundCarrier InboundCarrier => new(Extent.Start - PinPosition);
    public OutboundCarrier OutboundCarrier => new(Extent.End - PinPosition);
    public bool IsDegenerate => InboundCarrier.IsZero && OutboundCarrier.IsZero;
    public long GetPositionOn(IElement element) => element.GetPositionAt(this);

    public override string ToString() => $"{OutboundCarrier.Value}/{InboundCarrier.Value}";

    internal decimal ToDecimal() => ToDecimalRatio(InboundCarrier.Value, OutboundCarrier.Value);

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
