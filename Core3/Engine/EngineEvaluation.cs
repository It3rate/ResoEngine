namespace Core3.Engine;

internal static class EngineEvaluation
{
    internal static EngineElementOutcome Lower(GradedElement element) =>
        element switch
        {
            AtomicElement atomic => EngineElementOutcome.Exact(atomic),
            CompositeElement composite => LowerComposite(composite),
            _ => EngineElementOutcome.Exact(element)
        };

    internal static EngineElementOutcome LiftOrthogonal(
        GradedElement left,
        GradedElement right)
    {
        var targetGrade = Math.Max(left.Grade, right.Grade) + 1;

        // TODO: The first sparse-lift pass accepts any same-grade pair and
        // normalizes the lifted basis. Later this can broaden to mixed-grade
        // promotion by zero-filling the missing intermediate structure and can
        // optionally validate stronger coherence rules when a caller wants a
        // more meaningful lifted interior rather than just a lawful higher
        // shell.
        if (left.Grade != right.Grade)
        {
            return EngineElementOutcome.WithTension(
                CreateZeroLikeElement(targetGrade),
                CreateLiftProvenance(left, right),
                "Orthogonal lift preserved a zero-like lifted shell because the inputs were not the same grade.");
        }

        return EngineElementOutcome.Exact(
            new CompositeElement(
                NormalizeLiftBasis(left),
                NormalizeLiftBasis(right)));
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

    private static EngineElementOutcome LowerComposite(CompositeElement composite)
    {
        if (composite.Grade == 1)
        {
            return composite.Fold();
        }

        var recessiveZero = IsZeroLikeStructure(composite.Recessive);
        var dominantZero = IsZeroLikeStructure(composite.Dominant);

        if (recessiveZero && dominantZero)
        {
            return composite.Recessive.Lower();
        }

        if (recessiveZero)
        {
            return EngineElementOutcome.Exact(composite.Dominant);
        }

        if (dominantZero)
        {
            return EngineElementOutcome.Exact(composite.Recessive);
        }

        var recessiveOutcome = composite.Recessive.Lower();
        var dominantOutcome = composite.Dominant.Lower();
        var lowered = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);
        var tension = EngineTension.CombineTension(recessiveOutcome.Tension, dominantOutcome.Tension);

        if (tension is null)
        {
            return EngineElementOutcome.Exact(lowered);
        }

        return EngineElementOutcome.WithTension(
            lowered,
            tension,
            EngineTension.CombineNotes(
                recessiveOutcome.Note,
                dominantOutcome.Note,
                "Composite lower preserved child tension."));
    }

    private static GradedElement NormalizeLiftBasis(GradedElement element) =>
        element switch
        {
            AtomicElement atomic => new AtomicElement(atomic.Value, atomic.HasResolvedUnits ? Math.Abs(atomic.Unit) : 0),
            CompositeElement composite => new CompositeElement(
                NormalizeLiftBasis(composite.Recessive),
                NormalizeLiftBasis(composite.Dominant)),
            _ => element
        };

    internal static GradedElement CreateZeroLikeElement(int grade) =>
        grade <= 0
            ? new AtomicElement(0, 0)
            : new CompositeElement(
                CreateZeroLikeElement(grade - 1),
                CreateZeroLikeElement(grade - 1));

    private static GradedElement CreateLiftProvenance(GradedElement left, GradedElement right) =>
        left.Grade == right.Grade
            ? new CompositeElement(left, right)
            : left;

    private static bool IsZeroLikeStructure(GradedElement element) =>
        element switch
        {
            AtomicElement atomic => atomic.Value == 0 && atomic.Unit == 0,
            CompositeElement composite =>
                IsZeroLikeStructure(composite.Recessive) &&
                IsZeroLikeStructure(composite.Dominant),
            _ => false
        };

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
