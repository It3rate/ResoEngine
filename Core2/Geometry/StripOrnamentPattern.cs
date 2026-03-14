namespace Core2.Geometry;

public sealed record StripOrnamentPattern(
    string Key,
    string DisplayName,
    string Description,
    int StepsPerRepeat,
    int DefaultRepeats,
    IReadOnlyList<StripOrnamentStrand> Strands)
{
    public int TotalSteps(int repeats) => Math.Max(0, repeats) * StepsPerRepeat;
}
