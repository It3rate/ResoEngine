using Core2.Elements;
using ResoEngine.Core2.Support;

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
        if (count == 0)
        {
            return new Quantity<T>(ElementArithmeticResolver.Zero<T>(), Signature, PreferredUnit);
        }

        if (count < 0)
        {
            return RepeatAdditive(-count).Negate();
        }

        T result = ElementArithmeticResolver.Zero<T>();
        for (int i = 0; i < count; i++)
        {
            result = ElementArithmeticResolver.Add(result, Value);
        }

        return new Quantity<T>(result, Signature, PreferredUnit);
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

        T result = ElementArithmeticResolver.One<T>();
        for (int i = 0; i < exponent; i++)
        {
            result = ElementArithmeticResolver.Multiply(result, Value);
        }

        return new QuantityOperationResult<T>(
            new Quantity<T>(result, Signature.Pow(exponent)),
            []);
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

internal static class ElementArithmeticResolver
{
    public static T Zero<T>() where T : IElement => For<T>().Zero;

    public static T Add<T>(T left, T right) where T : IElement => For<T>().Add(left, right);

    public static T Multiply<T>(T left, T right) where T : IElement => For<T>().Multiply(left, right);

    public static T Negate<T>(T value) where T : IElement => For<T>().Negate(value);

    public static T One<T>() where T : IElement
    {
        object result = typeof(T) switch
        {
            var type when type == typeof(Scalar) => Scalar.One,
            var type when type == typeof(Proportion) => Proportion.One,
            var type when type == typeof(Axis) => Axis.One,
            var type when type == typeof(Area) => Area.One,
            _ => throw new NotSupportedException($"No multiplicative identity is registered for element type {typeof(T).Name}."),
        };

        return (T)result;
    }

    private static IArithmetic<T> For<T>() where T : IElement
    {
        object arithmetic = typeof(T) switch
        {
            var type when type == typeof(Scalar) => Scalar.Arithmetic,
            var type when type == typeof(Proportion) => Proportion.Arithmetic,
            var type when type == typeof(Axis) => Axis.Arithmetic,
            var type when type == typeof(Area) => Area.Arithmetic,
            _ => throw new NotSupportedException($"No arithmetic is registered for element type {typeof(T).Name}."),
        };

        return (IArithmetic<T>)arithmetic;
    }
}
