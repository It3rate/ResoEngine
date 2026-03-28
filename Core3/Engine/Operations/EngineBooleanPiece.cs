using Core3.Engine;

namespace Core3.Engine.Operations;

/// <summary>
/// Binary boolean piece metadata. The surviving segment remains a normal
/// composite element; this wrapper only preserves carrier and occupancy
/// provenance for the boolean read.
/// </summary>
public sealed record EngineBooleanPiece(
    CompositeElement Segment,
    CompositeElement Carrier,
    bool InPrimary,
    bool InSecondary);
