using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicReductionRepetitionFamily
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
}
