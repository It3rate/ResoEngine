using Core2.Elements;
using Core2.Units;

namespace Core2.Resolution;

public static class QuantityResolutionExtensions
{
    public static LayeredQuantity ToLayered(
        this Quantity<Scalar> quantity,
        ResolutionLadder ladder,
        bool preserveZeroDigits = true) =>
        ladder.Decompose(quantity, preserveZeroDigits);

    public static ResolutionReading ReadAt(
        this Quantity<Scalar> quantity,
        ResolutionFrame frame,
        ResolutionQuantizationRule rule = ResolutionQuantizationRule.Nearest) =>
        new LayeredQuantity([new ResolutionComponent(frame, quantity.Value / frame.Grain)], quantity.Signature, quantity.PreferredUnit)
            .ReadAt(frame, rule);
}
