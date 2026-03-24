using Core3.Elements;

namespace Tests.Core2;

public class Core3PrimitiveTests
{
    private static Proportion Ratio(long outbound, long inbound) => new(new RawExtent(-inbound, outbound));

    [Fact]
    public void InboundCarrier_FlipsRawValue_ForInboundSemantics()
    {
        var inbound = new InboundCarrier(-5);

        Assert.Equal(5, inbound.Value);
        Assert.True(inbound.IsPositive);
        Assert.False(inbound.IsNegative);
        Assert.Equal(-5, inbound.AsOutbound().Value);
        Assert.Equal(-5, ((OutboundCarrier)inbound).Value);
    }

    [Fact]
    public void OutboundCarrier_PreservesRawValue_AndCanBeViewedAsInbound()
    {
        var outbound = new OutboundCarrier(5);

        Assert.Equal(5, outbound.Value);
        Assert.True(outbound.IsPositive);
        Assert.False(outbound.IsNegative);
        Assert.Equal(-5, outbound.AsInbound().Value);
        Assert.Equal(-5, ((InboundCarrier)outbound).Value);
    }

    [Theory]
    [InlineData(-5, 10, 15)]
    [InlineData(10, 20, 10)]
    [InlineData(20, 10, 10)]
    [InlineData(100, 90, 10)]
    public void RawExtent_Reverse_PreservesMagnitude(long start, long end, long expectedMagnitude)
    {
        var extent = new RawExtent(start, end);
        var reversed = extent.Reverse();

        Assert.Equal(expectedMagnitude, extent.Magnitude);
        Assert.Equal(expectedMagnitude, reversed.Magnitude);
        Assert.Equal(extent.StartValue, reversed.EndValue);
        Assert.Equal(extent.EndValue, reversed.StartValue);
    }

