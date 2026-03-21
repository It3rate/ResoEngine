using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public interface ISymbolicStructuralContext
{
    bool TryResolveAnchorCarrier(
        AnchorReferenceTerm anchor,
        out CarrierId carrierId,
        out string? note);

    bool TryResolveAnchorPosition(
        AnchorReferenceTerm anchor,
        out Core2.Elements.Proportion position,
        out string? note);

    bool TryResolveRoute(
        SiteReferenceTerm site,
        RouteIncidentKind from,
        RouteIncidentKind to,
        out bool exists,
        out string? note);

    bool TryResolveJunctionSummary(
        SiteReferenceTerm site,
        out SymbolicJunctionKind kind,
        out string? note);

    bool TryResolveSiteFlag(
        SiteReferenceTerm site,
        SymbolicSiteFlagKind flag,
        out bool value,
        out string? note);

    bool TryResolveCount(
        CountTerm count,
        out Core2.Elements.Proportion value,
        out string? note);

    bool TryResolveCarrierCount(
        CarrierCountTerm count,
        out Core2.Elements.Proportion value,
        out string? note);

    bool TryResolveCarrierSpan(
        CarrierSpanTerm span,
        out Core2.Elements.Proportion value,
        out string? note);
}
