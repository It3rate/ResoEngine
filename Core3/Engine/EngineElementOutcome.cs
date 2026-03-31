namespace Core3.Engine;

/// <summary>
/// Carries a lawful element result together with any unresolved structure that
/// had to be preserved as tension instead of being erased into failure.
/// The outcome may carry one survivor or several co-present survivors; tension
/// simply means the result did not settle perfectly within the requested read.
/// </summary>
public sealed record EngineElementOutcome(
    IReadOnlyList<GradedElement> Results,
    GradedElement? Tension = null,
    string? Note = null)
{
    public GradedElement Result => Results[0];
    public GradedElement Left => Results[0];
    public GradedElement Right => Results[1];
    public bool IsExact => Tension is null;
    public IReadOnlyList<GradedElement> OutboundResults => Results;
    public bool HasAny => Results.Count > 0;
    public bool HasMany => Results.Count > 1;
    public int SurvivorCount => Results.Count;

    public static EngineElementOutcome Exact(GradedElement result) => new([result]);
    public static EngineElementOutcome Exact(GradedElement left, GradedElement right) => new([left, right]);

    public static EngineElementOutcome WithTension(
        GradedElement result,
        GradedElement tension,
        string? note = null) =>
        new([result], tension, note);

    public static EngineElementOutcome WithTension(
        GradedElement left,
        GradedElement right,
        GradedElement tension,
        string? note = null) =>
        new([left, right], tension, note);

    public bool TryGetPair(out GradedElement? left, out GradedElement? right)
    {
        if (Results.Count >= 2)
        {
            left = Results[0];
            right = Results[1];
            return true;
        }

        left = null;
        right = null;
        return false;
    }
}
