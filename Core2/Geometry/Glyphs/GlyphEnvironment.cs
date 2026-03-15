using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed record GlyphEnvironment(
    GlyphBox Box,
    IReadOnlyList<GlyphLandmark> Landmarks,
    IReadOnlyList<GlyphFieldEmitter> FieldEmitters,
    IReadOnlyList<TensionPacket> AmbientPackets)
{
    public IReadOnlyList<GlyphFieldInfluence> SampleInfluencesAt(GlyphVector point) =>
        FieldEmitters
            .SelectMany(emitter => emitter.EmitAt(point))
            .ToArray();
}
