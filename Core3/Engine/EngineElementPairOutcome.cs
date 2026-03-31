namespace Core3.Engine;

/// <summary>
/// Carries a lawful pair result together with any unresolved structure that had
/// to remain present as tension. Conceptually this is the co-present
/// two-survivor case of the broader outbound-family pattern used across Core3
/// results.
/// </summary>
public sealed record EngineElementPairOutcome(
    GradedElement Left,
    GradedElement Right,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public bool IsExact => Tension is null;
    public IReadOnlyList<GradedElement> OutboundResults => [Left, Right];
    public bool HasAny => true;
    public bool HasMany => true;
    public int SurvivorCount => 2;

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

    // TODO: Collapse this temporary shell into a real higher-grade Core3
    // element once tension can be represented natively as graded structure
    // rather than sidecar bookkeeping.
}
