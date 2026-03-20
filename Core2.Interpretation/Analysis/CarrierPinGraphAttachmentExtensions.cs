using Core2.Elements;

namespace Core2.Interpretation.Analysis;

public static class CarrierPinGraphAttachmentExtensions
{
    public static IReadOnlyList<CarrierAttachmentOccurrence> GetAttachments(this CarrierPinGraph graph, CarrierId carrierId)
    {
        ArgumentNullException.ThrowIfNull(graph);

        return graph.Sites
            .SelectMany(
                site => site.SideAttachments.Select(
                    attachment => new CarrierAttachmentOccurrence(
                        attachment.Carrier,
                        site,
                        attachment)))
            .Where(occurrence => occurrence.CarrierId == carrierId)
            .OrderBy(occurrence => occurrence.CarrierPosition)
            .ThenBy(occurrence => occurrence.SiteId.Value)
            .ToArray();
    }
}
