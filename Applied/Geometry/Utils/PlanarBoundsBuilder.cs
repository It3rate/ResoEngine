using Core2.Elements;

namespace Applied.Geometry.Utils;

public sealed class PlanarBoundsBuilder
{
    private Area? _bounds;

    public void Include(PlanarPoint point)
    {
        var pointBounds = new Area(
            Axis.FromCoordinates(point.Horizontal, point.Horizontal),
            Axis.FromCoordinates(point.Vertical, point.Vertical));

        _bounds = _bounds is null
            ? pointBounds
            : _bounds.Envelope(pointBounds);
    }

    public Area Build() =>
        _bounds ?? new Area(
            Axis.FromCoordinates(Proportion.Zero, Proportion.Zero),
            Axis.FromCoordinates(Proportion.Zero, Proportion.Zero));
}
