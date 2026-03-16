using Applied.Geometry.Utils;

namespace Applied.Geometry.Frieze;

public sealed record FriezePattern(
    string Key,
    string DisplayName,
    string Description,
    int StepsPerRepeat,
    int DefaultRepeats,
    IReadOnlyList<FriezeStrand> Strands)
{
    public string? CallPattern { get; init; }

    public EquationProgram? Program { get; init; }

    public int TotalSteps(int repeats) => Math.Max(0, repeats) * StepsPerRepeat;
}
