using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Family-wide boolean result metadata. In the broader operation-arc reading,
/// the context is the inbound side, the occupancy predicate is the origin law,
/// and the surviving pieces are the outbound family. The result segments
/// remain ordinary composite elements.
/// </summary>
public sealed record EngineFamilyBooleanResult : EngineArcResult
{
    public EngineFamilyBooleanResult(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        IReadOnlyList<EngineOperationPiece> pieces,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        Operation = operation;
        Pieces = pieces;
    }

    public EngineOccupancyOperation Operation { get; }
    public override string OriginLawName => Operation.ToString();
    public IReadOnlyList<EngineOperationPiece> Pieces { get; }
    public override IReadOnlyList<EngineOperationPiece> OutboundPieces => Pieces;
}
