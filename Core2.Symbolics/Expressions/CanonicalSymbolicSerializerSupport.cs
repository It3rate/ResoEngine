using Core2.Boolean;
using Core2.Elements;
using Core2.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class CanonicalSymbolicSerializer
{
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

    private static string SerializeBoundaryLaw(BoundaryContinuationLaw law) => law switch
    {
        BoundaryContinuationLaw.PeriodicWrap => "wrap",
        BoundaryContinuationLaw.ReflectiveBounce => "reflect",
        BoundaryContinuationLaw.Clamp => "clamp",
        BoundaryContinuationLaw.TensionPreserving => "tension",
        _ => law.ToString(),
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

    private static string SerializeCountKind(SymbolicCountKind kind) => kind switch
    {
        SymbolicCountKind.Carriers => "carriers",
        SymbolicCountKind.Sites => "sites",
        SymbolicCountKind.ParticipatingCarriers => "participating-carriers",
        SymbolicCountKind.ThroughCarriers => "through-carriers",
        _ => kind.ToString(),
    };

    private static string SerializeCarrierCountKind(SymbolicCarrierCountKind kind) => kind switch
    {
        SymbolicCarrierCountKind.HostedSites => "hosted-sites",
        SymbolicCarrierCountKind.Attachments => "attachments",
        SymbolicCarrierCountKind.ReferencingHosts => "referencing-hosts",
        SymbolicCarrierCountKind.ParticipatingSites => "participating-sites",
        SymbolicCarrierCountKind.ThroughSites => "through-sites",
        _ => kind.ToString(),
    };

    private static string SerializeCarrierFlag(SymbolicCarrierFlagKind kind) => kind switch
    {
        SymbolicCarrierFlagKind.Shared => "shared",
        SymbolicCarrierFlagKind.Recursive => "recursive",
        SymbolicCarrierFlagKind.Span => "span",
        SymbolicCarrierFlagKind.Hosted => "hosted",
        SymbolicCarrierFlagKind.Referenced => "referenced",
        _ => kind.ToString(),
    };

    private static string Escape(string value) =>
        $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
