using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicConstraintNegotiationSupport
{
    public static Dictionary<string, ConstraintNegotiationCandidate> BuildCandidateMap(
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

    public static bool HasEquivalentRank(ConstraintNegotiationCandidate left, ConstraintNegotiationCandidate right) =>
        left.RequirementSupportCount == right.RequirementSupportCount &&
        left.PreferenceWeight == right.PreferenceWeight &&
        left.PreferenceSupportCount == right.PreferenceSupportCount;

    public static BranchFamilyTerm BuildPreservedCandidateFamily(
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

    private static IEnumerable<string> CandidateKeys(BranchFamily<ValueTerm> family) =>
        family.Values.Select(CanonicalSymbolicSerializer.Serialize);
}
