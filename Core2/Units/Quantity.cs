using Core2.Algebra;
using Core2.Elements;
using Core2.Repetition;

namespace Core2.Units;

/// <summary>
/// A structural Core2 element paired with a dimensional signature.
/// The structure describes shape; the unit signature describes quantity kind.
/// </summary>
public readonly record struct Quantity<T>(T Value, UnitSignature Signature, UnitChoice? PreferredUnit = null)
    where T : IElement
{
    public Quantity<T> Negate() =>
        new(ElementArithmeticResolver.Negate(Value), Signature, PreferredUnit);

    public QuantityOperationResult<T> TryAdd(Quantity<T> other)
    {
        var tensions = new List<QuantityTension>();
        if (!Signature.Equals(other.Signature))
        {
            tensions.Add(new QuantityTension(
                QuantityTensionKind.SignatureMismatch,
                $"Cannot add quantities with signatures {Signature} and {other.Signature}."));
            return new QuantityOperationResult<T>(null, tensions);
        }

        UnitChoice? preferredUnit = ResolvePreferredUnit(other, tensions);
        return new QuantityOperationResult<T>(
            new Quantity<T>(ElementArithmeticResolver.Add(Value, other.Value), Signature, preferredUnit),
            tensions);
    }

    public QuantityOperationResult<T> TrySubtract(Quantity<T> other) =>
        TryAdd(other.Negate());

    public Quantity<T> Multiply(Quantity<T> other) =>
        new(
            ElementArithmeticResolver.Multiply(Value, other.Value),
            Signature.Multiply(other.Signature));

    public Quantity<T> RepeatAdditive(int count)
    {
        if (count < 0)
        {
            return RepeatAdditive(-count).Negate();
        }

        var trace = RepetitionEngine.RepeatAdditive(Value, count);
        return new Quantity<T>(trace.Result, Signature, PreferredUnit);
    }

    public QuantityOperationResult<T> TryPow(int exponent)
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

        var trace = RepetitionEngine.RepeatMultiplicative(Value, exponent);

        return new QuantityOperationResult<T>(
            new Quantity<T>(trace.Result, Signature.Pow(exponent)),
            []);
    }

    public PowerResult<Quantity<T>> TryPow(Proportion exponent)
    {
        if (!PowerEngine.TryNormalize(exponent, out _, out var error))
        {
            return new PowerResult<Quantity<T>>(
                [],
                default,
                [new PowerTension(PowerTensionKind.InvalidExponent, error)]);
        }

        var valueResult = QuantityPowerResolver.TryPow(Value, exponent);
        if (!valueResult.Succeeded)
        {
            UnitSignature failedSignature = Signature;
            UnitChoice? failedPreferredUnit = PreferredUnit;
            return new PowerResult<Quantity<T>>(
                valueResult.Branches.Map(candidate => new Quantity<T>(candidate, failedSignature, failedPreferredUnit)),
                valueResult.Tensions);
        }

        UnitSignature poweredSignature;
        try
        {
            poweredSignature = Signature.Pow(exponent);
        }
        catch (ArgumentException exception)
        {
            return new PowerResult<Quantity<T>>(
                [],
                default,
                [new PowerTension(PowerTensionKind.InvalidExponent, exception.Message)]);
        }

        UnitChoice? preferredUnit = PreferredUnit is not null && PreferredUnit.CanExpress(poweredSignature)
            ? PreferredUnit
            : null;

        Quantity<T> ToQuantity(T candidate) => new(candidate, poweredSignature, preferredUnit);

        var branches = valueResult.Branches.Map(ToQuantity);
        return new PowerResult<Quantity<T>>(branches, valueResult.Tensions);
    }

    public override string ToString() =>
        PreferredUnit is null
            ? $"{Value} [{Signature}]"
            : $"{Value} {PreferredUnit.Symbol}";

    private UnitChoice? ResolvePreferredUnit(Quantity<T> other, List<QuantityTension> tensions)
    {
        if (PreferredUnit is null)
        {
            return other.PreferredUnit;
        }

        if (other.PreferredUnit is null)
        {
            return PreferredUnit;
        }

        if (Equals(PreferredUnit, other.PreferredUnit))
        {
            return PreferredUnit;
        }

        tensions.Add(new QuantityTension(
            QuantityTensionKind.PreferredUnitMismatch,
            $"Added quantities share the signature {Signature} but prefer different named units: {PreferredUnit.Symbol} and {other.PreferredUnit.Symbol}."));
        return null;
    }
}
