namespace Core3.Elements;

/// <summary>
/// A stored outbound carrier value.
/// Its sign is read directly in outbound semantics.
/// </summary>
public readonly record struct OutboundCarrier
{
    private readonly long rawValue;

    public OutboundCarrier(long rawValue)
    {
        this.rawValue = rawValue;
    }

    internal long RawValue => rawValue;
    public long Value => rawValue;
    public bool IsZero => rawValue == 0;
    public bool IsPositive => Value > 0;
    public bool IsNegative => Value < 0;
    public InboundCarrier AsInbound() => new(rawValue);

    public static explicit operator InboundCarrier(OutboundCarrier carrier) => carrier.AsInbound();

    public override string ToString() => Value.ToString();
}

/// <summary>
/// A live outbound carrier reference calculated from an attachment and a pin.
/// </summary>
public readonly record struct OutboundCarrierRef(Pin Pin)
{
    public OutboundCarrier Carrier => Pin.Attachment.GetOutboundCarrier(Pin);
    public long Value => Carrier.Value;
    public InboundCarrier AsInbound() => Carrier.AsInbound();

    public override string ToString() => Value.ToString();
}
