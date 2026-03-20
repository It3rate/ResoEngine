using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public static class SymbolicConstraintNegotiator
{
    public static ConstraintNegotiationResult Negotiate(SymbolicTerm term, SymbolicEnvironment? environment = null) =>
        Negotiate(SymbolicConstraintEvaluator.Evaluate(term, environment));

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

        var candidateMap = BuildCandidateMap(requirementCandidateItems, preferenceCandidateItems);
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
                BuildPreservedCandidateFamily(candidates, only.Candidate),
                "A single lawful candidate remains after applying hard requirements and preference support.");
        }

        var best = candidates[0];
        bool hasUniqueBest = candidates.Length == 1 || !HasEquivalentRank(best, candidates[1]);
        if (hasUniqueBest)
        {
            return new ConstraintNegotiationResult(
                evaluation,
                ConstraintNegotiationStatus.Selected,
                best.Candidate,
                candidates,
                BuildPreservedCandidateFamily(candidates, best.Candidate),
                "Preference support selected one candidate from the remaining lawful family.");
        }

        var preserved = BuildPreservedCandidateFamily(candidates, null);
        return new ConstraintNegotiationResult(
            evaluation,
            ConstraintNegotiationStatus.PreservedCandidates,
            null,
            candidates,
            preserved,
            "Several lawful candidates remain tied, so the candidate family is preserved.");
    }

    private static Dictionary<string, ConstraintNegotiationCandidate> BuildCandidateMap(
        IReadOnlyList<ConstraintEvaluationItem> requirementCandidateItems,
        IReadOnlyList<ConstraintEvaluationItem> preferenceCandidateItems)
    {
        HashSet<string>? feasibleKeys = null;

        foreach (var item in requirementCandidateItems)
        {
            var keys = CandidateKeys(item.CandidateFamily!).ToHashSet(StringComparer.Ordinal);
            feasibleKeys = feasibleKeys is null ? keys : feasibleKeys.Intersect(keys, StringComparer.Ordinal).ToHashSet(StringComparer.Ordinal);
        }

        if (feasibleKeys is null)
        {
            feasibleKeys = preferenceCandidateItems
                .SelectMany(item => CandidateKeys(item.CandidateFamily!))
                .ToHashSet(StringComparer.Ordinal);
        }

        var candidateLookup = new Dictionary<string, ValueTerm>(StringComparer.Ordinal);
        foreach (var item in requirementCandidateItems.Concat(preferenceCandidateItems))
        {
            foreach (var value in item.CandidateFamily!.Values)
            {
                var key = CanonicalSymbolicSerializer.Serialize(value);
                if (feasibleKeys.Contains(key))
                {
                    candidateLookup[key] = value;
                }
            }
        }

        var result = new Dictionary<string, ConstraintNegotiationCandidate>(StringComparer.Ordinal);
        foreach (var key in feasibleKeys)
        {
            if (!candidateLookup.TryGetValue(key, out var value))
            {
                continue;
            }

            int requirementSupportCount = requirementCandidateItems.Count(item => CandidateKeys(item.CandidateFamily!).Contains(key, StringComparer.Ordinal));
            int preferenceSupportCount = preferenceCandidateItems.Count(item => CandidateKeys(item.CandidateFamily!).Contains(key, StringComparer.Ordinal));
            var preferenceWeight = preferenceCandidateItems
                .Where(item => CandidateKeys(item.CandidateFamily!).Contains(key, StringComparer.Ordinal))
                .Aggregate(Proportion.Zero, (sum, item) => sum + item.WeightOrZero);

            result[key] = new ConstraintNegotiationCandidate(
                value,
                preferenceWeight,
                requirementSupportCount,
                preferenceSupportCount);
        }

        return result;
    }

    private static IEnumerable<string> CandidateKeys(BranchFamily<ValueTerm> family) =>
        family.Values.Select(CanonicalSymbolicSerializer.Serialize);

    private static bool HasEquivalentRank(ConstraintNegotiationCandidate left, ConstraintNegotiationCandidate right) =>
        left.RequirementSupportCount == right.RequirementSupportCount &&
        left.PreferenceWeight == right.PreferenceWeight &&
        left.PreferenceSupportCount == right.PreferenceSupportCount;

    private static BranchFamilyTerm BuildPreservedCandidateFamily(
        IReadOnlyList<ConstraintNegotiationCandidate> candidates,
        ValueTerm? selectedCandidate)
    {
        int? selectedIndex = null;
        if (selectedCandidate is not null)
        {
            selectedIndex = Array.FindIndex(
                candidates.ToArray(),
                candidate => Equals(candidate.Candidate, selectedCandidate));
        }

        var family = BranchFamily<ValueTerm>.FromValues(
            BranchOrigin.Extension,
            BranchSemantics.Alternative,
            BranchDirection.Structural,
            candidates.Select(candidate => candidate.Candidate).ToArray(),
            selectedIndex,
            selectedIndex.HasValue ? BranchSelectionMode.Principal : BranchSelectionMode.None);

        return new BranchFamilyTerm(family);
    }
}
