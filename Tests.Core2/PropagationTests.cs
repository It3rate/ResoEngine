using Core2.Propagation;

namespace Tests.Core2;

public class PropagationTests
{
    [Fact]
    public void ResponseProfile_CanScatterIntoMixedCoPresentResponses()
    {
        var frame = new PropagationFrame("local", 0m, 2m, WrapTargetKey: "loop");
        var profile = new ResponseProfile(
            "mixed",
            [
                new ResponseTerm(
                    PropagationResponseMode.Reflect,
                    Portion: 0.75m,
                    IncidentDirection: PacketFlowDirection.Reverse,
                    Boundary: PropagationBoundaryKind.Minimum),
                new ResponseTerm(
                    PropagationResponseMode.Continue,
                    Portion: 0.25m,
                    IncidentDirection: PacketFlowDirection.Reverse,
                    Boundary: PropagationBoundaryKind.Minimum),
            ]);

        var incident = new TensionPacket(
            "stem",
            "local",
            0m,
            PacketFlowDirection.Reverse,
            2m);

        var family = profile.Scatter(incident, frame, PropagationBoundaryKind.Minimum);

        Assert.Equal(2, family.Members.Count);
        Assert.Equal(global::Core2.Branching.BranchSemantics.CoPresent, family.Semantics);
        Assert.Contains(family.Values, packet => packet.Direction == PacketFlowDirection.Forward && packet.Magnitude == 1.5m);
        Assert.Contains(family.Values, packet => packet.Direction == PacketFlowDirection.Reverse && packet.Magnitude == 0.5m);
    }

    [Fact]
    public void ResponseProfile_WrapsToOppositeBoundaryAndTargetFrame()
    {
        var frame = new PropagationFrame("local", 0m, 4m, WrapTargetKey: "periodic");
        var profile = ResponseProfile.Wrapping("wrap");
        var incident = new TensionPacket(
            "orbit",
            "local",
            4m,
            PacketFlowDirection.Forward,
            1m);

        var family = profile.Scatter(incident, frame, PropagationBoundaryKind.Maximum);
        var wrapped = Assert.Single(family.Values);

        Assert.Equal(0m, wrapped.Position);
        Assert.Equal("periodic", wrapped.FrameKey);
        Assert.Equal(PacketFlowDirection.Forward, wrapped.Direction);
    }
}
