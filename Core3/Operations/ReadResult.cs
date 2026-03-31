using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the outbound reads of an operation arc whose local law is
/// frame-relative reading. The reads remain ordinary graded elements; this
/// wrapper preserves the inbound context and any unresolved structure that
/// remained present during the read.
/// </summary>
public sealed record ReadResult : ArcResult
{
    public ReadResult(
        OperationContext context,
        IReadOnlyList<GradedElement> reads,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        Reads = reads;
    }

    public IReadOnlyList<GradedElement> Reads { get; }
    public override string OriginLawName => "Read";
    public override IReadOnlyList<OperationPiece> OutboundPieces =>
        Reads
            .Select((read, index) => new OperationPiece(read, Context.Frame, [index]))
            .ToArray();
}




