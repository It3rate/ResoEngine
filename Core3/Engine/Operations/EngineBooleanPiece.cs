using Core3.Engine;

namespace Core3.Engine.Operations;

public sealed record EngineBooleanPiece(
    CompositeElement Segment,
    CompositeElement Carrier,
    bool InPrimary,
    bool InSecondary);
