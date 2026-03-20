using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintParticipantSummary(
    string? ParticipantName,
    int SatisfiedRequirements,
    int UnsatisfiedRequirements,
    int UnresolvedRequirements,
    Proportion SatisfiedPreferenceWeight,
    Proportion UnsatisfiedPreferenceWeight,
    Proportion UnresolvedPreferenceWeight)
{
    public Proportion TotalPreferenceWeight =>
        SatisfiedPreferenceWeight + UnsatisfiedPreferenceWeight + UnresolvedPreferenceWeight;
}
