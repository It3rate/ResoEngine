using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicReductionRepetitionFamily
{
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
}
