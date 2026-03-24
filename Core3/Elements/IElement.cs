namespace Core3.Elements;

/// <summary>
/// An ordered thing that can support pinning.
/// It exposes start and end in its own stored order, and can then read
/// inbound and outbound carriers relative to a concrete pin.
/// </summary>
public interface IElement
{
    InboundCarrier Start { get; }
    OutboundCarrier End { get; }
    InboundCarrier GetInboundCarrier(Pin pin);
    OutboundCarrier GetOutboundCarrier(Pin pin);
}
