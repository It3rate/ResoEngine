namespace Core3.Elements;

/// <summary>
/// A completely unprocessed extent.
/// It carries stored endpoints so a computer can hold it, but it does not
/// yet imply a zero point, unit choice, or privileged direction.
/// </summary>
public readonly record struct RawExtent(long Start, long End) : IElement
{
    public long Lower => Math.Min(Start, End);
    public long Upper => Math.Max(Start, End);
    public long StoredDelta => End - Start;
    public long Magnitude => Math.Abs(StoredDelta);
    public bool IsDegenerate => Start == End;

    public RawExtent Reverse() => new(End, Start);

    public long GetPositionAt(IScalar relativePosition) =>
        checked((long)Math.Round(Start + ((End - Start) * relativePosition.ToScalar())));

    public long GetInboundCarrier(Pin pin) => pin.ResolvedPosition - Start;

    public long GetOutboundCarrier(Pin pin) => End - pin.ResolvedPosition;

    public Pin At(IScalar relativePosition) => new(relativePosition, this);

    public decimal ToScalar() => ToScalarRatio(-Start, End);

    public bool HasSameMagnitude(RawExtent other) => Magnitude == other.Magnitude;

    public override string ToString() => $"<{Start}..{End}> |{Magnitude}|";

    private static decimal ToScalarRatio(decimal inboundCarrier, decimal outboundCarrier)
    {
        if (inboundCarrier == 0m)
        {
            if (outboundCarrier == 0m)
            {
                return 0m;
            }

            return outboundCarrier > 0m ? decimal.MaxValue : decimal.MinValue;
        }

        return outboundCarrier / inboundCarrier;
    }
}
