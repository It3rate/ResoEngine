using Core3.Elements;
using Core3.Engine;

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
    public void Elements_ExposeCurrentGradeLadder()
    {
        Assert.Equal(0, new RawExtent(10, 20).Grade);
        Assert.Equal(1, new Proportion(new RawExtent(-5, 10)).Grade);
        Assert.Equal(2, new Axis(
            new LongCarrier(-5, CarrierSide.Inbound),
            new LongCarrier(10, CarrierSide.Outbound)).Grade);
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
    public void Proportion_CanBeBuiltFromPinReadout()
    {
        var halfPosition = new Proportion(new RawExtent(-10, 5));
        var pin = new Pin(
            halfPosition,
            new LongCarrier(10, CarrierSide.Inbound),
            new LongCarrier(20, CarrierSide.Outbound));

        var rebuilt = new Proportion(pin);

        Assert.Equal(5, rebuilt.Start.Value);
        Assert.Equal(5, rebuilt.End.Value);
        Assert.Equal(1m, (decimal)rebuilt.End.Value / rebuilt.Start.Value);
    }

    [Fact]
    public void Axis_NormalizesToInboundStartAndOutboundEnd()
    {
        var axis = new Axis(
            new LongCarrier(-5, CarrierSide.Outbound),
            new LongCarrier(10, CarrierSide.Inbound));

        Assert.Equal(CarrierSide.Inbound, axis.Start.Side);
        Assert.Equal(-5, axis.Start.RawValue);
        Assert.Equal(5, axis.Start.Value);

        Assert.Equal(CarrierSide.Outbound, axis.End.Side);
        Assert.Equal(10, axis.End.RawValue);
        Assert.Equal(10, axis.End.Value);
    }

    [Fact]
    public void Axis_CanBeBuiltFromPinReadout()
    {
        var halfPosition = new Proportion(new RawExtent(-10, 5));
        var pin = new Pin(
            halfPosition,
            new LongCarrier(10, CarrierSide.Inbound),
            new LongCarrier(20, CarrierSide.Outbound));

        var axis = new Axis(pin);

        Assert.Equal(pin.InboundCarrier.Side, axis.Start.Side);
        Assert.Equal(pin.InboundCarrier.RawValue, axis.Start.RawValue);
        Assert.Equal(pin.InboundCarrier.Value, axis.Start.Value);

        Assert.Equal(pin.OutboundCarrier.Side, axis.End.Side);
        Assert.Equal(pin.OutboundCarrier.RawValue, axis.End.RawValue);
        Assert.Equal(pin.OutboundCarrier.Value, axis.End.Value);
    }

    [Fact]
    public void Axis_Mirror_NegatesAndSwapsCarrierCoordinates()
    {
        var axis = new Axis(
            new LongCarrier(-5, CarrierSide.Inbound),
            new LongCarrier(10, CarrierSide.Outbound));

        var mirrored = Assert.IsType<Axis>(axis.Mirror());

        Assert.Equal(CarrierSide.Inbound, mirrored.Start.Side);
        Assert.Equal(-10, mirrored.Start.RawValue);
        Assert.Equal(10, mirrored.Start.Value);

        Assert.Equal(CarrierSide.Outbound, mirrored.End.Side);
        Assert.Equal(5, mirrored.End.RawValue);
        Assert.Equal(5, mirrored.End.Value);
    }

    [Fact]
    public void Axis_At_UsesCarrierCoordinatesForPinResolution()
    {
        var axis = new Axis(
            new LongCarrier(-5, CarrierSide.Inbound),
            new LongCarrier(15, CarrierSide.Outbound));

        var pin = axis.At(new Proportion(new RawExtent(-10, 5)));

        Assert.Equal(5, pin.ResolvedPosition.RawValue);
        Assert.Equal(5, pin.ResolvedPosition.Value);
        Assert.Equal(-10, pin.InboundCarrier.RawValue);
        Assert.Equal(10, pin.InboundCarrier.Value);
        Assert.Equal(10, pin.OutboundCarrier.RawValue);
        Assert.Equal(10, pin.OutboundCarrier.Value);
    }

    [Fact]
    public void CompositeFold_PreservesCarrierPolarityFromAtomicResult()
    {
        var orthogonalRatio = new CompositeElement(
            new AtomicElement(2, -1),
            new AtomicElement(1, -1));
        var reversedRatio = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(-1, 1));

        Assert.True(orthogonalRatio.TryFold(out var orthogonalFolded));
        Assert.True(reversedRatio.TryFold(out var reversedFolded));

        var orthogonal = Assert.IsType<AtomicElement>(orthogonalFolded);
        var reversed = Assert.IsType<AtomicElement>(reversedFolded);

        Assert.True(orthogonal.IsOrthogonalUnit);
        Assert.False(reversed.IsOrthogonalUnit);
        Assert.Equal(-2, orthogonal.Unit);
        Assert.Equal(-1, reversed.Value);
    }

    [Fact]
    public void CompositeFold_KeepsValueSignSeparateFromUnitPolarity()
    {
        var ratio = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(-3, 1));

        Assert.True(ratio.TryFold(out var folded));

        var atomic = Assert.IsType<AtomicElement>(folded);
        Assert.Equal(-3, atomic.Value);
        Assert.Equal(2, atomic.Unit);
        Assert.True(atomic.IsAlignedUnit);
    }

    [Fact]
    public void CompositeFold_RejectsContrastCarrierChildren()
    {
        var contrastive = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));

        Assert.False(contrastive.TryFold(out _));
    }

    [Fact]
    public void HigherGradeFold_StaysInsideElementSpace()
    {
        var gradeTwo = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(3, 1)),
            new CompositeElement(
                new AtomicElement(8, 1),
                new AtomicElement(2, 1)));

        Assert.True(gradeTwo.TryFold(out var folded));

        var lowered = Assert.IsType<CompositeElement>(folded);
        Assert.Equal(1, lowered.Grade);
        Assert.Equal(new AtomicElement(3, 10), lowered.Recessive);
        Assert.Equal(new AtomicElement(2, 8), lowered.Dominant);
    }

    [Fact]
    public void EnginePin_CanResolveHostedPositionFromAlignedRatioBearingElement()
    {
        var host = new CompositeElement(
            new AtomicElement(0, 1),
            new AtomicElement(10, 1));
        var ratioPosition = new CompositeElement(
            new AtomicElement(10, 1),
            new AtomicElement(3, 1));

        var pin = new EnginePin(host, ratioPosition);

        Assert.Equal(new AtomicElement(30, 10), pin.ResolvedPosition);
        Assert.Equal(new AtomicElement(30, 10), pin.Inbound);
        Assert.Equal(new AtomicElement(70, 10), pin.Outbound);
    }

    [Fact]
    public void AtomicScale_PreservesExactWorkingSupportWithoutReducing()
    {
        var whole = new AtomicElement(10, 1);
        var tenth = new AtomicElement(3, 10);
        var balanced = new AtomicElement(10, 10);

        Assert.True(whole.TryScale(tenth, out var scaledWhole));
        Assert.True(balanced.TryScale(new AtomicElement(10, 1), out var scaledValue));
        Assert.True(balanced.TryScale(new AtomicElement(1, 10), out var scaledSupport));

        Assert.Equal(new AtomicElement(30, 10), Assert.IsType<AtomicElement>(scaledWhole));
        Assert.Equal(new AtomicElement(100, 10), Assert.IsType<AtomicElement>(scaledValue));
        Assert.Equal(new AtomicElement(10, 100), Assert.IsType<AtomicElement>(scaledSupport));
    }

    [Fact]
    public void AtomicAdd_AlignsExactSupportBeforeCombining()
    {
        var half = new AtomicElement(1, 2);
        var quarter = new AtomicElement(1, 4);

        Assert.True(half.TryAdd(quarter, out var sum));
        Assert.True(half.TrySubtract(quarter, out var difference));

        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sum));
        Assert.Equal(new AtomicElement(1, 4), Assert.IsType<AtomicElement>(difference));
    }

    [Fact]
    public void AtomicCommitToSupport_ReexpressesExactlyWithoutChangingFoldedValue()
    {
        var ratio = new AtomicElement(3, 10);

        Assert.True(ratio.TryCommitToSupport(100, out var committed));
        Assert.True(ratio.TryCommitToSupport(50, out var refined));
        Assert.False(ratio.TryCommitToSupport(6, out _));

        Assert.Equal(new AtomicElement(30, 100), committed);
        Assert.Equal(new AtomicElement(15, 50), refined);
        Assert.Equal(new AtomicElement(3, 10), ratio);
    }

    [Fact]
    public void AtomicMultiply_PreservesOrthogonalCarrierWhenEitherFactorIsOrthogonal()
    {
        var aligned = new AtomicElement(2, 3);
        var orthogonal = new AtomicElement(4, -5);
        var orthogonalAgain = new AtomicElement(6, -7);

        Assert.True(aligned.TryMultiply(orthogonal, out var contrastProduct));
        Assert.True(orthogonal.TryMultiply(orthogonalAgain, out var orthogonalProduct));

        var contrast = Assert.IsType<AtomicElement>(contrastProduct);
        var orthogonalSquare = Assert.IsType<AtomicElement>(orthogonalProduct);

        Assert.Equal(8, contrast.Value);
        Assert.Equal(-15, contrast.Unit);
        Assert.True(contrast.IsOrthogonalUnit);

        Assert.Equal(24, orthogonalSquare.Value);
        Assert.Equal(-35, orthogonalSquare.Unit);
        Assert.True(orthogonalSquare.IsOrthogonalUnit);
    }

    [Fact]
    public void GradeOneCompositeMultiply_UsesFoldFirstExactMultiplication()
    {
        var left = new CompositeElement(
            new AtomicElement(10, 1),
            new AtomicElement(3, 1));
        var right = new CompositeElement(
            new AtomicElement(8, 1),
            new AtomicElement(2, 1));

        Assert.True(left.TryMultiply(right, out var product));

        var atomic = Assert.IsType<AtomicElement>(product);
        Assert.Equal(6, atomic.Value);
        Assert.Equal(80, atomic.Unit);
    }

    [Fact]
    public void GradeTwoCompositeMultiply_ReducesAlignedKernelLikeComplexPairing()
    {
        var left = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(2, 1)),
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(3, 1)));
        var right = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(4, 1)),
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(5, 1)));

        Assert.True(left.TryMultiply(right, out var product));

        var reduced = Assert.IsType<CompositeElement>(product);
        Assert.Equal(1, reduced.Grade);
        Assert.Equal(new AtomicElement(-7, 1), reduced.Recessive);
        Assert.Equal(new AtomicElement(22, 1), reduced.Dominant);
    }

    [Fact]
    public void GradeTwoCompositeMultiply_PreservesRawKernelWhenOrthogonalityPreventsReduction()
    {
        var left = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(2, 1)),
            new CompositeElement(new AtomicElement(1, -1), new AtomicElement(3, -1)));
        var right = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(4, 1)),
            new CompositeElement(new AtomicElement(1, -1), new AtomicElement(5, -1)));

        Assert.True(left.TryMultiply(right, out var product));

        var kernel = Assert.IsType<CompositeElement>(product);
        Assert.Equal(2, kernel.Grade);

        var squares = Assert.IsType<CompositeElement>(kernel.Recessive);
        var cross = Assert.IsType<CompositeElement>(kernel.Dominant);

        Assert.Equal(new AtomicElement(8, 1), squares.Recessive);
        Assert.Equal(new AtomicElement(15, -1), squares.Dominant);
        Assert.Equal(new AtomicElement(10, -1), cross.Recessive);
        Assert.Equal(new AtomicElement(12, -1), cross.Dominant);
    }

    [Fact]
    public void EnginePin_Multiply_IsNaturalOnlyForContrastSpace()
    {
        var contrastPin = new EnginePin(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));
        var sameSpacePin = new EnginePin(
            new AtomicElement(2, 1),
            new AtomicElement(3, 1));

        var contrastProduct = contrastPin.Multiply();
        var sameSpaceProduct = sameSpacePin.Multiply();

        Assert.IsType<CompositeElement>(contrastProduct);
        Assert.Null(sameSpaceProduct);
        Assert.True(sameSpacePin.MultiplyRequiresLift());
    }
}
