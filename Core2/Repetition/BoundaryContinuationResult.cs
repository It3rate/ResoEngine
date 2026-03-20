using Core2.Elements;

namespace Core2.Repetition;

public readonly record struct BoundaryContinuationResult(
    Proportion Value,
    IReadOnlyList<RepetitionTension> Tensions)
{
    public bool HasTension => Tensions.Count > 0;
}
