using Core2.Elements;

namespace Core2.Repetition;

public sealed record AxisTraversalDefinition(
    Axis? Frame,
    Proportion Step,
    BoundaryContinuationLaw Law = BoundaryContinuationLaw.TensionPreserving,
    Proportion? Seed = null)
{
    public AxisTraversalState CreateState() => new(this, Seed ?? Proportion.Zero);

    public IEnumerable<AxisTraversalStep> Iterate()
    {
        var state = CreateState();
        while (true)
        {
            yield return state.Fire();
        }
    }

    public IEnumerable<AxisTraversalStep> Iterate(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var state = CreateState();
        for (int index = 0; index < count; index++)
        {
            yield return state.Fire();
        }
    }
}
