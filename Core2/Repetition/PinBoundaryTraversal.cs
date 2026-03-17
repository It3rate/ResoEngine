using Core2.Elements;

namespace Core2.Repetition;

internal static class PinBoundaryTraversal
{
    public static PinTraversalAdvance Advance(
        Proportion start,
        Proportion stepMagnitude,
        int direction,
        Axis? frame,
        BoundaryPinPair? boundaryPins,
        IReadOnlyList<LocatedPin>? pins = null)
    {
        var fragments = new List<PinTraversalFragment>();

        if (stepMagnitude.IsZero)
        {
            return new PinTraversalAdvance(start, NormalizeDirection(direction), frame, boundaryPins, fragments);
        }

        Proportion current = start;
        Proportion remaining = stepMagnitude.Abs();
        int currentDirection = NormalizeDirection(direction);
        Axis? currentFrame = frame;
        BoundaryPinPair? currentPins = boundaryPins;
        IReadOnlyList<LocatedPin> locatedPins = pins ?? [];
        LocatedPin? recentlyExitedPin = null;

        while (remaining > Proportion.Zero)
        {
            if (currentFrame is null && locatedPins.Count == 0)
            {
                Proportion unboundedNext = Move(current, currentDirection, remaining);
                fragments.Add(new PinTraversalFragment(current, unboundedNext, [], false));
                current = unboundedNext;
                remaining = Proportion.Zero;
                break;
            }

            if (currentFrame is not null)
            {
                Proportion min = currentFrame.LeftCoordinate;
                Proportion max = currentFrame.RightCoordinate;

                if (current < min || current > max)
                {
                    Proportion outsideNext = Move(current, currentDirection, remaining);
                    fragments.Add(new PinTraversalFragment(
                        current,
                        outsideNext,
                        [CreateBoundaryTension(outsideNext, min, max)],
                        false));
                    current = outsideNext;
                    currentFrame = null;
                    currentPins = null;
                    remaining = Proportion.Zero;
                    break;
                }
            }

            Proportion? edge = currentFrame is null
                ? null
                : currentDirection > 0 ? currentFrame.RightCoordinate : currentFrame.LeftCoordinate;
            Proportion distanceToEdge = edge is Proportion edgeValue
                ? (edgeValue - current).Abs()
                : Proportion.Zero;
            PinTraversalEncounter? pinEncounter = FindNextPin(current, currentDirection, currentFrame, locatedPins, recentlyExitedPin);

            if (pinEncounter is { Distance.IsZero: true })
            {
                ResolveLocatedEncounter(
                    ref current,
                    pinEncounter.Value.Pin,
                    ref currentDirection,
                    ref currentFrame,
                    ref currentPins,
                    ref remaining,
                    fragments);
                recentlyExitedPin = pinEncounter.Value.Pin;
                continue;
            }

            if (edge is Proportion boundaryEdge && distanceToEdge.IsZero)
            {
                ResolveBoundaryEncounter(
                    ref current,
                    boundaryEdge,
                    ref currentDirection,
                    ref currentFrame,
                    ref currentPins,
                    ref remaining,
                    fragments);
                recentlyExitedPin = null;
                continue;
            }

            Proportion? encounterDistance = null;
            bool encounterIsPin = false;

            if (pinEncounter is not null)
            {
                encounterDistance = pinEncounter.Value.Distance;
                encounterIsPin = true;
            }

            if (edge is Proportion && (encounterDistance is null || distanceToEdge < encounterDistance))
            {
                encounterDistance = distanceToEdge;
                encounterIsPin = false;
            }

            if (encounterDistance is null || remaining <= encounterDistance)
            {
                Proportion next = Move(current, currentDirection, remaining);
                fragments.Add(new PinTraversalFragment(current, next, [], false));
                current = next;
                current = CanonicalizeBoundaryRestingPoint(current, currentDirection, currentPins);
                remaining = Proportion.Zero;
                break;
            }

            Proportion travel = encounterDistance;
            Proportion encounterPoint = Move(current, currentDirection, travel);
            fragments.Add(new PinTraversalFragment(current, encounterPoint, [], false));
            current = encounterPoint;
            remaining -= travel;
            recentlyExitedPin = null;

            if (encounterIsPin && pinEncounter is not null)
            {
                ResolveLocatedEncounter(
                    ref current,
                    pinEncounter.Value.Pin,
                    ref currentDirection,
                    ref currentFrame,
                    ref currentPins,
                    ref remaining,
                    fragments);
                recentlyExitedPin = pinEncounter.Value.Pin;
                continue;
            }

            if (edge is not Proportion encounteredEdge)
            {
                break;
            }

            ResolveBoundaryEncounter(
                ref current,
                encounteredEdge,
                ref currentDirection,
                ref currentFrame,
                ref currentPins,
                ref remaining,
                fragments);
            recentlyExitedPin = null;
        }

        return new PinTraversalAdvance(current, currentDirection, currentFrame, currentPins, fragments);
    }

