namespace Core3.Elements;

/// <summary>
/// A completely unprocessed extent.
/// It carries stored endpoints so a computer can hold it, but it does not
/// yet imply a zero point, unit choice, or privileged direction.
/// </summary>
public readonly record struct RawExtent(long Start, long End)
{
    public long Lower => Math.Min(Start, End);
    public long Upper => Math.Max(Start, End);
    public long StoredDelta => End - Start;
    public long Magnitude => Math.Abs(StoredDelta);
    public bool IsDegenerate => Start == End;

    public RawExtent Reverse() => new(End, Start);

    public bool HasSameMagnitude(RawExtent other) => Magnitude == other.Magnitude;

    public override string ToString() => $"<{Start}..{End}> |{Magnitude}|";
}
