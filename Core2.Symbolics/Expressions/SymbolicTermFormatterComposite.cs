using Core2.Branching;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicTermFormatter
{
    private static string FormatCount(CountTerm count) =>
        count.Site is null
            ? $"count({FormatCountKind(count.Kind)})"
            : $"count({Format(count.Site)}, {FormatCountKind(count.Kind)})";

    private static string FormatBoolean(AxisBooleanTerm boolean)
    {
        string head = FormatBooleanOperation(boolean.Operation);
        string primary = Format(boolean.Primary);
        string secondary = Format(boolean.Secondary);
        return boolean.Frame is null
            ? $"{head}({primary}, {secondary})"
            : $"{head}({primary}, {secondary}, {Format(boolean.Frame)})";
    }

    private static string FormatRequirement(RequirementTerm requirement) =>
        requirement.ParticipantName is null
            ? $"require({Format(requirement.Relation)})"
            : $"require({requirement.ParticipantName}, {Format(requirement.Relation)})";

    private static string FormatPreference(PreferenceTerm preference) =>
        preference.ParticipantName is null
            ? $"prefer({Format(preference.Relation)}, {preference.Weight})"
            : $"prefer({preference.ParticipantName}, {Format(preference.Relation)}, {preference.Weight})";

    private static string FormatPower(PowerTerm power) =>
        power.Reference is null && power.Rule == InverseContinuationRule.Principal
            ? $"pow({Format(power.Base)}, {power.Exponent})"
            : power.Reference is null
                ? $"pow({Format(power.Base)}, {power.Exponent}, {FormatRule(power.Rule)})"
                : $"pow({Format(power.Base)}, {power.Exponent}, {FormatRule(power.Rule)}, {Format(power.Reference)})";

    private static string FormatInverse(InverseContinueTerm inverse) =>
        inverse.Reference is null && inverse.Rule == InverseContinuationRule.Principal
            ? $"inverse({Format(inverse.Source)}, {inverse.Degree})"
            : inverse.Reference is null
                ? $"inverse({Format(inverse.Source)}, {inverse.Degree}, {FormatRule(inverse.Rule)})"
                : $"inverse({Format(inverse.Source)}, {inverse.Degree}, {FormatRule(inverse.Rule)}, {Format(inverse.Reference)})";

    private static string FormatBranchFamily(BranchFamilyTerm branchFamily) =>
        $"branch{{{string.Join(" | ", branchFamily.Family.Values.Select(Format))}}}";

    private static string FormatConstraintSet(ConstraintSetTerm set) =>
        $"constraints{{{string.Join(" | ", set.Constraints.Select(Format))}}}";
}
