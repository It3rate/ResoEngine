using Core2.Boolean;
using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class CanonicalSymbolicSerializer
{
    public static string Serialize(SymbolicTerm term)
    {
        ArgumentNullException.ThrowIfNull(term);

        return term switch
        {
            ElementLiteralTerm literal => $"value({SerializeElement(literal.Value)})",
            TransformLiteralTerm transform => $"transform({SerializeElement(transform.Code)})",
            ReferenceTerm reference => $"ref(sort={reference.ReferencedSort},name={Escape(reference.Name)})",
            ValueReferenceTerm reference => $"ref(sort=Value,name={Escape(reference.Name)})",
            TransformReferenceTerm reference => $"ref(sort=Transform,name={Escape(reference.Name)})",
            RelationReferenceTerm reference => $"ref(sort=Relation,name={Escape(reference.Name)})",
            CarrierReferenceTerm carrier => $"carrier({Escape(carrier.Name)})",
            SiteReferenceTerm site => $"site({Escape(site.SiteName)})",
            AnchorReferenceTerm anchor => $"anchor(owner={Escape(anchor.OwnerName)},name={Escape(anchor.AnchorName)})",
            IncidentReferenceTerm incident => $"incident({incident.Kind})",
            ApplyTransformTerm apply => $"apply(state={Serialize(apply.State)},transform={Serialize(apply.Transform)})",
            MultiplyValuesTerm multiply => $"multiply(left={Serialize(multiply.Left)},right={Serialize(multiply.Right)})",
            DivideValuesTerm divide => $"divide(left={Serialize(divide.Left)},right={Serialize(divide.Right)})",
            ContinueTerm continuation => $"continue(frame={Serialize(continuation.Frame)},value={Serialize(continuation.Value)},law={SerializeBoundaryLaw(continuation.Law)})",
            AnchorPositionTerm position => $"position(anchor={Serialize(position.Anchor)})",
            CountTerm count => SerializeCount(count),
            CarrierCountTerm count => $"count(carrier={Serialize(count.Carrier)},kind={SerializeCarrierCountKind(count.Kind)})",
            CarrierSpanTerm span => $"span(carrier={Serialize(span.Carrier)})",
            PowerTerm power => SerializePower(power),
            InverseContinueTerm inverse => SerializeInverse(inverse),
            PinTerm pin => SerializePin(pin),
            PinToPinTerm pinToPin => $"pin-to-pin(host={Serialize(pinToPin.HostAnchor)},applied={Serialize(pinToPin.AppliedAnchor)})",
            AxisBooleanTerm boolean => SerializeBoolean(boolean),
            FoldTerm fold => $"fold(kind={fold.Kind},source={Serialize(fold.Source)})",
            EqualityTerm equality => $"equal(left={Serialize(equality.Left)},right={Serialize(equality.Right)})",
            SharedCarrierTerm shared => $"share(left={Serialize(shared.Left)},right={Serialize(shared.Right)})",
            RouteTerm route => $"route(site={Serialize(route.Site)},from={Serialize(route.From)},to={Serialize(route.To)})",
            JunctionTerm junction => $"junction(site={Serialize(junction.Site)},kind={SerializeJunction(junction.Kind)})",
            SiteFlagTerm flag => $"has(site={Serialize(flag.Site)},flag={SerializeSiteFlag(flag.Kind)})",
            CarrierFlagTerm flag => $"has(carrier={Serialize(flag.Carrier)},flag={SerializeCarrierFlag(flag.Kind)})",
            RequirementTerm requirement => SerializeRequirement(requirement),
            PreferenceTerm preference => SerializePreference(preference),
            ConstraintSetTerm set => $"constraints([{string.Join(",", set.Constraints.Select(Serialize))}])",
            BranchFamilyTerm branchFamily => SerializeBranchFamily(branchFamily.Family),
            BindTerm bind => $"bind(name={Escape(bind.Target.QualifiedName)},value={Serialize(bind.Value)})",
            CommitTerm commit => $"commit(name={Escape(commit.Target.QualifiedName)},value={Serialize(commit.Value)})",
            EmitTerm emit => $"emit(value={Serialize(emit.Value)})",
            SequenceTerm sequence => $"sequence([{string.Join(",", sequence.Steps.Select(Serialize))}])",
            _ => term.ToString() ?? term.GetType().Name,
        };
    }
}
