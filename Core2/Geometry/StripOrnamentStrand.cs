namespace Core2.Geometry;

public sealed record StripOrnamentStrand(
    string Name,
    string Rhythm,
    string Description,
    IReadOnlyList<StripDelta> Cycle)
{
    public StripDelta DeltaAt(int stepIndex)
    {
        if (Cycle.Count == 0)
        {
            return StripDelta.Zero;
        }

        int index = ((stepIndex % Cycle.Count) + Cycle.Count) % Cycle.Count;
        return Cycle[index];
    }
}
