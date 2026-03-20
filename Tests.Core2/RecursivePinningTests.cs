using Core2.Elements;
using Core2.Interpretation.Analysis;
using Core2.Interpretation.Placement;

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

    [Fact]
    public void CarrierPinSiteRouting_CanRecognizeTrueCross()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(5)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "Cross");

        var routing = site.ResolveRouting();

        Assert.Equal(CarrierJunctionSummary.Cross, routing.Summary);
        Assert.True(routing.HostContinues);
        Assert.True(routing.HasNonHostThroughCarrier);
        Assert.True(routing.HasThroughRoute(CarrierIncidentKind.RecessiveSide, CarrierIncidentKind.DominantSide));
    }

    [Fact]
    public void CarrierPinSiteRouting_CanRecognizeTeeWhenSharedCarrierDoesNotPassThroughHost()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "Tee");

        var routing = site.ResolveRouting();

        Assert.Equal(CarrierJunctionSummary.Tee, routing.Summary);
        Assert.False(routing.HostContinues);
        Assert.True(routing.HasNonHostThroughCarrier);
    }

    [Fact]
    public void CarrierPinSiteRouting_CanRecognizeBranchFromOneDistinctCarrier()
    {
        var stem = CarrierIdentity.Create("Stem");
        var branch = CarrierIdentity.Create("Branch");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(0, 0, 3, -1), new Proportion(5)),
            dominantAttachment: new CarrierSideAttachment(PinSideRole.Dominant, branch, Proportion.Zero),
            name: "Branch");

        var routing = site.ResolveRouting();

        Assert.Equal(CarrierJunctionSummary.Branch, routing.Summary);
        Assert.True(routing.HostContinues);
        Assert.False(routing.HasNonHostThroughCarrier);
    }

    [Fact]
    public void CarrierPinSiteRouting_CanRecognizeHostBoundOrthogonalCusp()
    {
        var stem = CarrierIdentity.Create("Stem");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 0, 0), new Proportion(5)),
            recessiveAttachment: new CarrierSideAttachment(PinSideRole.Recessive, stem, new Proportion(5)),
            name: "Cusp");

        var routing = site.ResolveRouting();

        Assert.Equal(CarrierJunctionSummary.Cusp, routing.Summary);
        Assert.True(routing.HostContinues);
        Assert.False(routing.HasNonHostThroughCarrier);
    }

    [Fact]
    public void CarrierPinSiteRouting_DoesNotTreatUnboundOrthogonalPairAsTrueCross()
    {
        var stem = CarrierIdentity.Create("Stem");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(5)),
            name: "CrossProposal");

        var routing = site.ResolveRouting();

        Assert.Equal(CarrierJunctionSummary.Open, routing.Summary);
        Assert.True(routing.HostContinues);
        Assert.False(routing.HasNonHostThroughCarrier);
        Assert.True(routing.HasCrossShapedProposal);
    }

    [Fact]
    public void CarrierPinGraphAnalysis_CanExposeSiteRoutingSummaries()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var site = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(5)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "Cross");

        var analysis = new CarrierPinGraph([stem, bar], [site]).Analyze();
        var siteProfile = analysis.GetSiteProfile(site.Id);

        Assert.Equal(CarrierJunctionSummary.Cross, siteProfile.Summary);
        Assert.True(siteProfile.HostContinues);
        Assert.True(siteProfile.HasTrueCross);
        Assert.Equal(2, siteProfile.ParticipatingCarriers.Count);
        Assert.Equal(2, siteProfile.ThroughCarriers.Count);
    }

    [Fact]
    public void CarrierPinGraphAnalysis_CanCountCarrierThroughAndCrossSites()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var cross = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(5)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "Cross");
        var branch = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(0, 0, 3, -1), new Proportion(8)),
            dominantAttachment: new CarrierSideAttachment(PinSideRole.Dominant, bar, new Proportion(2)),
            name: "Branch");

        var analysis = new CarrierPinGraph([stem, bar], [cross, branch]).Analyze();
        var stemProfile = analysis.GetProfile(stem.Id);
        var barProfile = analysis.GetProfile(bar.Id);

        Assert.Equal(2, stemProfile.ParticipatingSiteCount);
        Assert.Equal(2, stemProfile.ThroughSiteCount);
        Assert.Equal(1, stemProfile.CrossSiteCount);
        Assert.Equal(1, stemProfile.BranchSiteCount);

        Assert.Equal(2, barProfile.ParticipatingSiteCount);
        Assert.Equal(1, barProfile.ThroughSiteCount);
        Assert.Equal(1, barProfile.CrossSiteCount);
        Assert.Equal(1, barProfile.BranchSiteCount);
    }
}
