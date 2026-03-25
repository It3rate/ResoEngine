namespace Core3.Elements;

/// <summary>
/// A carrier-backed directed element.
/// It keeps its current start and end as carriers rather than rebuilding them
/// from raw longs on every read.
/// </summary>
public readonly record struct Axis : IElement
{
    public Axis(ICarrier start, ICarrier end)
    {
        Start = start.AsInbound();
        End = end.AsOutbound();
    }

    public Axis(IElement element)
        : this(element.Start, element.End)
    {
    }

    public Axis(Pin pin)
        : this(pin.InboundCarrier, pin.OutboundCarrier)
    {
    }

    public int Grade => 2;
    public ICarrier Start { get; }
    public ICarrier End { get; }
    public bool IsDegenerate => Start.IsZero && End.IsZero;

    public IElement Mirror() => new Axis(
        End.Negate().AsInbound(),
        Start.Negate().AsOutbound());

    public Pin At(Proportion position) => new(position, Start, End);

    public override string ToString() => $"in {Start}, out {End}";
}
