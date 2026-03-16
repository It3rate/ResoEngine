using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.Frieze;

public sealed record FriezeEnvironment(
    Axis VerticalBounds,
    IReadOnlySet<PlanarPathEdge> OccupiedEdges)
{
    public static FriezeEnvironment Create(int minY, int maxY) =>
        new(Axis.FromCoordinates(minY, maxY), new HashSet<PlanarPathEdge>());

    public int MinY => PlanarValueConverter.ToInt(VerticalBounds.Left);
    public int MaxY => PlanarValueConverter.ToInt(VerticalBounds.Right);

    public bool Contains(PlanarPathEdge edge) => OccupiedEdges.Contains(edge.Normalize());

    public bool Contains(PlanarPoint point) => ContainsY(point.Vertical.Fold());

    public bool ContainsY(int y) => ContainsY(new Scalar(y));

    public bool ContainsY(Scalar y) => VerticalBounds.Contains(y);

    public FriezeEnvironment WithAddedEdges(IEnumerable<PlanarPathEdge> edges)
    {
        HashSet<PlanarPathEdge> occupied = [.. OccupiedEdges];
        foreach (var edge in edges)
        {
            occupied.Add(edge.Normalize());
        }

        return this with { OccupiedEdges = occupied };
    }
}