    private static void ResolveBoundaryEncounter(
        ref Proportion current,
        Proportion edge,
        ref int direction,
        ref Axis? frame,
        ref BoundaryPinPair? pins,
        ref Proportion remaining,
        List<PinTraversalFragment> fragments)
    {
        LocatedPin? pin = pins?.ResolveForDirection(direction);
        if (pin is null)
        {
            if (remaining > Proportion.Zero)
            {
                Proportion min = frame is null ? edge : frame.LeftCoordinate;
                Proportion max = frame is null ? edge : frame.RightCoordinate;
                Proportion overflow = Move(current, direction, remaining);
                fragments.Add(new PinTraversalFragment(
                    current,
                    overflow,
                    [CreateBoundaryTension(overflow, Proportion.Min(min, max), Proportion.Max(min, max))],
                    false));
                current = overflow;
                remaining = Proportion.Zero;
            }

            frame = null;
            pins = null;
            return;
        }

        if (pin.Absorbs || pin.OutputCount == 0)
        {
            remaining = Proportion.Zero;
            return;
        }

        PinEgress egress = pin.PrimaryOutput!;

        if (egress.Context == pins &&
            egress.Start == edge &&
            NormalizeDirection(egress.DirectionSign) == direction)
        {
            if (remaining > Proportion.Zero)
            {
                Proportion min = frame is null ? edge : frame.LeftCoordinate;
                Proportion max = frame is null ? edge : frame.RightCoordinate;
                Proportion overflow = Move(current, direction, remaining);
                fragments.Add(new PinTraversalFragment(
                    current,
                    overflow,
                    [CreateBoundaryTension(overflow, Proportion.Min(min, max), Proportion.Max(min, max))],
                    false));
                current = overflow;
                remaining = Proportion.Zero;
            }

            frame = null;
            pins = null;
            return;
        }

        bool breakAfter = egress.Start == edge && NormalizeDirection(egress.DirectionSign) != direction;
        if (fragments.Count > 0)
        {
            fragments[^1] = fragments[^1] with { BreakAfter = breakAfter };
        }

        direction = NormalizeDirection(egress.DirectionSign == 0 ? direction : egress.DirectionSign);
        frame = egress.Context is null && egress.PreservesCurrentContext ? frame : egress.Frame;
        pins = egress.Context is null && egress.PreservesCurrentContext ? pins : egress.Context;
        current = egress.Start;
    }

    private static void ResolveLocatedEncounter(
        ref Proportion current,
        LocatedPin pin,
        ref int direction,
        ref Axis? frame,
        ref BoundaryPinPair? boundaryPins,
        ref Proportion remaining,
        List<PinTraversalFragment> fragments)
    {
        List<RepetitionTension> tensions = [];
        if (pin.OutputCount > 1)
        {
            tensions.Add(new RepetitionTension(
                RepetitionTensionKind.MultipleOutputsDeferred,
                $"Pin '{pin.Name ?? pin.Location.ToString()}' has {pin.OutputCount} outputs; continuing through the primary output only."));
        }

        if (pin.Absorbs || pin.OutputCount == 0)
        {
            remaining = Proportion.Zero;
            AppendEncounterTensions(fragments, current, tensions);
            return;
        }

        PinEgress egress = pin.PrimaryOutput!;
        int nextDirection = NormalizeDirection(egress.DirectionSign == 0 ? direction : egress.DirectionSign);

        if (egress.PreservesCurrentContext &&
            egress.Context is null &&
            egress.Start == current &&
            nextDirection == direction)
        {
            tensions.Add(new RepetitionTension(
                RepetitionTensionKind.PinStalled,
                $"Pin '{pin.Name ?? pin.Location.ToString()}' produced no progress from {current}."));
            remaining = Proportion.Zero;
            AppendEncounterTensions(fragments, current, tensions);
            return;
        }

        if (fragments.Count > 0)
        {
            fragments[^1] = fragments[^1] with
            {
                BreakAfter = true,
                Tensions = fragments[^1].Tensions.Concat(tensions).ToArray(),
            };
        }
        else if (tensions.Count > 0)
        {
            fragments.Add(new PinTraversalFragment(current, current, tensions, true));
        }

        direction = nextDirection;

        if (egress.Context is not null)
        {
            frame = egress.Frame;
            boundaryPins = egress.Context;
        }
        else if (!egress.PreservesCurrentContext)
        {
            frame = null;
            boundaryPins = null;
        }

        current = egress.Start;
    }

