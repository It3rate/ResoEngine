namespace Core3.Engine;

/// <summary>
/// Carries a lawful pair result together with any unresolved structure that had
/// to remain present as tension.
/// </summary>
public sealed record EngineElementPairOutcome(
    GradedElement Left,
    GradedElement Right,
    GradedElement? Tension = null,
    string? Note = null)
{
    public bool IsExact => Tension is null;

    public static EngineElementPairOutcome Exact(
        GradedElement left,
        GradedElement right) =>
        new(left, right);

    public static EngineElementPairOutcome WithTension(
        GradedElement left,
        GradedElement right,
        GradedElement tension,
        string? note = null) =>
        new(left, right, tension, note);
}
