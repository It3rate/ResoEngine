using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Operation-scoped grouping of subjects read in one frame.
/// The frame is just an existing graded element in a frame role; it is not a
/// special ontology. For now this is also the main operation-time data area:
/// a place to hold members, ordering hints, focus derivations, and family-wide
/// laws without overcommitting the deeper runtime story yet.
/// </summary>
public sealed class EngineFamily
{
    private readonly List<GradedElement> _members = [];

    public EngineFamily(EngineOperationContext context)
        : this(
            context.Frame,
            context.IsOrdered,
            context.ParentContext is not null
                ? new EngineFamily(context.ParentContext)
                : null,
            context.ParentFocusIndex)
    {
        foreach (var member in context.Members)
        {
            AddMember(member);
        }
    }

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

    public void AddMember(GradedElement member) => _members.Add(member);

    public bool RemoveMember(GradedElement member) => _members.Remove(member);

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
        => TryFocusMember(_members.IndexOf(member), out focusedFamily);

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
        => member.TryReferenceToFrame(Frame, out read);

    public bool TryReadAllWithTension(out EngineReadResult? result)
    {
        var resolvedReads = new List<GradedElement>(_members.Count);
        GradedElement? tension = null;
        string? note = null;

        foreach (var member in _members)
        {
            var outcome = member.CommitToCalibrationWithTension(Frame);
            resolvedReads.Add(outcome.Result);
            tension = EngineTension.CombineTension(tension, outcome.Tension);
            note = EngineTension.CombineNotes(note, outcome.Note);
        }

        result = new EngineReadResult(CreateContext(), resolvedReads, tension, note);
        return true;
    }

    public CompositeElement GetMemberBoundaryAxis(GradedElement member)
        => TryReadMember(member, out var read) && read is not null
            ? EngineBoundary.GetAxis(Frame, read)
            : EngineBoundary.CreateUnknownAxis(Frame);

    public bool TryReadAll(out IReadOnlyList<GradedElement>? reads)
        => EngineExactness.TryProjectExact(
            TryReadAllWithTension(out var result),
            result,
            static item => item.Reads,
            out reads);

    public bool TryAddAll(out GradedElement? sum)
        => EngineExactness.TryProjectExact(
            TryAddAllWithTension(out var result),
            result,
            static item => item.Result,
            out sum);

    public bool TryAddAllWithTension(out EngineOperationResult? result) =>
        TryAccumulateAll(
            "Add",
            static (left, right) => left.AddWithTension(right),
            static family => family.Frame,
            out result);

    public bool TryMultiplyAll(out GradedElement? product)
        => EngineExactness.TryProjectExact(
            TryMultiplyAllWithTension(out var result),
            result,
            static item => item.Result,
            out product);

    public bool TryMultiplyAllWithTension(out EngineOperationResult? result) =>
        TryAccumulateAll(
            "Multiply",
            static (left, right) => left.MultiplyWithTension(right),
            static family =>
                family.TryDeriveMultiplyResultFrame(out var resultFrame) &&
                resultFrame is not null
                    ? resultFrame
                    : family.Frame,
            out result);

    public bool TryAddAllWithProvenance(out EngineOperationResult? result)
        => EngineExactness.TryGetExact(
            TryAddAllWithTension(out var candidate),
            candidate,
            out result);

    public bool TryMultiplyAllWithProvenance(out EngineOperationResult? result)
        => EngineExactness.TryGetExact(
            TryMultiplyAllWithTension(out var candidate),
            candidate,
            out result);

    public bool TryBoolean(EngineBooleanOperation operation, out EngineBooleanResult? result)
        => EngineExactness.TryGetExact(
            TryBooleanWithTension(operation, out var candidate),
            candidate,
            out result);

    public bool TryBooleanWithTension(
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        if (!TryReadBinaryCompositeFamily(
                out var frame,
                out var primary,
                out var secondary,
                out var tension,
                out var note))
        {
            result = null;
            return false;
        }

        return EngineBooleanProjection.TryResolveWithTension(
            frame,
            primary,
            secondary,
            operation,
            tension,
            note,
            out result);
    }

    public bool TryOccupancyBoolean(
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result) =>
        EngineExactness.TryGetExact(
            TryOccupancyBooleanWithTension(operation, out var candidate),
            candidate,
            out result);

    public bool TryOccupancyBooleanWithTension(
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result)
    {
        if (!TryReadCompositeFamily(
                out var frame,
                out var members,
                out var tension,
                out var note))
        {
            result = null;
            return false;
        }

        return EngineBooleanProjection.TryResolveFamilyWithTension(
            frame,
            members,
            IsOrdered,
            operation,
            tension,
            note,
            out result);
    }

