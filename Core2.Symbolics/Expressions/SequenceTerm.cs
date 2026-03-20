namespace Core2.Symbolics.Expressions;

public sealed record SequenceTerm : ProgramTerm
{
    public SequenceTerm(IReadOnlyList<ProgramTerm> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        Steps = steps.ToArray();
    }

    public IReadOnlyList<ProgramTerm> Steps { get; }
}
