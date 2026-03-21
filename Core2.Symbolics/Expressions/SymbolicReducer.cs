using Core2.Boolean;
using Core2.Algebra;
using Core2.Branching;
using Core2.Elements;
using Core2.Resolution;
using Core2.Symbolics.Repetition;

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
            ProgramTerm program => ReduceProgram(program, current, structuralContext),
            _ => new SymbolicReductionResult(current, ElaborateAndReduce(term, current, structuralContext)),
        };
    }

    private static SymbolicReductionResult ReduceProgram(
        ProgramTerm term,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext) =>
        term switch
        {
            BindTerm bind => ReduceBind(bind, environment, structuralContext),
            CommitTerm commit => ReduceCommit(commit, environment, structuralContext),
            EmitTerm emit => new SymbolicReductionResult(environment, ElaborateAndReduce(emit.Value, environment, structuralContext)),
            SequenceTerm sequence => ReduceSequence(sequence, environment, structuralContext),
            _ => new SymbolicReductionResult(environment, ElaborateAndReduce(term, environment, structuralContext)),
        };

    private static SymbolicReductionResult ReduceBind(
        BindTerm bind,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var reduced = ElaborateAndReduce(bind.Value, environment, structuralContext);
        var next = environment.Bind(bind.Target, reduced);
        return new SymbolicReductionResult(next, reduced);
    }

    private static SymbolicReductionResult ReduceCommit(
        CommitTerm commit,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var reduced = ElaborateAndReduce(commit.Value, environment, structuralContext);
        var current = environment;
        SymbolicTerm output = reduced;

        if (reduced is ConstraintTerm constraint)
        {
            var negotiation = SymbolicConstraintNegotiator.Negotiate(constraint, environment, structuralContext);
            if (negotiation.SelectedCandidate is not null)
            {
                output = negotiation.SelectedCandidate;
                current = negotiation.Evaluation.Environment.Bind(commit.Target, output);
            }
            else if (negotiation.PreservedCandidateFamily is not null)
            {
                output = negotiation.PreservedCandidateFamily;
                current = negotiation.Evaluation.Environment.Bind(commit.Target, output);
            }
            else
            {
                output = negotiation.Evaluation.Reduced;
            }

            return new SymbolicReductionResult(current, output);
        }

        if (reduced is BranchFamilyTerm branchFamily)
        {
            if (branchFamily.Family.SelectedValue is not null)
            {
                output = branchFamily.Family.SelectedValue;
            }

            current = environment.Bind(commit.Target, output);
            return new SymbolicReductionResult(current, output);
        }

        current = environment.Bind(commit.Target, output);
        return new SymbolicReductionResult(current, output);
    }

    private static SymbolicReductionResult ReduceSequence(
        SequenceTerm sequence,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var current = environment;
        SymbolicTerm? output = null;

        foreach (var step in sequence.Steps)
        {
            var result = ReduceProgram(step, current, structuralContext);
            current = result.Environment;
            output = result.Output;
        }

        return new SymbolicReductionResult(current, output);
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
            PinTerm pin => new PinTerm(
                (ValueTerm)ReduceChild(pin.Host),
                (ValueTerm)ReduceChild(pin.Applied),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)ReduceChild(pin.AppliedAnchor)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)ReduceChild(pinToPin.HostAnchor),
                (AnchorReferenceTerm)ReduceChild(pinToPin.AppliedAnchor)),
            AxisBooleanTerm boolean => SymbolicReductionStructuralFamily.ReduceBoolean(boolean, ReduceChild),
            FoldTerm fold => SymbolicReductionTransformFamily.ReduceFold(fold, ReduceChild),
            EqualityTerm equality => new EqualityTerm(
                ReduceChild(equality.Left),
                ReduceChild(equality.Right)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)ReduceChild(shared.Left),
                (ValueTerm)ReduceChild(shared.Right)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)ReduceChild(route.Site),
                route.From,
                route.To),
            JunctionTerm junction => new JunctionTerm(
                (SiteReferenceTerm)ReduceChild(junction.Site),
                junction.Kind),
            SiteFlagTerm flag => new SiteFlagTerm(
                (SiteReferenceTerm)ReduceChild(flag.Site),
                flag.Kind),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)ReduceChild(requirement.Relation),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)ReduceChild(preference.Relation),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)ReduceChild(constraint)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(
                branchFamily.Family.Map(value => (ValueTerm)ReduceChild(value))),
            _ => term,
        };
    }



}
