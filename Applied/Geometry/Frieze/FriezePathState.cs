using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.Frieze;

public sealed record FriezePathState(
    PlanarPoint Cursor,
    IReadOnlyList<PlanarPathEdge> Segments,
    Scalar MacroStepValue)
{
    public static FriezePathState Origin { get; } = new(PlanarPoint.Origin, [], Scalar.Zero);

    public int MacroStep => PlanarValueConverter.ToInt(MacroStepValue);
}
