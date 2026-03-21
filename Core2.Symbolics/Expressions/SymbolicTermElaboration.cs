using Core2.Branching;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicTermElaboration
{
    public static SymbolicTerm ElaborateTerm(
        SymbolicTerm term,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborate)
        => term switch
        {
            ReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is not null => resolved,
            ValueReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is ValueTerm value => value,
            TransformReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is TransformTerm transform => transform,
            RelationReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is RelationTerm relation => relation,
            SiteReferenceTerm reference when environment.TryResolve(reference.SiteName, out var resolved) && resolved is ValueTerm value => value,
            AnchorReferenceTerm reference when environment.TryResolve(reference.QualifiedName, out var resolved) && resolved is ValueTerm value => value,
            CarrierReferenceTerm => term,
            ApplyTransformTerm apply => new ApplyTransformTerm(
                (ValueTerm)elaborate(apply.State, environment),
                (TransformTerm)elaborate(apply.Transform, environment)),
            MultiplyValuesTerm multiply => new MultiplyValuesTerm(
                (ValueTerm)elaborate(multiply.Left, environment),
                (ValueTerm)elaborate(multiply.Right, environment)),
            DivideValuesTerm divide => new DivideValuesTerm(
                (ValueTerm)elaborate(divide.Left, environment),
                (ValueTerm)elaborate(divide.Right, environment)),
            ContinueTerm continuation => new ContinueTerm(
                (ValueTerm)elaborate(continuation.Frame, environment),
                (ValueTerm)elaborate(continuation.Value, environment),
                continuation.Law),
            AnchorPositionTerm position => new AnchorPositionTerm(
                (AnchorReferenceTerm)elaborate(position.Anchor, environment)),
            CountTerm count => new CountTerm(
                count.Site is null ? null : (SiteReferenceTerm)elaborate(count.Site, environment),
                count.Kind),
            CarrierCountTerm count => new CarrierCountTerm(
                (CarrierReferenceTerm)elaborate(count.Carrier, environment),
                count.Kind),
            CarrierSpanTerm span => new CarrierSpanTerm(
                (CarrierReferenceTerm)elaborate(span.Carrier, environment)),
            CarrierFlagTerm flag => new CarrierFlagTerm(
                (CarrierReferenceTerm)elaborate(flag.Carrier, environment),
                flag.Kind),
            PowerTerm power => new PowerTerm(
                (ValueTerm)elaborate(power.Base, environment),
                power.Exponent,
                power.Rule,
                power.Reference is null ? null : (ValueTerm)elaborate(power.Reference, environment)),
            InverseContinueTerm inverse => new InverseContinueTerm(
                (ValueTerm)elaborate(inverse.Source, environment),
                inverse.Degree,
                inverse.Rule,
                inverse.Reference is null ? null : (ValueTerm)elaborate(inverse.Reference, environment)),
            PinTerm pin => new PinTerm(
                (ValueTerm)elaborate(pin.Host, environment),
                (ValueTerm)elaborate(pin.Applied, environment),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)elaborate(pin.AppliedAnchor, environment)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)elaborate(pinToPin.HostAnchor, environment),
                (AnchorReferenceTerm)elaborate(pinToPin.AppliedAnchor, environment)),
            AxisBooleanTerm boolean => new AxisBooleanTerm(
                (ValueTerm)elaborate(boolean.Primary, environment),
                (ValueTerm)elaborate(boolean.Secondary, environment),
                boolean.Operation,
                boolean.Frame is null ? null : (ValueTerm)elaborate(boolean.Frame, environment)),
            FoldTerm fold => new FoldTerm((ValueTerm)elaborate(fold.Source, environment), fold.Kind),
            EqualityTerm equality => new EqualityTerm(
                elaborate(equality.Left, environment),
                elaborate(equality.Right, environment)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)elaborate(shared.Left, environment),
                (ValueTerm)elaborate(shared.Right, environment)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)elaborate(route.Site, environment),
                route.From,
                route.To),
            JunctionTerm junction => new JunctionTerm(
                (SiteReferenceTerm)elaborate(junction.Site, environment),
                junction.Kind),
            SiteFlagTerm flag => new SiteFlagTerm(
                (SiteReferenceTerm)elaborate(flag.Site, environment),
                flag.Kind),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)elaborate(requirement.Relation, environment),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)elaborate(preference.Relation, environment),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)elaborate(constraint, environment)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(ElaborateBranchFamily(branchFamily.Family, environment, elaborate)),
            _ => term,
        };

    private static BranchFamily<ValueTerm> ElaborateBranchFamily(
        BranchFamily<ValueTerm> family,
        SymbolicEnvironment environment,
        Func<SymbolicTerm, SymbolicEnvironment, SymbolicTerm> elaborate)
    {
        var members = family.Members
            .Select(member => new BranchMember<ValueTerm>(
                member.Id,
                (ValueTerm)elaborate(member.Value, environment),
                member.Parents,
                member.Annotations))
            .ToArray();

        return BranchFamily<ValueTerm>.FromMembers(
            family.Origin,
            family.Semantics,
            family.Direction,
            members,
            family.Selection,
            family.Tensions,
            family.Annotations);
    }
}
