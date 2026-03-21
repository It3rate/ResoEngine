namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionCompositeFamily
{
    public static SymbolicTerm ReduceComposite(SymbolicTerm term, Func<SymbolicTerm, SymbolicTerm> reduce) =>
        term switch
        {
            PinTerm pin => new PinTerm(
                (ValueTerm)reduce(pin.Host),
                (ValueTerm)reduce(pin.Applied),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)reduce(pin.AppliedAnchor)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)reduce(pinToPin.HostAnchor),
                (AnchorReferenceTerm)reduce(pinToPin.AppliedAnchor)),
            EqualityTerm equality => new EqualityTerm(
                reduce(equality.Left),
                reduce(equality.Right)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)reduce(shared.Left),
                (ValueTerm)reduce(shared.Right)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)reduce(route.Site),
                route.From,
                route.To),
            JunctionTerm junction => new JunctionTerm(
                (SiteReferenceTerm)reduce(junction.Site),
                junction.Kind),
            SiteFlagTerm flag => new SiteFlagTerm(
                (SiteReferenceTerm)reduce(flag.Site),
                flag.Kind),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)reduce(requirement.Relation),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)reduce(preference.Relation),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)reduce(constraint)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(
                branchFamily.Family.Map(value => (ValueTerm)reduce(value))),
            _ => term,
        };
}
