using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintEvaluationItem(
    ConstraintTerm Source,
    ConstraintTerm Reduced,
    ConstraintTruthKind Truth,
    string? ParticipantName,
    Proportion? Weight)
{
    public bool IsRequirement => Source is RequirementTerm;
    public bool IsPreference => Source is PreferenceTerm;
    public Proportion WeightOrZero => Weight ?? Proportion.Zero;
}
