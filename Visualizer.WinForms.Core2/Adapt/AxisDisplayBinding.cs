using Core2.Elements;
using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Adapt;

public sealed class AxisDisplayBinding
{
    public AxisDisplayBinding(Axis axis, string label)
    {
        Axis = axis;
        Label = label;
        Segment = AxisDisplayMapper.ToSegment(axis, label);
    }

    public Axis Axis { get; private set; }
    public string Label { get; }
    public DirectedSegment Segment { get; }

    public void CaptureInput()
    {
        Axis = AxisDisplayMapper.FromSegment(
            Segment,
            Axis.Recessive.Recessive,
            Axis.Dominant.Recessive);

        AxisDisplayMapper.CopyToSegment(Axis, Segment, Label);
    }

    public void SetAxis(Axis axis)
    {
        Axis = axis;
        AxisDisplayMapper.CopyToSegment(axis, Segment, Label);
    }
}
