using Core2.Boolean;
using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public static class SymbolicReducer
{
    public static SymbolicReductionResult Reduce(SymbolicTerm term, SymbolicEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        return term switch
        {
            ProgramTerm program => ReduceProgram(program, current),
            _ => new SymbolicReductionResult(current, ElaborateAndReduce(term, current)),
        };
    }

    private static SymbolicReductionResult ReduceProgram(ProgramTerm term, SymbolicEnvironment environment) =>
        term switch
        {
            BindTerm bind => ReduceBind(bind, environment),
            EmitTerm emit => new SymbolicReductionResult(environment, ElaborateAndReduce(emit.Value, environment)),
            SequenceTerm sequence => ReduceSequence(sequence, environment),
            _ => new SymbolicReductionResult(environment, ElaborateAndReduce(term, environment)),
        };

    private static SymbolicReductionResult ReduceBind(BindTerm bind, SymbolicEnvironment environment)
    {
        var reduced = ElaborateAndReduce(bind.Value, environment);
        var next = environment.Bind(bind.Name, reduced);
        return new SymbolicReductionResult(next, reduced);
    }

    private static SymbolicReductionResult ReduceSequence(SequenceTerm sequence, SymbolicEnvironment environment)
    {
        var current = environment;
        SymbolicTerm? output = null;

        foreach (var step in sequence.Steps)
        {
            var result = ReduceProgram(step, current);
            current = result.Environment;
            output = result.Output;
        }

        return new SymbolicReductionResult(current, output);
    }

    private static SymbolicTerm ElaborateAndReduce(SymbolicTerm term, SymbolicEnvironment environment)
    {
        var elaborated = SymbolicElaborator.Elaborate(term, environment);
        return ReduceResolvedTerm(elaborated.Output ?? term, environment);
    }

    private static SymbolicTerm ReduceResolvedTerm(SymbolicTerm term, SymbolicEnvironment environment) =>
        term switch
        {
            ApplyTransformTerm apply => ReduceApplyTransform(apply, environment),
            PinTerm pin => new PinTerm(
                (ValueTerm)ElaborateAndReduce(pin.Host, environment),
                (ValueTerm)ElaborateAndReduce(pin.Applied, environment),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)ElaborateAndReduce(pin.AppliedAnchor, environment)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)ElaborateAndReduce(pinToPin.HostAnchor, environment),
                (AnchorReferenceTerm)ElaborateAndReduce(pinToPin.AppliedAnchor, environment)),
            AxisBooleanTerm boolean => ReduceBoolean(boolean, environment),
            FoldTerm fold => ReduceFold(fold, environment),
            EqualityTerm equality => new EqualityTerm(
                ElaborateAndReduce(equality.Left, environment),
                ElaborateAndReduce(equality.Right, environment)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)ElaborateAndReduce(shared.Left, environment),
                (ValueTerm)ElaborateAndReduce(shared.Right, environment)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)ElaborateAndReduce(route.Site, environment),
                route.From,
                route.To),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)ElaborateAndReduce(requirement.Relation, environment),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)ElaborateAndReduce(preference.Relation, environment),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)ElaborateAndReduce(constraint, environment)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(
                branchFamily.Family.Map(value => (ValueTerm)ElaborateAndReduce(value, environment))),
            _ => term,
        };

    private static SymbolicTerm ReduceApplyTransform(ApplyTransformTerm apply, SymbolicEnvironment environment)
    {
        var state = (ValueTerm)ElaborateAndReduce(apply.State, environment);
        var transform = (TransformTerm)ElaborateAndReduce(apply.Transform, environment);

        if (state is ElementLiteralTerm stateLiteral &&
            transform is TransformLiteralTerm transformLiteral &&
            TryApplyTransform(stateLiteral.Value, transformLiteral.Code, out var result))
        {
            return new ElementLiteralTerm(result);
        }

        return new ApplyTransformTerm(state, transform);
    }

    private static SymbolicTerm ReduceBoolean(AxisBooleanTerm boolean, SymbolicEnvironment environment)
    {
        var primary = (ValueTerm)ElaborateAndReduce(boolean.Primary, environment);
        var secondary = (ValueTerm)ElaborateAndReduce(boolean.Secondary, environment);
        var frame = boolean.Frame is null ? null : (ValueTerm)ElaborateAndReduce(boolean.Frame, environment);

        if (TryGetAxisLiteral(primary, out var primaryAxis) &&
            TryGetAxisLiteral(secondary, out var secondaryAxis) &&
            TryGetOptionalAxisLiteral(frame, out var frameAxis))
        {
            var resolved = AxisBooleanProjection.Resolve(primaryAxis, secondaryAxis, boolean.Operation, frameAxis);
            return new BranchFamilyTerm(resolved.Branches.Map<ValueTerm>(axis => new ElementLiteralTerm(axis)));
        }

        return new AxisBooleanTerm(primary, secondary, boolean.Operation, frame);
    }

    private static SymbolicTerm ReduceFold(FoldTerm fold, SymbolicEnvironment environment)
    {
        var source = (ValueTerm)ElaborateAndReduce(fold.Source, environment);
        if (source is ElementLiteralTerm literal && TryFoldLiteral(literal.Value, out var folded))
        {
            return new ElementLiteralTerm(folded);
        }

        if (source is BranchFamilyTerm branchFamily &&
            TryFoldBranchFamily(branchFamily.Family, out var foldedFamily))
        {
            return new BranchFamilyTerm(foldedFamily);
        }

        return new FoldTerm(source, fold.Kind);
    }

    private static bool TryApplyTransform(IElement state, IElement transform, out IElement result)
    {
        switch (state)
        {
            case Scalar scalarState when transform is Scalar scalarTransform:
                result = scalarState * scalarTransform;
                return true;

            case Proportion proportionState when transform is Proportion proportionTransform:
                result = proportionState * proportionTransform;
                return true;

            case Axis axisState when transform is Axis axisTransform:
                result = axisState * axisTransform;
                return true;

            case Area areaState when transform is Area areaTransform:
                result = areaState * areaTransform;
                return true;

            default:
                result = null!;
                return false;
        }
    }

    private static bool TryFoldLiteral(IElement value, out IElement folded)
    {
        switch (value)
        {
            case Proportion proportion:
                folded = proportion.Fold();
                return true;

            case Axis axis:
                folded = axis.Fold();
                return true;

            case Area area:
                folded = area.Fold();
                return true;

            default:
                folded = null!;
                return false;
        }
    }

    private static bool TryFoldBranchFamily(
        BranchFamily<ValueTerm> family,
        out BranchFamily<ValueTerm> folded)
    {
        var members = new List<BranchMember<ValueTerm>>(family.Members.Count);
        foreach (var member in family.Members)
        {
            if (member.Value is not ElementLiteralTerm literal ||
                !TryFoldLiteral(literal.Value, out var foldedValue))
            {
                folded = null!;
                return false;
            }

            members.Add(new BranchMember<ValueTerm>(
                member.Id,
                new ElementLiteralTerm(foldedValue),
                member.Parents,
                member.Annotations));
        }

        folded = BranchFamily<ValueTerm>.FromMembers(
            family.Origin,
            family.Semantics,
            family.Direction,
            members,
            family.Selection,
            family.Tensions,
            family.Annotations);
        return true;
    }

    private static bool TryGetAxisLiteral(ValueTerm term, out Axis axis)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Axis typed)
        {
            axis = typed;
            return true;
        }

        axis = Axis.Zero;
        return false;
    }

    private static bool TryGetOptionalAxisLiteral(ValueTerm? term, out Axis? axis)
    {
        if (term is null)
        {
            axis = null;
            return true;
        }

        if (TryGetAxisLiteral(term, out var typed))
        {
            axis = typed;
            return true;
        }

        axis = null;
        return false;
    }
}
