using Core2.Elements;

namespace Core2.Repetition;

internal static class PinBoundaryTraversal
{
    public static PinTraversalAdvance Advance(
        Proportion start,
        Proportion stepMagnitude,
        int direction,
        Axis? frame,
        BoundaryPinPair? boundaryPins)
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

        while (remaining > Proportion.Zero)
        {
            if (currentFrame is null)
            {
                Proportion unboundedNext = Move(current, currentDirection, remaining);
                fragments.Add(new PinTraversalFragment(current, unboundedNext, [], false));
                current = unboundedNext;
                remaining = Proportion.Zero;
                break;
            }

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

            Proportion edge = currentDirection > 0 ? max : min;
            Proportion distanceToEdge = (edge - current).Abs();

            if (distanceToEdge.IsZero)
            {
                ResolveEncounter(
                    ref current,
                    edge,
                    currentDirection > 0 ? max : min,
                    ref currentDirection,
                    ref currentFrame,
                    ref currentPins,
                    ref remaining,
                    fragments);
                continue;
            }

            if (remaining <= distanceToEdge)
            {
                Proportion next = Move(current, currentDirection, remaining);
                fragments.Add(new PinTraversalFragment(current, next, [], false));
                current = next;
                current = CanonicalizeBoundaryRestingPoint(current, currentDirection, currentPins);
                remaining = Proportion.Zero;
                break;
            }

            LocatedPin? boundaryPin = currentPins?.ResolveForDirection(currentDirection);
            if (boundaryPin is null)
            {
                Proportion overflow = Move(current, currentDirection, remaining);
                fragments.Add(new PinTraversalFragment(
                    current,
                    overflow,
                    [CreateBoundaryTension(overflow, min, max)],
                    false));
                current = overflow;
                currentFrame = null;
                currentPins = null;
                remaining = Proportion.Zero;
                break;
            }

            fragments.Add(new PinTraversalFragment(current, edge, [], false));
            current = edge;
            remaining -= distanceToEdge;

            ResolveEncounter(
                ref current,
                edge,
                currentDirection > 0 ? max : min,
                ref currentDirection,
                ref currentFrame,
                ref currentPins,
                ref remaining,
                fragments);
        }

        return new PinTraversalAdvance(current, currentDirection, currentFrame, currentPins, fragments);
    }

    private static void ResolveEncounter(
        ref Proportion current,
        Proportion edge,
        Proportion oppositeEdge,
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
                fragments.Add(new PinTraversalFragment(
                    current,
                    Move(current, direction, remaining),
                    [CreateBoundaryTension(Move(current, direction, remaining), Proportion.Min(edge, oppositeEdge), Proportion.Max(edge, oppositeEdge))],
                    false));
                current = Move(current, direction, remaining);
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
                fragments.Add(new PinTraversalFragment(
                    current,
                    Move(current, direction, remaining),
                    [CreateBoundaryTension(Move(current, direction, remaining), Proportion.Min(edge, oppositeEdge), Proportion.Max(edge, oppositeEdge))],
                    false));
                current = Move(current, direction, remaining);
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
        frame = egress.Frame;
        pins = egress.Context;
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
