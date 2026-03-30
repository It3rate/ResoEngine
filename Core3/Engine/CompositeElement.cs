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

    public override EngineElementOutcome FoldWithTension()
    {
        if (Recessive is AtomicElement denominator &&
            Dominant is AtomicElement numerator)
        {
            return EngineEvaluation.ComposeRatio(numerator, denominator);
        }

        var recessiveOutcome = Recessive.FoldWithTension();
        var dominantOutcome = Dominant.FoldWithTension();
        var folded = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(folded);
        }

        return EngineElementOutcome.WithTension(
            folded,
            this,
            "Composite fold preserved child tension.");
    }

    public override EngineElementOutcome CommitToCalibrationWithTension(GradedElement calibration)
    {
        if (calibration is not CompositeElement compositeCalibration ||
            Grade != compositeCalibration.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Calibration preserved the composite unchanged because the calibration shape was incompatible.");
        }

        var recessiveOutcome = Recessive.CommitToCalibrationWithTension(compositeCalibration.Recessive);
        var dominantOutcome = Dominant.CommitToCalibrationWithTension(compositeCalibration.Dominant);
        var committed = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(committed);
        }

        return EngineElementOutcome.WithTension(
            committed,
            new CompositeElement(this, compositeCalibration),
            "Composite calibration preserved child tension.");
    }

    public override bool TryFold(out GradedElement? folded)
    {
        folded = FoldWithTension().Result;
        return true;
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

    public override EngineElementPairOutcome AlignWithTension(
        GradedElement other,
        ResolutionPolicy policy)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementPairOutcome.WithTension(
                this,
                other,
                this,
                "Alignment preserved the composites unchanged because their shapes were incompatible.");
        }

        var recessiveOutcome = Recessive.AlignWithTension(composite.Recessive, policy);
        var dominantOutcome = Dominant.AlignWithTension(composite.Dominant, policy);
        var left = new CompositeElement(recessiveOutcome.Left, dominantOutcome.Left);
        var right = new CompositeElement(recessiveOutcome.Right, dominantOutcome.Right);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementPairOutcome.Exact(left, right);
        }

        return EngineElementPairOutcome.WithTension(
            left,
            right,
            new CompositeElement(this, composite),
            "Composite alignment preserved child tension.");
    }

    public override EngineElementOutcome AddWithTension(GradedElement other)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Addition preserved the composite unchanged because the compared shape was incompatible.");
        }

        var recessiveOutcome = Recessive.AddWithTension(composite.Recessive);
        var dominantOutcome = Dominant.AddWithTension(composite.Dominant);
        var sum = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(sum);
        }

        return EngineElementOutcome.WithTension(
            sum,
            new CompositeElement(this, composite),
            "Composite addition preserved child tension.");
    }

    public override EngineElementOutcome SubtractWithTension(GradedElement other)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Subtraction preserved the composite unchanged because the compared shape was incompatible.");
        }

        var recessiveOutcome = Recessive.SubtractWithTension(composite.Recessive);
        var dominantOutcome = Dominant.SubtractWithTension(composite.Dominant);
        var difference = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(difference);
        }

        return EngineElementOutcome.WithTension(
            difference,
            new CompositeElement(this, composite),
            "Composite subtraction preserved child tension.");
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
        var outcome = AddWithTension(other);

        if (outcome.IsExact)
        {
            sum = outcome.Result;
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        var outcome = SubtractWithTension(other);

        if (outcome.IsExact)
        {
            difference = outcome.Result;
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
