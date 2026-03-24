namespace Core3.Elements;

/// <summary>
/// A stored inbound carrier value.
/// It keeps the raw measured value internally, but exposes the flipped
/// inbound reading as its public value.
/// </summary>
public readonly record struct InboundCarrier
{
    private readonly long rawValue;

    public InboundCarrier(long rawValue)
    {
        this.rawValue = rawValue;
    }

    internal long RawValue => rawValue;
    public long Value => -rawValue;
    public bool IsZero => rawValue == 0;
    public bool IsPositive => Value > 0;
    public bool IsNegative => Value < 0;
    public OutboundCarrier AsOutbound() => new(rawValue);

    public static explicit operator OutboundCarrier(InboundCarrier carrier) => carrier.AsOutbound();

    public override string ToString() => Value.ToString();
}

/// <summary>
/// A live inbound carrier reference calculated from an attachment and a pin.
/// </summary>
public readonly record struct InboundCarrierRef(Pin Pin)
{
    public InboundCarrier Carrier => Pin.Attachment.GetInboundCarrier(Pin);
    public long Value => Carrier.Value;
    public OutboundCarrier AsOutbound() => Carrier.AsOutbound();

    public override string ToString() => Value.ToString();
}
