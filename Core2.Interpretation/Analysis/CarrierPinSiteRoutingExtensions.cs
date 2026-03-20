using Core2.Elements;

namespace Core2.Interpretation.Analysis;

public static class CarrierPinSiteRoutingExtensions
{
    public static CarrierSiteRouting ResolveRouting(this CarrierPinSite site)
    {
        ArgumentNullException.ThrowIfNull(site);
        return CarrierSiteRouting.FromSite(site);
    }
}
