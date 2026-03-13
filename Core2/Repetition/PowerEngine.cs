using Core2.Elements;
using Core2.Support;

namespace Core2.Repetition;

public static class PowerEngine
{
    public static PowerResult<Scalar> Pow(
        Scalar value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null)
    {
        if (!TryNormalize(exponent, out var normalized, out var error))
        {
            return InvalidExponent<Scalar>(error);
        }

        if (normalized.Numerator < 0)
        {
            return UnsupportedNegativeExponent<Scalar>(exponent);
        }

        if (normalized.IsZero)
        {
            return new PowerResult<Scalar>([Scalar.One], Scalar.One, []);
        }

        if (normalized.IsInteger)
        {
            var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
            return new PowerResult<Scalar>([trace.Result], trace.Result, []);
        }

        var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
        if (!roots.Succeeded)
        {
            return FromInverseFailure<Scalar>(roots.Tensions);
        }

        List<Scalar> candidates = roots.Candidates
            .Select(root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result)
            .Distinct()
            .ToList();

        Scalar principal = SelectScalarPrincipal(candidates, rule, reference);
        return new PowerResult<Scalar>(candidates, principal, []);
    }

    public static PowerResult<Proportion> Pow(
        Proportion value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null)
    {
        if (!TryNormalize(exponent, out var normalized, out var error))
        {
            return InvalidExponent<Proportion>(error);
        }

        if (normalized.Numerator < 0)
        {
            return UnsupportedNegativeExponent<Proportion>(exponent);
        }

        if (normalized.IsZero)
        {
            return new PowerResult<Proportion>([Proportion.One], Proportion.One, []);
        }

        if (normalized.IsInteger)
        {
            var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
            return new PowerResult<Proportion>([trace.Result], trace.Result, []);
        }

        var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
        if (!roots.Succeeded)
        {
            return FromInverseFailure<Proportion>(roots.Tensions);
        }

        List<Proportion> candidates = roots.Candidates
            .Select(root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result)
            .Distinct()
            .ToList();

        Proportion principal = SelectProportionPrincipal(candidates, rule, reference);
        return new PowerResult<Proportion>(candidates, principal, []);
    }

    public static PowerResult<Axis> Pow(
        Axis value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null)
    {
        if (!TryNormalize(exponent, out var normalized, out var error))
        {
            return InvalidExponent<Axis>(error);
        }

        if (normalized.Numerator < 0)
        {
            return UnsupportedNegativeExponent<Axis>(exponent);
        }

        if (normalized.IsZero)
        {
            return new PowerResult<Axis>([Axis.One], Axis.One, []);
        }

        if (normalized.IsInteger)
        {
            var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
            return new PowerResult<Axis>([trace.Result], trace.Result, []);
        }

        var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
        if (!roots.Succeeded)
        {
            return FromInverseFailure<Axis>(roots.Tensions);
        }

        List<Axis> candidates = roots.Candidates
            .Select(root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result)
            .DistinctBy(candidate => RootKey(candidate))
            .ToList();

        Axis principal = SelectAxisPrincipal(candidates, rule, reference);
        return new PowerResult<Axis>(candidates, principal, []);
    }

    public static PowerResult<Axis> Pow(
        Area value,
        Proportion exponent,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null)
    {
        if (mode == AreaInverseContinuationMode.StructurePreserving)
        {
            return new PowerResult<Axis>(
                [],
                null,
                [
                    new PowerTension(
                        PowerTensionKind.ShapeChangingPower,
                        "Structure-preserving fractional powers for Area are not implemented yet. Use fold-first mode to power the folded Axis value.")
                ]);
        }

        return Pow(value.Value, exponent, rule, foldedReference);
    }

    internal static bool TryNormalize(Proportion exponent, out RationalExponent normalized, out string error)
    {
        if (!RationalExponent.TryFrom(exponent, out normalized))
        {
            error = "Fractional powers require rational exponents with integer numerator and nonzero integer denominator.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static PowerResult<T> InvalidExponent<T>(string message) =>
        new([], default, [new PowerTension(PowerTensionKind.InvalidExponent, message)]);

    private static PowerResult<T> UnsupportedNegativeExponent<T>(Proportion exponent) =>
        new([], default, [new PowerTension(PowerTensionKind.UnsupportedNegativeExponent, $"Negative exponents are not implemented yet for {exponent}.")]);

    private static PowerResult<T> FromInverseFailure<T>(IReadOnlyList<InverseContinuationTension> tensions) =>
        new(
            [],
            default,
            tensions.Select(tension => new PowerTension(PowerTensionKind.InverseContinuationFailed, tension.Message)).ToArray());

    private static Scalar SelectScalarPrincipal(
        IReadOnlyList<Scalar> candidates,
        InverseContinuationRule rule,
        Scalar? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates.OrderByDescending(candidate => candidate.Value).First(),
            InverseContinuationRule.PreferPositiveDominant => candidates.OrderByDescending(candidate => candidate.Value).First(),
            InverseContinuationRule.NearestToReference when reference.HasValue =>
                candidates.MinBy(candidate => Math.Abs(candidate.Value - reference.Value.Value))!,
            _ => candidates[0],
        };
    }

    private static Proportion SelectProportionPrincipal(
        IReadOnlyList<Proportion> candidates,
        InverseContinuationRule rule,
        Proportion? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates
                .OrderByDescending(candidate => candidate.Recessive.Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Value)
                .First(),
            InverseContinuationRule.PreferPositiveDominant => candidates
                .OrderByDescending(candidate => candidate.Dominant.Value > 0m)
                .ThenByDescending(candidate => candidate.Recessive.Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Value)
                .First(),
            InverseContinuationRule.NearestToReference when reference is not null => candidates
                .MinBy(candidate =>
                    Math.Abs(candidate.Dominant.Value - reference.Dominant.Value) +
                    Math.Abs(candidate.Recessive.Value - reference.Recessive.Value))!,
            _ => candidates[0],
        };
    }

    private static Axis SelectAxisPrincipal(
        IReadOnlyList<Axis> candidates,
        InverseContinuationRule rule,
        Axis? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates
                .OrderByDescending(candidate => candidate.Dominant.Fold().Value)
                .ThenByDescending(candidate => candidate.Recessive.Fold().Value)
                .First(),
            InverseContinuationRule.PreferPositiveDominant => candidates
                .OrderByDescending(candidate => candidate.Dominant.Fold().Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Fold().Value)
                .ThenByDescending(candidate => candidate.Recessive.Fold().Value)
                .First(),
            InverseContinuationRule.NearestToReference when reference is not null => candidates
                .MinBy(candidate =>
                    Math.Abs(candidate.Dominant.Fold().Value - reference.Dominant.Fold().Value) +
                    Math.Abs(candidate.Recessive.Fold().Value - reference.Recessive.Fold().Value))!,
            _ => candidates[0],
        };
    }

    private static (decimal Recessive, decimal Dominant) RootKey(Axis candidate) =>
        (Math.Round(candidate.Recessive.Fold().Value, 9), Math.Round(candidate.Dominant.Fold().Value, 9));
}
