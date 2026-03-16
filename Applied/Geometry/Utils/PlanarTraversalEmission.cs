using Core2.Elements;

namespace Applied.Geometry.Utils;

public sealed record PlanarTraversalEmission
{
    public static PlanarTraversalEmission Empty { get; } = new([]);

    public PlanarTraversalEmission(IReadOnlyList<PlanarTraversalMotion> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        Parts = parts.ToArray();
    }

    public IReadOnlyList<PlanarTraversalMotion> Parts { get; }

    public bool IsEmpty => Parts.Count == 0;

    public PlanarOffset NetDelta =>
        Parts.Aggregate(PlanarOffset.Zero, static (sum, part) => sum + part.Delta);
}
