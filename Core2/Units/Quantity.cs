using Core2.Algebra;
using Core2.Elements;
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
