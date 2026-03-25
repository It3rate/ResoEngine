namespace Core3.Elements;

/// <summary>
/// An ordered thing that can support pinning.
/// It exposes start and end in its own stored order, and carries its current
/// place in the grade ladder.
/// </summary>
public interface IElement
{
    int Grade { get; }
    ICarrier Start { get; }
    ICarrier End { get; }
    IElement Mirror();
}
