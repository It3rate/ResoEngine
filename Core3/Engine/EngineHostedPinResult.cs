namespace Core3.Engine;

/// <summary>
/// Carries the result of resolving a hosted pin request, including unresolved
/// hosted position or side structure when the placement cannot settle exactly.
/// </summary>
public sealed record EngineHostedPinResult(
    CompositeElement Host,
    GradedElement RequestedPosition,
    GradedElement ResolvedPosition,
    GradedElement Inbound,
    GradedElement Outbound,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public bool IsExact => Tension is null;
}
