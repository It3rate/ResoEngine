namespace Core3.Elements;

/// <summary>
/// An ordered thing that can support pinning.
/// The attachment decides how a relative proportion becomes a concrete position
/// in its dominant units, and how inbound and outbound carriers are read there.
/// </summary>
public interface IElement
{
    long GetPositionAt(Proportion relativePosition);
    long GetInboundCarrier(Pin pin);
    long GetOutboundCarrier(Pin pin);
}
