using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintSetEvaluation(
    SymbolicEnvironment Environment,
    ConstraintSetTerm Source,
    ConstraintSetTerm Reduced,
    IReadOnlyList<ConstraintEvaluationItem> Items,
    IReadOnlyList<ConstraintParticipantSummary> ParticipantSummaries)
{
    public bool HasRequirementFailure => Items.Any(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unsatisfied);
    public bool HasUnresolvedRequirements => Items.Any(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unresolved);
    public bool IsFullyResolved => Items.All(item => item.Truth != ConstraintTruthKind.Unresolved);
    public Proportion SatisfiedPreferenceWeight =>
        Items.Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Satisfied)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
    public Proportion UnsatisfiedPreferenceWeight =>
        Items.Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unsatisfied)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
    public Proportion UnresolvedPreferenceWeight =>
        Items.Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unresolved)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
}
