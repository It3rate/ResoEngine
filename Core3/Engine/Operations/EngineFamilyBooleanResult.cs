using Core3.Engine;
using Core3.Engine.Runtime;

namespace Core3.Engine.Operations;

/// <summary>
/// Family-wide boolean result metadata. The result segments remain ordinary
/// composite elements; this record preserves the frame, family, ordering hint,
/// and piece provenance for the occupancy read.
/// </summary>
public sealed record EngineFamilyBooleanResult
{
    public EngineFamilyBooleanResult(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        IReadOnlyList<EngineOperationPiece> pieces)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(pieces);

        Context = context;
        Operation = operation;
        Pieces = pieces;
    }

    public EngineOperationContext Context { get; }
    public CompositeElement Frame => (CompositeElement)Context.Frame;
    public IReadOnlyList<CompositeElement> Members => Context.Members.Cast<CompositeElement>().ToArray();
    public bool IsOrdered => Context.IsOrdered;
    public EngineOccupancyOperation Operation { get; }
    public IReadOnlyList<EngineOperationPiece> Pieces { get; }
    public bool HasAny => Pieces.Count > 0;
    public IReadOnlyList<CompositeElement> Segments => Pieces.Select(piece => (CompositeElement)piece.Result).ToArray();
}
