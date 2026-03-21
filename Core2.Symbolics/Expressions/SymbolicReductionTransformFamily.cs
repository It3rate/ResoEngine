using Core2.Branching;
using Core2.Algebra;
using Core2.Elements;
using Core2.Resolution;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionTransformFamily
{
    public static SymbolicTerm ReduceApplyTransform(
        ApplyTransformTerm apply,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var state = (ValueTerm)reduce(apply.State);
        var transform = (TransformTerm)reduce(apply.Transform);

        if (state is ElementLiteralTerm stateLiteral &&
            transform is TransformLiteralTerm transformLiteral &&
            TryApplyTransform(stateLiteral.Value, transformLiteral.Code, out var result))
        {
            return new ElementLiteralTerm(result);
        }

        return new ApplyTransformTerm(state, transform);
    }

    public static SymbolicTerm ReduceMultiply(
        MultiplyValuesTerm multiply,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var left = (ValueTerm)reduce(multiply.Left);
        var right = (ValueTerm)reduce(multiply.Right);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryMultiplyValues(leftLiteral.Value, rightLiteral.Value, out var product))
        {
            return new ElementLiteralTerm(product);
        }

        return new MultiplyValuesTerm(left, right);
    }

    public static SymbolicTerm ReduceDivide(
        DivideValuesTerm divide,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var left = (ValueTerm)reduce(divide.Left);
        var right = (ValueTerm)reduce(divide.Right);

        if (left is ElementLiteralTerm leftLiteral &&
            right is ElementLiteralTerm rightLiteral &&
            TryDivideValues(leftLiteral.Value, rightLiteral.Value, out var quotient))
        {
            return new ElementLiteralTerm(quotient);
        }

        return new DivideValuesTerm(left, right);
    }

    public static SymbolicTerm ReduceFold(
        FoldTerm fold,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var source = (ValueTerm)reduce(fold.Source);
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
        if (PrimitiveResolutionDefaults.ClassifyTransformApplication(state, transform) == PrimitiveSupportLaw.Inherit &&
            PrimitiveTransformRuntime.TryApplyPreservingSupport(state, transform, out result))
        {
            return true;
        }

        if (state is Proportion hostedValue &&
            transform is Proportion hostedScale &&
            PrimitiveResolutionDefaults.ClassifyHostedScale(hostedValue, hostedScale) == PrimitiveSupportLaw.Inherit)
        {
            result = PrimitiveResultSupportRuntime.ScaleHostedValue(hostedValue, hostedScale).CommittedValue;
            return true;
        }

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
        if (PrimitiveResolutionDefaults.ClassifyTransformApplication(left, right) == PrimitiveSupportLaw.Inherit &&
            PrimitiveTransformRuntime.TryDividePreservingSupport(left, right, out result))
        {
            return true;
        }

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
