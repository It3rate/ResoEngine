using Core3.Elements;
using Core3.Engine;
using Core3.Operations;
using Core3.Runtime;
using System.Numerics;

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
    public void EngineReference_CanReuseExistingCalibrationWithoutCopyingFrameOwnership()
    {
        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var subject = new AtomicElement(7, 1);

        var reference = new EngineReference(frame, subject);

        Assert.True(reference.TryMeasureOnCalibration(out var measured));

        var calibrated = Assert.IsType<CompositeElement>(measured);
        Assert.Equal(frame.Recessive, calibrated.Recessive);
        Assert.Equal(new AtomicElement(70, 10), calibrated.Dominant);
    }

    [Fact]
    public void AtomicAlignExact_UsesResolutionPolicy()
    {
        var host = new AtomicElement(1, 2);
        var applied = new AtomicElement(2, 4);

        Assert.True(host.TryAlignExact(applied, ResolutionPolicy.PreserveHost, out var hostPreserved, out var appliedCommitted));
        Assert.True(host.TryAlignExact(applied, ResolutionPolicy.PreserveApplied, out var hostCommitted, out var appliedPreserved));
        Assert.True(host.TryAlignExact(applied, ResolutionPolicy.ExactCommonFrame, out var commonLeft, out var commonRight));
        Assert.True(host.TryAlignExact(applied, ResolutionPolicy.ComposeSupport, out var composedLeft, out var composedRight));

        Assert.Equal(new AtomicElement(1, 2), Assert.IsType<AtomicElement>(hostPreserved));
        Assert.Equal(new AtomicElement(1, 2), Assert.IsType<AtomicElement>(appliedCommitted));

        Assert.Equal(new AtomicElement(2, 4), Assert.IsType<AtomicElement>(hostCommitted));
        Assert.Equal(new AtomicElement(2, 4), Assert.IsType<AtomicElement>(appliedPreserved));

        Assert.Equal(new AtomicElement(2, 4), Assert.IsType<AtomicElement>(commonLeft));
        Assert.Equal(new AtomicElement(2, 4), Assert.IsType<AtomicElement>(commonRight));

        Assert.Equal(new AtomicElement(4, 8), Assert.IsType<AtomicElement>(composedLeft));
        Assert.Equal(new AtomicElement(4, 8), Assert.IsType<AtomicElement>(composedRight));
    }

    [Fact]
    public void CompositeCommitToCalibration_RecursesChildwise()
    {
        var subject = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 12));
        var calibration = new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(10, 40));

        Assert.True(subject.TryCommitToCalibration(calibration, out var committed));

        var aligned = Assert.IsType<CompositeElement>(committed);
        Assert.Equal(new AtomicElement(2, 4), aligned.Recessive);
        Assert.Equal(new AtomicElement(10, 40), aligned.Dominant);
    }

    [Fact]
    public void EngineReference_UsesGenericGradedCommitPath()
    {
        var frame = new CompositeElement(
            new CompositeElement(
                new AtomicElement(2, 4),
                new AtomicElement(10, 40)),
            new CompositeElement(
                new AtomicElement(1, 1),
                new AtomicElement(3, 1)));
        var subject = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 12));

        var reference = new EngineReference(frame, subject);

        Assert.True(reference.TryMeasureOnCalibration(out var measured));

        var calibrated = Assert.IsType<CompositeElement>(measured);
        Assert.Equal(frame.Recessive, calibrated.Recessive);
        Assert.Equal(new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(10, 40)), calibrated.Dominant);
    }

    [Fact]
    public void EngineReference_CanReadSubjectIntoFrameWithoutBuildingMeasuredPair()
    {
        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var subject = new AtomicElement(7, 1);

        var reference = new EngineReference(frame, subject);

        Assert.True(reference.TryRead(out var read));
        Assert.Equal(new AtomicElement(70, 10), Assert.IsType<AtomicElement>(read));
    }

    [Fact]
    public void EngineReference_BoundaryAxis_UsesCalibrationAsRangeContext()
    {
        var frame = new CompositeElement(
            new AtomicElement(4, 4),
            new AtomicElement(0, 4));
        var inside = new EngineReference(frame, new AtomicElement(3, 4));
        var outside = new EngineReference(frame, new AtomicElement(7, 4));

        var insideAxis = inside.GetBoundaryAxis();
        var outsideAxis = outside.GetBoundaryAxis();

        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(0, 4)),
            insideAxis);
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(3, 4)),
            outsideAxis);
    }

    [Fact]
    public void EngineFamily_CanAddRemoveAndClearMembers()
    {
        var family = new EngineFamily(new AtomicElement(0, 4));
        var first = new AtomicElement(1, 2);
        var second = new AtomicElement(1, 4);

        family.AddMember(first);
        family.AddMember(second);

        Assert.Equal(2, family.Count);
        Assert.True(family.RemoveMember(first));
        Assert.Equal(1, family.Count);

        family.ClearMembers();

        Assert.Equal(0, family.Count);
    }

    [Fact]
    public void EngineFamily_CanCreateOrderedAndUnorderedViews()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 4);
        var second = new AtomicElement(2, 4);
        family.AddMember(first);
        family.AddMember(second);

        var unordered = family.CreateUnorderedCopy();
        var reordered = unordered.CreateOrderedCopy();

        Assert.False(unordered.IsOrdered);
        Assert.Same(family, unordered.ParentFamily);
        Assert.Equal(family.Members, unordered.Members);

        Assert.True(reordered.IsOrdered);
        Assert.Same(unordered, reordered.ParentFamily);
        Assert.Equal(unordered.Members, reordered.Members);
    }

    [Fact]
    public void EngineFamily_CanTemporarilyFocusMemberIntoFrameRole()
    {
        var family = new EngineFamily(new AtomicElement(4, 4), isOrdered: false);
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<EngineFamily>(focusedFamily);
        Assert.False(reframed.IsOrdered);
        Assert.Same(family, reframed.ParentFamily);
        Assert.Equal(1, reframed.ParentFocusIndex);
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(reframed.Frame));
        Assert.Equal(2, reframed.Count);
        Assert.Equal(first, reframed.Members[0]);
        Assert.Equal(third, reframed.Members[1]);

        Assert.True(reframed.TryAddAll(out var sum));
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineFamily_CanCollapseFocusedFamilyBackToParentFrame()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<EngineFamily>(focusedFamily);
        Assert.True(reframed.TryCollapseToParentFrame(out var collapsedFamily));

        var collapsed = Assert.IsType<EngineFamily>(collapsedFamily);
        Assert.Equal(family.Frame, collapsed.Frame);
        Assert.True(collapsed.IsOrdered);
        Assert.Null(collapsed.ParentFamily);
        Assert.Null(collapsed.ParentFocusIndex);
        Assert.Equal(3, collapsed.Count);
        Assert.Equal(first, collapsed.Members[0]);
        Assert.Equal(reframed.Frame, collapsed.Members[1]);
        Assert.Equal(third, collapsed.Members[2]);
    }

    [Fact]
    public void EngineFamily_CanSortMembersByFrameSlot()
    {
        var frame = new CompositeElement(
            new AtomicElement(0, 10),
            new AtomicElement(0, 10));
        var left = new CompositeElement(
            new AtomicElement(8, 10),
            new AtomicElement(1, 10));
        var middle = new CompositeElement(
            new AtomicElement(5, 10),
            new AtomicElement(5, 10));
        var right = new CompositeElement(
            new AtomicElement(2, 10),
            new AtomicElement(9, 10));
        var family = new EngineFamily(frame, isOrdered: false);
        family.AddMember(middle);
        family.AddMember(left);
        family.AddMember(right);

        Assert.True(family.TrySortByFrameSlot(0, descending: false, out var xSorted));
        Assert.True(family.TrySortByFrameSlot(1, descending: true, out var ySorted));

        var xAscending = Assert.IsType<EngineFamily>(xSorted);
        var yDescending = Assert.IsType<EngineFamily>(ySorted);

        Assert.True(xAscending.IsOrdered);
        Assert.Same(family, xAscending.ParentFamily);
        Assert.Equal([right, middle, left], xAscending.Members);

        Assert.True(yDescending.IsOrdered);
        Assert.Same(family, yDescending.ParentFamily);
        Assert.Equal([right, middle, left], yDescending.Members);
    }

    [Fact]
    public void EngineFamily_CanCreateDeterministicUnorderedShuffle()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 4);
        var second = new AtomicElement(2, 4);
        var third = new AtomicElement(3, 4);
        var fourth = new AtomicElement(4, 4);
        family.AddMember(first);
        family.AddMember(second);
        family.AddMember(third);
        family.AddMember(fourth);

        var shuffled = family.CreateShuffledCopy(seed: 7);

        Assert.False(shuffled.IsOrdered);
        Assert.Same(family, shuffled.ParentFamily);
        Assert.Equal(4, shuffled.Count);
        Assert.Equal([first, fourth, third, second], shuffled.Members);
    }

    [Fact]
    public void EngineFamily_CanReportMemberBoundaryAxis()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var insideCandidate = new AtomicElement(3, 4);
        var outsideCandidate = new AtomicElement(6, 4);

        var insideAxis = family.GetMemberBoundaryAxis(insideCandidate);
        var outsideAxis = family.GetMemberBoundaryAxis(outsideCandidate);

        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(0, 4)),
            insideAxis);
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(2, 4)),
            outsideAxis);
    }

    [Fact]
    public void EngineFamily_TryAddAll_UsesFrameReadouts()
    {
        var family = new EngineFamily(new AtomicElement(0, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(1, 4));

        Assert.True(family.TryAddAll(out var sum));
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineOperations_TryAdd_SupportsOneShotFramedAddition()
    {
        var frame = new AtomicElement(0, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 4)
        };

        Assert.True(EngineOperations.TryAdd(frame, members, out var sum));
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineOperations_TryAdd_CanRunDirectlyFromRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(0, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 4)
            ]);

        Assert.True(EngineOperations.TryAdd(context, out var sum));
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineOperations_TryAdd_CanReturnResultWithBoundaryAxisAndProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 2)
        };

        Assert.True(EngineOperations.TryAddWithProvenance(frame, members, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal("Add", operationResult.OperationName);
        Assert.Equal(frame, operationResult.SourceFrame);
        Assert.Equal(3, operationResult.SourceMembers.Count);
        Assert.Equal(new AtomicElement(5, 4), operationResult.Result);
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(1, 4)),
            operationResult.GetResultBoundaryAxis());
    }

    [Fact]
    public void EngineOperations_TryAddWithProvenance_PreservesRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 2)
            ],
            isOrdered: false);

        Assert.True(EngineOperations.TryAddWithProvenance(context, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal(context.Frame, operationResult.Context.Frame);
        Assert.Equal(context.IsOrdered, operationResult.Context.IsOrdered);
        Assert.Equal(context.Members, operationResult.Context.Members);
    }

    [Fact]
    public void EngineFamily_TryMultiplyAll_UsesFrameReadouts()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(3, 4));

        Assert.True(family.TryMultiplyAll(out var product));
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(product));
    }

    [Fact]
    public void EngineOperations_TryMultiply_SupportsOneShotFramedMultiplication()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(EngineOperations.TryMultiply(frame, members, out var product));
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(product));
    }

    [Fact]
    public void EngineOperations_TryMultiply_CanReturnResultWithDerivedFrameProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(EngineOperations.TryMultiplyWithProvenance(frame, members, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal("Multiply", operationResult.OperationName);
        Assert.Equal(frame, operationResult.SourceFrame);
        Assert.Equal(2, operationResult.SourceMembers.Count);
        Assert.Equal(new AtomicElement(6, 16), operationResult.Result);
        Assert.Equal(new AtomicElement(16, 16), Assert.IsType<AtomicElement>(operationResult.ResultFrame));
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 16), new AtomicElement(0, 16)),
            operationResult.GetResultBoundaryAxis());
    }

    [Fact]
    public void EngineOperations_TryBoolean_And_UsesSharedFrameOverlap()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var primary = CreateSegmentFrame(0, 10, 10);
        var secondary = CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.And,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.Single(booleanResult.Pieces);
        Assert.Equal(
            CreateSegmentFrame(3, 5, 10),
            booleanResult.Pieces[0].Result);
        Assert.Equal(primary, booleanResult.Pieces[0].Carrier);
        Assert.Equal([0, 1], booleanResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void EngineOperations_TryBoolean_Xor_PreservesTwoPieces()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var primary = CreateSegmentFrame(0, 10, 10);
        var secondary = CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.Xor,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryBoolean_NotSecondary_UsesFrameAsNegationCarrier()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var primary = CreateSegmentFrame(0, 10, 10);
        var secondary = CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.NotSecondary,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(frame, booleanResult.Pieces[0].Carrier);
        Assert.Equal(frame, booleanResult.Pieces[1].Carrier);
        Assert.Equal(CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_ExactlyOne_UsesSymmetricFamilyOccupancy()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var first = CreateSegmentFrame(0, 10, 10);
        var second = CreateSegmentFrame(3, 5, 10);
        var third = CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            frame,
            [first, second, third],
            EngineOccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.False(occupancyResult.IsOrdered);
        Assert.Equal(3, occupancyResult.Pieces.Count);
        Assert.Equal(CreateSegmentFrame(0, 3, 10), occupancyResult.Pieces[0].Result);
        Assert.Equal(CreateSegmentFrame(5, 6, 10), occupancyResult.Pieces[1].Result);
        Assert.Equal(CreateSegmentFrame(8, 10, 10), occupancyResult.Pieces[2].Result);
        Assert.Equal([0], occupancyResult.Pieces[0].SourceMemberIndices);
        Assert.Equal([0], occupancyResult.Pieces[1].SourceMemberIndices);
        Assert.Equal([0], occupancyResult.Pieces[2].SourceMemberIndices);
        Assert.Equal(first, occupancyResult.Pieces[0].Carrier);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_All_UsesFrameCarrierForCoPresentTruth()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var first = CreateSegmentFrame(0, 10, 10);
        var second = CreateSegmentFrame(0, 10, 10);
        var third = CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            frame,
            [first, second, third],
            EngineOccupancyOperation.All,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.Single(occupancyResult.Pieces);
        Assert.Equal(frame, occupancyResult.Pieces[0].Carrier);
        Assert.Equal(CreateSegmentFrame(3, 5, 10), occupancyResult.Pieces[0].Result);
        Assert.Equal([0, 1, 2], occupancyResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void EngineOperations_TryBooleanAdjacentPairs_UsesOrderedFamilyTraversal()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var first = CreateSegmentFrame(0, 10, 10);
        var second = CreateSegmentFrame(3, 5, 10);
        var third = CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryBooleanAdjacentPairs(
            frame,
            [first, second, third],
            EngineBooleanOperation.Xor,
            out var results));

        var pairwise = Assert.IsAssignableFrom<IReadOnlyList<EngineBooleanResult>>(results);
        Assert.Equal(2, pairwise.Count);

        Assert.Equal(CreateSegmentFrame(0, 3, 10), pairwise[0].Pieces[0].Result);
        Assert.Equal(CreateSegmentFrame(5, 10, 10), pairwise[0].Pieces[1].Result);
        Assert.Equal(CreateSegmentFrame(3, 5, 10), pairwise[1].Pieces[0].Result);
        Assert.Equal(CreateSegmentFrame(6, 8, 10), pairwise[1].Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_CanRunDirectlyFromRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            CreateSegmentFrame(0, 10, 10),
            [
                CreateSegmentFrame(0, 10, 10),
                CreateSegmentFrame(3, 5, 10),
                CreateSegmentFrame(6, 8, 10)
            ],
            isOrdered: false);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            context,
            EngineOccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.Equal(context.Frame, occupancyResult.Context.Frame);
        Assert.Equal(context.IsOrdered, occupancyResult.Context.IsOrdered);
        Assert.Equal(3, occupancyResult.Pieces.Count);
    }

    [Fact]
    public void EngineOperationContext_CanFocusSortShuffleAndCollapse()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(3, 4),
                new AtomicElement(1, 4)
            ],
            isOrdered: false);

        Assert.True(context.TryFocusMember(1, out var focused));

        var focusedContext = Assert.IsType<EngineOperationContext>(focused);
        Assert.Equal(new AtomicElement(3, 4), focusedContext.Frame);
        Assert.False(focusedContext.IsOrdered);
        Assert.NotNull(focusedContext.ParentContext);
        Assert.Equal(1, focusedContext.ParentFocusIndex);
        Assert.Equal(2, focusedContext.Count);

        Assert.True(focusedContext.TryCollapseToParentFrame(out var collapsed));

        var collapsedContext = Assert.IsType<EngineOperationContext>(collapsed);
        Assert.Equal(context.Frame, collapsedContext.Frame);
        Assert.False(collapsedContext.IsOrdered);
        Assert.Equal(3, collapsedContext.Count);
        Assert.Equal(focusedContext.Frame, collapsedContext.Members[1]);

        Assert.True(context.TrySortByFrameSlot(0, descending: false, out var sorted));

        var sortedContext = Assert.IsType<EngineOperationContext>(sorted);
        Assert.True(sortedContext.IsOrdered);
        Assert.Equal(
            [new AtomicElement(1, 4), new AtomicElement(1, 2), new AtomicElement(3, 4)],
            sortedContext.Members);

        var shuffled = context.CreateShuffledCopy(seed: 7);
        Assert.False(shuffled.IsOrdered);
        Assert.Equal(3, shuffled.Count);
        Assert.NotEqual(context.Members, shuffled.Members);
    }

    [Fact]
    public void EngineOperationContext_CanConvertBetweenOrderedAndUnorderedViews()
    {
        var ordered = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [new AtomicElement(1, 4), new AtomicElement(2, 4)],
            isOrdered: true);

        var unordered = ordered.AsUnordered();
        var reordered = unordered.AsOrdered();

        Assert.True(ordered.IsOrdered);
        Assert.False(unordered.IsOrdered);
        Assert.True(reordered.IsOrdered);
        Assert.Equal(ordered.Frame, unordered.Frame);
        Assert.Equal(ordered.Members, unordered.Members);
        Assert.Equal(unordered.Members, reordered.Members);
    }

    [Fact]
    public void EngineFamily_TryBooleanAdjacentPairs_RequiresOrderedFamily()
    {
        var family = new EngineFamily(CreateSegmentFrame(0, 10, 10), isOrdered: false);
        family.AddMember(CreateSegmentFrame(0, 10, 10));
        family.AddMember(CreateSegmentFrame(3, 5, 10));

        Assert.False(family.TryBooleanAdjacentPairs(EngineBooleanOperation.And, out _));
    }

    [Fact]
    public void EngineFamily_TryBoolean_CurrentlyRequiresExactlyTwoMembers()
    {
        var frame = CreateSegmentFrame(0, 10, 10);
        var family = new EngineFamily(frame);
        family.AddMember(CreateSegmentFrame(0, 10, 10));
        family.AddMember(CreateSegmentFrame(3, 5, 10));
        family.AddMember(CreateSegmentFrame(6, 8, 10));

        Assert.False(family.TryBoolean(EngineBooleanOperation.Or, out _));
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

    [Theory]
    [InlineData(2, 1, 3, 1, 4, 1, 5, 1)]
    [InlineData(3, 2, 5, 2, 7, 2, 1, 2)]
    [InlineData(-1, 4, 3, 4, 5, 4, -1, 4)]
    public void GradeTwoAlignedMultiply_MatchesCSharpComplexNumbers(
        long leftRecessiveValue,
        long leftRecessiveResolution,
        long leftDominantValue,
        long leftDominantResolution,
        long rightRecessiveValue,
        long rightRecessiveResolution,
        long rightDominantValue,
        long rightDominantResolution)
    {
        var left = CreateAxisLikeNumber(
            leftRecessiveValue,
            leftRecessiveResolution,
            leftDominantValue,
            leftDominantResolution);
        var right = CreateAxisLikeNumber(
            rightRecessiveValue,
            rightRecessiveResolution,
            rightDominantValue,
            rightDominantResolution);
        var expected = new Complex(
            (double)leftRecessiveValue / leftRecessiveResolution,
            (double)leftDominantValue / leftDominantResolution) *
            new Complex(
                (double)rightRecessiveValue / rightRecessiveResolution,
                (double)rightDominantValue / rightDominantResolution);

        Assert.True(left.TryMultiply(right, out var product));

        var reduced = Assert.IsType<CompositeElement>(product);
        Assert.Equal(1, reduced.Grade);

        var recessive = Assert.IsType<AtomicElement>(reduced.Recessive);
        var dominant = Assert.IsType<AtomicElement>(reduced.Dominant);

        Assert.Equal(expected.Real, ToDouble(recessive), 12);
        Assert.Equal(expected.Imaginary, ToDouble(dominant), 12);
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

    private static CompositeElement CreateAxisLikeNumber(
        long recessiveValue,
        long recessiveResolution,
        long dominantValue,
        long dominantResolution) =>
        new(
            CreateExactScalar(recessiveValue, recessiveResolution),
            CreateExactScalar(dominantValue, dominantResolution));

    private static CompositeElement CreateExactScalar(long value, long resolution) =>
        new(
            new AtomicElement(resolution, 1),
            new AtomicElement(value, 1));

    private static CompositeElement CreateSegmentFrame(long start, long end, long resolution) =>
        new(
            new AtomicElement(start, resolution),
            new AtomicElement(end, resolution));

    private static double ToDouble(AtomicElement atomic) =>
        (double)atomic.Value / atomic.Unit;
}
