namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionProgramFlow
{
    public static SymbolicReductionResult ReduceProgram(
        ProgramTerm term,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext,
        Func<SymbolicTerm, SymbolicEnvironment, ISymbolicStructuralContext?, SymbolicTerm> elaborateAndReduce)
        => term switch
        {
            BindTerm bind => ReduceBind(bind, environment, structuralContext, elaborateAndReduce),
            CommitTerm commit => ReduceCommit(commit, environment, structuralContext, elaborateAndReduce),
            EmitTerm emit => new SymbolicReductionResult(environment, elaborateAndReduce(emit.Value, environment, structuralContext)),
            SequenceTerm sequence => ReduceSequence(sequence, environment, structuralContext, elaborateAndReduce),
            _ => new SymbolicReductionResult(environment, elaborateAndReduce(term, environment, structuralContext)),
        };

    private static SymbolicReductionResult ReduceBind(
        BindTerm bind,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext,
        Func<SymbolicTerm, SymbolicEnvironment, ISymbolicStructuralContext?, SymbolicTerm> elaborateAndReduce)
    {
        var reduced = elaborateAndReduce(bind.Value, environment, structuralContext);
        var next = environment.Bind(bind.Target, reduced);
        return new SymbolicReductionResult(next, reduced);
    }

    private static SymbolicReductionResult ReduceCommit(
        CommitTerm commit,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext,
        Func<SymbolicTerm, SymbolicEnvironment, ISymbolicStructuralContext?, SymbolicTerm> elaborateAndReduce)
    {
        var reduced = elaborateAndReduce(commit.Value, environment, structuralContext);
        var current = environment;
        SymbolicTerm output = reduced;

        if (reduced is ConstraintTerm constraint)
        {
            var negotiation = SymbolicConstraintNegotiator.Negotiate(constraint, environment, structuralContext);
            if (negotiation.SelectedCandidate is not null)
            {
                output = negotiation.SelectedCandidate;
                current = negotiation.Evaluation.Environment.Bind(commit.Target, output);
            }
            else if (negotiation.PreservedCandidateFamily is not null)
            {
                output = negotiation.PreservedCandidateFamily;
                current = negotiation.Evaluation.Environment.Bind(commit.Target, output);
            }
            else
            {
                output = negotiation.Evaluation.Reduced;
            }

            return new SymbolicReductionResult(current, output);
        }

        if (reduced is BranchFamilyTerm branchFamily)
        {
            if (branchFamily.Family.SelectedValue is not null)
            {
                output = branchFamily.Family.SelectedValue;
            }

            current = environment.Bind(commit.Target, output);
            return new SymbolicReductionResult(current, output);
        }

        current = environment.Bind(commit.Target, output);
        return new SymbolicReductionResult(current, output);
    }

    private static SymbolicReductionResult ReduceSequence(
        SequenceTerm sequence,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext,
        Func<SymbolicTerm, SymbolicEnvironment, ISymbolicStructuralContext?, SymbolicTerm> elaborateAndReduce)
    {
        var current = environment;
        SymbolicTerm? output = null;

        foreach (var step in sequence.Steps)
        {
            var result = ReduceProgram(step, current, structuralContext, elaborateAndReduce);
            current = result.Environment;
            output = result.Output;
        }

        return new SymbolicReductionResult(current, output);
    }
}
