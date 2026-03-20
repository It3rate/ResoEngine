namespace Core2.Symbolics.Expressions;

public sealed record ConstraintNegotiationResult(
    ConstraintSetEvaluation Evaluation,
    ConstraintNegotiationStatus Status,
    ValueTerm? SelectedCandidate,
    IReadOnlyList<ConstraintNegotiationCandidate> Candidates,
    BranchFamilyTerm? PreservedCandidateFamily,
    string? Note);
