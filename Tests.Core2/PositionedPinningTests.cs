using Core2.Elements;

namespace Tests.Core2;

public class PositionedPinningTests
{
    [Fact]
    public void Axis_CanBePlacedAtAHostRelativePosition()
    {
        var local = Axis.FromCoordinates(new Proportion(-2), new Proportion(6));
        var placed = local.PlaceAt(new Proportion(10));

        Assert.Equal(new Proportion(10), placed.EmbeddedOrigin);
        Assert.True(placed.HasCollinearExtent);
        Assert.Equal(new Proportion(8), placed.EmbeddedCollinearStart);
        Assert.Equal(new Proportion(16), placed.EmbeddedCollinearEnd);
    }

    [Fact]
    public void PointPinning_PreservesHostAppliedAsymmetryAndPosition()
    {
        var host = Axis.FromCoordinates(new Proportion(-3), new Proportion(3));
        var applied = Axis.FromCoordinates(Proportion.Zero, new Proportion(4));
        var pinning = host.PinAt(applied, new Proportion(2));

        Assert.Equal(host, pinning.Host);
        Assert.Equal(applied, pinning.Applied);
        Assert.Equal(new Proportion(2), pinning.Position);
        Assert.Equal(Proportion.Zero, pinning.AppliedAnchor);
    }

    [Fact]
    public void PositionedAxis_PreservesIntrinsicBentSidesAtTheAttachmentSite()
    {
        var bent = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var placed = bent.PlaceAt(new Proportion(5));

        Assert.False(placed.HasCollinearExtent);
        Assert.Equal(new Proportion(5), placed.RecessiveSide.Origin);
        Assert.Equal(new Proportion(5), placed.DominantSide.Origin);
        Assert.Equal(1, placed.RecessiveSide.CarrierRank);
        Assert.Equal(0, placed.DominantSide.CarrierRank);
        Assert.Equal(new Proportion(-3), placed.RecessiveSide.SignedExtent);
        Assert.Equal(new Proportion(2), placed.DominantSide.SignedExtent);
    }

    [Fact]
    public void PositionedAxis_CanSummarizeCarrierResponseForInteriorAndBoundaryEncounters()
    {
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var interior = Axis.PinUnit.PlaceAt(new Proportion(5));
        var rightBoundary = Axis.PinUnit.PlaceAt(new Proportion(10));

        var forward = interior.ResolveCarrierResponse(+1);
        var reverse = interior.ResolveCarrierResponse(-1);
        var boundary = rightBoundary.ResolveCarrierResponse(+1, host: host, boundaryEncounter: true);

        Assert.Equal(PinSideRole.Dominant, forward.EncounteredSide.Role);
        Assert.True(forward.IsTransparent);
        Assert.True(forward.SupportsApproachIntoEncounter);
        Assert.False(forward.BlocksApproachIntoEncounter);
        Assert.Equal(PinSideRole.Recessive, reverse.EncounteredSide.Role);
        Assert.True(reverse.IsTransparent);
        Assert.True(reverse.BlocksApproachIntoEncounter);
        Assert.False(reverse.SupportsApproachIntoEncounter);
        Assert.Equal(PinSideRole.Recessive, boundary.EncounteredSide.Role);
        Assert.True(boundary.IsRedirect);
        Assert.Equal(-1, boundary.EncounterNextDirection);
    }

    [Fact]
    public void PositionedAxis_CarrierResponse_CapturesBlockAndOrthogonalPotential()
    {
        var dominantOpposing = new Axis(0, 1, -1, 1).PlaceAt(new Proportion(4));
        var recessiveOpposing = new Axis(-1, 1, 0, 1).PlaceAt(new Proportion(4));
        var bent = new Axis(1, -1, 1, 1).PlaceAt(new Proportion(4));

        var dominantResponse = dominantOpposing.ResolveCarrierResponse(+1);
        var recessiveResponse = recessiveOpposing.ResolveCarrierResponse(+1);
        var bentResponse = bent.ResolveCarrierResponse(+1);

        Assert.True(dominantResponse.BlocksHostNegativeSide);
        Assert.False(dominantResponse.BlocksHostPositiveSide);
        Assert.True(recessiveResponse.BlocksHostPositiveSide);
        Assert.True(bentResponse.HasOrthogonalOutlet);
        Assert.True(bentResponse.IsTransparent);
    }

    [Fact]
    public void PositionedAxisSide_DistinguishesDisplayDirectionFromTransportDirection()
    {
        var incoming = new Axis(1, 1, 0, 1).PlaceAt(new Proportion(4));
        var outgoing = new Axis(0, 1, 1, 1).PlaceAt(new Proportion(4));

        Assert.Equal(-1, incoming.RecessiveSide.DisplayDirectionSign);
        Assert.Equal(+1, incoming.RecessiveSide.TransportDirectionSign);
        Assert.Equal(+1, outgoing.DominantSide.DisplayDirectionSign);
        Assert.Equal(+1, outgoing.DominantSide.TransportDirectionSign);
    }

    [Fact]
    public void Proportion_CanParticipateInHostRelativePointPinning()
    {
        var host = new Proportion(8);
        var applied = new Proportion(3, -1);
        var pinning = host.PinAt(applied, new Proportion(2));

        Assert.Equal(host, pinning.Host);
        Assert.Equal(applied, pinning.Applied);
        Assert.Equal(new Proportion(2), pinning.Position);
    }
}