    private static Proportion Move(Proportion start, int direction, Proportion amount) =>
        direction < 0 ? start - amount : start + amount;

    private static int NormalizeDirection(int direction) => direction < 0 ? -1 : 1;

    private static Proportion CanonicalizeBoundaryRestingPoint(
        Proportion current,
        int direction,
        BoundaryPinPair? pins)
    {
        LocatedPin? pin = pins?.ResolveForDirection(direction);
        if (pin is null || pin.Absorbs || pin.OutputCount == 0)
        {
            return current;
        }

        if (pin.Location != current)
        {
            return current;
        }

        PinEgress egress = pin.PrimaryOutput!;
        return egress.Start != current ? egress.Start : current;
    }

    private static RepetitionTension CreateBoundaryTension(Proportion value, Proportion min, Proportion max) =>
        new(
            RepetitionTensionKind.BoundaryExceeded,
            $"Value {value} exceeded frame [{min}, {max}] and was preserved as tension.");

    private static void AppendEncounterTensions(
        List<PinTraversalFragment> fragments,
        Proportion current,
        IReadOnlyList<RepetitionTension> tensions)
    {
        if (tensions.Count == 0)
        {
            return;
        }

        if (fragments.Count > 0)
        {
            fragments[^1] = fragments[^1] with
            {
                Tensions = fragments[^1].Tensions.Concat(tensions).ToArray(),
            };
            return;
        }

        fragments.Add(new PinTraversalFragment(current, current, tensions, false));
    }

    private static PinTraversalEncounter? FindNextPin(
        Proportion current,
        int direction,
        Axis? frame,
        IReadOnlyList<LocatedPin> pins,
        LocatedPin? recentlyExitedPin)
    {
        if (pins.Count == 0)
        {
            return null;
        }

        Proportion? bestDistance = null;
        LocatedPin? bestPin = null;
        Proportion? min = frame?.LeftCoordinate;
        Proportion? max = frame?.RightCoordinate;

        foreach (var pin in pins)
        {
            Proportion location = pin.Location;

            if (recentlyExitedPin is not null &&
                ReferenceEquals(pin, recentlyExitedPin) &&
                location == current)
            {
                continue;
            }

            if (min is Proportion left && location < left)
            {
                continue;
            }

            if (max is Proportion right && location > right)
            {
                continue;
            }

            Proportion distance;
            if (direction > 0)
            {
                if (location < current)
                {
                    continue;
                }

                if (max is Proportion rightEdge && location == rightEdge)
                {
                    continue;
                }

                distance = location - current;
            }
            else
            {
                if (location > current)
                {
                    continue;
                }

                if (min is Proportion leftEdge && location == leftEdge)
                {
                    continue;
                }

                distance = current - location;
            }

            if (bestDistance is null || distance < bestDistance)
            {
                bestDistance = distance;
                bestPin = pin;
            }
        }

        return bestPin is null || bestDistance is null
            ? null
            : new PinTraversalEncounter(bestPin, bestDistance);
    }
}

internal readonly record struct PinTraversalFragment(
    Proportion Start,
    Proportion End,
    IReadOnlyList<RepetitionTension> Tensions,
    bool BreakAfter);

internal sealed record PinTraversalAdvance(
    Proportion FinalValue,
    int FinalDirection,
    Axis? ActiveFrame,
    BoundaryPinPair? ActivePins,
    IReadOnlyList<PinTraversalFragment> Fragments)
{
    public IReadOnlyList<RepetitionTension> Tensions =>
        Fragments.SelectMany(fragment => fragment.Tensions).ToArray();
}

internal readonly record struct PinTraversalEncounter(
    LocatedPin Pin,
    Proportion Distance);
