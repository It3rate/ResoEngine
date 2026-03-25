using Core3.Elements;

namespace Tests.Core3;

public sealed class Core3ElementTests
{
    [Fact]
    public void LongCarrier_UsesSideToInterpretValue()
    {
        var inbound = new LongCarrier(5, CarrierSide.Inbound);
        var outbound = new LongCarrier(5, CarrierSide.Outbound);

        Assert.Equal(-5, inbound.Value);
        Assert.Equal(5, outbound.Value);
    }

    [Fact]
    public void LongCarrier_Mirror_SwitchesSideWithoutChangingRawValue()
    {
        var outbound = new LongCarrier(7, CarrierSide.Outbound);

        var mirrored = outbound.Mirror();

        Assert.Equal(CarrierSide.Inbound, mirrored.Side);
        Assert.Equal(7, mirrored.RawValue);
        Assert.Equal(-7, mirrored.Value);
    }

    [Fact]
    public void LongCarrier_Subtract_PreservesCallerSide()
    {
        var start = new LongCarrier(10, CarrierSide.Inbound);
        var position = new LongCarrier(4, CarrierSide.Outbound);

        var difference = start.Subtract(position);

        Assert.Equal(CarrierSide.Inbound, difference.Side);
        Assert.Equal(6, difference.RawValue);
        Assert.Equal(-6, difference.Value);
    }

    [Fact]
    public void RawExtent_ExposesDualPerspectiveStartAndEnd()
    {
        var extent = new RawExtent(10, 20);

        Assert.Equal(CarrierSide.Inbound, extent.Start.Side);
        Assert.Equal(10, extent.Start.RawValue);
        Assert.Equal(-10, extent.Start.Value);

        Assert.Equal(CarrierSide.Outbound, extent.End.Side);
        Assert.Equal(20, extent.End.RawValue);
        Assert.Equal(20, extent.End.Value);
    }

    [Fact]
    public void RawExtent_Mirror_NegatesAndSwapsEndpoints()
    {
        var extent = new RawExtent(10, 20);

        var mirrored = Assert.IsType<RawExtent>(extent.Mirror());

        Assert.Equal(-20, mirrored.StartValue);
        Assert.Equal(-10, mirrored.EndValue);
    }

    [Fact]
    public void Proportion_DefaultsToZeroPinBootstrap()
    {
        var proportion = new Proportion(new RawExtent(-5, 10));

        Assert.Equal(5, proportion.Start.Value);
        Assert.Equal(10, proportion.End.Value);
        Assert.Equal(2m, (decimal)proportion.End.Value / proportion.Start.Value);
    }

    [Fact]
    public void Proportion_WithInteriorPin_UsesPinRelativeCarriers()
    {
        var proportion = new Proportion(new RawExtent(10, 20), 15);

        Assert.Equal(5, proportion.Start.Value);
        Assert.Equal(5, proportion.End.Value);
        Assert.Equal(1m, (decimal)proportion.End.Value / proportion.Start.Value);
    }

    [Fact]
    public void Proportion_Mirror_ReflectsAroundPinAndSwapsEndpoints()
    {
        var proportion = new Proportion(new RawExtent(10, 20), 12);

        var mirrored = Assert.IsType<Proportion>(proportion.Mirror());

        Assert.Equal(4, mirrored.Extent.StartValue);
        Assert.Equal(14, mirrored.Extent.EndValue);
        Assert.Equal(12, mirrored.PinPosition);
    }

    [Fact]
    public void Pin_ResolvesPositionOnSupport()
    {
        var pin = new Pin(
            new Proportion(new RawExtent(-5, 10)),
            new LongCarrier(10, CarrierSide.Inbound),
            new LongCarrier(20, CarrierSide.Outbound));

        Assert.Equal(30, pin.ResolvedPosition.RawValue);
        Assert.Equal(30, pin.ResolvedPosition.Value);
    }

    [Fact]
    public void Pin_ComputesInboundAndOutboundSpansRelativeToResolvedPosition()
    {
        var halfPosition = new Proportion(new RawExtent(-10, 5));
        var pin = new Pin(
            halfPosition,
            new LongCarrier(10, CarrierSide.Inbound),
            new LongCarrier(20, CarrierSide.Outbound));

        Assert.Equal(15, pin.ResolvedPosition.RawValue);
        Assert.Equal(-5, pin.InboundCarrier.RawValue);
        Assert.Equal(5, pin.InboundCarrier.Value);
        Assert.Equal(5, pin.OutboundCarrier.RawValue);
        Assert.Equal(5, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void Pin_CanBeBuiltFromElementConvenienceOverload()
    {
        var extent = new RawExtent(10, 20);
        var position = new Proportion(new RawExtent(-10, 5));

        var pin = new Pin(position, extent);

        Assert.Equal(15, pin.ResolvedPosition.RawValue);
        Assert.Equal(5, pin.InboundCarrier.Value);
        Assert.Equal(5, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void Pin_ToProportion_RebuildsPinRelativeMeasure()
    {
        var halfPosition = new Proportion(new RawExtent(-10, 5));
        var pin = new Pin(
            halfPosition,
            new LongCarrier(10, CarrierSide.Inbound),
            new LongCarrier(20, CarrierSide.Outbound));

        var rebuilt = pin.ToProportion();

        Assert.Equal(5, rebuilt.Start.Value);
        Assert.Equal(5, rebuilt.End.Value);
        Assert.Equal(1m, (decimal)rebuilt.End.Value / rebuilt.Start.Value);
    }
}
