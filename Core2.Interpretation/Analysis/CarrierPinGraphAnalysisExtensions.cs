using Core2.Elements;

namespace Core2.Interpretation.Analysis;

public static class CarrierPinGraphAnalysisExtensions
{
    public static CarrierPinGraphAnalysis Analyze(this CarrierPinGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        CarrierSiteStructuralProfile[] siteProfiles = graph.Sites
            .Select(site => new CarrierSiteStructuralProfile(site, site.ResolveRouting()))
            .ToArray();

        CarrierStructuralProfile[] profiles = graph.Carriers
            .Select(
                carrier => new CarrierStructuralProfile(
                    carrier,
                    graph.GetHostedSites(carrier.Id),
                    graph.GetAttachments(carrier.Id),
                    graph.GetReferencedCarriers(carrier.Id),
                    graph.GetReferencingSites(carrier.Id)
                        .Select(site => site.HostCarrier)
                        .DistinctBy(host => host.Id)
                        .ToArray(),
                    siteProfiles
                        .Where(profile => profile.Participates(carrier.Id))
                        .ToArray(),
                    siteProfiles
                        .Where(profile => profile.CarriesThrough(carrier.Id))
                        .ToArray(),
                    graph.ParticipatesInRecursiveCycle(carrier.Id)))
            .ToArray();

        return new CarrierPinGraphAnalysis(profiles, siteProfiles);
    }
}
