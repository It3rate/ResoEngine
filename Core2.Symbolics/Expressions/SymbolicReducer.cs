using Core2.Boolean;
using Core2.Algebra;
using Core2.Branching;
using Core2.Elements;
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
        ISymbolicStructuralContext? structuralContext) =>
        term switch
        {
            ApplyTransformTerm apply => ReduceApplyTransform(apply, environment, structuralContext),
            MultiplyValuesTerm multiply => ReduceMultiply(multiply, environment, structuralContext),
            DivideValuesTerm divide => ReduceDivide(divide, environment, structuralContext),
            CountTerm count => ReduceCount(count, structuralContext),
            PowerTerm power => ReducePower(power, environment, structuralContext),
            InverseContinueTerm inverse => ReduceInverseContinuation(inverse, environment, structuralContext),
            PinTerm pin => new PinTerm(
                (ValueTerm)ElaborateAndReduce(pin.Host, environment, structuralContext),
                (ValueTerm)ElaborateAndReduce(pin.Applied, environment, structuralContext),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)ElaborateAndReduce(pin.AppliedAnchor, environment, structuralContext)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)ElaborateAndReduce(pinToPin.HostAnchor, environment, structuralContext),
                (AnchorReferenceTerm)ElaborateAndReduce(pinToPin.AppliedAnchor, environment, structuralContext)),
            AxisBooleanTerm boolean => ReduceBoolean(boolean, environment, structuralContext),
            FoldTerm fold => ReduceFold(fold, environment, structuralContext),
            EqualityTerm equality => new EqualityTerm(
                ElaborateAndReduce(equality.Left, environment, structuralContext),
                ElaborateAndReduce(equality.Right, environment, structuralContext)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)ElaborateAndReduce(shared.Left, environment, structuralContext),
                (ValueTerm)ElaborateAndReduce(shared.Right, environment, structuralContext)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)ElaborateAndReduce(route.Site, environment, structuralContext),
                route.From,
                route.To),
            JunctionTerm junction => new JunctionTerm(
                (SiteReferenceTerm)ElaborateAndReduce(junction.Site, environment, structuralContext),
                junction.Kind),
            SiteFlagTerm flag => new SiteFlagTerm(
                (SiteReferenceTerm)ElaborateAndReduce(flag.Site, environment, structuralContext),
                flag.Kind),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)ElaborateAndReduce(requirement.Relation, environment, structuralContext),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)ElaborateAndReduce(preference.Relation, environment, structuralContext),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)ElaborateAndReduce(constraint, environment, structuralContext)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(
                branchFamily.Family.Map(value => (ValueTerm)ElaborateAndReduce(value, environment, structuralContext))),
            _ => term,
        };

    private static SymbolicTerm ReduceApplyTransform(
        ApplyTransformTerm apply,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var state = (ValueTerm)ElaborateAndReduce(apply.State, environment, structuralContext);
        var transform = (TransformTerm)ElaborateAndReduce(apply.Transform, environment, structuralContext);

        if (state is ElementLiteralTerm stateLiteral &&
            transform is TransformLiteralTerm transformLiteral &&
            TryApplyTransform(stateLiteral.Value, transformLiteral.Code, out var result))
        {
            return new ElementLiteralTerm(result);
        }

        return new ApplyTransformTerm(state, transform);
    }

    private static SymbolicTerm ReduceMultiply(
        MultiplyValuesTerm multiply,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var left = (ValueTerm)ElaborateAndReduce(multiply.Left, environment, structuralContext);
        var right = (ValueTerm)ElaborateAndReduce(multiply.Right, environment, structuralContext);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryMultiplyValues(leftLiteral.Value, rightLiteral.Value, out var product))
        {
            return new ElementLiteralTerm(product);
        }

        return new MultiplyValuesTerm(left, right);
    }

    private static SymbolicTerm ReduceDivide(
        DivideValuesTerm divide,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var left = (ValueTerm)ElaborateAndReduce(divide.Left, environment, structuralContext);
        var right = (ValueTerm)ElaborateAndReduce(divide.Right, environment, structuralContext);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryDivideValues(leftLiteral.Value, rightLiteral.Value, out var quotient))
        {
            return new ElementLiteralTerm(quotient);
        }

        return new DivideValuesTerm(left, right);
    }

    private static SymbolicTerm ReduceCount(CountTerm count, ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is not null &&
            structuralContext.TryResolveCount(count, out var value, out _))
        {
            return new ElementLiteralTerm(value);
        }

        return count;
    }

    private static SymbolicTerm ReducePower(
        PowerTerm power,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var @base = (ValueTerm)ElaborateAndReduce(power.Base, environment, structuralContext);
        var reference = power.Reference is null ? null : (ValueTerm)ElaborateAndReduce(power.Reference, environment, structuralContext);
        if (@base is ElementLiteralTerm literal &&
            TryReducePower(literal.Value, power.Exponent, power.Rule, reference, out var reduced))
        {
            return reduced;
        }

        return new PowerTerm(@base, power.Exponent, power.Rule, reference);
    }

    private static SymbolicTerm ReduceInverseContinuation(
        InverseContinueTerm inverse,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var source = (ValueTerm)ElaborateAndReduce(inverse.Source, environment, structuralContext);
        var reference = inverse.Reference is null ? null : (ValueTerm)ElaborateAndReduce(inverse.Reference, environment, structuralContext);
        if (source is ElementLiteralTerm literal &&
            TryReduceInverseContinuation(literal.Value, inverse.Degree, inverse.Rule, reference, out var reduced))
        {
            return reduced;
        }

        return new InverseContinueTerm(source, inverse.Degree, inverse.Rule, reference);
    }

    private static SymbolicTerm ReduceBoolean(
        AxisBooleanTerm boolean,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var primary = (ValueTerm)ElaborateAndReduce(boolean.Primary, environment, structuralContext);
        var secondary = (ValueTerm)ElaborateAndReduce(boolean.Secondary, environment, structuralContext);
        var frame = boolean.Frame is null ? null : (ValueTerm)ElaborateAndReduce(boolean.Frame, environment, structuralContext);

        if (TryGetAxisLiteral(primary, out var primaryAxis) &&
            TryGetAxisLiteral(secondary, out var secondaryAxis) &&
            TryGetOptionalAxisLiteral(frame, out var frameAxis))
        {
            var resolved = AxisBooleanProjection.Resolve(primaryAxis, secondaryAxis, boolean.Operation, frameAxis);
            return new BranchFamilyTerm(resolved.Branches.Map<ValueTerm>(axis => new ElementLiteralTerm(axis)));
        }

        return new AxisBooleanTerm(primary, secondary, boolean.Operation, frame);
    }

    private static SymbolicTerm ReduceFold(
        FoldTerm fold,
        SymbolicEnvironment environment,
        ISymbolicStructuralContext? structuralContext)
    {
        var source = (ValueTerm)ElaborateAndReduce(fold.Source, environment, structuralContext);
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

    private static bool TryReducePower(
        IElement value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        switch (value)
        {
            case Scalar scalar:
                return TryReducePowerScalar(scalar, exponent, rule, reference, out reduced);

            case Proportion proportion:
                return TryReducePowerProportion(proportion, exponent, rule, reference, out reduced);

            case Axis axis:
                return TryReducePowerAxis(axis, exponent, rule, reference, out reduced);

            case Area area:
                return TryReducePowerArea(area, exponent, rule, reference, out reduced);

            default:
                reduced = null!;
                return false;
        }
    }

    private static bool TryReduceInverseContinuation(
        IElement value,
        Proportion degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        if (!TryGetIntegerDegree(degree, out int normalizedDegree))
        {
            reduced = null!;
            return false;
        }

        switch (value)
        {
            case Scalar scalar:
                return TryReduceInverseScalar(scalar, normalizedDegree, rule, reference, out reduced);

            case Proportion proportion:
                return TryReduceInverseProportion(proportion, normalizedDegree, rule, reference, out reduced);

            case Axis axis:
                return TryReduceInverseAxis(axis, normalizedDegree, rule, reference, out reduced);

            case Area area:
                return TryReduceInverseArea(area, normalizedDegree, rule, reference, out reduced);

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

    private static bool TryProjectInverseContinuationResult<T>(
        InverseContinuationResult<T> result,
        out SymbolicTerm reduced)
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

    private static bool TryGetIntegerDegree(Proportion degree, out int normalizedDegree)
    {
        if (degree.Denominator != 1 || degree.Numerator <= 0 || degree.Numerator > int.MaxValue)
        {
            normalizedDegree = 0;
            return false;
        }

        normalizedDegree = (int)degree.Numerator;
        return true;
    }

    private static bool TryReducePowerScalar(
        Scalar value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Scalar? typedReference = null;
        Scalar scalarReference = default;
        if (reference is not null && !TryGetScalarLiteral(reference, out scalarReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = scalarReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerProportion(
        Proportion value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Proportion? typedReference = null;
        Proportion proportionReference = null!;
        if (reference is not null && !TryGetProportionLiteral(reference, out proportionReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = proportionReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerAxis(
        Axis value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, rule, typedReference), out reduced);
    }

    private static bool TryReducePowerArea(
        Area value,
        Proportion exponent,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectPowerResult(PowerEngine.Pow(value, exponent, AreaInverseContinuationMode.FoldFirst, rule, typedReference), out reduced);
    }

    private static bool TryReduceInverseScalar(
        Scalar value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Scalar? typedReference = null;
        Scalar scalarReference = default;
        if (reference is not null && !TryGetScalarLiteral(reference, out scalarReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = scalarReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseProportion(
        Proportion value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Proportion? typedReference = null;
        Proportion proportionReference = null!;
        if (reference is not null && !TryGetProportionLiteral(reference, out proportionReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = proportionReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseAxis(
        Axis value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, rule, typedReference),
            out reduced);
    }

    private static bool TryReduceInverseArea(
        Area value,
        int degree,
        InverseContinuationRule rule,
        ValueTerm? reference,
        out SymbolicTerm reduced)
    {
        Axis? typedReference = null;
        Axis axisReference = Axis.Zero;
        if (reference is not null && !TryGetAxisLiteral(reference, out axisReference))
        {
            reduced = null!;
            return false;
        }

        if (reference is not null)
        {
            typedReference = axisReference;
        }

        return TryProjectInverseContinuationResult(
            InverseContinuationEngine.InverseContinue(value, degree, AreaInverseContinuationMode.FoldFirst, rule, typedReference),
            out reduced);
    }

    private static bool TryGetScalarLiteral(ValueTerm term, out Scalar scalar)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Scalar typed)
        {
            scalar = typed;
            return true;
        }

        scalar = Scalar.Zero;
        return false;
    }

    private static bool TryGetProportionLiteral(ValueTerm term, out Proportion proportion)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Proportion typed)
        {
            proportion = typed;
            return true;
        }

        proportion = null!;
        return false;
    }
}
