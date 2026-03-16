namespace Applied.Geometry.Utils;

public readonly record struct PlanarPathEdge(PlanarPoint Start, PlanarPoint End)
{
    public PlanarPathEdge Normalize() =>
        Compare(Start, End) <= 0
            ? this
            : new PlanarPathEdge(End, Start);

    public bool Equals(PlanarPathEdge other) =>
        Start.Equals(other.Start) && End.Equals(other.End);

    public override int GetHashCode() => HashCode.Combine(Start, End);

    private static int Compare(PlanarPoint left, PlanarPoint right)
    {
        int x = left.Horizontal.CompareTo(right.Horizontal);
        return x != 0 ? x : left.Vertical.CompareTo(right.Vertical);
    }
}
