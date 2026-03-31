namespace Core3.Engine;

/// <summary>
/// Generic higher-grade element built from two equal-grade children.
/// This is the grade-first engine analogue of the named element layer.
/// </summary>
public sealed record CompositeElement : GradedElement
{
    // TODO: Long term, treat composite formation as an implicit origin-pinned
    // relation rather than only as a free recursive pair. Conceptually the
    // recessive and dominant children already meet at a virtual local origin.
    // The current engine stores them directly because grade-0 still bootstraps
    // from raw exact slots rather than from pin-owned child elements. When
    // pinning can lawfully span every grade, revisit composite construction so
    // higher-grade elements and pinned encounter structure converge.
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

    public override EngineElementOutcome Fold()
    {
        if (Recessive is AtomicElement denominator &&
            Dominant is AtomicElement numerator)
        {
            return EngineEvaluation.ComposeRatio(numerator, denominator);
        }

        var recessiveOutcome = Recessive.Fold();
        var dominantOutcome = Dominant.Fold();
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

    public override EngineElementOutcome CommitToCalibration(GradedElement calibration)
    {
        if (calibration is not CompositeElement compositeCalibration ||
            Grade != compositeCalibration.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Calibration preserved the composite unchanged because the calibration shape was incompatible.");
        }

        var recessiveOutcome = Recessive.CommitToCalibration(compositeCalibration.Recessive);
        var dominantOutcome = Dominant.CommitToCalibration(compositeCalibration.Dominant);
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

    public override EngineElementOutcome Align(
        GradedElement other,
        ResolutionPolicy policy)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                other,
                this,
                "Alignment preserved the composites unchanged because their shapes were incompatible.");
        }

        var recessiveOutcome = Recessive.Align(composite.Recessive, policy);
        var dominantOutcome = Dominant.Align(composite.Dominant, policy);

        if (!recessiveOutcome.TryGetPair(out var leftRecessive, out var rightRecessive) ||
            !dominantOutcome.TryGetPair(out var leftDominant, out var rightDominant) ||
            leftRecessive is null ||
            rightRecessive is null ||
            leftDominant is null ||
            rightDominant is null)
        {
            return EngineElementOutcome.WithTension(
                this,
                other,
                new CompositeElement(this, composite),
                "Composite alignment preserved an incomplete survivor family.");
        }

        var left = new CompositeElement(leftRecessive, leftDominant);
        var right = new CompositeElement(rightRecessive, rightDominant);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(left, right);
        }

        return EngineElementOutcome.WithTension(
            left,
            right,
            new CompositeElement(this, composite),
            "Composite alignment preserved child tension.");
    }

    public override EngineElementOutcome Add(GradedElement other)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Addition preserved the composite unchanged because the compared shape was incompatible.");
        }

        var recessiveOutcome = Recessive.Add(composite.Recessive);
        var dominantOutcome = Dominant.Add(composite.Dominant);
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

    public override EngineElementOutcome Subtract(GradedElement other)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Subtraction preserved the composite unchanged because the compared shape was incompatible.");
        }

        var recessiveOutcome = Recessive.Subtract(composite.Recessive);
        var dominantOutcome = Dominant.Subtract(composite.Dominant);
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

    public override EngineElementOutcome Multiply(GradedElement other)
    {
        if (other is not CompositeElement composite ||
            Grade != composite.Grade)
        {
            return EngineElementOutcome.WithTension(
                this,
                this,
                "Multiplication preserved the composite unchanged because the compared shape was incompatible.");
        }

        if (Grade == 1)
        {
            var leftFold = Fold();
            var rightFold = composite.Fold();

            if (leftFold.Result is AtomicElement leftAtomic &&
                rightFold.Result is AtomicElement rightAtomic)
            {
                var productOutcome = leftAtomic.Multiply(rightAtomic);

                if (leftFold.IsExact &&
                    rightFold.IsExact &&
                    productOutcome.IsExact)
                {
                    return productOutcome;
                }

                return EngineElementOutcome.WithTension(
                    productOutcome.Result,
                    new CompositeElement(this, composite),
                    "Grade-one multiply preserved fold or product tension.");
            }

            return EngineElementOutcome.WithTension(
                this,
                new CompositeElement(this, composite),
                "Grade-one multiply preserved unresolved fold structure.");
        }

        var rr = Recessive.Multiply(composite.Recessive);
        var rd = Recessive.Multiply(composite.Dominant);
        var dr = Dominant.Multiply(composite.Recessive);
        var dd = Dominant.Multiply(composite.Dominant);

        // This is the current explicit multiply kernel. Many familiar reads
        // later fold these four activities back into one result, but the
        // kernel itself is often important structure rather than disposable
        // intermediate bookkeeping.
        if (rr.IsExact &&
            rd.IsExact &&
            dr.IsExact &&
            dd.IsExact &&
            rr.Result.TrySubtract(dd.Result, out var squareDifference) &&
            squareDifference is not null &&
            rd.Result.TryAdd(dr.Result, out var crossSum) &&
            crossSum is not null &&
            squareDifference.Grade == crossSum.Grade)
        {
            return EngineElementOutcome.Exact(
                new CompositeElement(squareDifference, crossSum));
        }

        if (rr.IsExact &&
            rd.IsExact &&
            dr.IsExact &&
            dd.IsExact)
        {
            return EngineElementOutcome.Exact(
                new CompositeElement(
                    new CompositeElement(rr.Result, dd.Result),
                    new CompositeElement(rd.Result, dr.Result)));
        }

        return EngineElementOutcome.WithTension(
            new CompositeElement(
                new CompositeElement(rr.Result, dd.Result),
                new CompositeElement(rd.Result, dr.Result)),
            new CompositeElement(this, composite),
            "Composite multiplication preserved the raw kernel because exact reduction did not settle.");
    }

    public override EngineElementOutcome Scale(AtomicElement factor)
    {
        var recessiveOutcome = Recessive.Scale(factor);
        var dominantOutcome = Dominant.Scale(factor);
        var scaled = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(scaled);
        }

        return EngineElementOutcome.WithTension(
            scaled,
            this,
            "Composite scale preserved child tension.");
    }

    public bool TryMultiplyKernel(CompositeElement other, out CompositeElement? kernel)
    {
        if (Grade != other.Grade)
        {
            kernel = null;
            return false;
        }

        return EngineEvaluation.TryMultiplyKernel(this, other, out kernel);
    }

    public override string ToString() => $"<{Recessive} | {Dominant}>";
}
