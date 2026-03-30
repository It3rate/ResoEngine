using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

internal static class EngineBooleanProjection
{
    internal static bool TryResolve(
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        if (!TryReadAtomicSegment(frame, frame, out var frameSegment) ||
            !TryReadAtomicSegment(frame, primary, out var primarySegment) ||
            !TryReadAtomicSegment(frame, secondary, out var secondarySegment))
        {
            result = null;
            return false;
        }

        if (frameSegment.End <= frameSegment.Start)
        {
            result = new EngineBooleanResult(
                new EngineOperationContext(frame, [primary, secondary], true),
                operation,
                []);
            return true;
        }

        var context = new EngineOperationContext(frame, [primary, secondary], true);
        var boundaries = CollectBoundaries(frameSegment, primarySegment, secondarySegment);
        var pieces = new List<EngineOperationPiece>();
        decimal? currentLeft = null;
        decimal? currentRight = null;
        CompositeElement? currentCarrier = null;
        List<int>? currentPresentMembers = null;

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
                AreCompatibleCarriers(currentCarrier, carrier) &&
                currentPresentMembers is not null &&
                currentPresentMembers.SequenceEqual(ToPresentMemberIndices(inPrimary, inSecondary)))
            {
                currentRight = right;
                continue;
            }

            Flush();
            currentLeft = left;
            currentRight = right;
            currentCarrier = carrier;
            currentPresentMembers = ToPresentMemberIndices(inPrimary, inSecondary);
        }

        Flush();
        result = new EngineBooleanResult(context, operation, pieces);
        return true;

        void Flush()
        {
            if (currentLeft is null ||
                currentRight is null ||
                currentCarrier is null ||
                currentPresentMembers is null)
            {
                currentLeft = null;
                currentRight = null;
                currentCarrier = null;
                currentPresentMembers = null;
                return;
            }

            pieces.Add(new EngineOperationPiece(
                CreateSegmentLike(currentCarrier, currentLeft.Value, currentRight.Value),
                currentCarrier,
                currentPresentMembers.ToArray()));

            currentLeft = null;
            currentRight = null;
            currentCarrier = null;
            currentPresentMembers = null;
        }
    }

    internal static bool TryResolveFamily(
        CompositeElement frame,
        IReadOnlyList<CompositeElement> members,
        bool isOrdered,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result)
    {
        if (!TryReadAtomicSegment(frame, frame, out var frameSegment))
        {
            result = null;
            return false;
        }

        var memberSegments = new List<AtomicSegment>(members.Count);

        foreach (var member in members)
        {
            if (!TryReadAtomicSegment(frame, member, out var memberSegment))
            {
                result = null;
                return false;
            }

            memberSegments.Add(memberSegment);
        }

        if (frameSegment.End <= frameSegment.Start)
        {
            result = new EngineFamilyBooleanResult(
                new EngineOperationContext(frame, members.Cast<GradedElement>().ToArray(), isOrdered),
                operation,
                []);
            return true;
        }

        var context = new EngineOperationContext(
            frame,
            members.Cast<GradedElement>().ToArray(),
            isOrdered);
        var boundaries = CollectBoundaries(frameSegment, memberSegments);
        var pieces = new List<EngineOperationPiece>();
        decimal? currentLeft = null;
        decimal? currentRight = null;
        CompositeElement? currentCarrier = null;
        List<int>? currentPresentMembers = null;

        foreach (var (left, right) in boundaries.Zip(boundaries.Skip(1)))
        {
            if (right <= left)
            {
                continue;
            }

            var midpoint = (left + right) / 2m;
            var presentMembers = GetPresentMemberIndices(midpoint, memberSegments);

            if (!EvaluateFamily(operation, presentMembers.Count, members.Count))
            {
                Flush();
                continue;
            }

            var carrier = SelectFamilyCarrier(presentMembers, frame, members);

            if (currentCarrier is not null &&
                currentPresentMembers is not null &&
                currentRight == left &&
                AreCompatibleCarriers(currentCarrier, carrier) &&
                currentPresentMembers.SequenceEqual(presentMembers))
            {
                currentRight = right;
                continue;
            }

            Flush();
            currentLeft = left;
            currentRight = right;
            currentCarrier = carrier;
            currentPresentMembers = presentMembers;
        }

        Flush();
        result = new EngineFamilyBooleanResult(context, operation, pieces);
        return true;

        void Flush()
        {
            if (currentLeft is null ||
                currentRight is null ||
                currentCarrier is null ||
                currentPresentMembers is null)
            {
                currentLeft = null;
                currentRight = null;
                currentCarrier = null;
                currentPresentMembers = null;
                return;
            }

            pieces.Add(new EngineOperationPiece(
                CreateSegmentLike(currentCarrier, currentLeft.Value, currentRight.Value),
                currentCarrier,
                currentPresentMembers.ToArray()));

            currentLeft = null;
            currentRight = null;
            currentCarrier = null;
            currentPresentMembers = null;
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

    private static SortedSet<decimal> CollectBoundaries(
        AtomicSegment frame,
        IReadOnlyList<AtomicSegment> members)
    {
        var boundaries = new SortedSet<decimal> { frame.Start, frame.End };

        foreach (var member in members)
        {
            AddIfWithin(boundaries, member.Start, frame);
            AddIfWithin(boundaries, member.End, frame);
        }

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

    private static List<int> ToPresentMemberIndices(bool inPrimary, bool inSecondary)
    {
        var indices = new List<int>(2);

        if (inPrimary)
        {
            indices.Add(0);
        }

        if (inSecondary)
        {
            indices.Add(1);
        }

        return indices;
    }

    private static CompositeElement SelectFamilyCarrier(
        IReadOnlyList<int> presentMemberIndices,
        CompositeElement frame,
        IReadOnlyList<CompositeElement> members)
    {
        if (presentMemberIndices.Count == 1)
        {
            return members[presentMemberIndices[0]];
        }

        return frame;
    }

    private static bool AreCompatibleCarriers(CompositeElement left, CompositeElement right) =>
        left.Equals(right);

    private static List<int> GetPresentMemberIndices(
        decimal value,
        IReadOnlyList<AtomicSegment> memberSegments)
    {
        var presentMembers = new List<int>();

        for (var index = 0; index < memberSegments.Count; index++)
        {
            if (IsWithin(value, memberSegments[index]))
            {
                presentMembers.Add(index);
            }
        }

        return presentMembers;
    }

    private static bool EvaluateFamily(
        EngineOccupancyOperation operation,
        int presenceCount,
        int memberCount) =>
        operation switch
        {
            EngineOccupancyOperation.None => presenceCount == 0,
            EngineOccupancyOperation.Any => presenceCount >= 1,
            EngineOccupancyOperation.All => presenceCount == memberCount,
            EngineOccupancyOperation.NotAll => presenceCount < memberCount,
            EngineOccupancyOperation.ExactlyOne => presenceCount == 1,
            EngineOccupancyOperation.Odd => (presenceCount & 1) == 1,
            EngineOccupancyOperation.Even => (presenceCount & 1) == 0,
            _ => false
        };

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
