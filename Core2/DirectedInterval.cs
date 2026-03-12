namespace ResoEngine.Core2;

/// <summary>
/// The invariant object: two ordered endpoints with no built-in metric meaning.
/// Meaning appears only after a frame is applied.
/// </summary>
public readonly record struct DirectedInterval(long Start, long End)
{
    public long Span => End - Start;

    public DirectedInterval Scale(long factor, long origin = 0)
    {
        long scaledStart = origin + ((Start - origin) * factor);
        long scaledEnd = origin + ((End - origin) * factor);
        return new DirectedInterval(scaledStart, scaledEnd);
    }

    public override string ToString() => $"[{Start} -> {End}]";
}
