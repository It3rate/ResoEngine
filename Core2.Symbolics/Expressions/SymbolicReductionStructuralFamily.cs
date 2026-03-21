using Core2.Boolean;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionStructuralFamily
{
    public static SymbolicTerm ReduceCount(CountTerm count, ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is not null &&
            structuralContext.TryResolveCount(count, out var value, out _))
        {
            return new ElementLiteralTerm(value);
        }

        return count;
    }

    public static SymbolicTerm ReduceCarrierCount(
        CarrierCountTerm count,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is not null &&
            structuralContext.TryResolveCarrierCount(count, out var value, out _))
        {
            return new ElementLiteralTerm(value);
        }

        return count;
    }

    public static SymbolicTerm ReduceCarrierSpan(
        CarrierSpanTerm span,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is not null &&
            structuralContext.TryResolveCarrierSpan(span, out var value, out _))
        {
            return new ElementLiteralTerm(value);
        }

        return span;
    }

    public static SymbolicTerm ReduceAnchorPosition(
        AnchorPositionTerm position,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is not null &&
            structuralContext.TryResolveAnchorPosition(position.Anchor, out var value, out _))
        {
            return new ElementLiteralTerm(value);
        }

        return position;
    }

    public static SymbolicTerm ReduceBoolean(
        AxisBooleanTerm boolean,
        Func<SymbolicTerm, SymbolicTerm> reduce)
    {
        var primary = (ValueTerm)reduce(boolean.Primary);
        var secondary = (ValueTerm)reduce(boolean.Secondary);
        var frame = boolean.Frame is null ? null : (ValueTerm)reduce(boolean.Frame);

        if (SymbolicReductionLiterals.TryGetAxisLiteral(primary, out var primaryAxis) &&
            SymbolicReductionLiterals.TryGetAxisLiteral(secondary, out var secondaryAxis) &&
            SymbolicReductionLiterals.TryGetOptionalAxisLiteral(frame, out var frameAxis))
        {
            var resolved = AxisBooleanProjection.Resolve(primaryAxis, secondaryAxis, boolean.Operation, frameAxis);
            return new BranchFamilyTerm(resolved.Branches.Map<ValueTerm>(axis => new ElementLiteralTerm(axis)));
        }

        return new AxisBooleanTerm(primary, secondary, boolean.Operation, frame);
    }
}
