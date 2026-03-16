using Core2.Elements;

namespace Applied.Geometry.Utils;

public readonly record struct PlanarOffset(ElementGroup<Proportion> Components)
{
    public PlanarOffset(int dx, int dy)
        : this(new Proportion(dx), new Proportion(dy))
    {
    }

    public PlanarOffset(Proportion horizontal, Proportion vertical)
        : this(ElementGroup<Proportion>.Pair(horizontal, vertical))
    {
    }

    public static PlanarOffset Zero => new(Proportion.Zero, Proportion.Zero);
    public static PlanarOffset Right => new(1, 0);
    public static PlanarOffset Left => new(-1, 0);
    public static PlanarOffset Up => new(0, 1);
    public static PlanarOffset Down => new(0, -1);
    public static PlanarOffset UpRight => new(1, 1);
    public static PlanarOffset DownRight => new(1, -1);
    public static PlanarOffset UpLeft => new(-1, 1);
    public static PlanarOffset DownLeft => new(-1, -1);

    public Proportion Horizontal => Components[0];
    public Proportion Vertical => Components[1];
    public int Dx => PlanarValueConverter.ToInt(Horizontal);
    public int Dy => PlanarValueConverter.ToInt(Vertical);
    public bool IsZero => Horizontal.IsZero && Vertical.IsZero;

    public static PlanarOffset operator +(PlanarOffset left, PlanarOffset right) =>
        new(left.Horizontal + right.Horizontal, left.Vertical + right.Vertical);

    public static PlanarOffset operator *(PlanarOffset offset, Proportion scale) =>
        new(offset.Horizontal * scale, offset.Vertical * scale);

    public static PlanarOffset operator *(Proportion scale, PlanarOffset offset) => offset * scale;

    public bool Equals(PlanarOffset other) =>
        Horizontal == other.Horizontal && Vertical == other.Vertical;

    public override int GetHashCode() => HashCode.Combine(Horizontal, Vertical);

    public override string ToString() => $"{Dx:+#;-#;0},{Dy:+#;-#;0}";
}
