using Core2.Elements;

namespace Applied.Geometry.Utils;

public readonly record struct PlanarOffset
{
    public PlanarOffset(int dx, int dy)
        : this(new Proportion(dx), new Proportion(dy))
    {
    }

    public PlanarOffset(Proportion horizontal, Proportion vertical)
        : this(new Axis(-vertical, horizontal))
    {
    }

    public PlanarOffset(Axis value)
    {
        Value = value;
    }

    public Axis Value { get; }

    public static PlanarOffset Zero => new(Axis.Zero);
    public static PlanarOffset Right => new(Axis.One);
    public static PlanarOffset Left => new(Axis.NegativeOne);
    public static PlanarOffset Up => new(Axis.NegativeI);
    public static PlanarOffset Down => new(Axis.I);
    public static PlanarOffset UpRight => new(Axis.NegativeI + Axis.One);
    public static PlanarOffset DownRight => new(Axis.I + Axis.One);
    public static PlanarOffset UpLeft => new(Axis.NegativeI + Axis.NegativeOne);
    public static PlanarOffset DownLeft => new(Axis.I + Axis.NegativeOne);

    public Proportion Horizontal => Value.Dominant;
    public Proportion Vertical => -Value.Recessive;
    public int Dx => PlanarValueConverter.ToInt(Horizontal);
    public int Dy => PlanarValueConverter.ToInt(Vertical);
    public bool IsZero => Horizontal.IsZero && Vertical.IsZero;

    public static PlanarOffset operator +(PlanarOffset left, PlanarOffset right) =>
        new(left.Value + right.Value);

    public static PlanarOffset operator *(PlanarOffset offset, Proportion scale) =>
        new(offset.Horizontal * scale, offset.Vertical * scale);

    public static PlanarOffset operator *(Proportion scale, PlanarOffset offset) => offset * scale;

    public bool Equals(PlanarOffset other) =>
        Horizontal == other.Horizontal && Vertical == other.Vertical;

    public override int GetHashCode() => HashCode.Combine(Horizontal, Vertical);

    public override string ToString() => $"{Dx:+#;-#;0},{Dy:+#;-#;0}";
}
