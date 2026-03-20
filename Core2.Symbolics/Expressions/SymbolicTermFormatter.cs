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
            ApplyTransformTerm apply => $"{Format(apply.State)} * {Format(apply.Transform)}",
            PinTerm pin => $"{Format(pin.Host)} * {Format(pin.Applied)} @ {pin.Position}",
            FoldTerm fold => $"{FormatFoldKind(fold.Kind)}({Format(fold.Source)})",
            EqualityTerm equality => $"{Format(equality.Left)} == {Format(equality.Right)}",
            SharedCarrierTerm shared => $"share({Format(shared.Left)}, {Format(shared.Right)})",
            RouteTerm route => $"route({Format(route.Site)}, {Format(route.From)}, {Format(route.To)})",
            RequirementTerm requirement => $"require({Format(requirement.Relation)})",
            PreferenceTerm preference => $"prefer({Format(preference.Relation)}, {preference.Weight})",
            BranchFamilyTerm branchFamily => $"branch{{{string.Join(" | ", branchFamily.Family.Values.Select(Format))}}}",
            BindTerm bind => $"let {bind.Name} = {Format(bind.Value)}",
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
