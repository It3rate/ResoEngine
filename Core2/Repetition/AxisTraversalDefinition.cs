using Core2.Elements;

namespace Core2.Repetition;

public sealed record AxisTraversalDefinition(
    Axis? Frame,
    Scalar Step,
    BoundaryContinuationLaw Law = BoundaryContinuationLaw.TensionPreserving,
    Scalar? Seed = null)
{
    public AxisTraversalState CreateState() => new(this, Seed ?? Scalar.Zero);
}
