namespace Core2.Elements;

/// <summary>
/// In one dimension, perspective is binary: the dominant view and its opposite.
/// Opposite perspective swaps which physical side is interpreted as left/right.
/// </summary>
public enum Perspective
{
    Dominant = 1,
    Opposite = -1,
}

public static class PerspectiveExtensions
{
    public static Perspective Oppose(this Perspective perspective) =>
        perspective == Perspective.Dominant ? Perspective.Opposite : Perspective.Dominant;
}
