namespace Core3.Elements;

/// <summary>
/// A raw extent interpreted through a pin.
/// With no explicit pin supplied, it bootstraps itself by assuming a pin at zero.
/// </summary>
public readonly record struct Proportion(RawExtent Extent, long PinPosition) : IElement
{
    public Proportion(RawExtent extent)
        : this(extent, 0)
    {
    }

    public InboundCarrier Start => new(Extent.Start.RawValue - PinPosition);
    public OutboundCarrier End => new(Extent.End.RawValue - PinPosition);
    public InboundCarrier InboundCarrier => Start;
    public OutboundCarrier OutboundCarrier => End;
    public bool IsDegenerate => Start.IsZero && End.IsZero;
    public OutboundCarrier GetPositionOn(IElement element) =>
        new(checked((long)Math.Round(element.Start.RawValue + ((element.End.RawValue - element.Start.RawValue) * ToDecimal()))));

    public InboundCarrier GetInboundCarrier(Pin pin) => new(Start.RawValue - pin.ResolvedPosition.RawValue);

    public OutboundCarrier GetOutboundCarrier(Pin pin) => new(End.RawValue - pin.ResolvedPosition.RawValue);

    public override string ToString() => $"{OutboundCarrier.Value}/{InboundCarrier.Value}";

    private decimal ToDecimal() => ToDecimalRatio(InboundCarrier.Value, OutboundCarrier.Value);

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
