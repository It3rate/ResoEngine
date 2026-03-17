using Core2.Elements;

namespace Core2.Repetition;

public sealed class BoundaryPinPair
{
    private BoundaryPinPair(Axis frame)
    {
        Frame = frame;
    }

    public Axis Frame { get; }
    public LocatedPin? LeftPin { get; private set; }
    public LocatedPin? RightPin { get; private set; }

    public LocatedPin? ResolveForDirection(int direction) => direction < 0 ? LeftPin : RightPin;

    public static BoundaryPinPair FromLaw(Axis frame, BoundaryContinuationLaw law)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var context = new BoundaryPinPair(frame);
        Proportion left = frame.LeftCoordinate;
        Proportion right = frame.RightCoordinate;

        switch (law)
        {
            case BoundaryContinuationLaw.TensionPreserving:
                return context;

            case BoundaryContinuationLaw.Clamp:
                context.LeftPin = new LocatedPin(left, Axis.PinUnit, absorbs: true, name: "Clamp(left)");
                context.RightPin = new LocatedPin(right, Axis.PinUnit, absorbs: true, name: "Clamp(right)");
                return context;

            case BoundaryContinuationLaw.ReflectiveBounce:
                context.LeftPin = new LocatedPin(
                    left,
                    Axis.PinUnit,
                    [new PinEgress(left, +1, context, "Reflect(inward)")],
                    name: "Reflect(left)");
                context.RightPin = new LocatedPin(
                    right,
                    Axis.PinUnit,
                    [new PinEgress(right, -1, context, "Reflect(inward)")],
                    name: "Reflect(right)");
                return context;

            case BoundaryContinuationLaw.PeriodicWrap:
                context.LeftPin = new LocatedPin(
                    left,
                    Axis.PinUnit,
                    [new PinEgress(right, -1, context, "Wrap(to-right)")],
                    name: "Wrap(left)");
                context.RightPin = new LocatedPin(
                    right,
                    Axis.PinUnit,
                    [new PinEgress(left, +1, context, "Wrap(to-left)")],
                    name: "Wrap(right)");
                return context;

            default:
                throw new ArgumentOutOfRangeException(nameof(law), law, null);
        }
    }
}
