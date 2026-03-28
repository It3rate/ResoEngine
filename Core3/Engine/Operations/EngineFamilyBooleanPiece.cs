using Core3.Engine;

namespace Core3.Engine.Operations;

public sealed record EngineFamilyBooleanPiece(
    CompositeElement Segment,
    CompositeElement Carrier,
    IReadOnlyList<int> PresentMemberIndices)
{
    public int PresenceCount => PresentMemberIndices.Count;
}
