namespace Core3.Elements;

/// <summary>
/// A completely unprocessed extent.
/// It carries stored endpoints so a computer can hold it, but it does not
/// yet imply a zero point, unit choice, or privileged direction.
/// </summary>
public readonly record struct RawExtent(long StartValue, long EndValue) : IElement
{
    public int Grade => 0;
    public ICarrier Start => new LongCarrier(StartValue, CarrierSide.Inbound);
    public ICarrier End => new LongCarrier(EndValue, CarrierSide.Outbound);
    public long Lower => Math.Min(StartValue, EndValue);
    public long Upper => Math.Max(StartValue, EndValue);
    public long StoredDelta => EndValue - StartValue;
    public long Magnitude => Math.Abs(StoredDelta);
    public bool IsDegenerate => StartValue == EndValue;

    public RawExtent Reverse() => new(EndValue, StartValue);

    public IElement Mirror() => new RawExtent(-EndValue, -StartValue);

    public Pin At(Proportion relativePosition) => new(relativePosition, Start, End);

    public Proportion ToProportion() => new(this);

    public bool HasSameMagnitude(RawExtent other) => Magnitude == other.Magnitude;

    public override string ToString() => $"<{StartValue}..{EndValue}> |{Magnitude}|";
}
