namespace ResoEngine.Visualizer.Core;

/// <summary>
/// Minimal directed-segment shape needed by the current renderer and drag system.
/// Implementations may be plain display data or live projections of Core/Core2 elements.
/// </summary>
public interface ISegmentValue
{
    float Imaginary { get; set; }
    float Real { get; set; }
    string Label { get; set; }
}
