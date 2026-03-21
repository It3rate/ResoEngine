using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicTermFormatter
{
    public static string Format(SymbolicTerm term)
    {
        ArgumentNullException.ThrowIfNull(term);

        return term switch
        {
            ElementLiteralTerm literal => FormatElement(literal.Value),
            TransformLiteralTerm transform => FormatElement(transform.Code),
            ReferenceTerm reference => reference.Name,
            ValueReferenceTerm reference => reference.Name,
            TransformReferenceTerm reference => reference.Name,
            RelationReferenceTerm reference => reference.Name,
            CarrierReferenceTerm carrier => carrier.Name,
            SiteReferenceTerm site => site.SiteName,
            AnchorReferenceTerm anchor => anchor.QualifiedName,
            IncidentReferenceTerm incident => FormatIncident(incident.Kind),
            ApplyTransformTerm apply => $"{Format(apply.State)} * {Format(apply.Transform)}",
            MultiplyValuesTerm multiply => $"{Format(multiply.Left)} * {Format(multiply.Right)}",
            DivideValuesTerm divide => $"{Format(divide.Left)} / {Format(divide.Right)}",
            ContinueTerm continuation => $"continue({Format(continuation.Frame)}, {Format(continuation.Value)}, {FormatBoundaryLaw(continuation.Law)})",
            AnchorPositionTerm position => $"position({Format(position.Anchor)})",
            CountTerm count => FormatCount(count),
            CarrierCountTerm count => $"count({Format(count.Carrier)}, {FormatCarrierCountKind(count.Kind)})",
            CarrierSpanTerm span => $"span({Format(span.Carrier)})",
            PowerTerm power => FormatPower(power),
            InverseContinueTerm inverse => FormatInverse(inverse),
            PinTerm pin => $"{Format(pin.Host)} * {Format(pin.AppliedAnchor ?? pin.Applied)} @ {pin.Position}",
            PinToPinTerm pinToPin => $"pin({Format(pinToPin.HostAnchor)}, {Format(pinToPin.AppliedAnchor)})",
            AxisBooleanTerm boolean => FormatBoolean(boolean),
            FoldTerm fold => $"{FormatFoldKind(fold.Kind)}({Format(fold.Source)})",
            EqualityTerm equality => $"{Format(equality.Left)} == {Format(equality.Right)}",
            SharedCarrierTerm shared => $"share({Format(shared.Left)}, {Format(shared.Right)})",
            RouteTerm route => $"route({Format(route.Site)}, {Format(route.From)}, {Format(route.To)})",
            JunctionTerm junction => $"junction({Format(junction.Site)}, {FormatJunction(junction.Kind)})",
            SiteFlagTerm flag => $"has({Format(flag.Site)}, {FormatSiteFlag(flag.Kind)})",
            CarrierFlagTerm flag => $"has({Format(flag.Carrier)}, {FormatCarrierFlag(flag.Kind)})",
            RequirementTerm requirement => FormatRequirement(requirement),
            PreferenceTerm preference => FormatPreference(preference),
            ConstraintSetTerm set => FormatConstraintSet(set),
            BranchFamilyTerm branchFamily => FormatBranchFamily(branchFamily),
            BindTerm bind => $"let {bind.Target.QualifiedName} = {Format(bind.Value)}",
            CommitTerm commit => $"commit {commit.Target.QualifiedName} = {Format(commit.Value)}",
            EmitTerm emit => Format(emit.Value),
            SequenceTerm sequence => string.Join("; ", sequence.Steps.Select(Format)),
            _ => term.ToString() ?? term.GetType().Name,
        };
    }
}
