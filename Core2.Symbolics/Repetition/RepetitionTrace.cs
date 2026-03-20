using Core2.Elements;

namespace Core2.Symbolics.Repetition;

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
