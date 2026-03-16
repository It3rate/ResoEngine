using Core2.Elements;

namespace Applied.Geometry.Utils;

internal static class PlanarValueConverter
{
    public static int ToInt(Proportion value) => ToInt(value.Fold());

    public static int ToInt(Scalar value) =>
        decimal.ToInt32(decimal.Round(value.Value, 0, MidpointRounding.AwayFromZero));
}
