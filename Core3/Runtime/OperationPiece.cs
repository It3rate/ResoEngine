using Core3.Engine;

namespace Core3.Runtime;

/// <summary>
/// Generic runtime metadata for one outbound survivor of an operation arc. The
/// actual result and carrier remain ordinary graded elements; this wrapper only
/// preserves provenance about how that survivor arose within an operation
/// context.
/// </summary>
public sealed record OperationPiece(
    GradedElement Result,
    GradedElement Carrier,
    IReadOnlyList<int> SourceMemberIndices)
{
    public int SourceMemberCount => SourceMemberIndices.Count;
}

