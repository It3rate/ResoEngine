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

    public Proportion(ICarrier start, ICarrier end, long pinPosition)
        : this(new RawExtent(start.RawValue, end.RawValue), pinPosition)
    {
    }

    public Proportion(Pin pin)
        : this(pin.Start, pin.End, pin.ResolvedPosition.RawValue)
    {
    }

    public int Grade => 1;
    public ICarrier Start => new LongCarrier(Extent.Start.RawValue - PinPosition, CarrierSide.Inbound);
    public ICarrier End => new LongCarrier(Extent.End.RawValue - PinPosition, CarrierSide.Outbound);
    public bool IsDegenerate => Start.IsZero && End.IsZero;

    public IElement Mirror() =>
        new Proportion(
            new RawExtent(
                checked((2L * PinPosition) - Extent.EndValue),
                checked((2L * PinPosition) - Extent.StartValue)),
            PinPosition);

    public override string ToString() => $"{End.Value}/{Start.Value}";

    internal decimal ToDecimal() => ToDecimalRatio(Start.Value, End.Value);

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
