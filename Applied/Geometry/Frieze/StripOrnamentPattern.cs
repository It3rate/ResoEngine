namespace Applied.Geometry.Frieze;

public sealed record StripOrnamentPattern(
    string Key,
    string DisplayName,
    string Description,
    int StepsPerRepeat,
    int DefaultRepeats,
    IReadOnlyList<StripOrnamentStrand> Strands)
{
    public string? CallPattern { get; init; }

    public StripEquationProgram? Program { get; init; }

    public int TotalSteps(int repeats) => Math.Max(0, repeats) * StepsPerRepeat;
}
