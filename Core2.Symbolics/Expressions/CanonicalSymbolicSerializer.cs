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
            ApplyTransformTerm apply => $"apply(state={Serialize(apply.State)},transform={Serialize(apply.Transform)})",
            PinTerm pin => $"pin(host={Serialize(pin.Host)},applied={Serialize(pin.Applied)},at={SerializeElement(pin.Position)})",
            FoldTerm fold => $"fold(kind={fold.Kind},source={Serialize(fold.Source)})",
            EqualityTerm equality => $"equal(left={Serialize(equality.Left)},right={Serialize(equality.Right)})",
            SharedCarrierTerm shared => $"share(left={Serialize(shared.Left)},right={Serialize(shared.Right)})",
            RouteTerm route => $"route(site={Serialize(route.Site)},from={Serialize(route.From)},to={Serialize(route.To)})",
            RequirementTerm requirement => $"require(relation={Serialize(requirement.Relation)})",
            PreferenceTerm preference => $"prefer(relation={Serialize(preference.Relation)},weight={SerializeElement(preference.Weight)})",
            BranchFamilyTerm branchFamily => SerializeBranchFamily(branchFamily.Family),
            BindTerm bind => $"bind(name={Escape(bind.Name)},value={Serialize(bind.Value)})",
            SequenceTerm sequence => $"sequence([{string.Join(",", sequence.Steps.Select(Serialize))}])",
            _ => term.ToString() ?? term.GetType().Name,
        };
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

    private static string Escape(string value) =>
        $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
