using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed record PlanarSegmentDefinition(
    string Name,
    Axis Segment,
    BoundaryContinuationLaw Law,
    PlanarOffset AxisVector,
    Proportion Step,
    bool UseSegmentAsFrame = true,
    Proportion? Seed = null)
{
    public AxisTraversalDefinition CreateTraversal() =>
        new(
            UseSegmentAsFrame ? Segment : null,
            Step.Fold(),
            Law,
            Seed?.Fold() ?? Segment.Start);

    public Proportion ComputeStep() => Step;

    public string DescribeTraversal()
    {
        string frameText = UseSegmentAsFrame
            ? $"frame [{Format(Segment.StartCoordinate)}, {Format(Segment.EndCoordinate)}]"
            : "unbounded";
        string stepText = $"step {Format(Step)}";
        string lawText = Law switch
        {
            BoundaryContinuationLaw.ReflectiveBounce => "reflect",
            BoundaryContinuationLaw.PeriodicWrap => "wrap",
            BoundaryContinuationLaw.TensionPreserving => "continue+tension",
            BoundaryContinuationLaw.Clamp => "clamp",
            _ => Law.ToString(),
        };

        return $"{frameText} | {stepText} | {lawText}";
    }

    public PlanarOffset Project(Scalar delta)
        => Project(delta.Pin(Scalar.One));

    public PlanarOffset Project(Proportion delta) => AxisVector * delta;

    private static string Format(Proportion value) =>
        value.Recessive == 1 ? value.Dominant.ToString() : value.ToString();
}
