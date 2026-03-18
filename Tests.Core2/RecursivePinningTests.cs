using Core2.Elements;

namespace Tests.Core2;

public class RecursivePinningTests
{
    [Fact]
    public void CarrierPinSite_CanBindBothSidesToExplicitSharedCarriers()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var applied = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var pinning = host.PinAt(applied, new Proportion(0));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            pinning,
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, new Proportion(0)),
            name: "Top");

        Assert.Equal(stem, site.HostCarrier);
        Assert.Equal(host, site.Host);
        Assert.Equal(applied, site.Applied);
        Assert.Equal(new Proportion(0), site.HostPosition);
        Assert.Equal(stem.Id, site.RecessiveAttachment?.CarrierId);
        Assert.Equal(new Proportion(0), site.RecessiveAttachment?.CarrierPosition);
        Assert.Equal(bowl.Id, site.DominantAttachment?.CarrierId);
        Assert.Equal(new Proportion(0), site.DominantAttachment?.CarrierPosition);
    }

    [Fact]
    public void CarrierPinGraph_PreservesSharedCarrierIdentityAcrossMultipleSites()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var topApplied = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var bottomApplied = new Axis(new Proportion(-3, -1), new Proportion(2, 1));

        var top = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(topApplied, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, new Proportion(0)),
            name: "Top");
        var bottom = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(bottomApplied, new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(1)),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, new Proportion(1)),
            name: "Bottom");

        var graph = new CarrierPinGraph([stem, bowl], [top, bottom]);

        Assert.Equal(2, graph.GetHostedSites(stem.Id).Count);
        Assert.Equal(2, graph.GetReferencingSites(stem.Id).Count);
        Assert.Equal(2, graph.GetReferencingSites(bowl.Id).Count);
        Assert.Equal(
            [new Proportion(0), new Proportion(1)],
            graph.GetAttachments(bowl.Id).Select(attachment => attachment.CarrierPosition).ToArray());
        Assert.Single(graph.GetReferencedCarriers(stem.Id, includeSelf: false));
        Assert.Equal(bowl.Id, graph.GetReferencedCarriers(stem.Id, includeSelf: false)[0].Id);
    }

    [Fact]
    public void CarrierPinGraph_AllowsMutualRecursiveCarrierCycles()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var outward = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var inward = new Axis(new Proportion(1, 1), new Proportion(2, -1));

        var topOnStem = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(outward, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, new Proportion(0)),
            name: "TopOnStem");
        var bottomOnStem = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(-3, -1), new Proportion(2, 1)), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(1)),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, new Proportion(1)),
            name: "BottomOnStem");

        var topOnBowl = CarrierPinSite.FromPointPinning(
            bowl,
            host.PinAt(inward, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Recessive, bowl, new Proportion(0)),
            new CarrierSideAttachment(PinSideRole.Dominant, stem, new Proportion(0)),
            name: "TopOnBowl");
        var bottomOnBowl = CarrierPinSite.FromPointPinning(
            bowl,
            host.PinAt(new Axis(new Proportion(1, 1), new Proportion(-2, -1)), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, bowl, new Proportion(1)),
            new CarrierSideAttachment(PinSideRole.Dominant, stem, new Proportion(1)),
            name: "BottomOnBowl");

        var graph = new CarrierPinGraph([stem, bowl], [topOnStem, bottomOnStem, topOnBowl, bottomOnBowl]);

        Assert.True(graph.HasRecursiveCarrierCycle());
        Assert.False(new CarrierPinGraph([stem, bowl], [topOnStem, bottomOnStem]).HasRecursiveCarrierCycle());
        Assert.Contains(
            new CarrierPinGraph([stem, bowl], [topOnStem, bottomOnStem]).GetReferencedCarriers(stem.Id),
            carrier => carrier.Id == stem.Id);
    }

    [Fact]
    public void CarrierPinGraphAnalysis_CanSummarizeSharedCarrierSpansWithoutGeometry()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var top = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(3, -1), new Proportion(2, 1)), Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.Zero),
            name: "Top");
        var bottom = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(-3, -1), new Proportion(2, 1)), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.One),
            name: "Bottom");

        var analysis = new CarrierPinGraph([stem, bowl], [top, bottom]).Analyze();
        var bowlProfile = analysis.GetProfile(bowl.Id);

        Assert.True(bowlProfile.IsReferenced);
        Assert.True(bowlProfile.IsSharedAcrossSites);
        Assert.True(bowlProfile.HasAttachmentSpan);
        Assert.Equal(Proportion.Zero, bowlProfile.FirstAttachmentPosition);
        Assert.Equal(Proportion.One, bowlProfile.LastAttachmentPosition);
        Assert.False(bowlProfile.ParticipatesInRecursiveCycle);
    }

    [Fact]
    public void CarrierPinGraphAnalysis_CanMarkRecursiveCycleParticipants()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var topOnStem = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(3, -1), new Proportion(2, 1)), Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.Zero));
        var bottomOnStem = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(-3, -1), new Proportion(2, 1)), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.One));
        var topOnBowl = CarrierPinSite.FromPointPinning(
            bowl,
            host.PinAt(new Axis(new Proportion(1, 1), new Proportion(2, -1)), Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Recessive, bowl, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, stem, Proportion.Zero));
        var bottomOnBowl = CarrierPinSite.FromPointPinning(
            bowl,
            host.PinAt(new Axis(new Proportion(1, 1), new Proportion(-2, -1)), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, bowl, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, stem, Proportion.One));

        var analysis = new CarrierPinGraph([stem, bowl], [topOnStem, bottomOnStem, topOnBowl, bottomOnBowl]).Analyze();

        Assert.True(analysis.GetProfile(stem.Id).ParticipatesInRecursiveCycle);
        Assert.True(analysis.GetProfile(bowl.Id).ParticipatesInRecursiveCycle);
    }

    [Fact]
    public void CarrierPinSite_CanExposeItsPlacedAppliedAxis()
    {
        var stem = CarrierIdentity.Create("Stem");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var applied = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(applied, new Proportion(4)),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(4)),
            new CarrierSideAttachment(PinSideRole.Dominant, CarrierIdentity.Create("Bowl"), new Proportion(0)));

        var placed = site.PlaceApplied();

        Assert.Equal(new Proportion(4), placed.EmbeddedOrigin);
        Assert.Equal(applied, site.Applied);
        Assert.Equal(new Proportion(4), placed.RecessiveSide.Origin);
        Assert.Equal(new Proportion(4), placed.DominantSide.Origin);
    }
}
