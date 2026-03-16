using Core2.Elements;

namespace Core2.Repetition;

public readonly record struct AxisTraversalStep(
    Axis Segment,
    IReadOnlyList<RepetitionTension> Tensions,
    bool BreakAfter = false)
{
    public Proportion Start => Segment.StartCoordinate;
    public Proportion End => Segment.EndCoordinate;
    public Proportion Delta => Segment.CoordinateSpan;
    public bool IsDegenerate => Segment.IsDegenerate;
}
