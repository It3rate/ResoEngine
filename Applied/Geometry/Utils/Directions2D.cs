namespace Applied.Geometry.Utils;

public readonly record struct Directions2D(int Dx, int Dy)
{
    public static Directions2D Zero => new(0, 0);
    public static Directions2D Right => new(1, 0);
    public static Directions2D Left => new(-1, 0);
    public static Directions2D Up => new(0, 1);
    public static Directions2D Down => new(0, -1);
    public static Directions2D UpRight => new(1, 1);
    public static Directions2D DownRight => new(1, -1);
    public static Directions2D UpLeft => new(-1, 1);
    public static Directions2D DownLeft => new(-1, -1);

    public bool IsZero => Dx == 0 && Dy == 0;

    public static Directions2D operator +(Directions2D left, Directions2D right) =>
        new(left.Dx + right.Dx, left.Dy + right.Dy);

    public override string ToString() =>
        $"{Dx:+#;-#;0},{Dy:+#;-#;0}";
}
