namespace Core2.Symbolics.Expressions;

public static class SymbolicInspector
{
    public static SymbolicInspectionReport Inspect(string text, SymbolicEnvironment? environment = null)
    {
        var current = environment ?? SymbolicEnvironment.Empty;

        try
        {
            var parsed = SymbolicParser.Parse(text);
            var steps = BuildSteps(parsed, current, out var finalEnvironment);
            return new SymbolicInspectionReport(text, current, parsed, steps, finalEnvironment, null);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or FormatException)
        {
            return new SymbolicInspectionReport(text, current, null, [], current, ex.Message);
        }
    }

    private static IReadOnlyList<SymbolicInspectionStep> BuildSteps(
        SymbolicTerm parsed,
        SymbolicEnvironment environment,
        out SymbolicEnvironment finalEnvironment)
    {
        var steps = parsed is SequenceTerm sequence
            ? sequence.Steps.Cast<SymbolicTerm>().ToArray()
            : [parsed];

        var results = new List<SymbolicInspectionStep>(steps.Length);
        var current = environment;

        for (int index = 0; index < steps.Length; index++)
        {
            var step = steps[index];
            var elaboration = SymbolicElaborator.Elaborate(step, current);
            var reduction = SymbolicReducer.Reduce(step, current);

            ConstraintSetEvaluation? evaluation = null;
            ConstraintNegotiationResult? negotiation = null;

            if (TryGetConstraintSubject(step, out var constraintSubject))
            {
                evaluation = SymbolicConstraintEvaluator.Evaluate(constraintSubject, current);
                negotiation = SymbolicConstraintNegotiator.Negotiate(evaluation);
            }

            results.Add(new SymbolicInspectionStep(
                index + 1,
                DescribeStep(step, index + 1),
                step,
                current,
                elaboration,
                reduction,
                evaluation,
                negotiation));

            current = reduction.Environment;
        }

        finalEnvironment = current;
        return results;
    }

    private static bool TryGetConstraintSubject(SymbolicTerm term, out SymbolicTerm constraintSubject)
    {
        switch (term)
        {
            case ConstraintTerm constraint:
                constraintSubject = constraint;
                return true;

            case EmitTerm emit when emit.Value is ConstraintTerm constraint:
                constraintSubject = constraint;
                return true;

            case BindTerm bind when bind.Value is ConstraintTerm constraint:
                constraintSubject = constraint;
                return true;

            case CommitTerm commit when commit.Value is ConstraintTerm constraint:
                constraintSubject = constraint;
                return true;

            default:
                constraintSubject = null!;
                return false;
        }
    }

    private static string DescribeStep(SymbolicTerm step, int index) =>
        step switch
        {
            BindTerm bind => $"let {bind.Name}",
            CommitTerm commit => $"commit {commit.Name}",
            EmitTerm => "emit",
            ConstraintSetTerm => "constraints",
            RequirementTerm => "require",
            PreferenceTerm => "prefer",
            _ => $"step {index}",
        };
}
