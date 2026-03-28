using Core3.Engine;

namespace Core3.Engine.Operations;

/// <summary>
/// Family-wide boolean result metadata. The result segments remain ordinary
/// composite elements; this record preserves the frame, family, ordering hint,
/// and piece provenance for the occupancy read.
/// </summary>
public sealed record EngineFamilyBooleanResult
{
    public EngineFamilyBooleanResult(
        CompositeElement frame,
        IReadOnlyList<CompositeElement> members,
        bool isOrdered,
        EngineOccupancyOperation operation,
        IReadOnlyList<EngineFamilyBooleanPiece> pieces)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);
        ArgumentNullException.ThrowIfNull(pieces);

        Frame = frame;
        Members = members;
        IsOrdered = isOrdered;
        Operation = operation;
        Pieces = pieces;
    }

    public CompositeElement Frame { get; }
    public IReadOnlyList<CompositeElement> Members { get; }
    public bool IsOrdered { get; }
    public EngineOccupancyOperation Operation { get; }
    public IReadOnlyList<EngineFamilyBooleanPiece> Pieces { get; }
    public bool HasAny => Pieces.Count > 0;
    public IReadOnlyList<CompositeElement> Segments => Pieces.Select(piece => piece.Segment).ToArray();
}
