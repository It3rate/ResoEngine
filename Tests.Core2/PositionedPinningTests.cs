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
