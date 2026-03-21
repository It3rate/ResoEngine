using Core2.Elements;
using Core2.Repetition;

namespace Core2.Symbolics.Expressions;

public sealed record SymbolicContinueEvaluation(
    SymbolicEnvironment Environment,
    ContinueTerm Term,
    SymbolicTerm? Elaborated,
    SymbolicTerm? Reduced,
    BoundaryContinuationResult? Continuation)
{
    public bool HasTension => Continuation?.HasTension ?? false;

    public bool HasValue =>
        Continuation is not null ||
        Reduced is ElementLiteralTerm { Value: Proportion };

    public Proportion Value =>
        Continuation?.Value ??
        (Reduced is ElementLiteralTerm { Value: Proportion proportion } ? proportion : Proportion.Zero);
}

public static class SymbolicContinueEvaluator
{
    public static SymbolicContinueEvaluation Evaluate(
        ContinueTerm term,
        SymbolicEnvironment? environment = null,
        ISymbolicStructuralContext? structuralContext = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        var elaborated = SymbolicElaborator.Elaborate(term, current);
        var reduced = SymbolicReducer.Reduce(term, current, structuralContext);
        var continuation = TryResolveContinuation(elaborated.Output);

        return new SymbolicContinueEvaluation(
            reduced.Environment,
            term,
            elaborated.Output,
            reduced.Output,
            continuation);
    }

    private static BoundaryContinuationResult? TryResolveContinuation(SymbolicTerm? term)
    {
        if (term is not ContinueTerm continuation ||
            continuation.Frame is not ElementLiteralTerm { Value: Axis frameAxis } ||
            continuation.Value is not ElementLiteralTerm valueLiteral)
        {
            return null;
        }

        return valueLiteral.Value switch
        {
            Scalar scalar => frameAxis.Continue(scalar, continuation.Law),
            Proportion proportion => frameAxis.Continue(proportion, continuation.Law),
            _ => null,
        };
    }
}
