using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.Frieze;

public sealed record FriezePathState(
    PlanarPoint Cursor,
    IReadOnlyList<PlanarPathEdge> Segments,
    Proportion MacroStepValue)
{
    public static FriezePathState Origin { get; } = new(PlanarPoint.Origin, [], Proportion.Zero);

    public int MacroStep => PlanarValueConverter.ToInt(MacroStepValue);
}
