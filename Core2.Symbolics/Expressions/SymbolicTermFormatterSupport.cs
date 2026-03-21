using Core2.Boolean;
using Core2.Elements;
using Core2.Repetition;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicTermFormatter
{
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

    private static string FormatBoundaryLaw(BoundaryContinuationLaw law) => law switch
    {
        BoundaryContinuationLaw.PeriodicWrap => "wrap",
        BoundaryContinuationLaw.ReflectiveBounce => "reflect",
        BoundaryContinuationLaw.Clamp => "clamp",
        BoundaryContinuationLaw.TensionPreserving => "tension",
        _ => law.ToString(),
    };

    private static string FormatJunction(SymbolicJunctionKind kind) => kind switch
    {
        SymbolicJunctionKind.Open => "open",
        SymbolicJunctionKind.Cusp => "cusp",
        SymbolicJunctionKind.Branch => "branch",
        SymbolicJunctionKind.Tee => "tee",
        SymbolicJunctionKind.Cross => "cross",
        _ => kind.ToString(),
    };

    private static string FormatSiteFlag(SymbolicSiteFlagKind kind) => kind switch
    {
        SymbolicSiteFlagKind.HostThrough => "host-through",
        SymbolicSiteFlagKind.CrossProposal => "cross-proposal",
        SymbolicSiteFlagKind.TrueCross => "true-cross",
        _ => kind.ToString(),
    };

    private static string FormatCountKind(SymbolicCountKind kind) => kind switch
    {
        SymbolicCountKind.Carriers => "carriers",
        SymbolicCountKind.Sites => "sites",
        SymbolicCountKind.ParticipatingCarriers => "participating-carriers",
        SymbolicCountKind.ThroughCarriers => "through-carriers",
        _ => kind.ToString(),
    };

    private static string FormatCarrierCountKind(SymbolicCarrierCountKind kind) => kind switch
    {
        SymbolicCarrierCountKind.HostedSites => "hosted-sites",
        SymbolicCarrierCountKind.Attachments => "attachments",
        SymbolicCarrierCountKind.ReferencingHosts => "referencing-hosts",
        SymbolicCarrierCountKind.ParticipatingSites => "participating-sites",
        SymbolicCarrierCountKind.ThroughSites => "through-sites",
        _ => kind.ToString(),
    };

    private static string FormatCarrierFlag(SymbolicCarrierFlagKind kind) => kind switch
    {
        SymbolicCarrierFlagKind.Shared => "shared",
        SymbolicCarrierFlagKind.Recursive => "recursive",
        SymbolicCarrierFlagKind.Span => "span",
        SymbolicCarrierFlagKind.Hosted => "hosted",
        SymbolicCarrierFlagKind.Referenced => "referenced",
        _ => kind.ToString(),
    };

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

    private static string FormatRule(InverseContinuationRule rule) => rule switch
    {
        InverseContinuationRule.Principal => "principal",
        InverseContinuationRule.PreferPositiveDominant => "prefer-positive",
        InverseContinuationRule.NearestToReference => "nearest",
        _ => rule.ToString(),
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
