namespace Core3.Engine;

/// <summary>
/// Generic higher-grade element built from two equal-grade children.
/// This is the grade-first engine analogue of the named element layer.
/// </summary>
public sealed record CompositeElement : GradedElement
{
    public CompositeElement(GradedElement recessive, GradedElement dominant)
    {
        ArgumentNullException.ThrowIfNull(recessive);
        ArgumentNullException.ThrowIfNull(dominant);

        if (recessive.Grade != dominant.Grade)
        {
            throw new InvalidOperationException("Composite elements require children of the same grade.");
        }

        Recessive = recessive;
        Dominant = dominant;
    }

    public GradedElement Recessive { get; }
    public GradedElement Dominant { get; }

    public override int Grade => Recessive.Grade + 1;
    public override bool HasResolvedUnits => Recessive.HasResolvedUnits && Dominant.HasResolvedUnits;

    public override GradedElement Negate() => new CompositeElement(
        Recessive.Negate(),
        Dominant.Negate());

    public override GradedElement SwapOrder() => new CompositeElement(
        Dominant,
        Recessive);

    public override GradedElement FlipPerspective() => new CompositeElement(
        Dominant.FlipPerspective(),
        Recessive.FlipPerspective());

    public override bool TryFold(out GradedElement? folded)
    {
        if (Recessive is AtomicElement denominator &&
            Dominant is AtomicElement numerator)
        {
            return EngineEvaluation.TryComposeRatio(numerator, denominator, out folded);
        }

        if (Recessive.TryFold(out var foldedRecessive) &&
            Dominant.TryFold(out var foldedDominant) &&
            foldedRecessive is not null &&
            foldedDominant is not null)
        {
            folded = new CompositeElement(foldedRecessive, foldedDominant);
            return true;
        }

        folded = null;
        return false;
    }

    public override bool TryCommitToCalibration(GradedElement calibration, out GradedElement? committed)
    {
        if (calibration is CompositeElement compositeCalibration &&
            Grade == compositeCalibration.Grade &&
            Recessive.TryCommitToCalibration(compositeCalibration.Recessive, out var committedRecessive) &&
            Dominant.TryCommitToCalibration(compositeCalibration.Dominant, out var committedDominant) &&
            committedRecessive is not null &&
            committedDominant is not null)
        {
            committed = new CompositeElement(committedRecessive, committedDominant);
            return true;
        }

        committed = null;
        return false;
    }

    public override bool TryAlignExact(
        GradedElement other,
        ResolutionPolicy policy,
        out GradedElement? leftAligned,
        out GradedElement? rightAligned)
    {
        if (other is not CompositeElement composite || Grade != composite.Grade)
        {
            leftAligned = null;
            rightAligned = null;
            return false;
        }

        switch (policy)
        {
            case ResolutionPolicy.PreserveHost:
                if (composite.TryCommitToCalibration(this, out rightAligned) &&
                    rightAligned is not null)
                {
                    leftAligned = this;
                    return true;
                }

                break;

            case ResolutionPolicy.PreserveApplied:
                if (TryCommitToCalibration(composite, out leftAligned) &&
                    leftAligned is not null)
                {
                    rightAligned = composite;
                    return true;
                }

                break;

            case ResolutionPolicy.ExactCommonFrame:
            case ResolutionPolicy.ComposeSupport:
                if (Recessive.TryAlignExact(composite.Recessive, policy, out var leftRecessive, out var rightRecessive) &&
                    Dominant.TryAlignExact(composite.Dominant, policy, out var leftDominant, out var rightDominant) &&
                    leftRecessive is not null &&
                    rightRecessive is not null &&
                    leftDominant is not null &&
                    rightDominant is not null)
                {
                    leftAligned = new CompositeElement(leftRecessive, leftDominant);
                    rightAligned = new CompositeElement(rightRecessive, rightDominant);
                    return true;
                }

                break;
        }

        leftAligned = null;
        rightAligned = null;
        return false;
    }

    public override bool SharesUnitSpace(GradedElement other) =>
        other is CompositeElement composite &&
        HasResolvedUnits &&
        composite.HasResolvedUnits &&
        Recessive.SharesUnitSpace(composite.Recessive) &&
        Dominant.SharesUnitSpace(composite.Dominant);

    public override bool TryAdd(GradedElement other, out GradedElement? sum)
    {
        if (other is CompositeElement composite &&
            Recessive.TryAdd(composite.Recessive, out var recessiveSum) &&
            Dominant.TryAdd(composite.Dominant, out var dominantSum) &&
            recessiveSum is not null &&
            dominantSum is not null)
        {
            sum = new CompositeElement(recessiveSum, dominantSum);
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        if (other is CompositeElement composite &&
            Recessive.TrySubtract(composite.Recessive, out var recessiveDifference) &&
            Dominant.TrySubtract(composite.Dominant, out var dominantDifference) &&
            recessiveDifference is not null &&
            dominantDifference is not null)
        {
            difference = new CompositeElement(recessiveDifference, dominantDifference);
            return true;
        }

        difference = null;
        return false;
    }

    public override bool TryMultiply(GradedElement other, out GradedElement? product)
    {
        if (other is not CompositeElement composite || Grade != composite.Grade)
        {
            product = null;
            return false;
        }

        if (Grade == 1 &&
            EngineEvaluation.TryFoldToAtomic(this, out var left) &&
            left is not null &&
            EngineEvaluation.TryFoldToAtomic(composite, out var right) &&
            right is not null)
        {
            return EngineEvaluation.TryMultiplyAtomic(left, right, out product);
        }

        return EngineEvaluation.TryMultiplyKernel(this, composite, out product);
    }

    public override bool TryScale(AtomicElement factor, out GradedElement? scaled)
    {
        ArgumentNullException.ThrowIfNull(factor);

        if (Recessive.TryScale(factor, out var recessiveScaled) &&
            Dominant.TryScale(factor, out var dominantScaled) &&
            recessiveScaled is not null &&
            dominantScaled is not null)
        {
            scaled = new CompositeElement(recessiveScaled, dominantScaled);
            return true;
        }

        scaled = null;
        return false;
    }

    public override string ToString() => $"<{Recessive} | {Dominant}>";
}
