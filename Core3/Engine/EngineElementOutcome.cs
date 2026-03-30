namespace Core3.Engine;

/// <summary>
/// Carries a lawful element result together with any unresolved structure that
/// had to be preserved as tension instead of being erased into failure.
/// </summary>
public sealed record EngineElementOutcome(
    GradedElement Result,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public bool IsExact => Tension is null;

    public static EngineElementOutcome Exact(GradedElement result) => new(result);

    public static EngineElementOutcome WithTension(
        GradedElement result,
        GradedElement tension,
        string? note = null) =>
        new(result, tension, note);
}
