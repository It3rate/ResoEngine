namespace Applied.Geometry.Utils;

public sealed record EquationProgram(
    IReadOnlyList<PlanarSegmentDefinition> Equations,
    IReadOnlyList<EquationCommand> Loop,
    IReadOnlyList<EquationCommand>? Prelude = null)
{
    public IEnumerable<EquationCommand> EnumerateCommands(int repeats)
    {
        if (Prelude is not null)
        {
            foreach (var command in Prelude)
            {
                yield return command;
            }
        }

        for (int repeat = 0; repeat < repeats; repeat++)
        {
            foreach (var command in Loop)
            {
                yield return command;
            }
        }
    }

    public IEnumerable<EquationCommand> EnumerateLoopForever()
    {
        while (true)
        {
            foreach (var command in Loop)
            {
                yield return command;
            }
        }
    }
}
