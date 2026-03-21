using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public static class SymbolicConstraintNegotiator
{
    public static ConstraintNegotiationResult Negotiate(
        SymbolicTerm term,
        SymbolicEnvironment? environment = null,
        ISymbolicStructuralContext? structuralContext = null) =>
        Negotiate(SymbolicConstraintEvaluator.Evaluate(term, environment, structuralContext));

    public static ConstraintNegotiationResult Negotiate(ConstraintSetEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        if (evaluation.HasRequirementFailure)
        {
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Failed,
                null,
                [],
                null,
                "One or more hard requirements are unsatisfied.");
        }

        var requirementCandidateItems = evaluation.Items
            .Where(item => item.IsRequirement && item.CandidateFamily is not null)
            .ToArray();
        var preferenceCandidateItems = evaluation.Items
            .Where(item => item.IsPreference && item.CandidateFamily is not null)
            .ToArray();
        var blockedRequirementItems = evaluation.Items
            .Where(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unresolved && item.CandidateFamily is null)
            .ToArray();

        if (blockedRequirementItems.Length > 0)
        {
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Blocked,
                null,
                [],
                null,
                blockedRequirementItems[0].Note ?? "A hard requirement remains unresolved without candidate alternatives.");
        }

        if (requirementCandidateItems.Length == 0 && preferenceCandidateItems.Length == 0)
        {
            return evaluation.IsFullyResolved
                ? new ConstraintNegotiationResult(
                    evaluation,
                    ConstraintNegotiationStatus.Resolved,
                    null,
                    [],
                    null,
                    "No alternative candidate negotiation was required.")
                : new ConstraintNegotiationResult(
                    evaluation,
                    ConstraintNegotiationStatus.Blocked,
                    null,
                    [],
                    null,
                    "Unresolved constraints remain, but they do not currently expose explicit candidate families.");
        }

        var candidateMap = SymbolicConstraintNegotiationSupport.BuildCandidateMap(requirementCandidateItems, preferenceCandidateItems);
        if (candidateMap.Count == 0)
        {
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Failed,
                null,
                [],
                null,
                "No common candidate satisfies the active hard requirements.");
        }

        var candidates = candidateMap.Values
            .OrderByDescending(candidate => candidate.RequirementSupportCount)
            .ThenByDescending(candidate => candidate.PreferenceWeight.Fold().Value)
            .ThenByDescending(candidate => candidate.PreferenceSupportCount)
            .ThenBy(candidate => CanonicalSymbolicSerializer.Serialize(candidate.Candidate), StringComparer.Ordinal)
            .ToArray();

        if (candidates.Length == 1)
        {
            var only = candidates[0];
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Selected,
                only.Candidate,
                candidates,
                SymbolicConstraintNegotiationSupport.BuildPreservedCandidateFamily(candidates, only.Candidate),
                "A single lawful candidate remains after applying hard requirements and preference support.");
        }

        var best = candidates[0];
        bool hasUniqueBest = candidates.Length == 1 || !SymbolicConstraintNegotiationSupport.HasEquivalentRank(best, candidates[1]);
        if (hasUniqueBest)
        {
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Selected,
                best.Candidate,
                candidates,
                SymbolicConstraintNegotiationSupport.BuildPreservedCandidateFamily(candidates, best.Candidate),
                "Preference support selected one candidate from the remaining lawful family.");
        }

        var preserved = SymbolicConstraintNegotiationSupport.BuildPreservedCandidateFamily(candidates, null);
        return new ConstraintNegotiationResult(
            evaluation,
            ConstraintNegotiationStatus.PreservedCandidates,
            null,
            candidates,
            preserved,
            "Several lawful candidates remain tied, so the candidate family is preserved.");
    }
}
