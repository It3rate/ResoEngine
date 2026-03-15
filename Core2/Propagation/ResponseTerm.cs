namespace Core2.Propagation;

public sealed record ResponseTerm(
    PropagationResponseMode Mode,
    decimal Portion = 1m,
    PacketFlowDirection? IncidentDirection = null,
    PropagationBoundaryKind? Boundary = null,
    PacketFlowDirection? OutgoingDirection = null,
    decimal MagnitudeScale = 1m,
    decimal MobilityScale = 1m,
    decimal DissipationDelta = 0m,
    string? CarrierKey = null,
    string? FrameKey = null,
    string? Note = null)
{
    public bool AppliesTo(PropagationBoundaryKind boundaryKind, PacketFlowDirection direction) =>
        (Boundary is null || Boundary == boundaryKind) &&
        (IncidentDirection is null || IncidentDirection == direction);
}
