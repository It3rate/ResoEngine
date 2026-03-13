using Core2.Elements;

namespace Core2.Units;

public readonly record struct QuantityOperationResult<T>(
    Quantity<T>? Quantity,
    IReadOnlyList<QuantityTension> Tensions)
    where T : IElement
{
    public bool Succeeded => Quantity.HasValue;
}
