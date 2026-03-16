using Core2.Elements;

namespace Applied.Geometry.Utils;

public readonly record struct PlanarPoint(ElementGroup<Proportion> Components)
{
    public PlanarPoint(int x, int y)
        : this(new Proportion(x), new Proportion(y))
    {
    }

    public PlanarPoint(Proportion horizontal, Proportion vertical)
        : this(ElementGroup<Proportion>.Pair(horizontal, vertical))
    {
    }

    public static PlanarPoint Origin => new(Proportion.Zero, Proportion.Zero);

    public Proportion Horizontal => Components[0];
    public Proportion Vertical => Components[1];
    public int X => PlanarValueConverter.ToInt(Horizontal);
    public int Y => PlanarValueConverter.ToInt(Vertical);

    public static PlanarPoint operator +(PlanarPoint point, PlanarOffset delta) =>
        new(point.Horizontal + delta.Horizontal, point.Vertical + delta.Vertical);

    public bool Equals(PlanarPoint other) =>
        Horizontal == other.Horizontal && Vertical == other.Vertical;

    public override int GetHashCode() => HashCode.Combine(Horizontal, Vertical);

    public override string ToString() => $"({X}, {Y})";
}
