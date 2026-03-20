using Core2.Elements;
using Core2.Interpretation.Construction;

namespace Tests.Core2;

public class PinningTests
{
    [Fact]
    public void Axis_And_Area_ExposePinnedMetadata()
    {
        var axis = new Proportion(3).Pin(new Proportion(5));
        var area = axis.Pin(Axis.I);

        var axisPin = Assert.IsAssignableFrom<IPinnedElement<Proportion, Proportion>>(axis);
        var areaPin = Assert.IsAssignableFrom<IPinnedElement<Axis, Axis>>(area);

        Assert.Equal(PinRelation.CollinearOpposed, axisPin.Relation);
        Assert.Equal(PinRelationMode.Collinear, axisPin.Relation.Mode);
        Assert.Equal(PinPolarityMode.Opposed, axisPin.Relation.Polarity);
        Assert.Equal(new Proportion(3), axisPin.RecessiveElement);
        Assert.Equal(new Proportion(5), axisPin.DominantElement);

        Assert.Equal(PinRelation.CollinearOpposed, areaPin.Relation);
        Assert.Equal(PinRelationMode.Collinear, areaPin.Relation.Mode);
        Assert.Equal(PinPolarityMode.Opposed, areaPin.Relation.Polarity);
        Assert.Equal(axis, areaPin.RecessiveElement);
        Assert.Equal(Axis.I, areaPin.DominantElement);
    }

    [Fact]
    public void PinnedPair_RaisesGradeFromTheHighestPinnedChild()
    {
        var axis = Axis.One;
        var area = new Area(Axis.One, Axis.I);
        var pair = Pinning.Pin(axis, area, PinRelation.Parallel);

        Assert.Equal(4, pair.Degree);
        Assert.Equal(PinRelation.Parallel, pair.Relation);
    }

    [Fact]
    public void TwistedAreaPairs_CanBeRepresentedBeforeVolumeMathIsSettled()
    {
        var top = new Area(
            new Axis(new Proportion(1), new Proportion(2)),
            new Axis(new Proportion(3), new Proportion(4)));
        var bottom = new Area(
            new Axis(new Proportion(5), new Proportion(6)),
            new Axis(new Proportion(7), new Proportion(8)));

        var twisted = top.Pin(bottom, PinRelation.Twisted(PinContactMode.Point, quarterTurns: 1));

        Assert.Equal(4, twisted.Degree);
        Assert.Equal(PinRelationMode.Twisted, twisted.Relation.Mode);
        Assert.Equal(PinContactMode.Point, twisted.Relation.Contact);
        Assert.Equal(1, twisted.Relation.QuarterTurns);
        Assert.Equal(top, twisted.RecessiveElement);
        Assert.Equal(bottom, twisted.DominantElement);
    }

    [Fact]
    public void PinRelation_OffersFriendlyAliasesForCommonInterpretations()
    {
        Assert.Equal(PinRelation.CollinearSame, PinRelation.Parallel);
        Assert.Equal(PinRelation.OrthogonalDirect, PinRelation.Orthogonal);
        Assert.Equal("Collinear(Opposed)", PinRelation.CollinearOpposed.ToString());
        Assert.Equal("Orthogonal(Mirrored)", PinRelation.OrthogonalMirrored.ToString());
    }

    [Fact]
    public void Axis_PinResolution_DetectsSequentialReinforcement()
    {
        var axis = new Axis(new Proportion(-3, 1), new Proportion(5, 1));

        Assert.True(axis.IsSequentialReinforcement);
        Assert.Equal(PinRelation.CollinearSame, axis.Relation);
        Assert.Equal(0, axis.PreferredCarrierRank);
    }

    [Fact]
    public void Axis_PinResolution_PreservesNegativeUnitAsOrthogonalLift()
    {
        var axis = new Axis(new Proportion(3, -1), new Proportion(5, 1));

        Assert.True(axis.IsOrthogonalStructure);
        Assert.Equal(PinRelationMode.Orthogonal, axis.Relation.Mode);
        Assert.Null(axis.PreferredCarrierRank);
        Assert.Equal(PinLiftKind.OrthogonalBreakout, axis.PinResolution.RecessiveSide.LiftKind);
    }

    [Fact]
    public void PinAxisResolution_DerivesBehaviorAndRelationFromResolvedSides()
    {
        var resolution = new PinAxisResolution(
            new PinResolvedSide(PinSideRole.Recessive, 1, -2, 0, 1, PinLiftKind.None),
            new PinResolvedSide(PinSideRole.Dominant, 1, 3, 0, 1, PinLiftKind.None));

        Assert.Equal(PinBehaviorKind.SequentialReinforcement, resolution.Behavior);
        Assert.Equal(PinRelation.CollinearSame, resolution.Relation);
        Assert.Equal(0, resolution.SharedCarrierRank);
    }
}
