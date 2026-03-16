using Applied.Geometry.Utils;

namespace Applied.Geometry.Frieze;

public sealed record StripOrnamentStrand(
    string Name,
    string Rhythm,
    string Description,
    IReadOnlyList<Directions2D> Cycle)
{
    public Directions2D DeltaAt(int stepIndex)
    {
        if (Cycle.Count == 0)
        {
            return Directions2D.Zero;
        }

        int index = ((stepIndex % Cycle.Count) + Cycle.Count) % Cycle.Count;
        return Cycle[index];
    }
}
