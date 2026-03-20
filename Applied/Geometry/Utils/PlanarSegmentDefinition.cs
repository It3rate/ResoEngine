using Core2.Elements;
using Core2.Interpretation.Traversal;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed record PlanarSegmentDefinition(
    string Name,
    Axis Segment,
    BoundaryContinuationLaw Law,
    PlanarOffset AxisVector,
    Proportion Step,
    bool UseSegmentAsFrame = true,
    Proportion? Seed = null,
    BoundaryPinPair? BoundaryPins = null)
{
    public AxisTraversalDefinition CreateTraversal() =>
        new(
            UseSegmentAsFrame ? Segment : null,
            Step,
            BoundaryLawSummary,
            Seed ?? Segment.StartCoordinate,
            UseSegmentAsFrame && BoundaryPins is not null ? ResolveBoundaryPins(Segment) : null);

    public BoundaryContinuationLaw BoundaryLawSummary =>
        BoundaryPins?.SummaryLaw ?? Law;

    public BoundaryPinPair? ResolveBoundaryPins(Axis frame)
    {
        if (BoundaryPins is not null)
        {
            return BoundaryPins.Reframe(frame);
        }

        return Law == BoundaryContinuationLaw.TensionPreserving
            ? BoundaryPinPair.Open(frame)
            : BoundaryPinPair.FromLaw(frame, Law);
    }

    public Proportion ComputeStep() => Step;

    public string DescribeTraversal()
    {
        string frameText = UseSegmentAsFrame
            ? $"frame [{Format(Segment.StartCoordinate)}, {Format(Segment.EndCoordinate)}]"
            : "unbounded";
        string stepText = $"step {Format(Step)}";
        string lawText = BoundaryPins is not null && BoundaryPins.SummaryLaw is null
            ? "custom pins"
            : BoundaryLawSummary switch
            {
                BoundaryContinuationLaw.ReflectiveBounce => "reflect",
                BoundaryContinuationLaw.PeriodicWrap => "wrap",
                BoundaryContinuationLaw.TensionPreserving => "continue+tension",
                BoundaryContinuationLaw.Clamp => "clamp",
                _ => BoundaryLawSummary.ToString(),
            };

        return $"{frameText} | {stepText} | {lawText}";
    }

    public PlanarOffset Project(Proportion delta) => AxisVector * delta;

    private static string Format(Proportion value) =>
        value.Recessive == 1 ? value.Dominant.ToString() : value.ToString();
}
