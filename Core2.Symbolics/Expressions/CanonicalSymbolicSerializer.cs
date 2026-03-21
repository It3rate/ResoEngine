using Core2.Boolean;
using Core2.Branching;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public static class CanonicalSymbolicSerializer
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
            SiteReferenceTerm site => $"site({Escape(site.SiteName)})",
            AnchorReferenceTerm anchor => $"anchor(owner={Escape(anchor.OwnerName)},name={Escape(anchor.AnchorName)})",
            IncidentReferenceTerm incident => $"incident({incident.Kind})",
            ApplyTransformTerm apply => $"apply(state={Serialize(apply.State)},transform={Serialize(apply.Transform)})",
            MultiplyValuesTerm multiply => $"multiply(left={Serialize(multiply.Left)},right={Serialize(multiply.Right)})",
            DivideValuesTerm divide => $"divide(left={Serialize(divide.Left)},right={Serialize(divide.Right)})",
            AnchorPositionTerm position => $"position(anchor={Serialize(position.Anchor)})",
            CountTerm count => SerializeCount(count),
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

    private static string SerializePin(PinTerm pin)
    {
        string anchorPart = pin.AppliedAnchor is null
            ? string.Empty
            : $",appliedAnchor={Serialize(pin.AppliedAnchor)}";

        return $"pin(host={Serialize(pin.Host)},applied={Serialize(pin.Applied)},at={SerializeElement(pin.Position)}{anchorPart})";
    }

    private static string SerializeBoolean(AxisBooleanTerm boolean)
    {
        string framePart = boolean.Frame is null
            ? string.Empty
            : $",frame={Serialize(boolean.Frame)}";

        return $"bool(op={SerializeBooleanOperation(boolean.Operation)},primary={Serialize(boolean.Primary)},secondary={Serialize(boolean.Secondary)}{framePart})";
    }

    private static string SerializePower(PowerTerm power)
    {
        string referencePart = power.Reference is null
            ? string.Empty
            : $",reference={Serialize(power.Reference)}";
        return $"power(base={Serialize(power.Base)},exponent={SerializeElement(power.Exponent)},rule={SerializeRule(power.Rule)}{referencePart})";
    }

    private static string SerializeInverse(InverseContinueTerm inverse)
    {
        string referencePart = inverse.Reference is null
            ? string.Empty
            : $",reference={Serialize(inverse.Reference)}";
        return $"inverse(source={Serialize(inverse.Source)},degree={SerializeElement(inverse.Degree)},rule={SerializeRule(inverse.Rule)}{referencePart})";
    }

    private static string SerializeRequirement(RequirementTerm requirement)
    {
        string participantPart = requirement.ParticipantName is null
            ? string.Empty
            : $"participant={Escape(requirement.ParticipantName)},";
        return $"require({participantPart}relation={Serialize(requirement.Relation)})";
    }

    private static string SerializePreference(PreferenceTerm preference)
    {
        string participantPart = preference.ParticipantName is null
            ? string.Empty
            : $"participant={Escape(preference.ParticipantName)},";
        return $"prefer({participantPart}relation={Serialize(preference.Relation)},weight={SerializeElement(preference.Weight)})";
    }

    private static string SerializeBranchFamily(BranchFamily<ValueTerm> family)
    {
        string members = string.Join(",", family.Members.Select(SerializeBranchMember));
        string selected = family.Selection.SelectedId?.ToString() ?? "none";
        string reason = family.Selection.Reason is null ? "none" : Escape(family.Selection.Reason);

        return $"branch(origin={family.Origin},semantics={family.Semantics},direction={family.Direction},selectionMode={family.Selection.Mode},selected={selected},reason={reason},members=[{members}])";
    }

    private static string SerializeBranchMember(BranchMember<ValueTerm> member)
    {
        string parents = string.Join(",", member.Parents.Select(parent => parent.ToString()));
        return $"member(id={member.Id},parents=[{parents}],value={Serialize(member.Value)})";
    }

    private static string SerializeElement(IElement element) => element switch
    {
        Scalar scalar => $"scalar({scalar})",
        Proportion proportion => $"proportion({proportion.Numerator}/{proportion.Denominator})",
        Axis axis => $"axis(recessive={SerializeElement(axis.Recessive)},dominant={SerializeElement(axis.Dominant)},basis={axis.Basis})",
        Area area => $"area(recessive={SerializeElement(area.Recessive)},dominant={SerializeElement(area.Dominant)})",
        _ => $"{element.GetType().Name}({Escape(element.ToString() ?? string.Empty)})",
    };

    private static string SerializeBooleanOperation(AxisBooleanOperation operation) => operation switch
    {
        AxisBooleanOperation.False => "false-op",
        AxisBooleanOperation.True => "true-op",
        AxisBooleanOperation.TransferA => "transfer-a",
        AxisBooleanOperation.TransferB => "transfer-b",
        AxisBooleanOperation.And => "and",
        AxisBooleanOperation.Or => "or",
        AxisBooleanOperation.Nand => "nand",
        AxisBooleanOperation.Nor => "nor",
        AxisBooleanOperation.NotA => "not-a",
        AxisBooleanOperation.NotB => "not-b",
        AxisBooleanOperation.Implication => "implication",
        AxisBooleanOperation.ReverseImplication => "reverse-implication",
        AxisBooleanOperation.Inhibition => "inhibition",
        AxisBooleanOperation.ReverseInhibition => "reverse-inhibition",
        AxisBooleanOperation.Xor => "xor",
        AxisBooleanOperation.Xnor => "xnor",
        _ => operation.ToString(),
    };

    private static string SerializeRule(Core2.Symbolics.Repetition.InverseContinuationRule rule) => rule switch
    {
        Core2.Symbolics.Repetition.InverseContinuationRule.Principal => "principal",
        Core2.Symbolics.Repetition.InverseContinuationRule.PreferPositiveDominant => "prefer-positive",
        Core2.Symbolics.Repetition.InverseContinuationRule.NearestToReference => "nearest",
        _ => rule.ToString(),
    };

    private static string SerializeJunction(SymbolicJunctionKind kind) => kind switch
    {
        SymbolicJunctionKind.Open => "open",
        SymbolicJunctionKind.Cusp => "cusp",
        SymbolicJunctionKind.Branch => "branch",
        SymbolicJunctionKind.Tee => "tee",
        SymbolicJunctionKind.Cross => "cross",
        _ => kind.ToString(),
    };

    private static string SerializeSiteFlag(SymbolicSiteFlagKind kind) => kind switch
    {
        SymbolicSiteFlagKind.HostThrough => "host-through",
        SymbolicSiteFlagKind.CrossProposal => "cross-proposal",
        SymbolicSiteFlagKind.TrueCross => "true-cross",
        _ => kind.ToString(),
    };

    private static string SerializeCount(CountTerm count)
    {
        string sitePart = count.Site is null
            ? string.Empty
            : $"site={Serialize(count.Site)},";
        return $"count({sitePart}kind={SerializeCountKind(count.Kind)})";
    }

    private static string SerializeCountKind(SymbolicCountKind kind) => kind switch
    {
        SymbolicCountKind.Carriers => "carriers",
        SymbolicCountKind.Sites => "sites",
        SymbolicCountKind.ParticipatingCarriers => "participating-carriers",
        SymbolicCountKind.ThroughCarriers => "through-carriers",
        _ => kind.ToString(),
    };

    private static string Escape(string value) =>
        $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
