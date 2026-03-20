using Core2.Elements;
using Core2.Repetition;

namespace Core2.Interpretation.Traversal;

public sealed class BoundaryPinPair
{
    private BoundaryPinPair(Axis frame)
    {
        Frame = frame;
    }

    public Axis Frame { get; }
    public LocatedPin? LeftPin { get; private set; }
    public LocatedPin? RightPin { get; private set; }
    public BoundaryContinuationLaw? SummaryLaw =>
        TrySummarizeLaw(out var law) ? law : null;

    public LocatedPin? ResolveForDirection(int direction) => direction < 0 ? LeftPin : RightPin;

    public static BoundaryPinPair Open(Axis frame) => new(frame);

    public static BoundaryPinPair Create(
        Axis frame,
        LocatedPin? leftPin = null,
        LocatedPin? rightPin = null)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var pair = new BoundaryPinPair(frame);
        pair.SetPins(leftPin, rightPin);
        return pair;
    }

    public static BoundaryPinPair FromLaw(Axis frame, BoundaryContinuationLaw law)
    {
        ArgumentNullException.ThrowIfNull(frame);

        Proportion left = frame.LeftCoordinate;
        Proportion right = frame.RightCoordinate;

        switch (law)
        {
            case BoundaryContinuationLaw.TensionPreserving:
                return Open(frame);

            case BoundaryContinuationLaw.Clamp:
                return Create(
                    frame,
                    new LocatedPin(left, Axis.PinUnit, absorbs: true, name: "Clamp(left)"),
                    new LocatedPin(right, Axis.PinUnit, absorbs: true, name: "Clamp(right)"));

            case BoundaryContinuationLaw.ReflectiveBounce:
                return Create(
                    frame,
                    new LocatedPin(
                        left,
                        Axis.PinUnit,
                        [new PinEgress(left, +1, name: "Reflect(inward)")],
                        name: "Reflect(left)"),
                    new LocatedPin(
                        right,
                        Axis.PinUnit,
                        [new PinEgress(right, -1, name: "Reflect(inward)")],
                        name: "Reflect(right)"));

            case BoundaryContinuationLaw.PeriodicWrap:
                return Create(
                    frame,
                    new LocatedPin(
                        left,
                        Axis.PinUnit,
                        [new PinEgress(right, -1, name: "Wrap(to-right)")],
                        name: "Wrap(left)"),
                    new LocatedPin(
                        right,
                        Axis.PinUnit,
                        [new PinEgress(left, +1, name: "Wrap(to-left)")],
                        name: "Wrap(right)"));

            default:
                throw new ArgumentOutOfRangeException(nameof(law), law, null);
        }
    }

    public void SetPins(LocatedPin? leftPin, LocatedPin? rightPin)
    {
        ValidatePinLocation(leftPin, Frame.LeftCoordinate, nameof(leftPin));
        ValidatePinLocation(rightPin, Frame.RightCoordinate, nameof(rightPin));
        LeftPin = leftPin;
        RightPin = rightPin;
    }

    public bool TrySummarizeLaw(out BoundaryContinuationLaw law)
    {
        if (LeftPin is null && RightPin is null)
        {
            law = BoundaryContinuationLaw.TensionPreserving;
            return true;
        }

        if (IsClamp(LeftPin, Frame.LeftCoordinate) && IsClamp(RightPin, Frame.RightCoordinate))
        {
            law = BoundaryContinuationLaw.Clamp;
            return true;
        }

        if (IsReflect(LeftPin, Frame.LeftCoordinate, +1) && IsReflect(RightPin, Frame.RightCoordinate, -1))
        {
            law = BoundaryContinuationLaw.ReflectiveBounce;
            return true;
        }

        if (IsImplicitReflect(LeftPin, Frame.LeftCoordinate, -1, +1) &&
            IsImplicitReflect(RightPin, Frame.RightCoordinate, +1, -1))
        {
            law = BoundaryContinuationLaw.ReflectiveBounce;
            return true;
        }

        if (IsWrap(LeftPin, Frame.LeftCoordinate, Frame.RightCoordinate, -1) &&
            IsWrap(RightPin, Frame.RightCoordinate, Frame.LeftCoordinate, +1))
        {
            law = BoundaryContinuationLaw.PeriodicWrap;
            return true;
        }

        law = default;
        return false;
    }

    public BoundaryPinPair Reframe(Axis frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        if (Frame == frame)
        {
            return this;
        }

        if (TrySummarizeLaw(out var law))
        {
            return law == BoundaryContinuationLaw.TensionPreserving
                ? Open(frame)
                : FromLaw(frame, law);
        }

        if (TryReframeCustomPins(frame, out var reframed))
        {
            return reframed;
        }

        throw new InvalidOperationException(
            $"Boundary pin pair on frame [{Frame.LeftCoordinate}, {Frame.RightCoordinate}] cannot yet be reframed to [{frame.LeftCoordinate}, {frame.RightCoordinate}] because it has no known summary law.");
    }

    private bool TryReframeCustomPins(Axis frame, out BoundaryPinPair reframed)
    {
        if (!TryReframePin(LeftPin, Frame.LeftCoordinate, Frame.RightCoordinate, frame, out var leftPin) ||
            !TryReframePin(RightPin, Frame.LeftCoordinate, Frame.RightCoordinate, frame, out var rightPin))
        {
            reframed = null!;
            return false;
        }

        reframed = Create(frame, leftPin, rightPin);
        return true;
    }

    private static void ValidatePinLocation(LocatedPin? pin, Proportion expectedLocation, string parameterName)
    {
        if (pin is null)
        {
            return;
        }

        if (pin.Location != expectedLocation)
        {
            throw new ArgumentException(
                $"Boundary pin '{pin.Name ?? pin.Location.ToString()}' must lie at boundary coordinate {expectedLocation}.",
                parameterName);
        }
    }

    private bool TryReframePin(
        LocatedPin? pin,
        Proportion oldLeft,
        Proportion oldRight,
        Axis newFrame,
        out LocatedPin? reframed)
    {
        if (pin is null)
        {
            reframed = null;
            return true;
        }

        if (!TryMapBoundaryCoordinate(pin.Location, oldLeft, oldRight, newFrame, out var newLocation))
        {
            reframed = null;
            return false;
        }

        List<PinEgress>? outputs = null;
        if (pin.OutputCount > 0)
        {
            outputs = [];
            foreach (var output in pin.Outputs)
            {
                if (!TryReframeEgress(output, oldLeft, oldRight, newFrame, out var reframedEgress))
                {
                    reframed = null;
                    return false;
                }

                outputs.Add(reframedEgress);
            }
        }

        reframed = new LocatedPin(newLocation, pin.Applied, outputs, pin.Absorbs, pin.Name, pin.SideAttachments);
        return true;
    }

    private bool TryReframeEgress(
        PinEgress egress,
        Proportion oldLeft,
        Proportion oldRight,
        Axis newFrame,
        out PinEgress reframed)
    {
        if (!TryMapBoundaryCoordinate(egress.Start, oldLeft, oldRight, newFrame, out var newStart))
        {
            reframed = null!;
            return false;
        }

        if (ReferenceEquals(egress.Context, this))
        {
            reframed = null!;
            return false;
        }

        BoundaryPinPair? context = null;
        if (egress.Context is not null)
        {
            context = egress.Context.Frame == Frame
                ? egress.Context.Reframe(newFrame)
                : egress.Context;
        }

        reframed = new PinEgress(
            newStart,
            egress.DirectionSign,
            context,
            egress.Name,
            egress.PreservesCurrentContext);
        return true;
    }

    private static bool TryMapBoundaryCoordinate(
        Proportion coordinate,
        Proportion oldLeft,
        Proportion oldRight,
        Axis newFrame,
        out Proportion mapped)
    {
        if (coordinate == oldLeft)
        {
            mapped = newFrame.LeftCoordinate;
            return true;
        }

        if (coordinate == oldRight)
        {
            mapped = newFrame.RightCoordinate;
            return true;
        }

        mapped = Proportion.Zero;
        return false;
    }

    private static bool IsClamp(LocatedPin? pin, Proportion location) =>
        pin is not null &&
        pin.Location == location &&
        pin.Absorbs &&
        pin.OutputCount == 0;

    private static bool IsReflect(LocatedPin? pin, Proportion location, int inwardDirection) =>
        pin is not null &&
        pin.Location == location &&
        !pin.Absorbs &&
        pin.OutputCount == 1 &&
        pin.PrimaryOutput is { } egress &&
        egress.Start == location &&
        Math.Sign(egress.DirectionSign) == inwardDirection &&
        egress.PreservesCurrentContext;

    private static bool IsWrap(LocatedPin? pin, Proportion location, Proportion target, int direction) =>
        pin is not null &&
        pin.Location == location &&
        !pin.Absorbs &&
        pin.OutputCount == 1 &&
        pin.PrimaryOutput is { } egress &&
        egress.Start == target &&
        Math.Sign(egress.DirectionSign) == direction &&
        egress.PreservesCurrentContext;

    private bool IsImplicitReflect(
        LocatedPin? pin,
        Proportion location,
        int boundaryDirection,
        int inwardDirection)
    {
        if (pin is null || pin.Location != location || pin.Absorbs || pin.OutputCount != 0)
        {
            return false;
        }

        LocatedPinTraversalResolution resolution = pin.ResolveImplicitTraversal(Frame, boundaryDirection, boundaryEncounter: true);
        return resolution.Handled &&
               !resolution.Absorbs &&
               !resolution.TransparentContinue &&
               resolution.PrimaryOutput is { } egress &&
               egress.Start == location &&
               Math.Sign(egress.DirectionSign) == inwardDirection &&
               egress.PreservesCurrentContext;
    }

    public IReadOnlyList<PointPinning<Axis, Axis>> ResolvePointPins()
    {
        List<PointPinning<Axis, Axis>> pins = [];
        if (LeftPin is not null)
        {
            pins.Add(LeftPin.AttachTo(Frame));
        }

        if (RightPin is not null)
        {
            pins.Add(RightPin.AttachTo(Frame));
        }

        return pins;
    }

    public IReadOnlyList<CarrierPinSite> ResolveCarrierSites(CarrierIdentity hostCarrier)
    {
        ArgumentNullException.ThrowIfNull(hostCarrier);

        List<CarrierPinSite> sites = [];
        if (LeftPin is not null)
        {
            sites.Add(LeftPin.ResolveCarrierPinSite(Frame, hostCarrier));
        }

        if (RightPin is not null)
        {
            sites.Add(RightPin.ResolveCarrierPinSite(Frame, hostCarrier));
        }

        return sites;
    }

    public CarrierPinGraph ResolveCarrierPinGraph(CarrierIdentity hostCarrier) =>
        new([hostCarrier], ResolveCarrierSites(hostCarrier));
}
