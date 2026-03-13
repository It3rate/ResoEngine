using Core2.Elements;
using Core2.Repetition;

namespace Core2.Units;

internal static class QuantityPowerResolver
{
    public static PowerResult<T> TryPow<T>(T value, Proportion exponent)
        where T : IElement
    {
        return typeof(T) switch
        {
            var type when type == typeof(Scalar) => Cast<T, Scalar>(PowerEngine.Pow((Scalar)(object)value, exponent)),
            var type when type == typeof(Proportion) => Cast<T, Proportion>(PowerEngine.Pow((Proportion)(object)value, exponent)),
            var type when type == typeof(Axis) => Cast<T, Axis>(PowerEngine.Pow((Axis)(object)value, exponent)),
            var type when type == typeof(Area) => new PowerResult<T>(
                [],
                default,
                [
                    new PowerTension(
                        PowerTensionKind.ShapeChangingPower,
                        "Quantity<Area> fractional powers change structural degree through fold-first semantics. Use Area.TryPow(...) directly and then re-attach units explicitly.")
                ]),
            _ => new PowerResult<T>(
                [],
                default,
                [
                    new PowerTension(
                        PowerTensionKind.ShapeChangingPower,
                        $"No fractional power resolver is registered for element type {typeof(T).Name}.")
                ]),
        };
    }

    private static PowerResult<T> Cast<T, TValue>(PowerResult<TValue> result)
        where T : IElement
        where TValue : IElement
    {
        IReadOnlyList<T> candidates = result.Candidates.Select(candidate => (T)(object)candidate).ToArray();
        T principal = result.PrincipalCandidate is null ? default! : (T)(object)result.PrincipalCandidate;
        return new PowerResult<T>(candidates, principal, result.Tensions);
    }
}
