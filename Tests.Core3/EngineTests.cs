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
}
