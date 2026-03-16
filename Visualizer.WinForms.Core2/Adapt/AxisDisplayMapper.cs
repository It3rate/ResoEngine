using Core2.Elements;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Adapt;

/// <summary>
/// Live display adapter over a Core2 Axis.
/// It does not own display-state copies; it projects from the Axis on demand and
/// writes drag changes back into the referenced Axis using its current supports.
/// </summary>
public sealed class AxisDisplayMapper : ISegmentValue, ISegmentDragConfig
{
    public AxisDisplayMapper(Axis axis, string label = "", float snapIncrement = 0.5f)
    {
        Axis = axis;
        Label = label;
        SnapIncrement = snapIncrement;
    }

    public Axis Axis { get; private set; }
    public string Label { get; set; }
    public float SnapIncrement { get; }

    public float Imaginary
    {
        get => ToFloat(Axis.Start);
        set => Axis = Axis.FromCoordinates(
            ToCoordinate(value),
            Axis.EndCoordinate,
            Axis.Basis);
    }

    public float Real
    {
        get => ToFloat(Axis.End);
        set => Axis = Axis.FromCoordinates(
            Axis.StartCoordinate,
            ToCoordinate(value),
            Axis.Basis);
    }

    public void SetAxis(Axis axis)
    {
        Axis = axis;
    }

    public static float ToFloat(Scalar value) => (float)(decimal)value;
    public static float ToFloat(Proportion value) => ToFloat(value.Fold());

    private static Proportion ToCoordinate(float value) =>
        ((Scalar)(decimal)value).AsProportion();
}
