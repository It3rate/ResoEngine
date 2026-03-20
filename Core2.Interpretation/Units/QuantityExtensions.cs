using Core2.Elements;
using Core2.Units;

namespace Core2.Interpretation.Units;

public static class QuantityExtensions
{
    public static Quantity<T> AsQuantity<T>(this T value, UnitSignature signature, UnitChoice? preferredUnit = null)
        where T : IElement =>
        new(value, signature, preferredUnit);
}
