using Core3.Engine;
using Core3.Engine.Runtime;

namespace Core3.Engine.Operations;

/// <summary>
/// Operation-scoped grouping of subjects read in one frame.
/// The frame is just an existing graded element in a frame role; it is not a
/// special ontology.
/// </summary>
public sealed class EngineFamily
{
    private readonly List<GradedElement> _members = [];

    public EngineFamily(GradedElement frame, bool isOrdered = true)
        : this(frame, isOrdered, null, null)
    {
    }

    private EngineFamily(
        GradedElement frame,
        bool isOrdered,
        EngineFamily? parentFamily,
        int? parentFocusIndex)
    {
        ArgumentNullException.ThrowIfNull(frame);
        Frame = frame;
        IsOrdered = isOrdered;
        ParentFamily = parentFamily;
        ParentFocusIndex = parentFocusIndex;
    }

    public GradedElement Frame { get; }
    public bool IsOrdered { get; }
    public IReadOnlyList<GradedElement> Members => _members;
    public int Count => _members.Count;
    public EngineFamily? ParentFamily { get; }
    public int? ParentFocusIndex { get; }

    public void AddMember(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member);
    }

    public bool RemoveMember(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);
        return _members.Remove(member);
    }

    public void ClearMembers() => _members.Clear();

    public EngineOperationContext CreateContext() => new(
        Frame,
        _members.ToArray(),
        IsOrdered,
        ParentFamily?.CreateContext(),
        ParentFocusIndex);

    public EngineFamily CreateOrderedCopy()
    {
        var orderedFamily = CreateDerivedFamily(Frame, isOrdered: true);

        foreach (var member in _members)
        {
            orderedFamily.AddMember(member);
        }

        return orderedFamily;
    }

    public EngineFamily CreateUnorderedCopy()
    {
        var unorderedFamily = CreateDerivedFamily(Frame, isOrdered: false);

        foreach (var member in _members)
        {
            unorderedFamily.AddMember(member);
        }

        return unorderedFamily;
    }

    public EngineFamily CreateShuffledCopy(int? seed = null)
    {
        var random = seed is int fixedSeed ? new Random(fixedSeed) : Random.Shared;
        var shuffledMembers = _members.ToList();

        for (var index = shuffledMembers.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (shuffledMembers[index], shuffledMembers[swapIndex]) =
                (shuffledMembers[swapIndex], shuffledMembers[index]);
        }

        var shuffledFamily = CreateDerivedFamily(Frame, isOrdered: false);

        foreach (var member in shuffledMembers)
        {
            shuffledFamily.AddMember(member);
        }

        return shuffledFamily;
    }

    public bool TryFocusMember(int index, out EngineFamily? focusedFamily)
    {
        if (index < 0 || index >= _members.Count)
        {
            focusedFamily = null;
            return false;
        }

        var focusedMember = _members[index];

        if (!TryReadMember(focusedMember, out var focusedFrame) ||
            focusedFrame is null)
        {
            focusedFamily = null;
            return false;
        }

        focusedFamily = CreateDerivedFamily(
            focusedFrame,
            IsOrdered,
            index);

        for (var memberIndex = 0; memberIndex < _members.Count; memberIndex++)
        {
            if (memberIndex == index)
            {
                continue;
            }

            focusedFamily.AddMember(_members[memberIndex]);
        }

        return true;
    }

    public bool TryFocusMember(GradedElement member, out EngineFamily? focusedFamily)
    {
        ArgumentNullException.ThrowIfNull(member);
        return TryFocusMember(_members.IndexOf(member), out focusedFamily);
    }

    public bool TrySortByFrameSlot(
        int slotIndex,
        bool descending,
        out EngineFamily? sortedFamily)
    {
        if (slotIndex < 0)
        {
            sortedFamily = null;
            return false;
        }

        var sortableMembers = new List<SortableMember>(_members.Count);

        for (var memberIndex = 0; memberIndex < _members.Count; memberIndex++)
        {
            var member = _members[memberIndex];

            if (!TryReadMember(member, out var read) ||
                read is null ||
                !TryGetAtomicSlot(read, slotIndex, out var slot) ||
                slot is null)
            {
                sortedFamily = null;
                return false;
            }

            sortableMembers.Add(new SortableMember(memberIndex, member, slot));
        }

        if (!AreMutuallyComparable(sortableMembers))
        {
            sortedFamily = null;
            return false;
        }

        sortableMembers.Sort((left, right) =>
        {
            var comparison = CompareAtomicSlots(left.Slot, right.Slot);

            if (descending)
            {
                comparison = -comparison;
            }

            return comparison != 0
                ? comparison
                : left.OriginalIndex.CompareTo(right.OriginalIndex);
        });

        sortedFamily = CreateDerivedFamily(Frame, isOrdered: true);

        foreach (var sortableMember in sortableMembers)
        {
            sortedFamily.AddMember(sortableMember.Member);
        }

        return true;
    }

    public bool TryCollapseToParentFrame(out EngineFamily? collapsedFamily)
    {
        if (ParentFamily is null ||
            ParentFocusIndex is null)
        {
            collapsedFamily = null;
            return false;
        }

        if (!Frame.TryReferenceToFrame(ParentFamily.Frame, out var collapsedFrameMember) ||
            collapsedFrameMember is null)
        {
            collapsedFamily = null;
            return false;
        }

        collapsedFamily = ParentFamily.ParentFamily is null
            ? new EngineFamily(ParentFamily.Frame, ParentFamily.IsOrdered)
            : ParentFamily.ParentFamily.CreateDerivedFamily(
                ParentFamily.Frame,
                ParentFamily.IsOrdered,
                ParentFamily.ParentFocusIndex);

        for (var memberIndex = 0; memberIndex <= _members.Count; memberIndex++)
        {
            if (memberIndex == ParentFocusIndex.Value)
            {
                collapsedFamily.AddMember(collapsedFrameMember);
            }

            if (memberIndex < _members.Count)
            {
                collapsedFamily.AddMember(_members[memberIndex]);
            }
        }

        return true;
    }

    public bool TryReadMember(GradedElement member, out GradedElement? read)
    {
        ArgumentNullException.ThrowIfNull(member);
        return member.TryReferenceToFrame(Frame, out read);
    }

    public CompositeElement GetMemberBoundaryAxis(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return TryReadMember(member, out var read) && read is not null
            ? EngineBoundary.GetAxis(Frame, read)
            : EngineBoundary.CreateUnknownAxis(Frame);
    }

    public bool TryReadAll(out IReadOnlyList<GradedElement>? reads)
    {
        var resolvedReads = new List<GradedElement>(_members.Count);

        foreach (var member in _members)
        {
            if (!member.TryReferenceToFrame(Frame, out var read) || read is null)
            {
                reads = null;
                return false;
            }

            resolvedReads.Add(read);
        }

        reads = resolvedReads;
        return true;
    }

    public bool TryAddAll(out GradedElement? sum)
    {
        if (!TryReadAll(out var reads) ||
            reads is null ||
            reads.Count == 0)
        {
            sum = null;
            return false;
        }

        var current = reads[0];

        for (var index = 1; index < reads.Count; index++)
        {
            if (!current.TryAdd(reads[index], out var next) || next is null)
            {
                sum = null;
                return false;
            }

            current = next;
        }

        sum = current;
        return true;
    }

    public bool TryMultiplyAll(out GradedElement? product)
    {
        if (!TryReadAll(out var reads) ||
            reads is null ||
            reads.Count == 0)
        {
            product = null;
            return false;
        }

        var current = reads[0];

        for (var index = 1; index < reads.Count; index++)
        {
            if (!current.TryMultiply(reads[index], out var next) || next is null)
            {
                product = null;
                return false;
            }

            current = next;
        }

        product = current;
        return true;
    }

    public bool TryAddAllWithProvenance(out EngineOperationResult? result)
    {
        GradedElement? sum;

        if (TryAddAll(out sum) &&
            sum is not null)
        {
            result = new EngineOperationResult("Add", CreateContext(), sum, Frame);
            return true;
        }

        result = null;
        return false;
    }

    public bool TryMultiplyAllWithProvenance(out EngineOperationResult? result)
    {
        GradedElement? product;

        if (TryMultiplyAll(out product) &&
            product is not null)
        {
            result = new EngineOperationResult(
                "Multiply",
                CreateContext(),
                product,
                TryDeriveMultiplyResultFrame(out var resultFrame) && resultFrame is not null
                    ? resultFrame
                    : Frame);
            return true;
        }

        result = null;
        return false;
    }

    public bool TryBoolean(EngineBooleanOperation operation, out EngineBooleanResult? result)
    {
        if (_members.Count != 2)
        {
            result = null;
            return false;
        }

        if (!TryReadAll(out var reads) ||
            reads is null ||
            Frame is not CompositeElement frame ||
            reads[0] is not CompositeElement primary ||
            reads[1] is not CompositeElement secondary)
        {
            result = null;
            return false;
        }

        return EngineBooleanProjection.TryResolve(frame, primary, secondary, operation, out result);
    }

    public bool TryOccupancyBoolean(
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result)
    {
        if (!TryReadAll(out var reads) ||
            reads is null ||
            Frame is not CompositeElement frame)
        {
            result = null;
            return false;
        }

        var members = new List<CompositeElement>(reads.Count);

        foreach (var read in reads)
        {
            if (read is not CompositeElement composite)
            {
                result = null;
                return false;
            }

            members.Add(composite);
        }

        return EngineBooleanProjection.TryResolveFamily(
            frame,
            members,
            IsOrdered,
            operation,
            out result);
    }

    public bool TryBooleanAdjacentPairs(
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        if (!IsOrdered || _members.Count < 2 || Frame is not CompositeElement frame)
        {
            results = null;
            return false;
        }

        var pairwiseResults = new List<EngineBooleanResult>(_members.Count - 1);

        for (var index = 0; index < _members.Count - 1; index++)
        {
            if (!TryReadMember(_members[index], out var leftRead) ||
                leftRead is not CompositeElement left ||
                !TryReadMember(_members[index + 1], out var rightRead) ||
                rightRead is not CompositeElement right ||
                !EngineBooleanProjection.TryResolve(frame, left, right, operation, out var pairResult) ||
                pairResult is null)
            {
                results = null;
                return false;
            }

            pairwiseResults.Add(pairResult);
        }

        results = pairwiseResults;
        return true;
    }

    private bool TryDeriveMultiplyResultFrame(out GradedElement? resultFrame)
    {
        if (_members.Count == 0)
        {
            resultFrame = null;
            return false;
        }

        var current = Frame;

        for (var index = 1; index < _members.Count; index++)
        {
            if (!current.TryMultiply(Frame, out var next) || next is null)
            {
                resultFrame = null;
                return false;
            }

            current = next;
        }

        resultFrame = current;
        return true;
    }

    private EngineFamily CreateDerivedFamily(
        GradedElement frame,
        bool isOrdered,
        int? parentFocusIndex = null) =>
        new(frame, isOrdered, this, parentFocusIndex);

    private static bool AreMutuallyComparable(IReadOnlyList<SortableMember> sortableMembers)
    {
        for (var leftIndex = 0; leftIndex < sortableMembers.Count; leftIndex++)
        {
            for (var rightIndex = leftIndex + 1; rightIndex < sortableMembers.Count; rightIndex++)
            {
                if (!sortableMembers[leftIndex].Slot.TryAlignExact(
                        sortableMembers[rightIndex].Slot,
                        out _,
                        out _))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static int CompareAtomicSlots(AtomicElement left, AtomicElement right)
    {
        if (!left.TryAlignExact(right, out var leftAligned, out var rightAligned) ||
            leftAligned is null ||
            rightAligned is null)
        {
            throw new InvalidOperationException("Family slot comparison requires aligned atomic reads.");
        }

        return leftAligned.Value.CompareTo(rightAligned.Value);
    }

    private static bool TryGetAtomicSlot(
        GradedElement element,
        int slotIndex,
        out AtomicElement? slot)
    {
        var currentIndex = 0;
        return TryGetAtomicSlot(element, slotIndex, ref currentIndex, out slot);
    }

    private static bool TryGetAtomicSlot(
        GradedElement element,
        int slotIndex,
        ref int currentIndex,
        out AtomicElement? slot)
    {
        switch (element)
        {
            case AtomicElement atomic:
                if (currentIndex == slotIndex)
                {
                    slot = atomic;
                    return true;
                }

                currentIndex++;
                slot = null;
                return false;

            case CompositeElement composite:
                if (TryGetAtomicSlot(composite.Recessive, slotIndex, ref currentIndex, out slot))
                {
                    return true;
                }

                return TryGetAtomicSlot(composite.Dominant, slotIndex, ref currentIndex, out slot);

            default:
                slot = null;
                return false;
        }
    }

    private sealed record SortableMember(int OriginalIndex, GradedElement Member, AtomicElement Slot);
}
