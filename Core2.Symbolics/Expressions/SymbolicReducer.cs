using Core2.Boolean;
using Core2.Algebra;
using Core2.Branching;
using Core2.Elements;
using Core2.Symbolics.Repetition;

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
            CommitTerm commit => ReduceCommit(commit, environment),
            EmitTerm emit => new SymbolicReductionResult(environment, ElaborateAndReduce(emit.Value, environment)),
            SequenceTerm sequence => ReduceSequence(sequence, environment),
            _ => new SymbolicReductionResult(environment, ElaborateAndReduce(term, environment)),
        };

    private static SymbolicReductionResult ReduceBind(BindTerm bind, SymbolicEnvironment environment)
    {
        var reduced = ElaborateAndReduce(bind.Value, environment);
        var next = environment.Bind(bind.Target, reduced);
        return new SymbolicReductionResult(next, reduced);
    }

    private static SymbolicReductionResult ReduceCommit(CommitTerm commit, SymbolicEnvironment environment)
    {
        var reduced = ElaborateAndReduce(commit.Value, environment);
        var current = environment;
        SymbolicTerm output = reduced;

        if (reduced is ConstraintTerm constraint)
        {
            var negotiation = SymbolicConstraintNegotiator.Negotiate(constraint, environment);
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
            MultiplyValuesTerm multiply => ReduceMultiply(multiply, environment),
            DivideValuesTerm divide => ReduceDivide(divide, environment),
            PowerTerm power => ReducePower(power, environment),
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

    private static SymbolicTerm ReduceMultiply(MultiplyValuesTerm multiply, SymbolicEnvironment environment)
    {
        var left = (ValueTerm)ElaborateAndReduce(multiply.Left, environment);
        var right = (ValueTerm)ElaborateAndReduce(multiply.Right, environment);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryMultiplyValues(leftLiteral.Value, rightLiteral.Value, out var product))
        {
            return new ElementLiteralTerm(product);
        }

        return new MultiplyValuesTerm(left, right);
    }

    private static SymbolicTerm ReduceDivide(DivideValuesTerm divide, SymbolicEnvironment environment)
    {
        var left = (ValueTerm)ElaborateAndReduce(divide.Left, environment);
        var right = (ValueTerm)ElaborateAndReduce(divide.Right, environment);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryDivideValues(leftLiteral.Value, rightLiteral.Value, out var quotient))
        {
            return new ElementLiteralTerm(quotient);
        }

        return new DivideValuesTerm(left, right);
    }

    private static SymbolicTerm ReducePower(PowerTerm power, SymbolicEnvironment environment)
    {
        var @base = (ValueTerm)ElaborateAndReduce(power.Base, environment);
        if (@base is ElementLiteralTerm literal &&
            TryReducePower(literal.Value, power.Exponent, out var reduced))
        {
            return reduced;
        }

        return new PowerTerm(@base, power.Exponent);
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

    private static bool TryMultiplyValues(IElement left, IElement right, out IElement result)
    {
        switch (left)
        {
            case Scalar leftScalar when right is Scalar rightScalar:
                result = leftScalar * rightScalar;
                return true;

            case Proportion leftProportion when right is Proportion rightProportion:
                result = leftProportion * rightProportion;
                return true;

            case Axis leftAxis when right is Axis rightAxis:
                result = leftAxis * rightAxis;
                return true;

            case Area leftArea when right is Area rightArea:
                result = leftArea * rightArea;
                return true;

            default:
                result = null!;
                return false;
        }
    }

    private static bool TryDivideValues(IElement left, IElement right, out IElement result)
    {
        switch (left)
        {
            case Scalar leftScalar when right is Scalar rightScalar:
                result = leftScalar / rightScalar;
                return true;

            case Proportion leftProportion when right is Proportion rightProportion:
                result = leftProportion / rightProportion;
                return true;

            case Axis leftAxis when right is Axis rightAxis:
                return TryDivideAxis(leftAxis, rightAxis, out result);

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

    private static bool TryReducePower(IElement value, Proportion exponent, out SymbolicTerm reduced)
    {
        switch (value)
        {
            case Scalar scalar:
                return TryProjectPowerResult(PowerEngine.Pow(scalar, exponent), out reduced);

            case Proportion proportion:
                return TryProjectPowerResult(PowerEngine.Pow(proportion, exponent), out reduced);

            case Axis axis:
                return TryProjectPowerResult(PowerEngine.Pow(axis, exponent), out reduced);

            case Area area:
                return TryProjectPowerResult(PowerEngine.Pow(area, exponent), out reduced);

            default:
                reduced = null!;
                return false;
        }
    }

    private static bool TryProjectPowerResult<T>(PowerResult<T> result, out SymbolicTerm reduced)
        where T : IElement
    {
        if (!result.Succeeded)
        {
            reduced = null!;
            return false;
        }

        if (result.PrincipalCandidate is not null && result.Candidates.Count == 1)
        {
            reduced = new ElementLiteralTerm(result.PrincipalCandidate);
            return true;
        }

        reduced = new BranchFamilyTerm(
            result.Branches.Map<ValueTerm>(candidate => new ElementLiteralTerm(candidate)));
        return true;
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

    private static bool TryDivideAxis(Axis dividend, Axis divisor, out IElement result)
    {
        if (dividend.Basis != divisor.Basis)
        {
            result = null!;
            return false;
        }

        switch (dividend.Basis)
        {
            case AxisBasis.Complex:
            {
                Proportion determinant = (divisor.Dominant * divisor.Dominant) + (divisor.Recessive * divisor.Recessive);
                if (determinant.IsZero)
                {
                    result = null!;
                    return false;
                }

                Proportion recessive = ((divisor.Dominant * dividend.Recessive) - (divisor.Recessive * dividend.Dominant)) / determinant;
                Proportion dominant = ((divisor.Recessive * dividend.Recessive) + (divisor.Dominant * dividend.Dominant)) / determinant;
                result = new Axis(recessive, dominant, dividend.Basis);
                return true;
            }

            case AxisBasis.SplitComplex:
            {
                Proportion determinant = (divisor.Dominant * divisor.Dominant) - (divisor.Recessive * divisor.Recessive);
                if (determinant.IsZero)
                {
                    result = null!;
                    return false;
                }

                Proportion recessive = ((divisor.Dominant * dividend.Recessive) - (divisor.Recessive * dividend.Dominant)) / determinant;
                Proportion dominant = ((-divisor.Recessive * dividend.Recessive) + (divisor.Dominant * dividend.Dominant)) / determinant;
                result = new Axis(recessive, dominant, dividend.Basis);
                return true;
            }

            default:
                result = null!;
                return false;
        }
    }
}
