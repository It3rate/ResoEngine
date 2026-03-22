using Applied.Geometry.Utils;

namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationState(
    string Key,
    int StepIndex,
    LetterFormationEnvironment Environment,
    IReadOnlyList<LetterFormationSiteState> Sites,
    IReadOnlyList<LetterFormationCarrierState> Carriers,
    IReadOnlyList<LetterFormationTension> Tensions)
{
    public LetterFormationSiteState GetSite(string siteId) =>
        Sites.First(site => string.Equals(site.Id, siteId, StringComparison.Ordinal));

    public LetterFormationCarrierState GetCarrier(string carrierId) =>
        Carriers.First(carrier => string.Equals(carrier.Id, carrierId, StringComparison.Ordinal));

    public bool TryGetSite(string siteId, out LetterFormationSiteState? site)
    {
        site = Sites.FirstOrDefault(candidate => string.Equals(candidate.Id, siteId, StringComparison.Ordinal));
        return site is not null;
    }

    public bool TryGetCarrier(string carrierId, out LetterFormationCarrierState? carrier)
    {
        carrier = Carriers.FirstOrDefault(candidate => string.Equals(candidate.Id, carrierId, StringComparison.Ordinal));
        return carrier is not null;
    }

    public PlanarPoint GetStartPoint(string carrierId) =>
        GetSite(GetCarrier(carrierId).StartSiteId).Position;

    public PlanarPoint GetEndPoint(string carrierId) =>
        GetSite(GetCarrier(carrierId).EndSiteId).Position;
}
