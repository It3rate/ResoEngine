namespace Core2.Propagation;

public sealed record CarrierState(
    string Key,
    PropagationFrame Frame,
    ResponseProfile MinimumBoundaryResponse,
    ResponseProfile MaximumBoundaryResponse,
    decimal ForwardMobility = 1m,
    decimal ReverseMobility = 1m,
    IReadOnlyList<CouplingRule>? Couplings = null)
{
    public decimal MobilityFor(PacketFlowDirection direction) =>
        direction == PacketFlowDirection.Forward ? ForwardMobility : ReverseMobility;

    public ResponseProfile ResponseFor(PropagationBoundaryKind boundaryKind) =>
        boundaryKind switch
        {
            PropagationBoundaryKind.Minimum => MinimumBoundaryResponse,
            PropagationBoundaryKind.Maximum => MaximumBoundaryResponse,
            _ => MaximumBoundaryResponse,
        };
}
