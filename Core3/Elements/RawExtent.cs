namespace Core3.Elements;

/// <summary>
/// A completely unprocessed extent.
/// It carries stored endpoints so a computer can hold it, but it does not
/// yet imply a zero point, unit choice, or privileged direction.
/// </summary>
public readonly record struct RawExtent(long StartValue, long EndValue) : IElement
{
    public InboundCarrier Start => new(StartValue);
    public OutboundCarrier End => new(EndValue);
    public long Lower => Math.Min(StartValue, EndValue);
    public long Upper => Math.Max(StartValue, EndValue);
    public long StoredDelta => EndValue - StartValue;
    public long Magnitude => Math.Abs(StoredDelta);
    public bool IsDegenerate => StartValue == EndValue;

    public RawExtent Reverse() => new(EndValue, StartValue);

    public InboundCarrier GetInboundCarrier(Pin pin) => new(Start.RawValue - pin.ResolvedPosition.RawValue);

    public OutboundCarrier GetOutboundCarrier(Pin pin) => new(End.RawValue - pin.ResolvedPosition.RawValue);

    public Pin At(Proportion relativePosition) => new(relativePosition, this);

    public Proportion ToProportion() => new(this);

    public bool HasSameMagnitude(RawExtent other) => Magnitude == other.Magnitude;

    public override string ToString() => $"<{StartValue}..{EndValue}> |{Magnitude}|";
}
