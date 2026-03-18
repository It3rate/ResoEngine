using Core2.Elements;

namespace Core2.Repetition;

public sealed record AxisTraversalDefinition(
    Axis? Frame,
    Proportion Step,
    BoundaryContinuationLaw Law = BoundaryContinuationLaw.TensionPreserving,
    Proportion? Seed = null,
    BoundaryPinPair? BoundaryPins = null,
    IReadOnlyList<LocatedPin>? Pins = null)
{
    public AxisTraversalState CreateState() => new(this, Seed ?? Proportion.Zero);

    public BoundaryPinPair? ResolveBoundaryPins()
    {
        if (BoundaryPins is not null)
        {
            return BoundaryPins;
        }

        if (Frame is null)
        {
            return null;
        }

        return Law == BoundaryContinuationLaw.TensionPreserving
            ? BoundaryPinPair.Open(Frame)
            : BoundaryPinPair.FromLaw(Frame, Law);
    }

    public IReadOnlyList<PointPinning<Axis, Axis>> ResolvePointPins()
    {
        if (Frame is null || Pins is null || Pins.Count == 0)
        {
            return [];
        }

        return Pins.Select(pin => pin.AttachTo(Frame)).ToArray();
    }

    public IReadOnlyList<PositionedAxis> ResolvePlacedAppliedAxes() =>
        Pins is null || Pins.Count == 0
            ? []
            : Pins.Select(pin => pin.PlaceApplied()).ToArray();

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
