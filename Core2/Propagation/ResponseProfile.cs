using Core2.Branching;

namespace Core2.Propagation;

public sealed record ResponseProfile(
    string Key,
    IReadOnlyList<ResponseTerm> Terms,
    string? Note = null)
{
    public IReadOnlyList<ResponseTerm> Match(PropagationBoundaryKind boundaryKind, PacketFlowDirection direction) =>
        Terms.Where(term => term.AppliesTo(boundaryKind, direction)).ToArray();

    public BranchFamily<TensionPacket> Scatter(
        TensionPacket incident,
        PropagationFrame frame,
        PropagationBoundaryKind boundaryKind,
        IReadOnlyList<BranchId>? parents = null)
    {
        ArgumentNullException.ThrowIfNull(incident);
        ArgumentNullException.ThrowIfNull(frame);

        var matched = Match(boundaryKind, incident.Direction);
        if (matched.Count == 0)
        {
            return BranchFamily<TensionPacket>.Empty(
                BranchOrigin.Continuation,
                BranchSemantics.CoPresent,
                BranchDirection.Forward);
        }

        var members = new List<BranchMember<TensionPacket>>(matched.Count);
        foreach (var term in matched)
        {
            decimal magnitude = Math.Max(0m, incident.Magnitude * term.Portion * term.MagnitudeScale);
            decimal mobility = Math.Max(0m, incident.Mobility * term.MobilityScale);
            if (term.Mode is PropagationResponseMode.Clamp or PropagationResponseMode.Dissipate)
            {
                mobility = 0m;
            }

            if (magnitude == 0m && mobility == 0m)
            {
                continue;
            }

            PacketFlowDirection outgoingDirection = term.OutgoingDirection ?? term.Mode switch
            {
                PropagationResponseMode.Reflect => incident.OppositeDirection(),
                _ => incident.Direction,
            };

            decimal position = term.Mode switch
            {
                PropagationResponseMode.Wrap => frame.WrapPosition(boundaryKind),
                _ => boundaryKind is PropagationBoundaryKind.Minimum or PropagationBoundaryKind.Maximum
                    ? frame.BoundaryPosition(boundaryKind)
                    : incident.Position,
            };

            string frameKey = term.FrameKey
                ?? (term.Mode == PropagationResponseMode.Wrap && frame.WrapTargetKey is not null
                    ? frame.WrapTargetKey
                    : incident.FrameKey);

            var packet = new TensionPacket(
                term.CarrierKey ?? incident.CarrierKey,
                frameKey,
                position,
                outgoingDirection,
                magnitude,
                incident.Phase,
                mobility,
                incident.Dissipation + term.DissipationDelta,
                incident.Sources,
                incident.Annotations);

            members.Add(new BranchMember<TensionPacket>(
                BranchId.New(),
                packet,
                parents ?? [],
                incident.Annotations ?? []));
        }

        return BranchFamily<TensionPacket>.FromMembers(
            BranchOrigin.Continuation,
            members.Count > 1 ? BranchSemantics.CoPresent : BranchSemantics.Mixed,
            BranchDirection.Forward,
            members);
    }

    public static ResponseProfile Reflecting(string key) =>
        new(
            key,
            [
                new ResponseTerm(PropagationResponseMode.Reflect, IncidentDirection: PacketFlowDirection.Forward, Boundary: PropagationBoundaryKind.Maximum),
                new ResponseTerm(PropagationResponseMode.Reflect, IncidentDirection: PacketFlowDirection.Reverse, Boundary: PropagationBoundaryKind.Minimum),
            ]);

    public static ResponseProfile Wrapping(string key) =>
        new(
            key,
            [
                new ResponseTerm(PropagationResponseMode.Wrap, IncidentDirection: PacketFlowDirection.Forward, Boundary: PropagationBoundaryKind.Maximum),
                new ResponseTerm(PropagationResponseMode.Wrap, IncidentDirection: PacketFlowDirection.Reverse, Boundary: PropagationBoundaryKind.Minimum),
            ]);

    public static ResponseProfile Continuing(string key) =>
        new(
            key,
            [
                new ResponseTerm(PropagationResponseMode.Continue, Boundary: PropagationBoundaryKind.Minimum),
                new ResponseTerm(PropagationResponseMode.Continue, Boundary: PropagationBoundaryKind.Maximum),
                new ResponseTerm(PropagationResponseMode.Continue, Boundary: PropagationBoundaryKind.Junction),
                new ResponseTerm(PropagationResponseMode.Continue, Boundary: PropagationBoundaryKind.Ambient),
            ]);
}
