using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the outbound reads of an operation arc whose local law is
/// frame-relative reading. The reads remain ordinary graded elements; this
/// wrapper preserves the inbound context and any unresolved structure that
/// remained present during the read.
/// </summary>
public sealed record EngineReadResult : EngineArcResult
{
    public EngineReadResult(
        EngineOperationContext context,
        IReadOnlyList<GradedElement> reads,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        Reads = reads;
    }

    public GradedElement Frame => Context.Frame;
    public bool IsOrdered => Context.IsOrdered;
    public IReadOnlyList<GradedElement> Reads { get; }
    public IReadOnlyList<GradedElement> OutboundReads => Reads;
    public override string OriginLawName => "Read";
    public override IReadOnlyList<EngineOperationPiece> OutboundPieces =>
        Reads
            .Select((read, index) => new EngineOperationPiece(read, Frame, [index]))
            .ToArray();
}
