using Core2.Elements;
using Core2.Repetition;

namespace Core2.Geometry;

public sealed record StripSegmentDefinition(
    string Name,
    Axis Segment,
    BoundaryContinuationLaw Law,
    StripDelta AxisVector,
    StripSegmentStepMode StepMode = StripSegmentStepMode.Unit,
    bool UseSegmentAsFrame = true,
    Scalar? Seed = null)
{
    public AxisTraversalDefinition CreateTraversal() =>
        new(
            UseSegmentAsFrame ? Segment : null,
            ComputeStep(),
            Law,
            Seed ?? Segment.Start);

    public Scalar ComputeStep()
    {
        decimal delta = Segment.End.Value - Segment.Start.Value;
        return StepMode switch
        {
            StripSegmentStepMode.Unit when delta > 0m => Scalar.One,
            StripSegmentStepMode.Unit when delta < 0m => -Scalar.One,
            StripSegmentStepMode.Unit => Scalar.Zero,
            StripSegmentStepMode.Span => new Scalar(delta),
            _ => throw new ArgumentOutOfRangeException(nameof(StepMode), StepMode, null),
        };
    }

    public string DescribeTraversal()
    {
        string frameText = UseSegmentAsFrame
            ? $"frame [{Format(Segment.Start)}, {Format(Segment.End)}]"
            : "unbounded";
        string stepText = $"step {Format(ComputeStep())}";
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

    public StripDelta Project(Scalar delta)
    {
        int amount = decimal.ToInt32(decimal.Round(delta.Value, 0, MidpointRounding.AwayFromZero));
        return new StripDelta(AxisVector.Dx * amount, AxisVector.Dy * amount);
    }

    private static string Format(Scalar value) =>
        value.Value == decimal.Truncate(value.Value)
            ? value.Value.ToString("0")
            : value.Value.ToString("0.##");
}
