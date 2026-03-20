using Core2.Elements;
using Core2.Units;

namespace Core2.Symbolics.Repetition;

public static class SymbolicOperationExtensions
{
    public static InverseContinuationResult<Scalar> InverseContinue(
        this Scalar value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null) =>
        InverseContinuationEngine.InverseContinue(value, degree, rule, reference);

    public static PowerResult<Scalar> TryPow(
        this Scalar value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null) =>
        PowerEngine.Pow(value, exponent, rule, reference);

    public static InverseContinuationResult<Proportion> InverseContinue(
        this Proportion value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null) =>
        InverseContinuationEngine.InverseContinue(value, degree, rule, reference);

    public static PowerResult<Proportion> TryPow(
        this Proportion value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null) =>
        PowerEngine.Pow(value, exponent, rule, reference);

    public static InverseContinuationResult<Axis> InverseContinue(
        this Axis value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null) =>
        InverseContinuationEngine.InverseContinue(value, degree, rule, reference);

    public static PowerResult<Axis> TryPow(
        this Axis value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null) =>
        PowerEngine.Pow(value, exponent, rule, reference);

    public static InverseContinuationResult<Axis> InverseContinue(
        this Area value,
        int degree,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null) =>
        InverseContinuationEngine.InverseContinue(value, degree, mode, rule, foldedReference);

    public static PowerResult<Axis> TryPow(
        this Area value,
        Proportion exponent,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null) =>
        PowerEngine.Pow(value, exponent, mode, rule, foldedReference);

    public static Quantity<T> RepeatAdditive<T>(this Quantity<T> quantity, int count)
        where T : IElement
    {
        if (count < 0)
        {
            return quantity.RepeatAdditive(-count).Negate();
        }

        var trace = RepetitionEngine.RepeatAdditive(quantity.Value, count);
        return new Quantity<T>(trace.Result, quantity.Signature, quantity.PreferredUnit);
    }

    public static QuantityOperationResult<T> TryPow<T>(this Quantity<T> quantity, int exponent)
        where T : IElement
    {
        if (exponent < 0)
        {
            return new QuantityOperationResult<T>(
                null,
                [
                    new QuantityTension(
                        QuantityTensionKind.UnsupportedExponent,
                        "Negative multiplicative exponents are not implemented yet for structural quantities.")
                ]);
        }

        var trace = RepetitionEngine.RepeatMultiplicative(quantity.Value, exponent);

        return new QuantityOperationResult<T>(
            new Quantity<T>(trace.Result, quantity.Signature.Pow(exponent), quantity.PreferredUnit),
            []);
    }

    public static PowerResult<Quantity<T>> TryPow<T>(this Quantity<T> quantity, Proportion exponent)
        where T : IElement
    {
        if (!PowerEngine.TryNormalize(exponent, out _, out var error))
        {
            return new PowerResult<Quantity<T>>(
                [],
                default,
                [new PowerTension(PowerTensionKind.InvalidExponent, error)]);
        }

        var valueResult = QuantityPowerResolver.TryPow(quantity.Value, exponent);
        if (!valueResult.Succeeded)
        {
            UnitSignature failedSignature = quantity.Signature;
            UnitChoice? failedPreferredUnit = quantity.PreferredUnit;
            return new PowerResult<Quantity<T>>(
                valueResult.Branches.Map(candidate => new Quantity<T>(candidate, failedSignature, failedPreferredUnit)),
                valueResult.Tensions);
        }

        UnitSignature poweredSignature;
        try
        {
            poweredSignature = quantity.Signature.Pow(exponent);
        }
        catch (ArgumentException exception)
        {
            return new PowerResult<Quantity<T>>(
                [],
                default,
                [new PowerTension(PowerTensionKind.InvalidExponent, exception.Message)]);
        }

        UnitChoice? preferredUnit = quantity.PreferredUnit is not null && quantity.PreferredUnit.CanExpress(poweredSignature)
            ? quantity.PreferredUnit
            : null;

        Quantity<T> ToQuantity(T candidate) => new(candidate, poweredSignature, preferredUnit);

        var branches = valueResult.Branches.Map(ToQuantity);
        return new PowerResult<Quantity<T>>(branches, valueResult.Tensions);
    }
}
