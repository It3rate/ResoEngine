namespace Core3.Elements;

/// <summary>
/// An ordered thing that can support pinning.
/// It exposes start and end in its own stored order.
/// </summary>
public interface IElement
{
    ICarrier Start { get; }
    ICarrier End { get; }
    IElement Mirror();
}
