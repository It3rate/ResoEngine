using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the outbound reads of an operation arc whose local law is
/// frame-relative reading. The reads remain ordinary graded elements; this
/// wrapper preserves the inbound context and any unresolved structure that
/// remained present during the read.
/// </summary>
public sealed record EngineReadResult(
    EngineOperationContext Context,
    IReadOnlyList<GradedElement> Reads,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public EngineOperationContext Inbound => Context;
    public GradedElement Frame => Context.Frame;
    public bool IsOrdered => Context.IsOrdered;
    public IReadOnlyList<GradedElement> OutboundReads => Reads;
    public bool IsExact => Tension is null;
}
