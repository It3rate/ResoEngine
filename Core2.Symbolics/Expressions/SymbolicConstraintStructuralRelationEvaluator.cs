namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicConstraintRelationEvaluator
{
    private static ConstraintRelationAssessment EvaluateSharedCarrier(
        SharedCarrierTerm shared,
        ISymbolicStructuralContext? structuralContext)
    {
        if (Equals(shared.Left, shared.Right))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied);
        }

        if (structuralContext is not null &&
            shared.Left is AnchorReferenceTerm leftAnchor &&
            shared.Right is AnchorReferenceTerm rightAnchor)
        {
            bool hasLeft = structuralContext.TryResolveAnchorCarrier(leftAnchor, out var leftCarrier, out var leftNote);
            bool hasRight = structuralContext.TryResolveAnchorCarrier(rightAnchor, out var rightCarrier, out var rightNote);

            if (hasLeft && hasRight)
            {
                return leftCarrier == rightCarrier
                    ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
                    : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "Anchors resolve to different structural carriers.");
            }

            string note = leftNote ?? rightNote ?? "Shared-carrier evaluation requires carrier graph context.";
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note);
        }

        return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Shared-carrier evaluation requires carrier graph context.");
    }

    private static ConstraintRelationAssessment EvaluateRoute(
        RouteTerm route,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is null)
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Route evaluation requires carrier routing context.");
        }

        if (!structuralContext.TryResolveRoute(route.Site, route.From.Kind, route.To.Kind, out bool exists, out var note))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note ?? "Route evaluation requires carrier routing context.");
        }

        return exists
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "No such structural route exists at the named site.");
    }

    private static ConstraintRelationAssessment EvaluateJunction(
        JunctionTerm junction,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is null)
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Junction evaluation requires carrier graph context.");
        }

        if (!structuralContext.TryResolveJunctionSummary(junction.Site, out var kind, out var note))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note ?? "Junction evaluation requires carrier graph context.");
        }

        return kind == junction.Kind
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(
                ConstraintTruthKind.Unsatisfied,
                null,
                $"Site resolves to junction '{kind.ToString().ToLowerInvariant()}', not '{junction.Kind.ToString().ToLowerInvariant()}'.");
    }

    private static ConstraintRelationAssessment EvaluateSiteFlag(
        SiteFlagTerm flag,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is null)
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Site-flag evaluation requires carrier graph context.");
        }

        if (!structuralContext.TryResolveSiteFlag(flag.Site, flag.Kind, out bool value, out var note))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note ?? "Site-flag evaluation requires carrier graph context.");
        }

        return value
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(
                ConstraintTruthKind.Unsatisfied,
                null,
                $"Site does not satisfy flag '{SymbolicConstraintTermSupport.FormatSiteFlagKind(flag.Kind)}'.");
    }

    private static ConstraintRelationAssessment EvaluateCarrierFlag(
        CarrierFlagTerm flag,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is null)
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Carrier-flag evaluation requires carrier graph context.");
        }

        if (!structuralContext.TryResolveCarrierFlag(flag.Carrier, flag.Kind, out bool value, out var note))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note ?? "Carrier-flag evaluation requires carrier graph context.");
        }

        return value
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(
                ConstraintTruthKind.Unsatisfied,
                null,
                $"Carrier does not satisfy flag '{SymbolicConstraintTermSupport.FormatCarrierFlagKind(flag.Kind)}'.");
    }
}
