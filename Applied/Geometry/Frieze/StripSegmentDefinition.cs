using Applied.Geometry.Utils;
using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public sealed record StripSegmentDefinition(
    string Name,
    Axis Segment,
    BoundaryContinuationLaw Law,
    Directions2D AxisVector,
    Scalar Step,
    bool UseSegmentAsFrame = true,
    Scalar? Seed = null)
{
    public AxisTraversalDefinition CreateTraversal() =>
        new(
            UseSegmentAsFrame ? Segment : null,
            Step,
            Law,
            Seed ?? Segment.Start);

    public Scalar ComputeStep() => Step;

    public string DescribeTraversal()
    {
        string frameText = UseSegmentAsFrame
            ? $"frame [{Format(Segment.Start)}, {Format(Segment.End)}]"
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

        return $"{frameText} · {stepText} · {lawText}";
    }

    public Directions2D Project(Scalar delta)
    {
        int amount = decimal.ToInt32(decimal.Round(delta.Value, 0, MidpointRounding.AwayFromZero));
        return new Directions2D(AxisVector.Dx * amount, AxisVector.Dy * amount);
    }

    private static string Format(Scalar value) =>
        value.Value == decimal.Truncate(value.Value)
            ? value.Value.ToString("0")
            : value.Value.ToString("0.##");
}
