namespace Core3.Elements;

/// <summary>
/// An ordered thing that can support pinning.
/// The attachment decides how a scalar-like relative position becomes a concrete
/// position in its dominant units, and how inbound and outbound carriers are read there.
/// </summary>
public interface IElement : IScalar
{
    long GetPositionAt(IScalar relativePosition);
    long GetInboundCarrier(Pin pin);
    long GetOutboundCarrier(Pin pin);
}
