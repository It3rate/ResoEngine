using Core2.Elements;

namespace Applied.Geometry.Utils;

public sealed class PlanarBoundsBuilder
{
    private Proportion? _minX;
    private Proportion? _maxX;
    private Proportion? _minY;
    private Proportion? _maxY;

    public void Include(PlanarPoint point) =>
        Include(point.Horizontal, point.Vertical);

    public Area Build()
    {
        Proportion minX = _minX ?? Proportion.Zero;
        Proportion maxX = _maxX ?? Proportion.Zero;
        Proportion minY = _minY ?? Proportion.Zero;
        Proportion maxY = _maxY ?? Proportion.Zero;

        return new Area(
            Axis.FromCoordinates(minX, maxX),
            Axis.FromCoordinates(minY, maxY));
    }

    private void Include(Proportion x, Proportion y)
    {
        _minX = _minX is null ? x : Proportion.Min(_minX, x);
        _maxX = _maxX is null ? x : Proportion.Max(_maxX, x);
        _minY = _minY is null ? y : Proportion.Min(_minY, y);
        _maxY = _maxY is null ? y : Proportion.Max(_maxY, y);
    }
}
