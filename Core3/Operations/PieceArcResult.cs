using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Shared outbound-piece result for reads, boolean projection, occupancy, and
/// any other operation arc whose main result is a family of co-present
/// survivors rather than one distinguished result element.
/// </summary>
public sealed record PieceArcResult : ArcResult
{
    public PieceArcResult(
        string originLawName,
        OperationContext context,
        IReadOnlyList<OperationPiece> pieces,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        OriginLawName = originLawName;
        Pieces = pieces;
    }

    public override string OriginLawName { get; }
    public IReadOnlyList<OperationPiece> Pieces { get; }
    public override IReadOnlyList<OperationPiece> OutboundPieces => Pieces;
    public IReadOnlyList<GradedElement> Results =>
        Pieces.Select(static piece => piece.Result).ToArray();

    public static PieceArcResult FromResults(
        string originLawName,
        OperationContext context,
        IReadOnlyList<GradedElement> results,
        GradedElement? tension = null,
        string? note = null) =>
        new(
            originLawName,
            context,
            results
                .Select((result, index) => new OperationPiece(result, context.Frame, [index]))
                .ToArray(),
            tension,
            note);
}
