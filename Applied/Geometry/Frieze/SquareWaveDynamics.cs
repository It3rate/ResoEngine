namespace Applied.Geometry.Frieze;

public static class SquareWaveDynamics
{
    public static Core2.Symbolics.Dynamic.DynamicTrace<FriezePathState, FriezeEnvironment, Applied.Geometry.Utils.PlanarTraversalEmission> Run(int steps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(steps);

        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "square-wave");
        return FriezeProgramDynamics.Run(pattern, steps);
    }
}
