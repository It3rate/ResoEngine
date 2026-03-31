using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Family-wide boolean result metadata. In the broader operation-arc reading,
/// the context is the inbound side, the occupancy predicate is the origin law,
/// and the surviving pieces are the outbound family. The result segments
/// remain ordinary composite elements.
/// </summary>
public sealed record EngineFamilyBooleanResult : IExactResult
{
    public EngineFamilyBooleanResult(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        IReadOnlyList<EngineOperationPiece> pieces,
        GradedElement? tension = null,
        string? note = null)
    {
        Context = context;
        Operation = operation;
        Pieces = pieces;
        Tension = tension;
        Note = note;
    }

    public EngineOperationContext Context { get; }
    public EngineOperationContext Inbound => Context;
    public CompositeElement Frame => (CompositeElement)Context.Frame;
    public IReadOnlyList<CompositeElement> Members => Context.Members.Cast<CompositeElement>().ToArray();
    public bool IsOrdered => Context.IsOrdered;
    public EngineOccupancyOperation Operation { get; }
    public EngineOccupancyOperation OriginLaw => Operation;
    public string OriginLawName => Operation.ToString();
    public IReadOnlyList<EngineOperationPiece> Pieces { get; }
    public IReadOnlyList<EngineOperationPiece> OutboundPieces => Pieces;
    public GradedElement? Tension { get; }
    public string? Note { get; }
    public bool IsExact => Tension is null;
    public bool HasAny => Pieces.Count > 0;
    public bool HasMany => Pieces.Count > 1;
    public IReadOnlyList<CompositeElement> Segments => Pieces.Select(piece => (CompositeElement)piece.Result).ToArray();
}
