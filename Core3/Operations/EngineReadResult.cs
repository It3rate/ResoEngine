using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the frame-read family together with any unresolved structure that
/// remained present during the read.
/// </summary>
public sealed record EngineReadResult(
    EngineOperationContext Context,
    IReadOnlyList<GradedElement> Reads,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public GradedElement Frame => Context.Frame;
    public bool IsOrdered => Context.IsOrdered;
    public bool IsExact => Tension is null;
}
