using Core3.Engine;

namespace Core3.Engine.Operations;

internal static class EngineBooleanProjection
{
    internal static bool TryResolve(
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(primary);
        ArgumentNullException.ThrowIfNull(secondary);

        if (!TryReadAtomicSegment(frame, frame, out var frameSegment) ||
            !TryReadAtomicSegment(frame, primary, out var primarySegment) ||
            !TryReadAtomicSegment(frame, secondary, out var secondarySegment))
        {
            result = null;
            return false;
        }

        if (frameSegment.End <= frameSegment.Start)
        {
            result = new EngineBooleanResult(frame, primary, secondary, operation, []);
            return true;
        }

        var boundaries = CollectBoundaries(frameSegment, primarySegment, secondarySegment);
        var pieces = new List<EngineBooleanPiece>();
        decimal? currentLeft = null;
        decimal? currentRight = null;
        CompositeElement? currentCarrier = null;
        bool currentInPrimary = false;
        bool currentInSecondary = false;

        foreach (var (left, right) in boundaries.Zip(boundaries.Skip(1)))
        {
            if (right <= left)
            {
                continue;
            }

            var midpoint = (left + right) / 2m;
            var inPrimary = IsWithin(midpoint, primarySegment);
            var inSecondary = IsWithin(midpoint, secondarySegment);

            if (!operation.Evaluate(inPrimary, inSecondary))
            {
                Flush();
                continue;
            }

            var carrier = SelectCarrier(inPrimary, inSecondary, frame, primary, secondary);

            if (currentCarrier is not null &&
                currentRight == left &&
                AreCompatibleCarriers(currentCarrier, carrier))
            {
                currentRight = right;
                currentInPrimary |= inPrimary;
                currentInSecondary |= inSecondary;
                continue;
            }

            Flush();
            currentLeft = left;
            currentRight = right;
            currentCarrier = carrier;
            currentInPrimary = inPrimary;
            currentInSecondary = inSecondary;
        }

        Flush();
        result = new EngineBooleanResult(frame, primary, secondary, operation, pieces);
        return true;

        void Flush()
        {
            if (currentLeft is null ||
                currentRight is null ||
                currentCarrier is null)
            {
                currentLeft = null;
                currentRight = null;
                currentCarrier = null;
                currentInPrimary = false;
                currentInSecondary = false;
                return;
            }

            pieces.Add(new EngineBooleanPiece(
                CreateSegmentLike(currentCarrier, currentLeft.Value, currentRight.Value),
                currentCarrier,
                currentInPrimary,
                currentInSecondary));

            currentLeft = null;
            currentRight = null;
            currentCarrier = null;
            currentInPrimary = false;
            currentInSecondary = false;
        }
    }

    private static bool TryReadAtomicSegment(
        CompositeElement frame,
        CompositeElement segment,
        out AtomicSegment atomicSegment)
    {
        if (!segment.TryReferenceToFrame(frame, out var read) ||
            read is not CompositeElement readComposite ||
            readComposite.Recessive is not AtomicElement start ||
            readComposite.Dominant is not AtomicElement end)
        {
            atomicSegment = default;
            return false;
        }

        atomicSegment = new AtomicSegment(
            Math.Min(ToDecimal(start), ToDecimal(end)),
            Math.Max(ToDecimal(start), ToDecimal(end)));
        return true;
    }

    private static SortedSet<decimal> CollectBoundaries(
        AtomicSegment frame,
        AtomicSegment primary,
        AtomicSegment secondary)
    {
        var boundaries = new SortedSet<decimal> { frame.Start, frame.End };
        AddIfWithin(boundaries, primary.Start, frame);
        AddIfWithin(boundaries, primary.End, frame);
        AddIfWithin(boundaries, secondary.Start, frame);
        AddIfWithin(boundaries, secondary.End, frame);
        return boundaries;
    }

    private static void AddIfWithin(SortedSet<decimal> boundaries, decimal value, AtomicSegment frame)
    {
        if (value > frame.Start && value < frame.End)
        {
            boundaries.Add(value);
        }
    }

    private static bool IsWithin(decimal value, AtomicSegment segment) =>
        value > segment.Start && value < segment.End;

    private static CompositeElement SelectCarrier(
        bool inPrimary,
        bool inSecondary,
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary)
    {
        if (inPrimary)
        {
            return primary;
        }

        if (inSecondary)
        {
            return secondary;
        }

        return frame;
    }

    private static bool AreCompatibleCarriers(CompositeElement left, CompositeElement right) =>
        left.Equals(right);

    private static CompositeElement CreateSegmentLike(
        CompositeElement template,
        decimal left,
        decimal right)
    {
        if (template.Recessive is not AtomicElement start ||
            template.Dominant is not AtomicElement end)
        {
            throw new InvalidOperationException("Boolean segment pieces currently require atomic endpoints.");
        }

        var forward = ToDecimal(start) <= ToDecimal(end);
        var leftAtomic = FromDecimal(left, start.Unit);
        var rightAtomic = FromDecimal(right, start.Unit);

        return forward
            ? new CompositeElement(leftAtomic, rightAtomic)
            : new CompositeElement(rightAtomic, leftAtomic);
    }

    private static decimal ToDecimal(AtomicElement atomic) =>
        atomic.Unit == 0 ? 0m : (decimal)atomic.Value / atomic.Unit;

    private static AtomicElement FromDecimal(decimal value, long unit)
    {
        if (unit == 0)
        {
            return new AtomicElement(0, 0);
        }

        var scaled = value * unit;
        if (decimal.Truncate(scaled) != scaled)
        {
            throw new InvalidOperationException("Boolean partition could not be expressed exactly in the carrier resolution.");
        }

        return new AtomicElement((long)scaled, unit);
    }

    private readonly record struct AtomicSegment(decimal Start, decimal End);
}
