namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicConstraintRelationEvaluator
{
    public static ConstraintRelationAssessment EvaluateRelation(
        RelationTerm relation,
        ISymbolicStructuralContext? structuralContext) =>
        relation switch
        {
            EqualityTerm equality => EvaluateEquality(equality, structuralContext),
            SharedCarrierTerm shared => EvaluateSharedCarrier(shared, structuralContext),
            RouteTerm route => EvaluateRoute(route, structuralContext),
            JunctionTerm junction => EvaluateJunction(junction, structuralContext),
            SiteFlagTerm flag => EvaluateSiteFlag(flag, structuralContext),
            CarrierFlagTerm flag => EvaluateCarrierFlag(flag, structuralContext),
            _ => new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Relation form is not yet directly evaluable."),
        };
}
