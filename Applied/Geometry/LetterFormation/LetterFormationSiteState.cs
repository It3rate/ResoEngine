using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationSiteState(
    string Id,
    PlanarPoint Position,
    Axis Descriptor,
    PlanarOffset Momentum,
    IReadOnlyList<LetterFormationDesire> Desires)
{
    public LetterFormationSiteState WithPosition(PlanarPoint position) => this with { Position = position };
    public LetterFormationSiteState WithDescriptor(Axis descriptor) => this with { Descriptor = descriptor };
    public LetterFormationSiteState WithMomentum(PlanarOffset momentum) => this with { Momentum = momentum };
}
