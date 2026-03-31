using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Binary boolean result metadata. In the broader operation-arc reading, the
/// context is the inbound side, the boolean operator is the origin law, and
/// the surviving pieces are the outbound family. The actual pieces remain
/// ordinary composite elements.
/// </summary>
public sealed record EngineBooleanResult : EngineArcResult
{
    public EngineBooleanResult(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        IReadOnlyList<EngineOperationPiece> pieces,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        if (context.Count != 2 ||
            context.Frame is not CompositeElement ||
            context.Members[0] is not CompositeElement ||
            context.Members[1] is not CompositeElement)
        {
            throw new InvalidOperationException("Binary boolean results require a composite frame and two composite members.");
        }

        Operation = operation;
        Pieces = pieces;
    }

    public EngineBooleanOperation Operation { get; }
    public override string OriginLawName => Operation.ToString();
    public IReadOnlyList<EngineOperationPiece> Pieces { get; }
    public override IReadOnlyList<EngineOperationPiece> OutboundPieces => Pieces;
}
