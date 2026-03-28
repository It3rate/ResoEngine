using Core3.Engine;

namespace Core3.Runtime;

/// <summary>
/// Generic runtime piece metadata for operation results. The actual result and
/// carrier remain ordinary graded elements; this wrapper only preserves
/// provenance about how that piece arose within an operation context.
/// </summary>
public sealed record EngineOperationPiece(
    GradedElement Result,
    GradedElement Carrier,
    IReadOnlyList<int> SourceMemberIndices)
{
    public int SourceMemberCount => SourceMemberIndices.Count;
}
