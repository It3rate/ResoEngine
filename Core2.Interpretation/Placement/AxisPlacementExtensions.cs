using Core2.Elements;

namespace Core2.Interpretation.Placement;

public static class AxisPlacementExtensions
{
    public static PositionedAxis PlaceAt(this Axis axis, Proportion position)
    {
        ArgumentNullException.ThrowIfNull(axis);
        return new PositionedAxis(axis, position);
    }

    public static PositionedAxis PlaceApplied(this CarrierPinSite site)
    {
        ArgumentNullException.ThrowIfNull(site);
        return site.Applied.PlaceAt(site.HostPosition);
    }
}
