using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

internal static class LetterFormationGeometry
{
    private const long SimulationSupport = 1000;

    public static double ToDouble(Proportion value) => (double)value.Numerator / value.Denominator;

    public static Proportion FromDouble(double value) =>
        new((long)Math.Round(value * 1000d), 1000);

    public static double ToDirectionalComponent(Proportion value)
    {
        if (value.Numerator == 0)
        {
            return 0d;
        }

        return value.Denominator == 0
            ? value.Numerator
            : ToDouble(value);
    }

    public static (double X, double Y) Normalize(double x, double y)
    {
        double length = Math.Sqrt((x * x) + (y * y));
        if (length < 0.0001d)
        {
            return (0d, 0d);
        }

        return (x / length, y / length);
    }

    public static double Distance(PlanarPoint left, PlanarPoint right)
    {
        double dx = ToDouble(right.Horizontal - left.Horizontal);
        double dy = ToDouble(right.Vertical - left.Vertical);
        return Math.Sqrt((dx * dx) + (dy * dy));
    }

    public static double ResolveRawProjection(PlanarPoint point, Axis projection)
    {
        double projectionX = ToDirectionalComponent(projection.Dominant);
        double projectionY = ToDirectionalComponent(projection.Recessive);
        return (ToDouble(point.Horizontal) * projectionX) + (ToDouble(point.Vertical) * projectionY);
    }

    public static double ResolveRelativeProjection(LetterFormationEnvironment environment, PlanarPoint point, Axis projection)
    {
        double current = ResolveRawProjection(point, projection);
        double left = ResolveRawProjection(environment.TopLeft, projection);
        double right = ResolveRawProjection(environment.BottomRight, projection);
        double span = right - left;
        if (Math.Abs(span) < 0.0001d)
        {
            return 0d;
        }

        return (current - left) / span;
    }

    public static (double X, double Y) ResolveDirection(Axis axis) =>
        Normalize(
            ToDirectionalComponent(axis.Dominant),
            ToDirectionalComponent(axis.Recessive));

    public static PlanarOffset ToOffset(double horizontal, double vertical) =>
        new(FromDouble(horizontal), FromDouble(vertical));

    public static PlanarPoint Reexpress(PlanarPoint point) =>
        new(
            Reexpress(point.Horizontal),
            Reexpress(point.Vertical));

    public static PlanarOffset Reexpress(PlanarOffset offset) =>
        new(
            Reexpress(offset.Horizontal),
            Reexpress(offset.Vertical));

    public static PlanarPoint ClampPoint(LetterFormationEnvironment environment, PlanarPoint point) =>
        new(
            Clamp(point.Horizontal, environment.Left, environment.Right),
            Clamp(point.Vertical, environment.Top, environment.Bottom));

    public static double ClampMagnitude(double value, double maxMagnitude) =>
        Math.Clamp(value, -maxMagnitude, maxMagnitude);

    private static Proportion Reexpress(Proportion value) =>
        value.Fold().AsProportion(SimulationSupport);

    private static Proportion Clamp(Proportion value, Proportion minimum, Proportion maximum)
    {
        if (value < minimum)
        {
            return minimum;
        }

        if (value > maximum)
        {
            return maximum;
        }

        return value;
    }
}
