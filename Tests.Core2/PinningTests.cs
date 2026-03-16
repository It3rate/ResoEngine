using Core2.Elements;

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

        Assert.Equal(PinRelation.OrthogonalDirect, areaPin.Relation);
        Assert.Equal(PinRelationMode.Orthogonal, areaPin.Relation.Mode);
        Assert.Equal(PinHandednessMode.Direct, areaPin.Relation.Handedness);
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
}
