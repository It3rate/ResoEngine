using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public static class SymbolicConstraintEvaluator
{
    public static ConstraintSetEvaluation Evaluate(SymbolicTerm term, SymbolicEnvironment? environment = null)
        => Evaluate(term, environment, null);

    public static ConstraintSetEvaluation Evaluate(
        SymbolicTerm term,
        SymbolicEnvironment? environment,
        ISymbolicStructuralContext? structuralContext)
    {
        ArgumentNullException.ThrowIfNull(term);

        var reduced = SymbolicReducer.Reduce(term, environment, structuralContext);
        var reducedConstraint = reduced.Output switch
        {
            ConstraintSetTerm set => set,
            ConstraintTerm constraint => new ConstraintSetTerm([constraint]),
            _ => throw new InvalidOperationException("Constraint evaluation requires a constraint term or a program whose final output is a constraint term."),
        };

        var sourceConstraint = term switch
        {
            ConstraintSetTerm set => set,
            ConstraintTerm constraint => new ConstraintSetTerm([constraint]),
            _ => reducedConstraint,
        };

        var items = reducedConstraint.Constraints
            .Select((constraint, index) => EvaluateConstraint(
                sourceConstraint.Constraints.ElementAtOrDefault(index) ?? constraint,
                constraint,
                structuralContext))
            .ToArray();

        var summaries = items
            .GroupBy(item => item.ParticipantName, StringComparer.Ordinal)
            .Select(SymbolicConstraintSummaryBuilder.CreateSummary)
            .OrderBy(summary => summary.ParticipantName is null ? 1 : 0)
            .ThenBy(summary => summary.ParticipantName, StringComparer.Ordinal)
            .ToArray();

        return new ConstraintSetEvaluation(
            reduced.Environment,
            sourceConstraint,
            reducedConstraint,
            items,
            summaries);
    }

    private static ConstraintEvaluationItem EvaluateConstraint(
        ConstraintTerm source,
        ConstraintTerm reduced,
        ISymbolicStructuralContext? structuralContext) =>
        reduced switch
        {
            RequirementTerm requirement => new ConstraintEvaluationItem(
                source,
                reduced,
                SymbolicConstraintRelationEvaluator.EvaluateRelation(requirement.Relation, structuralContext),
                requirement.ParticipantName,
                null),
            PreferenceTerm preference => new ConstraintEvaluationItem(
                source,
                reduced,
                SymbolicConstraintRelationEvaluator.EvaluateRelation(preference.Relation, structuralContext),
                preference.ParticipantName,
                preference.Weight),
            ConstraintSetTerm set => new ConstraintEvaluationItem(
                source,
                reduced,
                set.Constraints.All(inner => EvaluateConstraint(inner, inner, structuralContext).Truth == ConstraintTruthKind.Satisfied)
                    ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
                    : set.Constraints.Any(inner => EvaluateConstraint(inner, inner, structuralContext).Truth == ConstraintTruthKind.Unsatisfied)
                        ? new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied)
                        : new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Nested constraint family remains unresolved."),
                null,
                null),
            _ => new ConstraintEvaluationItem(
                source,
                reduced,
                new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Constraint form is not yet directly evaluable."),
                null,
                null),
        };
}
