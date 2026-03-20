using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public sealed record ConstraintRelationAssessment(
    ConstraintTruthKind Truth,
    BranchFamily<ValueTerm>? CandidateFamily = null,
    string? Note = null);
