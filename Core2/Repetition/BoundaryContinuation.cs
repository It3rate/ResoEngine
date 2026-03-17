using Core2.Elements;

namespace Core2.Repetition;

public static class BoundaryContinuation
{
    public static BoundaryContinuationResult Continue(Axis frame, Scalar value, BoundaryContinuationLaw law) =>
        Continue(frame, value.AsProportion(), law);

    public static BoundaryContinuationResult Continue(Axis frame, Scalar value, BoundaryPinPair? boundaryPins) =>
        Continue(frame, value.AsProportion(), boundaryPins);

    public static BoundaryContinuationResult Continue(Axis frame, Proportion value, BoundaryContinuationLaw law) =>
        Continue(frame, value, BoundaryPinPair.FromLaw(frame, law));

    public static BoundaryContinuationResult Continue(Axis frame, Proportion value, BoundaryPinPair? boundaryPins) =>
        Continue(frame.LeftCoordinate, frame.RightCoordinate, value, boundaryPins ?? BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.TensionPreserving));

    public static BoundaryContinuationResult Continue(
        Proportion min,
        Proportion max,
        Proportion value,
        BoundaryContinuationLaw law)
    {
        var frame = Axis.FromCoordinates(min, max);
        return Continue(frame, value, BoundaryPinPair.FromLaw(frame, law));
    }

    public static BoundaryContinuationResult Continue(
        Proportion min,
        Proportion max,
        Proportion value,
        BoundaryPinPair? boundaryPins)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (min == max)
        {
            return new BoundaryContinuationResult(min, []);
        }

        if (value >= min && value <= max)
        {
            return new BoundaryContinuationResult(value, []);
        }

        var frame = Axis.FromCoordinates(min, max);
        int direction = value > max ? 1 : -1;
        Proportion start = direction > 0 ? max : min;
        Proportion overshoot = direction > 0 ? value - max : min - value;
        var advance = PinBoundaryTraversal.Advance(start, overshoot, direction, frame, boundaryPins);
        return new BoundaryContinuationResult(advance.FinalValue, advance.Tensions);
    }
}