    [Fact]
    public void RawExtent_ToProportion_UsesImplicitZeroPinBootstrap()
    {
        var proportion = new RawExtent(-5, 10).ToProportion();

        Assert.Equal(5, proportion.Start.Value);
        Assert.Equal(10, proportion.End.Value);
        Assert.False(proportion.IsDegenerate);
        Assert.Equal("10/5", proportion.ToString());
    }

    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(1, 4, 2)]
    [InlineData(1, 2, 4)]
    [InlineData(3, 4, 6)]
    [InlineData(1, 1, 8)]
    [InlineData(3, 1, 24)]
    public void Proportion_GetPositionOn_ResolvesExpectedPositions_OnEightTickExtent(
        long outbound,
        long inbound,
        long expectedPosition)
    {
        var proportion = Ratio(outbound, inbound);
        var position = proportion.GetPositionOn(new RawExtent(0, 8));

        Assert.Equal(expectedPosition, position.Value);
    }

    [Fact]
    public void Pin_OnForwardExtent_ReadsBalancedMidpointCarriers()
    {
        var pin = new RawExtent(10, 20).At(Ratio(1, 2));

        Assert.Equal(15, pin.ResolvedPosition.Value);
        Assert.Equal(5, pin.InboundCarrier.Value);
        Assert.Equal(5, pin.OutboundCarrier.Value);
    }

    [Theory]
    [InlineData(0, 1, 10, 0, 10)]
    [InlineData(1, 1, 20, 10, 0)]
    [InlineData(1, 4, 12, 2, 8)]
    [InlineData(3, 4, 18, 8, 2)]
    public void Pin_OnForwardExtent_TracksEndpointAndQuarterCases_WithoutOffByOne(
        long outbound,
        long inbound,
        long expectedPosition,
        long expectedInbound,
        long expectedOutbound)
    {
        var pin = new RawExtent(10, 20).At(Ratio(outbound, inbound));

        Assert.Equal(expectedPosition, pin.ResolvedPosition.Value);
        Assert.Equal(expectedInbound, pin.InboundCarrier.Value);
        Assert.Equal(expectedOutbound, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void Pin_OutsideForwardExtent_ReadsPositiveInboundAndNegativeOutbound()
    {
        var pin = new RawExtent(10, 20).At(Ratio(49, 1));

        Assert.Equal(500, pin.ResolvedPosition.Value);
        Assert.Equal(490, pin.InboundCarrier.Value);
        Assert.Equal(-480, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void Pin_OnReversedExtent_ReadsNegativeInboundAndNegativeOutbound()
    {
        var pin = new RawExtent(20, 10).At(Ratio(1, 2));

        Assert.Equal(15, pin.ResolvedPosition.Value);
        Assert.Equal(-5, pin.InboundCarrier.Value);
        Assert.Equal(-5, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void CarrierReferences_MatchDirectCarrierValues()
    {
        var pin = new RawExtent(10, 20).At(Ratio(1, 2));

        Assert.Equal(pin.InboundCarrier.Value, pin.InboundCarrierRef.Value);
        Assert.Equal(pin.OutboundCarrier.Value, pin.OutboundCarrierRef.Value);
        Assert.Equal(pin.InboundCarrier.AsOutbound().Value, pin.InboundCarrierRef.AsOutbound().Value);
        Assert.Equal(pin.OutboundCarrier.AsInbound().Value, pin.OutboundCarrierRef.AsInbound().Value);
    }

    [Fact]
    public void Pin_ToProportion_RoundTripsLiveCarrierValues()
    {
        var pin = new RawExtent(10, 20).At(Ratio(1, 2));
        var proportion = pin.ToProportion();

        Assert.Equal(pin.InboundCarrier.Value, proportion.InboundCarrier.Value);
        Assert.Equal(pin.OutboundCarrier.Value, proportion.OutboundCarrier.Value);
        Assert.Equal("5/5", proportion.ToString());
    }

    [Fact]
    public void Proportion_ImplementsElementSurface()
    {
        var proportion = Ratio(10, 5);

        Assert.Equal(5, proportion.Start.Value);
        Assert.Equal(10, proportion.End.Value);
        Assert.Equal(5, proportion.InboundCarrier.Value);
        Assert.Equal(10, proportion.OutboundCarrier.Value);
    }

    [Fact]
    public void Proportion_CanActAsAnElement_ForNestedPinning()
    {
        var baseElement = new Proportion(new RawExtent(-2, 4));
        var selector = Ratio(1, 2);
        var pin = new Pin(selector, baseElement);

        Assert.Equal(1, selector.GetPositionOn(baseElement).Value);
        Assert.Equal(1, pin.ResolvedPosition.Value);
        Assert.Equal(3, pin.InboundCarrier.Value);
        Assert.Equal(3, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void DegenerateProportion_IsDetected()
    {
        var proportion = new Proportion(new RawExtent(0, 0));

        Assert.True(proportion.IsDegenerate);
        Assert.True(proportion.Start.IsZero);
        Assert.True(proportion.End.IsZero);
    }

    [Fact]
    public void RawExtent_StartAndEndExposeTypedCarrierSides()
    {
        var extent = new RawExtent(-5, 10);

        Assert.Equal(5, extent.Start.Value);
        Assert.Equal(10, extent.End.Value);
    }

    [Fact]
    public void PinnedResolvedPosition_IsAnOutboundCarrierView()
    {
        var pin = new RawExtent(10, 20).At(Ratio(1, 2));

        Assert.Equal(15, pin.ResolvedPosition.Value);
        Assert.Equal(-15, pin.ResolvedPosition.AsInbound().Value);
    }

    [Theory]
    [InlineData(-5, 10, 5, 10)]
    [InlineData(10, 20, -10, 20)]
    [InlineData(20, 10, -20, 10)]
    public void RawExtent_StartEndSurface_MatchesStoredOrderThroughCarrierSemantics(
        long start,
        long end,
        long expectedStartValue,
        long expectedEndValue)
    {
        var extent = new RawExtent(start, end);

        Assert.Equal(expectedStartValue, extent.Start.Value);
        Assert.Equal(expectedEndValue, extent.End.Value);
    }
}
