using Core3.Engine;
using System.Numerics;

namespace Tests.Core3;

public sealed class EngineTests
{
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
    public void CompositeFold_PreservesContrastCarrierAsUnresolvedAtomicResult()
    {
        var contrastive = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));

        var outcome = contrastive.Fold();

        Assert.True(contrastive.TryFold(out var folded));
        Assert.False(outcome.IsExact);
        Assert.Equal(
            new CompositeElement(
                new AtomicElement(2, 1),
                new AtomicElement(3, -1)),
            outcome.Tension);

        var atomic = Assert.IsType<AtomicElement>(folded);
        Assert.Equal(new AtomicElement(3, 0), atomic);
        Assert.True(atomic.IsUnresolvedUnit);
    }

    [Fact]
    public void CompositeFold_PreservesZeroDenominatorAsUnresolvedAtomicResult()
    {
        var zeroLikeRatio = new CompositeElement(
            new AtomicElement(0, 10),
            new AtomicElement(3, 10));

        var outcome = zeroLikeRatio.Fold();

        Assert.True(zeroLikeRatio.TryFold(out var folded));
        Assert.False(outcome.IsExact);
        Assert.Equal(zeroLikeRatio, outcome.Tension);

        var atomic = Assert.IsType<AtomicElement>(folded);
        Assert.Equal(new AtomicElement(30, 0), atomic);
        Assert.True(atomic.IsUnresolvedUnit);
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
    public void HigherGradeFold_CarriesChildTensionForwardWhileStillLoweringGrade()
    {
        var gradeTwo = new CompositeElement(
            new CompositeElement(
                new AtomicElement(2, 1),
                new AtomicElement(3, -1)),
            new CompositeElement(
                new AtomicElement(8, 1),
                new AtomicElement(2, 1)));

        var outcome = gradeTwo.Fold();

        Assert.False(outcome.IsExact);

        var lowered = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(1, lowered.Grade);
        Assert.Equal(new AtomicElement(3, 0), lowered.Recessive);
        Assert.Equal(new AtomicElement(2, 8), lowered.Dominant);
        Assert.Equal(gradeTwo, outcome.Tension);
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
    public void EnginePin_ResolveHostedWithTension_PreservesUnresolvedHostedPlacement()
    {
        var host = new CompositeElement(
            new AtomicElement(0, 1),
            new AtomicElement(10, 1));
        var contrastiveRatio = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));

        var outcome = EnginePin.ResolveHostedWithTension(host, contrastiveRatio);

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(3, 0), Assert.IsType<AtomicElement>(outcome.ResolvedPosition));
        Assert.Equal(new AtomicElement(3, 0), Assert.IsType<AtomicElement>(outcome.Inbound));
        Assert.Equal(new AtomicElement(7, 0), Assert.IsType<AtomicElement>(outcome.Outbound));
        Assert.Equal(contrastiveRatio, outcome.Tension);
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
    public void EngineReference_Read_PreservesUnresolvedBorrowedRead()
    {
        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var subject = new AtomicElement(7, 0);

        var reference = new EngineReference(frame, subject);
        var outcome = reference.Read();

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(70, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(subject, frame.Recessive), outcome.Tension);
    }

    [Fact]
    public void EngineReference_MeasureOnCalibration_PreservesUnresolvedBorrowedRead()
    {
        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var subject = new AtomicElement(7, 0);

        var reference = new EngineReference(frame, subject);
        var outcome = reference.MeasureOnCalibration();

        Assert.False(outcome.IsExact);

        var measured = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(frame.Recessive, measured.Recessive);
        Assert.Equal(new AtomicElement(70, 0), measured.Dominant);
        Assert.Equal(new CompositeElement(subject, frame.Recessive), outcome.Tension);
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
    public void AtomicReexpressToSupportWithTension_PreservesUnresolvedProjectionWhenSupportDoesNotDivide()
    {
        var ratio = new AtomicElement(3, 10);

        var outcome = ratio.ReexpressToSupportWithTension(6);

        Assert.False(outcome.IsExact);
        Assert.Equal(
            new AtomicElement(30, 0),
            Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(
            new CompositeElement(
                new AtomicElement(3, 10),
                new AtomicElement(6, 1)),
            outcome.Tension);
    }

    [Fact]
    public void AtomicCommitToCalibrationWithTension_PreservesCarrierContrastAsUnresolvedRead()
    {
        var subject = new AtomicElement(3, 1);
        var calibration = new AtomicElement(4, -4);

        var outcome = subject.CommitToCalibrationWithTension(calibration);

        Assert.False(outcome.IsExact);
        Assert.Equal(
            new AtomicElement(12, 0),
            Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(
            new CompositeElement(subject, calibration),
            outcome.Tension);
    }

    [Fact]
    public void AtomicAlignWithTension_PreservesUnresolvedPairWhenCarrierSpaceDiffers()
    {
        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var outcome = left.AlignWithTension(right);

        Assert.False(outcome.IsExact);
        Assert.True(outcome.HasAny);
        Assert.True(outcome.HasMany);
        Assert.Equal(2, outcome.OutboundResults.Count);
        Assert.Equal(new AtomicElement(4, 0), Assert.IsType<AtomicElement>(outcome.Left));
        Assert.Equal(new AtomicElement(4, 0), Assert.IsType<AtomicElement>(outcome.Right));
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
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
    public void CompositeCommitToCalibrationWithTension_CarriesChildTensionForward()
    {
        var subject = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 1));
        var calibration = new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(4, -4));

        var outcome = subject.CommitToCalibrationWithTension(calibration);

        Assert.False(outcome.IsExact);

        var committed = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(new AtomicElement(2, 4), committed.Recessive);
        Assert.Equal(new AtomicElement(12, 0), committed.Dominant);
        Assert.Equal(new CompositeElement(subject, calibration), outcome.Tension);
    }

    [Fact]
    public void CompositeAlignWithTension_CarriesChildTensionForward()
    {
        var left = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 1));
        var right = new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(4, -4));

        var outcome = left.AlignWithTension(right);

        Assert.False(outcome.IsExact);

        var alignedLeft = Assert.IsType<CompositeElement>(outcome.Left);
        var alignedRight = Assert.IsType<CompositeElement>(outcome.Right);

        Assert.Equal(new AtomicElement(2, 4), alignedLeft.Recessive);
        Assert.Equal(new AtomicElement(12, 0), alignedLeft.Dominant);
        Assert.Equal(new AtomicElement(2, 4), alignedRight.Recessive);
        Assert.Equal(new AtomicElement(16, 0), alignedRight.Dominant);
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
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
        Assert.Equal(
            new CompositeElement(
                new AtomicElement(2, 4),
                new AtomicElement(10, 40)),
            calibrated.Dominant);
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
    public void AtomicAddWithTension_PreservesUnresolvedSupportWhenCarrierSpaceDiffers()
    {
        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var outcome = left.AddWithTension(right);

        Assert.False(outcome.IsExact);
        Assert.True(outcome.HasAny);
        Assert.False(outcome.HasMany);
        Assert.Single(outcome.OutboundResults);
        Assert.Equal(new AtomicElement(8, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
    }

    [Fact]
    public void AtomicSubtractWithTension_PreservesUnresolvedSupportWhenCarrierSpaceDiffers()
    {
        var left = new AtomicElement(3, 2);
        var right = new AtomicElement(1, -4);

        var outcome = left.SubtractWithTension(right);

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(8, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
    }

    [Fact]
    public void DirectLift_RemainsAvailableAfterMixedCarrierAdd()
    {
        var aligned = new AtomicElement(3, 1);
        var orthogonal = new AtomicElement(3, -1);

        var addOutcome = aligned.AddWithTension(orthogonal);
        var explicitLift = aligned.Lift(orthogonal);

        Assert.False(addOutcome.IsExact);
        Assert.True(explicitLift.IsExact);
        Assert.Equal(
            new CompositeElement(
                new AtomicElement(3, 1),
                new AtomicElement(3, 1)),
            Assert.IsType<CompositeElement>(explicitLift.Result));
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
    public void CompositeAddWithTension_CarriesChildTensionForward()
    {
        var left = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 1));
        var right = new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(4, -4));

        var outcome = left.AddWithTension(right);

        Assert.False(outcome.IsExact);

        var sum = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(new AtomicElement(4, 4), sum.Recessive);
        Assert.Equal(new AtomicElement(28, 0), sum.Dominant);
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
    }

    [Fact]
    public void CompositeSubtractWithTension_CarriesChildTensionForward()
    {
        var left = new CompositeElement(
            new AtomicElement(3, 2),
            new AtomicElement(3, 1));
        var right = new CompositeElement(
            new AtomicElement(2, 4),
            new AtomicElement(4, -4));

        var outcome = left.SubtractWithTension(right);

        Assert.False(outcome.IsExact);

        var difference = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(new AtomicElement(4, 4), difference.Recessive);
        Assert.Equal(new AtomicElement(-4, 0), difference.Dominant);
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
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
    public void AtomicMultiplyWithTension_PreservesUnresolvedSupportWhenAUnitIsUnresolved()
    {
        var left = new AtomicElement(2, 3);
        var right = new AtomicElement(4, 0);

        var outcome = left.MultiplyWithTension(right);

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(8, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
    }

    [Fact]
    public void AtomicScaleWithTension_PreservesUnresolvedSupportWhenFactorUnitIsUnresolved()
    {
        var value = new AtomicElement(10, 1);
        var factor = new AtomicElement(3, 0);

        var outcome = value.ScaleWithTension(factor);

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(30, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(value, factor), outcome.Tension);
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
    public void GradeOneCompositeMultiplyWithTension_PreservesUnresolvedAtomicProductWhenFoldCannotSettle()
    {
        var left = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));
        var right = new CompositeElement(
            new AtomicElement(8, 1),
            new AtomicElement(2, 1));

        var outcome = left.MultiplyWithTension(right);

        Assert.False(outcome.IsExact);
        Assert.Equal(new AtomicElement(6, 0), Assert.IsType<AtomicElement>(outcome.Result));
        Assert.Equal(new CompositeElement(left, right), outcome.Tension);
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
    public void CompositeScaleWithTension_CarriesChildTensionForward()
    {
        var composite = new CompositeElement(
            new AtomicElement(1, 2),
            new AtomicElement(3, 1));
        var factor = new AtomicElement(2, 0);

        var outcome = composite.ScaleWithTension(factor);

        Assert.False(outcome.IsExact);

        var scaled = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(new AtomicElement(2, 0), scaled.Recessive);
        Assert.Equal(new AtomicElement(6, 0), scaled.Dominant);
        Assert.Equal(composite, outcome.Tension);
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
        var left = Core3TestHelpers.CreateAxisLikeNumber(
            leftRecessiveValue,
            leftRecessiveResolution,
            leftDominantValue,
            leftDominantResolution);
        var right = Core3TestHelpers.CreateAxisLikeNumber(
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

        Assert.Equal(expected.Real, Core3TestHelpers.ToDouble(recessive), 12);
        Assert.Equal(expected.Imaginary, Core3TestHelpers.ToDouble(dominant), 12);
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

    [Fact]
    public void AtomicOrthogonalLift_PromotesMixedCarrierPairIntoPositiveAxisBasis()
    {
        var aligned = new AtomicElement(3, 1);
        var orthogonal = new AtomicElement(2, -1);

        var outcome = aligned.Lift(orthogonal);

        Assert.True(outcome.IsExact);

        var lifted = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(1, lifted.Grade);
        Assert.Equal(new AtomicElement(3, 1), lifted.Recessive);
        Assert.Equal(new AtomicElement(2, 1), lifted.Dominant);
    }

    [Fact]
    public void OrthogonalLift_AllowsParallelSameGradeLift()
    {
        var left = new AtomicElement(3, 1);
        var right = new AtomicElement(2, 1);

        var outcome = left.Lift(right);

        Assert.True(outcome.IsExact);

        var lifted = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(1, lifted.Grade);
        Assert.Equal(new AtomicElement(3, 1), Assert.IsType<AtomicElement>(lifted.Recessive));
        Assert.Equal(new AtomicElement(2, 1), Assert.IsType<AtomicElement>(lifted.Dominant));
    }

    [Fact]
    public void CompositeOrthogonalLift_NormalizesLiftedBasis()
    {
        var horizontal = new CompositeElement(
            new AtomicElement(-3, 1),
            new AtomicElement(5, 1));
        var vertical = new CompositeElement(
            new AtomicElement(2, -1),
            new AtomicElement(7, -1));

        var outcome = horizontal.Lift(vertical);

        Assert.True(outcome.IsExact);

        var lifted = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(2, lifted.Grade);

        var recessive = Assert.IsType<CompositeElement>(lifted.Recessive);
        var dominant = Assert.IsType<CompositeElement>(lifted.Dominant);

        Assert.Equal(new AtomicElement(-3, 1), recessive.Recessive);
        Assert.Equal(new AtomicElement(5, 1), recessive.Dominant);
        Assert.Equal(new AtomicElement(2, 1), dominant.Recessive);
        Assert.Equal(new AtomicElement(7, 1), dominant.Dominant);
    }

    [Fact]
    public void AxisCanPromoteIntoSparseAreaByLiftingAgainstZeroLikePeer()
    {
        var horizontalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(3, 1)),
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(7, 1)));

        var outcome = horizontalAxis.Lift(horizontalAxis.CreateZeroLikePeer());

        Assert.True(outcome.IsExact);

        var areaLike = Assert.IsType<CompositeElement>(outcome.Result);
        Assert.Equal(3, areaLike.Grade);
        Assert.Equal(horizontalAxis, areaLike.Recessive);

        var zeroAxis = Assert.IsType<CompositeElement>(areaLike.Dominant);
        Assert.Equal(new AtomicElement(0, 0), Assert.IsType<AtomicElement>(Assert.IsType<CompositeElement>(zeroAxis.Recessive).Recessive));
        Assert.Equal(new AtomicElement(0, 0), Assert.IsType<AtomicElement>(Assert.IsType<CompositeElement>(zeroAxis.Recessive).Dominant));
        Assert.Equal(new AtomicElement(0, 0), Assert.IsType<AtomicElement>(Assert.IsType<CompositeElement>(zeroAxis.Dominant).Recessive));
        Assert.Equal(new AtomicElement(0, 0), Assert.IsType<AtomicElement>(Assert.IsType<CompositeElement>(zeroAxis.Dominant).Dominant));
    }

    [Fact]
    public void SparseArea_LowersToItsActiveAxis()
    {
        var horizontalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(3, 1)),
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(7, 1)));
        var sparseArea = horizontalAxis.Lift(horizontalAxis.CreateZeroLikePeer()).Result;

        var outcome = sparseArea.Lower();

        Assert.True(outcome.IsExact);
        Assert.Equal(horizontalAxis, outcome.Result);
    }

    [Fact]
    public void FullArea_LowersOneGradeIntoCornerAxis()
    {
        var horizontalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(3, 1)),
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(7, 1)));
        var verticalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(2, -1)),
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(5, -1)));
        var area = horizontalAxis.Lift(verticalAxis).Result;

        var outcome = area.Lower();

        Assert.True(outcome.IsExact);
        Assert.Equal(
            new CompositeElement(
                new CompositeElement(
                    new AtomicElement(3, 10),
                    new AtomicElement(7, 10)),
                new CompositeElement(
                    new AtomicElement(2, 10),
                    new AtomicElement(5, 10))),
            outcome.Result);
    }

    [Fact]
    public void CarrierPreservingLower_CanTakeLonePositiveIToNegativeUnitAtomic()
    {
        var positiveI = new CompositeElement(
            new AtomicElement(1, -1),
            new AtomicElement(3, -1));
        var sparseAxis = new CompositeElement(
            positiveI,
            positiveI.CreateZeroLikePeer());

        var axisOutcome = sparseAxis.Lower();
        var atomicOutcome = axisOutcome.Result.Lower();

        Assert.True(axisOutcome.IsExact);
        Assert.Equal(positiveI, axisOutcome.Result);
        Assert.True(atomicOutcome.IsExact);
        Assert.Equal(new AtomicElement(3, -1), atomicOutcome.Result);
    }

    [Fact]
    public void SparseAreaReference_ToFullArea_CanLowerToHorizontalSnapshot()
    {
        var horizontalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(3, 1)),
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(7, 1)));
        var verticalAxis = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(2, -1)),
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(5, -1)));
        var fullArea = horizontalAxis.Lift(verticalAxis).Result;
        var sparseArea = horizontalAxis.Lift(horizontalAxis.CreateZeroLikePeer()).Result;

        var reference = new EngineReference(
            Assert.IsType<CompositeElement>(fullArea),
            sparseArea);
        var readOutcome = reference.Read();
        var loweredOutcome = readOutcome.Result.Lower();

        Assert.Equal(horizontalAxis, loweredOutcome.Result);
    }

    [Fact]
    public void EngineChain_CanBeBuiltManuallyFromProportionToVolume()
    {
        var proportionA = new CompositeElement(
            new AtomicElement(10, 1),
            new AtomicElement(3, 1));
        var proportionB = new CompositeElement(
            new AtomicElement(10, 1),
            new AtomicElement(7, 1));
        var proportionC = new CompositeElement(
            new AtomicElement(10, -1),
            new AtomicElement(2, -1));
        var proportionD = new CompositeElement(
            new AtomicElement(10, -1),
            new AtomicElement(5, -1));

        var axisHorizontal = new CompositeElement(proportionA, proportionB);
        var axisVertical = new CompositeElement(proportionC, proportionD);
        var areaLike = new CompositeElement(axisHorizontal, axisVertical);

        var depthAxisNear = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(1, 1)),
            new CompositeElement(
                new AtomicElement(10, 1),
                new AtomicElement(4, 1)));
        var depthAxisFar = new CompositeElement(
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(6, -1)),
            new CompositeElement(
                new AtomicElement(10, -1),
                new AtomicElement(9, -1)));
        var areaLikeDepth = new CompositeElement(depthAxisNear, depthAxisFar);
        var volumeLike = new CompositeElement(areaLike, areaLikeDepth);

        Assert.Equal(1, proportionA.Grade);
        Assert.Equal(2, axisHorizontal.Grade);
        Assert.Equal(3, areaLike.Grade);
        Assert.Equal(4, volumeLike.Grade);

        Assert.Equal(proportionA, axisHorizontal.Recessive);
        Assert.Equal(proportionB, axisHorizontal.Dominant);
        Assert.Equal(axisHorizontal, areaLike.Recessive);
        Assert.Equal(axisVertical, areaLike.Dominant);
        Assert.Equal(areaLike, volumeLike.Recessive);
        Assert.Equal(areaLikeDepth, volumeLike.Dominant);
    }
}
