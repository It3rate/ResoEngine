using Core2.Branching;
using Core2.Dynamic;

namespace Core2.Propagation;

public sealed record TensionPacket(
    string CarrierKey,
    string FrameKey,
    decimal Position,
    PacketFlowDirection Direction,
    decimal Magnitude,
    decimal Phase = 0m,
    decimal Mobility = 1m,
    decimal Dissipation = 0m,
    IReadOnlyList<DynamicTension>? Sources = null,
    IReadOnlyList<IBranchAnnotation>? Annotations = null)
{
    public decimal SignedMobility => Direction == PacketFlowDirection.Forward ? Mobility : -Mobility;

    public bool IsDepleted => Magnitude <= 0m || Mobility == 0m;

    public PacketFlowDirection OppositeDirection() =>
        Direction == PacketFlowDirection.Forward ? PacketFlowDirection.Reverse : PacketFlowDirection.Forward;
}
