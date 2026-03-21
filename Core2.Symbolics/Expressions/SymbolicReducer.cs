using Core2.Boolean;

namespace Core2.Symbolics.Expressions;

public static class SymbolicReducer
{
    public static SymbolicReductionResult Reduce(
        SymbolicTerm term,
        SymbolicEnvironment? environment = null,
        ISymbolicStructuralContext? structuralContext = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        return term switch
        {
            ProgramTerm program => SymbolicReductionProgramFlow.ReduceProgram(program, current, structuralContext, ElaborateAndReduce),
            _ => new SymbolicReductionResult(current, ElaborateAndReduce(term, current, structuralContext)),
        };
    }

    private static SymbolicTerm ElaborateAndReduce(
        SymbolicTerm term,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var elaborated = SymbolicElaborator.Elaborate(term, environment);
        return ReduceResolvedTerm(elaborated.Output ?? term, environment, structuralContext);
    }

    private static SymbolicTerm ReduceResolvedTerm(
        SymbolicTerm term,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        SymbolicTerm ReduceChild(SymbolicTerm child) => ElaborateAndReduce(child, environment, structuralContext);

        return term switch
        {
            ApplyTransformTerm apply => SymbolicReductionTransformFamily.ReduceApplyTransform(apply, ReduceChild),
            MultiplyValuesTerm multiply => SymbolicReductionTransformFamily.ReduceMultiply(multiply, ReduceChild),
            DivideValuesTerm divide => SymbolicReductionTransformFamily.ReduceDivide(divide, ReduceChild),
            ContinueTerm continuation => SymbolicReductionRepetitionFamily.ReduceContinue(continuation, ReduceChild),
            AnchorPositionTerm position => SymbolicReductionStructuralFamily.ReduceAnchorPosition(position, structuralContext),
            CountTerm count => SymbolicReductionStructuralFamily.ReduceCount(count, structuralContext),
            CarrierCountTerm count => SymbolicReductionStructuralFamily.ReduceCarrierCount(count, structuralContext),
            CarrierSpanTerm span => SymbolicReductionStructuralFamily.ReduceCarrierSpan(span, structuralContext),
            PowerTerm power => SymbolicReductionRepetitionFamily.ReducePower(power, ReduceChild),
            InverseContinueTerm inverse => SymbolicReductionRepetitionFamily.ReduceInverseContinuation(inverse, ReduceChild),
            AxisBooleanTerm boolean => SymbolicReductionStructuralFamily.ReduceBoolean(boolean, ReduceChild),
            FoldTerm fold => SymbolicReductionTransformFamily.ReduceFold(fold, ReduceChild),
            PinTerm or PinToPinTerm or EqualityTerm or SharedCarrierTerm or RouteTerm or JunctionTerm or
                SiteFlagTerm or RequirementTerm or PreferenceTerm or ConstraintSetTerm or BranchFamilyTerm
                => SymbolicReductionCompositeFamily.ReduceComposite(term, ReduceChild),
            _ => term,
        };
    }



}
