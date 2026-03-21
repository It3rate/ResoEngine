using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicReductionRepetitionFamily
{
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
