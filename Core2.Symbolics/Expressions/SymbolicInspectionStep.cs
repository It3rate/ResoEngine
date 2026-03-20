namespace Core2.Symbolics.Expressions;

public sealed record SymbolicInspectionStep(
    int Index,
    string Label,
    SymbolicTerm Source,
    SymbolicEnvironment EnvironmentBefore,
    SymbolicElaborationResult Elaboration,
    SymbolicReductionResult Reduction,
    ConstraintSetEvaluation? Evaluation,
    ConstraintNegotiationResult? Negotiation)
{
    public SymbolicEnvironment EnvironmentAfter => Reduction.Environment;
}
