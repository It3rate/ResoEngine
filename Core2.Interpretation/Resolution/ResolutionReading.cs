using Core2.Elements;
using Core2.Units;

namespace Core2.Interpretation.Resolution;

public readonly record struct ResolutionReading(
    ResolutionFrame Frame,
    Scalar RawTickCount,
    Scalar TickCount,
    ResolutionQuantizationRule Rule,
    Quantity<Scalar> ExactQuantity,
    Quantity<Scalar> Representative,
    Quantity<Scalar> Residual)
{
    public bool IsExact => Residual.Value.IsZero;

    public LayeredQuantity Collapse() =>
        new([new ResolutionComponent(Frame, TickCount)], ExactQuantity.Signature, ExactQuantity.PreferredUnit);

    public override string ToString() => $"{TickCount} x {Frame.Symbol}";
}
