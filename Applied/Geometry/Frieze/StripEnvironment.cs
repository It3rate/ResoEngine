namespace Applied.Geometry.Frieze;

public sealed record StripEnvironment(
    int MinY,
    int MaxY,
    IReadOnlySet<StripPathEdge> OccupiedEdges)
{
    public static StripEnvironment Create(int minY, int maxY) =>
        new(minY, maxY, new HashSet<StripPathEdge>());

    public bool Contains(StripPathEdge edge) => OccupiedEdges.Contains(edge.Normalize());

    public bool ContainsY(int y) => y >= MinY && y <= MaxY;

    public StripEnvironment WithAddedEdges(IEnumerable<StripPathEdge> edges)
    {
        HashSet<StripPathEdge> occupied = [.. OccupiedEdges];
        foreach (var edge in edges)
        {
            occupied.Add(edge.Normalize());
        }

        return this with { OccupiedEdges = occupied };
    }
}
