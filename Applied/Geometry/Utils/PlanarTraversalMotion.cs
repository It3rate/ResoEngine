namespace Applied.Geometry.Utils;

public readonly record struct PlanarTraversalMotion(PlanarOffset Delta, bool IsVisible = true)
{
    public static PlanarTraversalMotion Hidden(PlanarOffset delta) => new(delta, IsVisible: false);
    public static PlanarTraversalMotion Visible(PlanarOffset delta) => new(delta, IsVisible: true);
}