    public bool TryBooleanAdjacentPairsWithTension(
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
            if (!TryReadAdjacentCompositePair(
                    index,
                    out _,
                    out var left,
                    out var right,
                    out var tension,
                    out var note) ||
                !EngineBooleanProjection.TryResolveWithTension(
                    frame,
                    left,
                    right,
                    operation,
                    tension,
                    note,
                    out var pairResult) ||
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

    public bool TryBooleanAdjacentPairs(
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        if (TryBooleanAdjacentPairsWithTension(operation, out results) &&
            EngineExactness.AreAllExact(results))
        {
            return true;
        }

        results = null;
        return false;
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

    private bool TryReadBinaryCompositeFamily(
        out CompositeElement frame,
        out CompositeElement primary,
        out CompositeElement secondary,
        out GradedElement? tension,
        out string? note)
    {
        frame = default!;
        primary = default!;
        secondary = default!;
        tension = null;
        note = null;

        if (_members.Count != 2 ||
            !TryReadCompositeFamily(out frame, out var members, out tension, out note))
        {
            return false;
        }

        primary = members[0];
        secondary = members[1];
        return true;
    }

    private bool TryReadCompositeFamily(
        out CompositeElement frame,
        out IReadOnlyList<CompositeElement> members,
        out GradedElement? tension,
        out string? note)
    {
        if (Frame is not CompositeElement compositeFrame ||
            !TryReadAllWithTension(out var readResult) ||
            readResult is null ||
            !TryAsCompositeReads(readResult.Reads, out var compositeMembers))
        {
            frame = default!;
            members = [];
            tension = null;
            note = null;
            return false;
        }

        frame = compositeFrame;
        members = compositeMembers;
        tension = readResult.Tension;
        note = readResult.Note;
        return true;
    }

    private bool TryReadAdjacentCompositePair(
        int leftIndex,
        out CompositeElement frame,
        out CompositeElement left,
        out CompositeElement right,
        out GradedElement? tension,
        out string? note)
    {
        if (Frame is not CompositeElement compositeFrame ||
            leftIndex < 0 ||
            leftIndex + 1 >= _members.Count)
        {
            frame = default!;
            left = default!;
            right = default!;
            tension = null;
            note = null;
            return false;
        }

        var leftOutcome = _members[leftIndex].CommitToCalibrationWithTension(Frame);
        var rightOutcome = _members[leftIndex + 1].CommitToCalibrationWithTension(Frame);

        if (leftOutcome.Result is not CompositeElement leftComposite ||
            rightOutcome.Result is not CompositeElement rightComposite)
        {
            frame = default!;
            left = default!;
            right = default!;
            tension = null;
            note = null;
            return false;
        }

        frame = compositeFrame;
        left = leftComposite;
        right = rightComposite;
        tension = EngineTension.CombineTension(leftOutcome.Tension, rightOutcome.Tension);
        note = EngineTension.CombineNotes(leftOutcome.Note, rightOutcome.Note);
        return true;
    }

    private bool TryAccumulateAll(
        string operationName,
        Func<GradedElement, GradedElement, EngineElementOutcome> localLaw,
        Func<EngineFamily, GradedElement> resultFrameSelector,
        out EngineOperationResult? result)
    {
        if (!TryReadAllWithTension(out var readResult) ||
            readResult is null ||
            readResult.Reads.Count == 0)
        {
            result = null;
            return false;
        }

        var current = readResult.Reads[0];
        var tension = readResult.Tension;
        var note = readResult.Note;

        for (var index = 1; index < readResult.Reads.Count; index++)
        {
            var stepOutcome = localLaw(current, readResult.Reads[index]);
            current = stepOutcome.Result;
            tension = EngineTension.CombineTension(tension, stepOutcome.Tension);
            note = EngineTension.CombineNotes(note, stepOutcome.Note);
        }

        result = new EngineOperationResult(
            operationName,
            CreateContext(),
            current,
            resultFrameSelector(this),
            tension,
            note);
        return true;
    }

    private static bool TryAsCompositeReads(
        IReadOnlyList<GradedElement> reads,
        out List<CompositeElement> composites)
    {
        composites = new List<CompositeElement>(reads.Count);

        foreach (var read in reads)
        {
            if (read is not CompositeElement composite)
            {
                composites = [];
                return false;
            }

            composites.Add(composite);
        }

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
