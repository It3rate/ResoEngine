namespace Core2.Symbolics.Expressions;

internal static class SymbolicElaborationProgramFlow
{
    public static SymbolicElaborationResult ElaborateProgram(
        ProgramTerm term,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborateTerm)
        => term switch
        {
            BindTerm bind => ElaborateBind(bind, environment, elaborateTerm),
            CommitTerm commit => ElaborateCommit(commit, environment, elaborateTerm),
            EmitTerm emit => new SymbolicElaborationResult(environment, elaborateTerm(emit.Value, environment)),
            SequenceTerm sequence => ElaborateSequence(sequence, environment, elaborateTerm),
            _ => new SymbolicElaborationResult(environment, term),
        };

    private static SymbolicElaborationResult ElaborateBind(
        BindTerm bind,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborateTerm)
    {
        var value = elaborateTerm(bind.Value, environment);
        var next = environment.Bind(bind.Target, value);
        return new SymbolicElaborationResult(next, value);
    }

    private static SymbolicElaborationResult ElaborateCommit(
        CommitTerm commit,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborateTerm)
    {
        var value = elaborateTerm(commit.Value, environment);
        var next = environment.Bind(commit.Target, value);
        return new SymbolicElaborationResult(next, value);
    }

    private static SymbolicElaborationResult ElaborateSequence(
        SequenceTerm sequence,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborateTerm)
    {
        var current = environment;
        SymbolicTerm? output = null;

        foreach (var step in sequence.Steps)
        {
            var result = ElaborateProgram(step, current, elaborateTerm);
            current = result.Environment;
            output = result.Output;
        }

        return new SymbolicElaborationResult(current, output);
    }
}
