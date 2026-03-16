namespace Applied.Geometry.Frieze;

public readonly record struct StripPathEdge(StripPoint Start, StripPoint End)
{
    public StripPathEdge Normalize() =>
        Compare(Start, End) <= 0
            ? this
            : new StripPathEdge(End, Start);

    private static int Compare(StripPoint left, StripPoint right)
    {
        int x = left.X.CompareTo(right.X);
        return x != 0 ? x : left.Y.CompareTo(right.Y);
    }
}
