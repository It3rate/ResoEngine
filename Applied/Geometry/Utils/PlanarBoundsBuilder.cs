using Core2.Elements;

namespace Applied.Geometry.Utils;

public sealed class PlanarBoundsBuilder
{
    private Scalar? _minX;
    private Scalar? _maxX;
    private Scalar? _minY;
    private Scalar? _maxY;

    public void Include(PlanarPoint point) =>
        Include(point.Horizontal.Fold(), point.Vertical.Fold());

    public Area Build()
    {
        Scalar minX = _minX ?? Scalar.Zero;
        Scalar maxX = _maxX ?? Scalar.Zero;
        Scalar minY = _minY ?? Scalar.Zero;
        Scalar maxY = _maxY ?? Scalar.Zero;

        return new Area(
            Axis.FromCoordinates(minX, maxX),
            Axis.FromCoordinates(minY, maxY));
    }

    private void Include(Scalar x, Scalar y)
    {
        _minX = _minX is null ? x : Scalar.Min(_minX.Value, x);
        _maxX = _maxX is null ? x : Scalar.Max(_maxX.Value, x);
        _minY = _minY is null ? y : Scalar.Min(_minY.Value, y);
        _maxY = _maxY is null ? y : Scalar.Max(_maxY.Value, y);
    }
}
