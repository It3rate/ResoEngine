using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

internal static class BooleanProjection
{
    // TODO: Fold boolean projection into the same generic operation pipeline as
    // add/multiply/fold/alignment/resolution so the engine only sees laws over
    // structure and does not need a separate boolean-special execution path.
    internal static bool TryResolveResult(
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary,
        BooleanOperation operation,
        GradedElement? inheritedTension,
        string? inheritedNote,
        out PieceArcResult? result)
    {
        var context = new OperationContext(frame, [primary, secondary], true);
        var tension = inheritedTension;
        var note = inheritedNote;
        if (!TryReadSegmentFamily(
                frame,
                [primary, secondary],
                tension,
                note,
                out var frameSegment,
                out var memberSegments,
                out tension,
                out note))
        {
            result = new PieceArcResult(operation.ToString(), context, [], tension, note);
            return true;
        }

        if (frameSegment.End <= frameSegment.Start)
        {
            result = new PieceArcResult(operation.ToString(), context, [], tension, note);
            return true;
        }

        var pieces = CollectPieces(
            CollectBoundaries(frameSegment, memberSegments),
            midpoint => GetPresentMemberIndices(midpoint, memberSegments),
            presentMembers => EvaluateBinary(operation, presentMembers),
            presentMembers => SelectBinaryCarrier(presentMembers, frame, primary, secondary),
            ref tension,
            ref note);
        result = new PieceArcResult(operation.ToString(), context, pieces, tension, note);
        return true;
    }

    internal static bool TryResolveFamilyResult(
        CompositeElement frame,
        IReadOnlyList<CompositeElement> members,
        bool isOrdered,
        OccupancyOperation operation,
        GradedElement? inheritedTension,
        string? inheritedNote,
        out PieceArcResult? result)
    {
        var context = new OperationContext(
            frame,
            members.Cast<GradedElement>().ToArray(),
            isOrdered);
        var tension = inheritedTension;
        var note = inheritedNote;
        if (!TryReadSegmentFamily(
                frame,
                members,
                tension,
                note,
                out var frameSegment,
                out var memberSegments,
                out tension,
                out note))
        {
            result = new PieceArcResult(operation.ToString(), context, [], tension, note);
            return true;
        }

        if (frameSegment.End <= frameSegment.Start)
        {
            result = new PieceArcResult(operation.ToString(), context, [], tension, note);
            return true;
        }

        var pieces = CollectPieces(
            CollectBoundaries(frameSegment, memberSegments),
            midpoint => GetPresentMemberIndices(midpoint, memberSegments),
            presentMembers => EvaluateFamily(operation, presentMembers.Count, members.Count),
            presentMembers => SelectFamilyCarrier(presentMembers, frame, members),
            ref tension,
            ref note);
        result = new PieceArcResult(operation.ToString(), context, pieces, tension, note);
        return true;
    }

    private static bool TryReadSegmentFamily(
        CompositeElement frame,
        IReadOnlyList<CompositeElement> members,
        GradedElement? inheritedTension,
        string? inheritedNote,
        out AtomicSegment frameSegment,
        out IReadOnlyList<AtomicSegment> memberSegments,
        out GradedElement? tension,
        out string? note)
    {
        tension = inheritedTension;
        note = inheritedNote;
        memberSegments = [];

        if (!TryReadAtomicSegment(frame, out frameSegment, out var frameTension, out var frameNote))
        {
            tension = EngineTension.CombineTension(tension, frameTension, frame);
            note = EngineTension.CombineNotes(note, frameNote);
            return false;
        }

        var resolvedMembers = new List<AtomicSegment>(members.Count);

        foreach (var member in members)
        {
            if (!TryReadAtomicSegment(member, out var memberSegment, out var memberTension, out var memberNote))
            {
                tension = EngineTension.CombineTension(tension, memberTension, member);
                note = EngineTension.CombineNotes(note, memberNote);
                return false;
            }

            resolvedMembers.Add(memberSegment);
        }

        memberSegments = resolvedMembers;
        return true;
    }

