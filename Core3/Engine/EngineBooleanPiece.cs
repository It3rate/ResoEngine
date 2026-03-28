namespace Core3.Engine;

public sealed record EngineBooleanPiece(
    CompositeElement Segment,
    CompositeElement Carrier,
    bool InPrimary,
    bool InSecondary);
