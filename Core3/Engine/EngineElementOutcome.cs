namespace Core3.Engine;

/// <summary>
/// Carries a lawful element result together with any unresolved structure that
/// had to be preserved as tension instead of being erased into failure.
/// Conceptually this is the one-survivor case of the broader outbound-family
/// pattern used across Core3 results.
/// </summary>
public sealed record EngineElementOutcome(
    GradedElement Result,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public bool IsExact => Tension is null;
    public IReadOnlyList<GradedElement> OutboundResults => [Result];
    public bool HasAny => true;
    public bool HasMany => false;
    public int SurvivorCount => 1;

    public static EngineElementOutcome Exact(GradedElement result) => new(result);

    public static EngineElementOutcome WithTension(
        GradedElement result,
        GradedElement tension,
        string? note = null) =>
        new(result, tension, note);

    public bool TryGetRawPair(out EngineElementPairOutcome? pair)
    {
        // Temporary inspection shell. Long term this wants to become an
        // ordinary frame/family read over preserved structure rather than a
        // bespoke "raw pair" helper.
        if (Tension is CompositeElement composite)
        {
            pair = EngineElementPairOutcome.Exact(
                composite.Recessive,
                composite.Dominant);
            return true;
        }

        pair = null;
        return false;
    }
}