    private static List<OperationPiece> CollectPieces(
        SortedSet<decimal> boundaries,
        Func<decimal, List<int>> presentMemberSelector,
        Func<IReadOnlyList<int>, bool> shouldKeep,
        Func<IReadOnlyList<int>, CompositeElement> carrierSelector,
        ref GradedElement? tension,
        ref string? note)
    {
        var pieces = new List<OperationPiece>();
        var updatedTension = tension;
        var updatedNote = note;
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

            var presentMembers = presentMemberSelector((left + right) / 2m);

            if (!shouldKeep(presentMembers))
            {
                Flush();
                continue;
            }

            var carrier = carrierSelector(presentMembers);

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
        tension = updatedTension;
        note = updatedNote;
        return pieces;

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

            if (TryCreateSegmentLike(
                    currentCarrier,
                    currentLeft.Value,
                    currentRight.Value,
                    out var piece,
                    out var pieceTension,
                    out var pieceNote) &&
                piece is not null)
            {
                pieces.Add(new OperationPiece(
                    piece,
                    currentCarrier,
                    currentPresentMembers.ToArray()));
            }
            else
            {
                updatedTension = EngineTension.CombineTension(updatedTension, pieceTension, currentCarrier);
                updatedNote = EngineTension.CombineNotes(updatedNote, pieceNote);
            }

            currentLeft = null;
            currentRight = null;
            currentCarrier = null;
            currentPresentMembers = null;
        }
    }

    private static bool TryReadAtomicSegment(
        CompositeElement segment,
        out AtomicSegment atomicSegment,
        out GradedElement? tension,
        out string? note)
    {
        if (segment.Recessive is not AtomicElement start ||
            segment.Dominant is not AtomicElement end)
        {
            atomicSegment = default;
            tension = segment;
            note = "Boolean projection currently requires atomic segment endpoints.";
            return false;
        }

        if (!TryToDecimal(start, out var startValue) ||
            !TryToDecimal(end, out var endValue))
        {
            atomicSegment = default;
            tension = segment;
            note = "Boolean projection preserved unresolved support because one or more segment endpoints could not be placed exactly on the current carrier.";
            return false;
        }

        atomicSegment = new AtomicSegment(
            Math.Min(startValue, endValue),
            Math.Max(startValue, endValue));
        tension = null;
        note = null;
        return true;
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

    private static bool EvaluateBinary(
        BooleanOperation operation,
        IReadOnlyList<int> presentMembers) =>
        operation.Evaluate(
            presentMembers.Contains(0),
            presentMembers.Contains(1));

    private static CompositeElement SelectBinaryCarrier(
        IReadOnlyList<int> presentMembers,
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary)
    {
        if (presentMembers.Contains(0))
        {
            return primary;
        }

        if (presentMembers.Contains(1))
        {
            return secondary;
        }

        return frame;
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
        OccupancyOperation operation,
        int presenceCount,
        int memberCount) =>
        operation switch
        {
            OccupancyOperation.None => presenceCount == 0,
            OccupancyOperation.Any => presenceCount >= 1,
            OccupancyOperation.All => presenceCount == memberCount,
            OccupancyOperation.NotAll => presenceCount < memberCount,
            OccupancyOperation.ExactlyOne => presenceCount == 1,
            OccupancyOperation.Odd => (presenceCount & 1) == 1,
            OccupancyOperation.Even => (presenceCount & 1) == 0,
            _ => false
        };

    private static bool TryCreateSegmentLike(
        CompositeElement template,
        decimal left,
        decimal right,
        out CompositeElement? segment,
        out GradedElement? tension,
        out string? note)
    {
        if (template.Recessive is not AtomicElement start ||
            template.Dominant is not AtomicElement end)
        {
            segment = null;
            tension = template;
            note = "Boolean projection currently requires atomic segment endpoints.";
            return false;
        }

        if (!TryToDecimal(start, out var startValue) ||
            !TryToDecimal(end, out var endValue) ||
            start.Unit != end.Unit ||
            !TryFromDecimal(left, start.Unit, out var leftAtomic) ||
            !TryFromDecimal(right, start.Unit, out var rightAtomic))
        {
            segment = null;
            tension = template;
            note = "Boolean projection preserved unresolved support because one or more partition pieces could not be expressed exactly in the carrier resolution.";
            return false;
        }

        var forward = startValue <= endValue;

        segment = forward
            ? new CompositeElement(leftAtomic, rightAtomic)
            : new CompositeElement(rightAtomic, leftAtomic);
        tension = null;
        note = null;
        return true;
    }

    private static bool TryToDecimal(AtomicElement atomic, out decimal value)
    {
        if (atomic.Unit <= 0)
        {
            value = default;
            return false;
        }

        value = (decimal)atomic.Value / atomic.Unit;
        return true;
    }

    private static bool TryFromDecimal(decimal value, long unit, out AtomicElement atomic)
    {
        if (unit <= 0)
        {
            atomic = default!;
            return false;
        }

        var scaled = value * unit;
        if (decimal.Truncate(scaled) != scaled)
        {
            atomic = default!;
            return false;
        }

        atomic = new AtomicElement((long)scaled, unit);
        return true;
    }

    private readonly record struct AtomicSegment(decimal Start, decimal End);
}








