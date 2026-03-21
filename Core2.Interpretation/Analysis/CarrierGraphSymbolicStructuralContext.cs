using Core2.Elements;
using Core2.Symbolics.Expressions;

namespace Core2.Interpretation.Analysis;

public sealed class CarrierGraphSymbolicStructuralContext : ISymbolicStructuralContext
{
    private readonly IReadOnlyDictionary<string, CarrierSiteStructuralProfile> _sitesByName;

    public CarrierGraphSymbolicStructuralContext(CarrierPinGraphAnalysis analysis)
    {
        ArgumentNullException.ThrowIfNull(analysis);

        Analysis = analysis;
        _sitesByName = analysis.SiteProfiles
            .Where(profile => !string.IsNullOrWhiteSpace(profile.Name))
            .ToDictionary(profile => profile.Name!, StringComparer.Ordinal);
    }

    public CarrierPinGraphAnalysis Analysis { get; }

    public bool TryResolveAnchorCarrier(
        AnchorReferenceTerm anchor,
        out CarrierId carrierId,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        carrierId = default;
        note = null;

        if (!_sitesByName.TryGetValue(anchor.OwnerName, out var siteProfile))
        {
            note = $"No named site '{anchor.OwnerName}' exists in the structural context.";
            return false;
        }

        if (anchor.SideRole is null)
        {
            note = $"Anchor '{anchor.QualifiedName}' does not identify a carrier side.";
            return false;
        }

        var attachment = siteProfile.Site.GetAttachment(anchor.SideRole.Value);
        if (attachment is null)
        {
            note = $"Anchor '{anchor.QualifiedName}' is not structurally bound to a carrier.";
            return false;
        }

        carrierId = attachment.CarrierId;
        return true;
    }

    public bool TryResolveAnchorPosition(
        AnchorReferenceTerm anchor,
        out Proportion position,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        position = Proportion.Zero;
        note = null;

        if (!_sitesByName.TryGetValue(anchor.OwnerName, out var siteProfile))
        {
            note = $"No named site '{anchor.OwnerName}' exists in the structural context.";
            return false;
        }

        if (anchor.SideRole is null)
        {
            note = $"Anchor '{anchor.QualifiedName}' does not identify a carrier side.";
            return false;
        }

        var attachment = siteProfile.Site.GetAttachment(anchor.SideRole.Value);
        if (attachment is null)
        {
            note = $"Anchor '{anchor.QualifiedName}' has no structural attachment position.";
            return false;
        }

        position = attachment.CarrierPosition;
        return true;
    }

    public bool TryResolveRoute(
        SiteReferenceTerm site,
        RouteIncidentKind from,
        RouteIncidentKind to,
        out bool exists,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(site);

        exists = false;
        note = null;

        if (!_sitesByName.TryGetValue(site.SiteName, out var siteProfile))
        {
            note = $"No named site '{site.SiteName}' exists in the structural context.";
            return false;
        }

        exists = siteProfile.Routing.HasThroughRoute(MapIncident(from), MapIncident(to));
        return true;
    }

    public bool TryResolveJunctionSummary(
        SiteReferenceTerm site,
        out SymbolicJunctionKind kind,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(site);

        kind = default;
        note = null;

        if (!_sitesByName.TryGetValue(site.SiteName, out var siteProfile))
        {
            note = $"No named site '{site.SiteName}' exists in the structural context.";
            return false;
        }

        kind = MapJunction(siteProfile.Summary);
        return true;
    }

    public bool TryResolveSiteFlag(
        SiteReferenceTerm site,
        SymbolicSiteFlagKind flag,
        out bool value,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(site);

        value = false;
        note = null;

        if (!_sitesByName.TryGetValue(site.SiteName, out var siteProfile))
        {
            note = $"No named site '{site.SiteName}' exists in the structural context.";
            return false;
        }

        value = flag switch
        {
            SymbolicSiteFlagKind.HostThrough => siteProfile.HostContinues,
            SymbolicSiteFlagKind.CrossProposal => siteProfile.HasCrossShapedProposal,
            SymbolicSiteFlagKind.TrueCross => siteProfile.HasTrueCross,
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null),
        };
        return true;
    }

    public bool TryResolveCount(
        CountTerm count,
        out Proportion value,
        out string? note)
    {
        ArgumentNullException.ThrowIfNull(count);

        note = null;

        if (count.IsGlobal)
        {
            value = count.Kind switch
            {
                SymbolicCountKind.Carriers => new Proportion(Analysis.Profiles.Count),
                SymbolicCountKind.Sites => new Proportion(Analysis.SiteProfiles.Count),
                _ =>
                    throw new InvalidOperationException(
                        $"Global count kind '{count.Kind}' requires site scope."),
            };
            return true;
        }

        if (!_sitesByName.TryGetValue(count.Site!.SiteName, out var siteProfile))
        {
            value = Proportion.Zero;
            note = $"No named site '{count.Site.SiteName}' exists in the structural context.";
            return false;
        }

        value = count.Kind switch
        {
            SymbolicCountKind.ParticipatingCarriers => new Proportion(siteProfile.ParticipatingCarriers.Count),
            SymbolicCountKind.ThroughCarriers => new Proportion(siteProfile.ThroughCarriers.Count),
            _ => throw new InvalidOperationException(
                $"Site count kind '{count.Kind}' is not valid for named-site counting."),
        };
        return true;
    }

    private static CarrierIncidentKind MapIncident(RouteIncidentKind kind) => kind switch
    {
        RouteIncidentKind.HostNegative => CarrierIncidentKind.HostNegative,
        RouteIncidentKind.HostPositive => CarrierIncidentKind.HostPositive,
        RouteIncidentKind.RecessiveSide => CarrierIncidentKind.RecessiveSide,
        RouteIncidentKind.DominantSide => CarrierIncidentKind.DominantSide,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };

    private static SymbolicJunctionKind MapJunction(CarrierJunctionSummary summary) => summary switch
    {
        CarrierJunctionSummary.Open => SymbolicJunctionKind.Open,
        CarrierJunctionSummary.Cusp => SymbolicJunctionKind.Cusp,
        CarrierJunctionSummary.Branch => SymbolicJunctionKind.Branch,
        CarrierJunctionSummary.Tee => SymbolicJunctionKind.Tee,
        CarrierJunctionSummary.Cross => SymbolicJunctionKind.Cross,
        _ => throw new ArgumentOutOfRangeException(nameof(summary), summary, null),
    };
}
