namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying an exact signed amount and an exact signed
/// unit number. The unit sign carries carrier relation (positive aligned,
/// negative orthogonal, zero unresolved), while the unit magnitude carries
/// exact resolution support. Opposite perspective negates the realized value
/// but preserves the unit.
/// </summary>
public sealed record AtomicElement(long Value, long Unit) : GradedElement
{
    public override int Grade => 0;
    public override bool HasResolvedUnits => Unit != 0;
    public bool IsAlignedUnit => Unit > 0;
    public bool IsOrthogonalUnit => Unit < 0;
    public bool IsUnresolvedUnit => Unit == 0;
    public bool IsClampLike => Unit == 0 && Value == 0;
    public long Resolution => Math.Abs(Unit);

    public override GradedElement Negate() => new AtomicElement(-Value, Unit);

    public override GradedElement SwapOrder() => this;

    public override GradedElement FlipPerspective() => Negate();

    public override EngineElementOutcome FoldWithTension() => EngineElementOutcome.Exact(this);

    public EngineElementOutcome ReexpressToSupportWithTension(long targetResolution)
    {
        if (TryReexpressToSupport(targetResolution, out var reexpressed) &&
            reexpressed is not null)
        {
            return EngineElementOutcome.Exact(reexpressed);
        }

        return EngineElementOutcome.WithTension(
            CreateUnresolvedProjection(targetResolution),
            new CompositeElement(this, new AtomicElement(targetResolution, 1)),
            CreateReexpressionNote(targetResolution));
    }

    public override bool TryFold(out GradedElement? folded)
    {
        folded = this;
        return true;
    }

    // Resolution is preserved during exact working arithmetic.
    // Re-expression, alignment, and support commit are explicit operations so
    // the engine does not silently collapse support by coincidence.
    public bool TryReexpressToSupport(long targetResolution, out AtomicElement? reexpressed)
    {
        if (!HasResolvedUnits || targetResolution <= 0)
        {
            reexpressed = null;
            return false;
        }

        var scaledValue = checked(Value * targetResolution);

        if (scaledValue % Resolution != 0)
        {
            reexpressed = null;
            return false;
        }

        reexpressed = new AtomicElement(
            scaledValue / Resolution,
            checked(Math.Sign(Unit) * targetResolution));
        return true;
    }

    public bool TryCommitToSupport(long targetResolution, out AtomicElement? committed) =>
        TryReexpressToSupport(targetResolution, out committed);

    public override EngineElementOutcome CommitToCalibrationWithTension(GradedElement calibration)
    {
        if (calibration is AtomicElement atomicCalibration)
        {
            if (TryCommitToCalibration(atomicCalibration, out var committed) &&
                committed is not null)
            {
                return EngineElementOutcome.Exact(committed);
            }

            return EngineElementOutcome.WithTension(
                CreateUnresolvedProjection(atomicCalibration.Resolution),
                new CompositeElement(this, atomicCalibration),
                CreateCalibrationNote(atomicCalibration));
        }

        return EngineElementOutcome.WithTension(
            CreateUnresolvedProjection(0),
            calibration,
            "Calibration preserved an unresolved atomic read because the calibration grade was incompatible.");
    }

    public override bool TryCommitToCalibration(GradedElement calibration, out GradedElement? committed)
    {
        if (calibration is AtomicElement atomicCalibration &&
            SharesUnitSpace(atomicCalibration) &&
            TryCommitToSupport(atomicCalibration.Resolution, out var committedAtomic) &&
            committedAtomic is not null)
        {
            committed = committedAtomic;
            return true;
        }

        committed = null;
        return false;
    }

    public bool TryAlignExact(AtomicElement other, out AtomicElement? leftAligned, out AtomicElement? rightAligned)
    {
        if (TryAlignExact(other, ResolutionPolicy.ExactCommonFrame, out var leftCommitted, out var rightCommitted) &&
            leftCommitted is AtomicElement leftAtomic &&
            rightCommitted is AtomicElement rightAtomic)
        {
            leftAligned = leftAtomic;
            rightAligned = rightAtomic;
            return true;
        }

        leftAligned = null;
        rightAligned = null;
        return false;
    }

    public override EngineElementPairOutcome AlignWithTension(
        GradedElement other,
        ResolutionPolicy policy)
    {
        if (other is AtomicElement atomic)
        {
            if (TryAlignExact(atomic, policy, out var leftAligned, out var rightAligned) &&
                leftAligned is not null &&
                rightAligned is not null)
            {
                return EngineElementPairOutcome.Exact(leftAligned, rightAligned);
            }

            var targetResolution = ResolveSuggestedAlignmentResolution(atomic, policy);
            return EngineElementPairOutcome.WithTension(
                CreateUnresolvedProjection(targetResolution),
                atomic.CreateUnresolvedProjection(targetResolution),
                new CompositeElement(this, atomic),
                CreateAlignmentNote(atomic, policy));
        }

        return EngineElementPairOutcome.WithTension(
            CreateUnresolvedProjection(0),
            other,
            this,
            "Alignment preserved an unresolved atomic read because the compared element was not atomic.");
    }

    public override bool TryAlignExact(
        GradedElement other,
        ResolutionPolicy policy,
        out GradedElement? leftAligned,
        out GradedElement? rightAligned)
    {
        if (other is not AtomicElement atomic)
        {
            leftAligned = null;
            rightAligned = null;
            return false;
        }

        if (!SharesUnitSpace(atomic) ||
            !TryResolveAlignmentResolution(atomic, policy, out var targetResolution) ||
            !TryCommitToSupport(targetResolution, out var committedLeft) ||
            committedLeft is null ||
            !atomic.TryCommitToSupport(targetResolution, out var committedRight) ||
            committedRight is null)
        {
            leftAligned = null;
            rightAligned = null;
            return false;
        }

        leftAligned = committedLeft;
        rightAligned = committedRight;
        return true;
    }

