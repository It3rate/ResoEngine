using Core2.Elements;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Adapt;

/// <summary>
/// Live display adapter over a Core2 Axis.
/// It does not own display-state copies; it projects from the Axis on demand and
/// writes drag changes back into the referenced Axis using its current supports.
/// </summary>
public sealed class AxisDisplayMapper : ISegmentValue
{
    public AxisDisplayMapper(Axis axis, string label = "")
    {
        Axis = axis;
        Label = label;
    }

    public Axis Axis { get; private set; }
    public string Label { get; set; }

    public float Imaginary
    {
        get => ToFloat(Axis.Start);
        set => Axis = Axis.FromCoordinates(
            (Scalar)(decimal)value,
            Axis.End,
            Axis.Recessive.Recessive,
            Axis.Dominant.Recessive);
    }

    public float Real
    {
        get => ToFloat(Axis.End);
        set => Axis = Axis.FromCoordinates(
            Axis.Start,
            (Scalar)(decimal)value,
            Axis.Recessive.Recessive,
            Axis.Dominant.Recessive);
    }

    public void SetAxis(Axis axis)
    {
        Axis = axis;
    }

    public static float ToFloat(Scalar value) => (float)(decimal)value;
    public static float ToFloat(Proportion value) => ToFloat(value.Fold());
}
