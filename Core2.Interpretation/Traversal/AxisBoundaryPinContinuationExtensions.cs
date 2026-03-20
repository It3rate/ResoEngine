using Core2.Elements;
using Core2.Repetition;

namespace Core2.Interpretation.Traversal;

public static class AxisBoundaryPinContinuationExtensions
{
    public static BoundaryContinuationResult Continue(this Axis frame, Scalar value, BoundaryPinPair? boundaryPins) =>
        Continue(frame, value.AsProportion(), boundaryPins);

    public static BoundaryContinuationResult Continue(this Axis frame, Proportion value, BoundaryPinPair? boundaryPins)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return BoundaryPinContinuation.Continue(frame.LeftCoordinate, frame.RightCoordinate, value, boundaryPins ?? BoundaryPinPair.Open(frame));
    }
}

public static class BoundaryPinContinuation
{
    public static BoundaryContinuationResult Continue(Axis frame, Scalar value, BoundaryPinPair? boundaryPins) =>
        Continue(frame, value.AsProportion(), boundaryPins);

    public static BoundaryContinuationResult Continue(Axis frame, Proportion value, BoundaryPinPair? boundaryPins)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return Continue(frame.LeftCoordinate, frame.RightCoordinate, value, boundaryPins ?? BoundaryPinPair.Open(frame));
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
        var advance = PinBoundaryTraversal.Advance(start, overshoot, direction, frame, boundaryPins ?? BoundaryPinPair.Open(frame));
        return new BoundaryContinuationResult(advance.FinalValue, advance.Tensions);
    }
}
