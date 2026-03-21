using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionRepetitionFamily
{
    public static SymbolicTerm ReduceContinue(
        ContinueTerm continuation,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var frame = (ValueTerm)reduce(continuation.Frame);
        var value = (ValueTerm)reduce(continuation.Value);

        if (SymbolicReductionLiterals.TryGetAxisLiteral(frame, out var frameAxis))
        {
            if (SymbolicReductionLiterals.TryGetScalarLiteral(value, out var scalar))
            {
                return new ElementLiteralTerm(frameAxis.Continue(scalar, continuation.Law).Value);
            }

            if (SymbolicReductionLiterals.TryGetProportionLiteral(value, out var proportion))
            {
                return new ElementLiteralTerm(frameAxis.Continue(proportion, continuation.Law).Value);
            }
        }

        return new ContinueTerm(frame, value, continuation.Law);
    }

    public static SymbolicTerm ReducePower(
        PowerTerm power,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var @base = (ValueTerm)reduce(power.Base);
        var reference = power.Reference is null ? null : (ValueTerm)reduce(power.Reference);
        if (@base is ElementLiteralTerm literal &&
            TryReducePower(literal.Value, power.Exponent, power.Rule, reference, out var reduced))
        {
            return reduced;
        }

        return new PowerTerm(@base, power.Exponent, power.Rule, reference);
    }

    public static SymbolicTerm ReduceInverseContinuation(
        InverseContinueTerm inverse,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var source = (ValueTerm)reduce(inverse.Source);
        var reference = inverse.Reference is null ? null : (ValueTerm)reduce(inverse.Reference);
        if (source is ElementLiteralTerm literal &&
            TryReduceInverseContinuation(literal.Value, inverse.Degree, inverse.Rule, reference, out var reduced))
        {
            return reduced;
        }

        return new InverseContinueTerm(source, inverse.Degree, inverse.Rule, reference);
    }

    private static bool TryReducePower(
        IElement value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        switch (value)
        {
            case Scalar scalar:
                return TryReducePowerScalar(scalar, exponent, rule, reference, out reduced);

            case Proportion proportion:
                return TryReducePowerProportion(proportion, exponent, rule, reference, out reduced);

            case Axis axis:
                return TryReducePowerAxis(axis, exponent, rule, reference, out reduced);

            case Area area:
                return TryReducePowerArea(area, exponent, rule, reference, out reduced);

            default:
                reduced = null!;
                return false;
        }
    }

    private static bool TryReduceInverseContinuation(
        IElement value,
        Proportion degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        if (!TryGetIntegerDegree(degree, out int normalizedDegree))
        {
            reduced = null!;
            return false;
        }

        switch (value)
        {
            case Scalar scalar:
                return TryReduceInverseScalar(scalar, normalizedDegree, rule, reference, out reduced);

            case Proportion proportion:
                return TryReduceInverseProportion(proportion, normalizedDegree, rule, reference, out reduced);

            case Axis axis:
                return TryReduceInverseAxis(axis, normalizedDegree, rule, reference, out reduced);

            case Area area:
                return TryReduceInverseArea(area, normalizedDegree, rule, reference, out reduced);

            default:
                reduced = null!;
                return false;
        }
    }

    private static bool TryProjectPowerResult<T>(PowerResult<T> result, out SymbolicTerm reduced)
        where T : IElement
    {
        if (!result.Succeeded)
        {
            reduced = null!;
            return false;
        }

        if (result.PrincipalCandidate is not null && result.Candidates.Count == 1)
        {
            reduced = new ElementLiteralTerm(result.PrincipalCandidate);
            return true;
        }

        reduced = new BranchFamilyTerm(
            result.Branches.Map<ValueTerm>(candidate => new ElementLiteralTerm(candidate)));
        return true;
    }

    private static bool TryProjectInverseContinuationResult<T>(
        InverseContinuationResult<T> result,
        out SymbolicTerm reduced)
        where T : IElement
    {
        if (!result.Succeeded)
        {
            reduced = null!;
            return false;
        }

        if (result.PrincipalCandidate is not null && result.Candidates.Count == 1)
        {
            reduced = new ElementLiteralTerm(result.PrincipalCandidate);
            return true;
        }

        reduced = new BranchFamilyTerm(
            result.Branches.Map<ValueTerm>(candidate => new ElementLiteralTerm(candidate)));
        return true;
    }

    private static bool TryGetIntegerDegree(Proportion degree, out int normalizedDegree)
    {
        if (degree.Denominator != 1 || degree.Numerator <= 0 || degree.Numerator > int.MaxValue)
        {
            normalizedDegree = 0;
            return false;
        }

        normalizedDegree = (int)degree.Numerator;
        return true;
    }

    private static bool TryReducePowerScalar(
        Scalar value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Scalar? typedReference = null;
        Scalar scalarReference = default;
        if (reference is not null && !SymbolicReductionLiterals.TryGetScalarLiteral(reference, out scalarReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = scalarReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerProportion(
        Proportion value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Proportion? typedReference = null;
        Proportion proportionReference = null!;
        if (reference is not null && !SymbolicReductionLiterals.TryGetProportionLiteral(reference, out proportionReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = proportionReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerAxis(
        Axis value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !SymbolicReductionLiterals.TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerArea(
        Area value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !SymbolicReductionLiterals.TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, AreaInverseContinuationMode.FoldFirst, rule, typedReference), out reduced);
    }

    private static bool TryReduceInverseScalar(
        Scalar value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Scalar? typedReference = null;
        Scalar scalarReference = default;
        if (reference is not null && !SymbolicReductionLiterals.TryGetScalarLiteral(reference, out scalarReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = scalarReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseProportion(
        Proportion value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Proportion? typedReference = null;
        Proportion proportionReference = null!;
        if (reference is not null && !SymbolicReductionLiterals.TryGetProportionLiteral(reference, out proportionReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = proportionReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseAxis(
        Axis value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !SymbolicReductionLiterals.TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseArea(
        Area value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !SymbolicReductionLiterals.TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, AreaInverseContinuationMode.FoldFirst, rule, typedReference),
            out reduced);
    }
}
