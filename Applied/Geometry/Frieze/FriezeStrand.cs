using Applied.Geometry.Utils;

namespace Applied.Geometry.Frieze;

public sealed record FriezeStrand(
    string Name,
    string Rhythm,
    string Description,
    IReadOnlyList<PlanarOffset> Cycle)
{
    public PlanarOffset DeltaAt(int stepIndex)
    {
        if (Cycle.Count == 0)
        {
            return PlanarOffset.Zero;
        }

        int index = ((stepIndex % Cycle.Count) + Cycle.Count) % Cycle.Count;
        return Cycle[index];
    }
}
