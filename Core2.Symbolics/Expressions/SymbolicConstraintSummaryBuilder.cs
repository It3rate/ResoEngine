using Core2.Elements;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicConstraintSummaryBuilder
{
    public static ConstraintParticipantSummary CreateSummary(IGrouping<string?, ConstraintEvaluationItem> group)
    {
        int satisfiedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Satisfied);
        int unsatisfiedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unsatisfied);
        int unresolvedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unresolved);

        var satisfiedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Satisfied)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
        var unsatisfiedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unsatisfied)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
        var unresolvedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unresolved)
            .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);

        return new ConstraintParticipantSummary(
            group.Key,
            satisfiedRequirements,
            unsatisfiedRequirements,
            unresolvedRequirements,
            satisfiedPreferenceWeight,
            unsatisfiedPreferenceWeight,
            unresolvedPreferenceWeight);
    }
}
