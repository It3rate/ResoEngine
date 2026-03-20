using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintEvaluationItem(
    ConstraintTerm Source,
    ConstraintTerm Reduced,
    ConstraintRelationAssessment Assessment,
    string? ParticipantName,
    Proportion? Weight)
{
    public bool IsRequirement => Source is RequirementTerm;
    public bool IsPreference => Source is PreferenceTerm;
    public ConstraintTruthKind Truth => Assessment.Truth;
    public BranchFamily<ValueTerm>? CandidateFamily => Assessment.CandidateFamily;
    public string? Note => Assessment.Note;
    public Proportion WeightOrZero => Weight ?? Proportion.Zero;
}
