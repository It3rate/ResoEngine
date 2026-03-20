using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintNegotiationCandidate(
    ValueTerm Candidate,
    Proportion PreferenceWeight,
    int RequirementSupportCount,
    int PreferenceSupportCount);
