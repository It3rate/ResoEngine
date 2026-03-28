using Core3.Engine;

namespace Core3.Engine.Operations;

/// <summary>
/// Family-occupancy boolean piece metadata. The segment itself remains an
/// ordinary composite element; this wrapper tracks which family members were
/// present when the piece survived.
/// </summary>
public sealed record EngineFamilyBooleanPiece(
    CompositeElement Segment,
    CompositeElement Carrier,
    IReadOnlyList<int> PresentMemberIndices)
{
    public int PresenceCount => PresentMemberIndices.Count;
}
