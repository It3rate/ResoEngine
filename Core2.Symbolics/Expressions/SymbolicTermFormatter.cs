using Core2.Boolean;
using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public static class SymbolicTermFormatter
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
            SiteReferenceTerm site => site.SiteName,
            AnchorReferenceTerm anchor => anchor.QualifiedName,
            IncidentReferenceTerm incident => FormatIncident(incident.Kind),
            ApplyTransformTerm apply => $"{Format(apply.State)} * {Format(apply.Transform)}",
            PinTerm pin => $"{Format(pin.Host)} * {Format(pin.AppliedAnchor ?? pin.Applied)} @ {pin.Position}",
            PinToPinTerm pinToPin => $"pin({Format(pinToPin.HostAnchor)}, {Format(pinToPin.AppliedAnchor)})",
            AxisBooleanTerm boolean => FormatBoolean(boolean),
            FoldTerm fold => $"{FormatFoldKind(fold.Kind)}({Format(fold.Source)})",
            EqualityTerm equality => $"{Format(equality.Left)} == {Format(equality.Right)}",
            SharedCarrierTerm shared => $"share({Format(shared.Left)}, {Format(shared.Right)})",
            RouteTerm route => $"route({Format(route.Site)}, {Format(route.From)}, {Format(route.To)})",
            RequirementTerm requirement => FormatRequirement(requirement),
            PreferenceTerm preference => FormatPreference(preference),
            ConstraintSetTerm set => $"constraints{{{string.Join(" | ", set.Constraints.Select(Format))}}}",
            BranchFamilyTerm branchFamily => $"branch{{{string.Join(" | ", branchFamily.Family.Values.Select(Format))}}}",
            BindTerm bind => $"let {bind.Name} = {Format(bind.Value)}",
            CommitTerm commit => $"commit {commit.Name} = {Format(commit.Value)}",
            EmitTerm emit => Format(emit.Value),
            SequenceTerm sequence => string.Join("; ", sequence.Steps.Select(Format)),
            _ => term.ToString() ?? term.GetType().Name,
        };
    }

    private static string FormatFoldKind(SymbolicFoldKind kind) => kind switch
    {
        SymbolicFoldKind.Canonical => "fold",
        SymbolicFoldKind.FoldFirst => "fold-first",
        SymbolicFoldKind.StructurePreserving => "structure-preserving",
        _ => "fold",
    };

    private static string FormatIncident(RouteIncidentKind kind) => kind switch
    {
        RouteIncidentKind.HostNegative => "host-",
        RouteIncidentKind.HostPositive => "host+",
        RouteIncidentKind.RecessiveSide => "i",
        RouteIncidentKind.DominantSide => "u",
        _ => kind.ToString(),
    };

    private static string FormatBoolean(AxisBooleanTerm boolean)
    {
        string head = FormatBooleanOperation(boolean.Operation);
        string primary = Format(boolean.Primary);
        string secondary = Format(boolean.Secondary);
        return boolean.Frame is null
            ? $"{head}({primary}, {secondary})"
            : $"{head}({primary}, {secondary}, {Format(boolean.Frame)})";
    }

    private static string FormatBooleanOperation(AxisBooleanOperation operation) => operation switch
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

    private static string FormatRequirement(RequirementTerm requirement) =>
        requirement.ParticipantName is null
            ? $"require({Format(requirement.Relation)})"
            : $"require({requirement.ParticipantName}, {Format(requirement.Relation)})";

    private static string FormatPreference(PreferenceTerm preference) =>
        preference.ParticipantName is null
            ? $"prefer({Format(preference.Relation)}, {preference.Weight})"
            : $"prefer({preference.ParticipantName}, {Format(preference.Relation)}, {preference.Weight})";

    private static string FormatElement(IElement element) => element switch
    {
        Scalar scalar => scalar.ToString(),
        Proportion proportion => proportion.ToString(),
        Axis axis when axis == Axis.One => "1",
        Axis axis when axis == Axis.I => "i",
        Axis axis when axis == Axis.NegativeOne => "-1",
        Axis axis when axis == Axis.NegativeI => "-i",
        Axis axis => axis.ToString(),
        Area area => area.ToString(),
        _ => element.ToString() ?? element.GetType().Name,
    };
}