    public override bool SharesUnitSpace(GradedElement other) =>
        other is AtomicElement atomic &&
        HasResolvedUnits &&
        atomic.HasResolvedUnits &&
        Math.Sign(Unit) == Math.Sign(atomic.Unit);

    public override bool TryAdd(GradedElement other, out GradedElement? sum)
    {
        if (TryAlignExact(other, ResolutionPolicy.ExactCommonFrame, out var leftAligned, out var rightAligned) &&
            leftAligned is AtomicElement leftAtomic &&
            rightAligned is AtomicElement rightAtomic)
        {
            sum = new AtomicElement(
                checked(leftAtomic.Value + rightAtomic.Value),
                leftAtomic.Unit);
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        if (TryAlignExact(other, ResolutionPolicy.ExactCommonFrame, out var leftAligned, out var rightAligned) &&
            leftAligned is AtomicElement leftAtomic &&
            rightAligned is AtomicElement rightAtomic)
        {
            difference = new AtomicElement(
                checked(leftAtomic.Value - rightAtomic.Value),
                leftAtomic.Unit);
            return true;
        }

        difference = null;
        return false;
    }

    public override bool TryMultiply(GradedElement other, out GradedElement? product)
    {
        if (other is AtomicElement atomic)
        {
            return EngineEvaluation.TryMultiplyAtomic(this, atomic, out product);
        }

        product = null;
        return false;
    }

    public override bool TryScale(AtomicElement factor, out GradedElement? scaled)
    {
        ArgumentNullException.ThrowIfNull(factor);

        if (!HasResolvedUnits || !factor.HasResolvedUnits)
        {
            scaled = null;
            return false;
        }

        var scaledValue = checked(Value * factor.Value);
        var scaledUnit = checked(Unit * factor.Unit);
        scaled = new AtomicElement(scaledValue, scaledUnit);
        return true;
    }

    public override string ToString() => $"{Value}/{Unit}";

    private AtomicElement CreateUnresolvedProjection(long targetResolution)
    {
        var preservedSupport = Math.Max(
            1L,
            Math.Max(Resolution, Math.Abs(targetResolution)));
        return new AtomicElement(
            checked(Value * preservedSupport),
            0);
    }

    private string CreateReexpressionNote(long targetResolution) =>
        !HasResolvedUnits
            ? "Support re-expression preserved unresolved support because the source unit slot was unresolved."
            : targetResolution <= 0
                ? "Support re-expression preserved unresolved support because the requested support was zero-like or negative."
                : "Support re-expression preserved unresolved support because the requested support did not divide exactly.";

    private string CreateCalibrationNote(AtomicElement calibration) =>
        !HasResolvedUnits || !calibration.HasResolvedUnits
            ? "Calibration preserved unresolved support because one or both unit slots were unresolved."
            : Math.Sign(Unit) != Math.Sign(calibration.Unit)
                ? "Calibration preserved carrier contrast as unresolved support."
                : "Calibration preserved unresolved support because the requested calibration did not divide exactly.";

    private string CreateAlignmentNote(AtomicElement other, ResolutionPolicy policy) =>
        !HasResolvedUnits || !other.HasResolvedUnits
            ? "Alignment preserved unresolved support because one or both unit slots were unresolved."
            : Math.Sign(Unit) != Math.Sign(other.Unit)
                ? "Alignment preserved carrier contrast as unresolved support."
                : policy == ResolutionPolicy.ComposeSupport
                    ? "Alignment preserved unresolved support under composed support."
                    : "Alignment preserved unresolved support because the compared values did not align exactly under the requested policy.";

    private long ResolveSuggestedAlignmentResolution(AtomicElement other, ResolutionPolicy policy)
    {
        var leftResolution = Math.Max(1L, Resolution);
        var rightResolution = Math.Max(1L, other.Resolution);

        return policy switch
        {
            ResolutionPolicy.PreserveHost => leftResolution,
            ResolutionPolicy.PreserveApplied => rightResolution,
            ResolutionPolicy.ComposeSupport => checked(leftResolution * rightResolution),
            ResolutionPolicy.ExactCommonFrame => checked((leftResolution / GreatestCommonDivisor(leftResolution, rightResolution)) * rightResolution),
            _ => Math.Max(leftResolution, rightResolution)
        };
    }

    private bool TryResolveAlignmentResolution(
        AtomicElement other,
        ResolutionPolicy policy,
        out long targetResolution)
    {
        targetResolution = policy switch
        {
            ResolutionPolicy.PreserveHost => Resolution,
            ResolutionPolicy.PreserveApplied => other.Resolution,
            ResolutionPolicy.ExactCommonFrame => LeastCommonMultiple(Resolution, other.Resolution),
            ResolutionPolicy.ComposeSupport => checked(Resolution * other.Resolution),
            _ => 0
        };

        return targetResolution > 0;
    }

    private static long LeastCommonMultiple(long left, long right) =>
        checked((left / GreatestCommonDivisor(left, right)) * right);

    private static long GreatestCommonDivisor(long left, long right)
    {
        while (right != 0)
        {
            var remainder = left % right;
            left = right;
            right = remainder;
        }

        return left == 0 ? 1 : left;
    }
}
