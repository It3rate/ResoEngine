using Core2.Elements;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Adapt;

public static class AxisDisplayMapper
{
    public static DirectedSegment ToSegment(Axis axis, string label = "")
    {
        var segment = new DirectedSegment(0f, 0f, label);
        CopyToSegment(axis, segment, label);
        return segment;
    }

    public static void CopyToSegment(Axis axis, DirectedSegment segment, string? label = null)
    {
        segment.Imaginary = ToFloat(axis.Start);
        segment.Real = ToFloat(axis.End);
        if (label != null)
        {
            segment.Label = label;
        }
    }

    public static Axis FromSegment(
        DirectedSegment segment,
        Scalar recessiveSupport,
        Scalar dominantSupport) =>
        Axis.FromCoordinates(
            (Scalar)(decimal)segment.Imaginary,
            (Scalar)(decimal)segment.Real,
            recessiveSupport,
            dominantSupport);

    public static float ToFloat(Scalar value) => (float)(decimal)value;
    public static float ToFloat(Proportion value) => ToFloat(value.Fold());
}
