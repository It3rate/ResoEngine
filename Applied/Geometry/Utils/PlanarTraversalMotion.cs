namespace Applied.Geometry.Utils;

public readonly record struct PlanarTraversalMotion(PlanarOffset Delta, bool IsVisible = true, bool EndsStroke = false)
{
    public static PlanarTraversalMotion Hidden(PlanarOffset delta) => new(delta, IsVisible: false);
    public static PlanarTraversalMotion Visible(PlanarOffset delta, bool endsStroke = false) =>
        new(delta, IsVisible: true, endsStroke);
}
