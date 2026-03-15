using Core2.Elements;
using Core2.Resolution;
using Core2.Units;

namespace Tests.Core2;

public class ResolutionTests
{
    private static readonly PhysicalReferent DistanceReferent = new("distance", "Distance");
    private static readonly UnitGenerator Length = new("length", "L", DistanceReferent);
    private static readonly UnitSignature LengthSignature = UnitSignature.From(Length);
    private static readonly UnitChoice Mile = new("mile", "mi", LengthSignature, Scalar.One, DistanceReferent);
    private static readonly UnitChoice HundredMiles = new("hundred-mile", "100mi", LengthSignature, new Scalar(100m), DistanceReferent);

    [Fact]
    public void ResolutionLadder_DecomposesQuantityAcrossFrames()
    {
        var ladder = new ResolutionLadder(
        [
            ResolutionFrame.FromUnitChoice(HundredMiles),
            ResolutionFrame.FromUnitChoice(Mile),
        ]);

        var quantity = new Scalar(109m).AsQuantity(LengthSignature, Mile);

        var layered = quantity.ToLayered(ladder);

        Assert.Collection(
            layered.Components,
            component =>
            {
                Assert.Equal("100mi", component.Frame.Symbol);
                Assert.Equal(new Scalar(1m), component.Count);
            },
            component =>
            {
                Assert.Equal("mi", component.Frame.Symbol);
                Assert.Equal(new Scalar(9m), component.Count);
            });
        Assert.Equal(new Scalar(109m), layered.Fold().Value);
    }

    [Fact]
    public void LayeredQuantity_AllowsSignedDigits()
    {
        var signature = UnitSignature.Dimensionless;
        var hundreds = new ResolutionFrame("hundreds", "H", signature, new Scalar(100m));
        var tens = new ResolutionFrame("tens", "T", signature, new Scalar(10m));
        var ones = new ResolutionFrame("ones", "O", signature, Scalar.One);

        var layered = new LayeredQuantity(
        [
            new ResolutionComponent(hundreds, new Scalar(1m)),
            new ResolutionComponent(tens, Scalar.Zero),
            new ResolutionComponent(ones, new Scalar(-2m)),
        ],
        signature);

        Assert.Equal(new Scalar(98m), layered.Fold().Value);
    }

    [Fact]
    public void ReadAt_CoarseFrameProducesRepresentativeAndResidual()
    {
        var ladder = new ResolutionLadder(
        [
            ResolutionFrame.FromUnitChoice(HundredMiles),
            ResolutionFrame.FromUnitChoice(Mile),
        ]);

        var quantity = new Scalar(109m).AsQuantity(LengthSignature, Mile);
        var layered = quantity.ToLayered(ladder);

        var reading = layered.ReadAt(ResolutionFrame.FromUnitChoice(HundredMiles));

        Assert.Equal(new Scalar(1.09m), reading.RawTickCount);
        Assert.Equal(new Scalar(1m), reading.TickCount);
        Assert.Equal(new Scalar(100m), reading.Representative.Value);
        Assert.Equal(new Scalar(9m), reading.Residual.Value);
    }

    [Fact]
    public void FineDetailCollapsesToZeroAtCoarseRead()
    {
        var hundredFrame = ResolutionFrame.FromUnitChoice(HundredMiles);
        var mileFrame = ResolutionFrame.FromUnitChoice(Mile);

        var layered = new LayeredQuantity(
        [
            new ResolutionComponent(hundredFrame, new Scalar(1m)),
            new ResolutionComponent(mileFrame, new Scalar(1m)),
        ],
        LengthSignature,
        Mile);

        var reading = layered.ReadAt(hundredFrame);

        Assert.Equal(new Scalar(101m), reading.ExactQuantity.Value);
        Assert.Equal(new Scalar(1.01m), reading.RawTickCount);
        Assert.Equal(new Scalar(1m), reading.TickCount);
        Assert.Equal(new Scalar(100m), reading.Representative.Value);
        Assert.Equal(new Scalar(1m), reading.Residual.Value);
    }

    [Fact]
    public void ReadAt_FinerFrameRestoresDetailedCount()
    {
        var hundredFrame = ResolutionFrame.FromUnitChoice(HundredMiles);
        var mileFrame = ResolutionFrame.FromUnitChoice(Mile);

        var layered = new LayeredQuantity(
        [
            new ResolutionComponent(hundredFrame, new Scalar(1m)),
            new ResolutionComponent(mileFrame, new Scalar(1m)),
        ],
        LengthSignature,
        Mile);

        var reading = layered.ReadAt(mileFrame);

        Assert.Equal(new Scalar(101m), reading.TickCount);
        Assert.Equal(new Scalar(101m), reading.Representative.Value);
        Assert.True(reading.IsExact);
    }

    [Fact]
    public void CollapseTo_ReplacesDetailWithChosenResolution()
    {
        var hundredFrame = ResolutionFrame.FromUnitChoice(HundredMiles);
        var mileFrame = ResolutionFrame.FromUnitChoice(Mile);

        var layered = new LayeredQuantity(
        [
            new ResolutionComponent(hundredFrame, new Scalar(1m)),
            new ResolutionComponent(mileFrame, new Scalar(9m)),
        ],
        LengthSignature,
        Mile);

        var collapsed = layered.CollapseTo(hundredFrame);

        Assert.Single(collapsed.Components);
        Assert.Equal(new Scalar(1m), collapsed.Components[0].Count);
        Assert.Equal(new Scalar(100m), collapsed.Fold().Value);
    }
}
