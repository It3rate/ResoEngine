namespace Core3.Engine;

internal static class EngineEvaluation
{
    internal static EngineElementOutcome LiftOrthogonal(
        GradedElement left,
        GradedElement right)
    {
        var targetGrade = Math.Max(left.Grade, right.Grade) + 1;

        // Preferred path: lawful operations emit lift candidates and callers
        // choose whether to promote them. This direct lift shell remains useful
        // for compatibility and tests, but the candidate path should become the
        // ordinary way higher-grade promotion is requested.
        // TODO: This first pass only checks simple structural directional
        // coherence. Later, successful lift should also verify that the
        // orthogonal pressure has consolidated into a meaningful higher
        // relation rather than any arbitrary cross-carrier contrast.
        if (!TryCreateLiftCandidate(left, right, out var lifted) ||
            lifted is null)
        {
            return EngineElementOutcome.WithTension(
                CreateZeroLikeElement(targetGrade),
                CreateLiftProvenance(left, right),
                "Orthogonal lift preserved a zero-like lifted shell because the inputs were not a directionally coherent lift candidate.");
        }

        return EngineElementOutcome.Exact(lifted);
    }

    internal static bool TryCreateLiftCandidate(
        GradedElement left,
        GradedElement right,
        out GradedElement? candidate)
    {
        if (!IsLiftCandidate(left, right))
        {
            candidate = null;
            return false;
        }

        candidate = new CompositeElement(
            NormalizeLiftBasis(left),
            NormalizeLiftBasis(right));
        return true;
    }

    internal static EngineElementOutcome ComposeRatio(
        AtomicElement numerator,
        AtomicElement denominator)
    {
        var ratio = new CompositeElement(denominator, numerator);

        if (!numerator.HasResolvedUnits || !denominator.HasResolvedUnits)
        {
            return EngineElementOutcome.WithTension(
                CreateUnresolvedRatioResult(numerator, denominator),
                ratio,
                "Ratio fold preserved unresolved support because one or both unit slots were unresolved.");
        }

        if (Math.Sign(numerator.Unit) != Math.Sign(denominator.Unit))
        {
            return EngineElementOutcome.WithTension(
                CreateUnresolvedRatioResult(numerator, denominator),
                ratio,
                "Ratio fold preserved carrier contrast as unresolved support.");
        }

        if (denominator.Value == 0)
        {
            return EngineElementOutcome.WithTension(
                CreateUnresolvedRatioResult(numerator, denominator),
                ratio,
                "Ratio fold preserved a zero-like denominator as unresolved support.");
        }

        var value = checked(numerator.Value * Math.Abs(denominator.Unit));

        if (denominator.Value < 0)
        {
            value = checked(-value);
        }

        var unit = checked(Math.Sign(numerator.Unit) * Math.Abs(numerator.Unit) * Math.Abs(denominator.Value));
        return EngineElementOutcome.Exact(new AtomicElement(value, unit));
    }

    internal static bool TryFoldToAtomic(GradedElement element, out AtomicElement? folded)
    {
        var current = element;

        while (current is not AtomicElement atomic)
        {
            if (!current.TryFold(out var next) || next is null)
            {
                folded = null;
                return false;
            }

            current = next;
        }

        folded = (AtomicElement)current;
        return true;
    }

    internal static bool TryComposeRatio(
        AtomicElement numerator,
        AtomicElement denominator,
        out GradedElement? folded) =>
        EngineExactness.TryProjectExact(
            ComposeRatio(numerator, denominator),
            static outcome => outcome.Result,
            out folded);

    internal static bool TryMultiplyAtomic(
        AtomicElement left,
        AtomicElement right,
        out GradedElement? product)
    {
        if (!left.HasResolvedUnits || !right.HasResolvedUnits)
        {
            product = null;
            return false;
        }

        var value = checked(left.Value * right.Value);
        var unitMagnitude = checked(Math.Abs(left.Unit) * Math.Abs(right.Unit));
        var unitSign = left.IsOrthogonalUnit || right.IsOrthogonalUnit ? -1L : 1L;
        var unit = checked(unitSign * unitMagnitude);
        product = new AtomicElement(value, unit);
        return true;
    }

    internal static bool TryMultiplyKernel(
        CompositeElement left,
        CompositeElement right,
        out GradedElement? product)
    {
        if (!left.Recessive.TryMultiply(right.Recessive, out var rr) ||
            rr is null ||
            !left.Recessive.TryMultiply(right.Dominant, out var rd) ||
            rd is null ||
            !left.Dominant.TryMultiply(right.Recessive, out var dr) ||
            dr is null ||
            !left.Dominant.TryMultiply(right.Dominant, out var dd) ||
            dd is null)
        {
            product = null;
            return false;
        }

        if (rr.TrySubtract(dd, out var squareDifference) &&
            squareDifference is not null &&
            rd.TryAdd(dr, out var crossSum) &&
            crossSum is not null &&
            squareDifference.Grade == crossSum.Grade)
        {
            product = new CompositeElement(squareDifference, crossSum);
            return true;
        }

        product = new CompositeElement(
            new CompositeElement(rr, dd),
            new CompositeElement(rd, dr));
        return true;
    }

    private static bool IsLiftCandidate(GradedElement left, GradedElement right) =>
        left.Grade == right.Grade &&
        !left.SharesUnitSpace(right) &&
        IsCollinear(left) &&
        IsCollinear(right);

    private static bool IsCollinear(GradedElement element) =>
        element switch
        {
            AtomicElement atomic => atomic.HasResolvedUnits,
            CompositeElement composite =>
                composite.Recessive.SharesUnitSpace(composite.Dominant) &&
                IsCollinear(composite.Recessive) &&
                IsCollinear(composite.Dominant),
            _ => false
        };

    private static GradedElement NormalizeLiftBasis(GradedElement element) =>
        element switch
        {
            AtomicElement atomic => new AtomicElement(atomic.Value, atomic.HasResolvedUnits ? Math.Abs(atomic.Unit) : 0),
            CompositeElement composite => new CompositeElement(
                NormalizeLiftBasis(composite.Recessive),
                NormalizeLiftBasis(composite.Dominant)),
            _ => element
        };

    private static GradedElement CreateZeroLikeElement(int grade) =>
        grade <= 0
            ? new AtomicElement(0, 0)
            : new CompositeElement(
                CreateZeroLikeElement(grade - 1),
                CreateZeroLikeElement(grade - 1));

    private static GradedElement CreateLiftProvenance(GradedElement left, GradedElement right) =>
        left.Grade == right.Grade
            ? new CompositeElement(left, right)
            : left;

    private static AtomicElement CreateUnresolvedRatioResult(
        AtomicElement numerator,
        AtomicElement denominator)
    {
        var preservedSupport = Math.Max(
            1L,
            Math.Max(Math.Abs(numerator.Unit), Math.Abs(denominator.Unit)));
        var value = checked(numerator.Value * preservedSupport);

        if (denominator.Value < 0)
        {
            value = checked(-value);
        }

        return new AtomicElement(value, 0);
    }
}
