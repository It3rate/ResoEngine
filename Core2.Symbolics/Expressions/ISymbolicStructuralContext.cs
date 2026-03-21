using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public interface ISymbolicStructuralContext
{
    bool TryResolveAnchorCarrier(
        AnchorReferenceTerm anchor,
        out CarrierId carrierId,
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
}
