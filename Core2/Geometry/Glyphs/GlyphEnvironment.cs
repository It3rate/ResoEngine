using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphEnvironment(
    GlyphBox Box,
    IReadOnlyList<GlyphLandmark> Landmarks,
    IReadOnlyList<GlyphFieldEmitter> FieldEmitters,
    IReadOnlyList<TensionPacket> AmbientPackets)
{
    public IReadOnlyList<GlyphLandmark> GetLandmarks(GlyphLandmarkKind kind) =>
        Landmarks
            .Where(landmark => landmark.Kind == kind)
            .ToArray();

    public GlyphLandmark? FindClosestLandmark(
        GlyphVector point,
        params GlyphLandmarkKind[] kinds) =>
        Landmarks
            .Where(landmark => kinds.Contains(landmark.Kind))
            .OrderBy(landmark => landmark.Position.DistanceTo(point))
            .FirstOrDefault();

    public IReadOnlyList<GlyphFieldInfluence> SampleInfluencesAt(GlyphVector point) =>
        FieldEmitters
            .SelectMany(emitter => emitter.EmitAt(point))
            .ToArray();
}
