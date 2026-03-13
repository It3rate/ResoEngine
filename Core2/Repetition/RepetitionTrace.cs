namespace Core2.Repetition;

/// <summary>
/// A first-pass repetition trace. States include the initial seed/identity state,
/// then one entry for each repetition step.
/// </summary>
public sealed record RepetitionTrace<T>(
    RepetitionKind Kind,
    IReadOnlyList<T> States)
{
    public T Result => States[^1];
    public int Steps => Math.Max(0, States.Count - 1);
}

public readonly record struct BoundaryContinuationResult(
    decimal Value,
    IReadOnlyList<RepetitionTension> Tensions)
{
    public bool HasTension => Tensions.Count > 0;
}
