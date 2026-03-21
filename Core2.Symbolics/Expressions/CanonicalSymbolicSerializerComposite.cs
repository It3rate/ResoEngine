using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public static partial class CanonicalSymbolicSerializer
{
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

    private static string SerializeCount(CountTerm count)
    {
        string sitePart = count.Site is null
            ? string.Empty
            : $"site={Serialize(count.Site)},";
        return $"count({sitePart}kind={SerializeCountKind(count.Kind)})";
    }
}
