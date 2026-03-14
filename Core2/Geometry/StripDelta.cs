namespace Core2.Geometry;

public readonly record struct StripDelta(int Dx, int Dy)
{
    public static StripDelta Zero => new(0, 0);
    public static StripDelta Right => new(1, 0);
    public static StripDelta Left => new(-1, 0);
    public static StripDelta Up => new(0, 1);
    public static StripDelta Down => new(0, -1);
    public static StripDelta UpRight => new(1, 1);
    public static StripDelta DownRight => new(1, -1);
    public static StripDelta UpLeft => new(-1, 1);
    public static StripDelta DownLeft => new(-1, -1);

    public bool IsZero => Dx == 0 && Dy == 0;

    public static StripDelta operator +(StripDelta left, StripDelta right) =>
        new(left.Dx + right.Dx, left.Dy + right.Dy);

    public override string ToString() =>
        $"{Dx:+#;-#;0},{Dy:+#;-#;0}";
}
