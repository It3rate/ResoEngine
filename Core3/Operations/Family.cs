using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Operation-scoped grouping of subjects read in one frame.
/// The frame is just an existing graded element in a frame role; it is not a
/// special ontology. For now this is also the main operation-time data area:
/// a neutral holder for members, ordering hints, focus derivations, and
/// family-wide laws without overcommitting the deeper runtime story yet.
///
/// This should stay broad enough to support several higher-layer readings of
/// the same held data. A family may later be viewed as an ordered span, a bag,
/// a graph-like local neighborhood, a route slice, or another domain-friendly
/// collection surface. The important thing is to keep one holder and let views
/// and laws interpret it, rather than inventing several unrelated collection
/// ontologies.
///
/// Future law cleanup should keep that holder/view split intact, including
/// compatibility with the experimental Core3.Data layer.
/// </summary>
public sealed class Family
{
    private readonly List<GradedElement> _members = [];

    public Family(OperationContext context)
        : this(
            context.Frame,
            context.IsOrdered,
            context.ParentContext is not null
                ? new Family(context.ParentContext)
                : null,
            context.ParentFocusIndex)
    {
        foreach (var member in context.Members)
        {
            AddMember(member);
        }
    }

    public Family(GradedElement frame, bool isOrdered = true)
        : this(frame, isOrdered, null, null)
    {
    }

    private Family(
        GradedElement frame,
        bool isOrdered,
        Family? parentFamily,
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
    public Family? ParentFamily { get; }
    public int? ParentFocusIndex { get; }

    public void AddMember(GradedElement member) => _members.Add(member);

    public bool RemoveMember(GradedElement member) => _members.Remove(member);

    public void ClearMembers() => _members.Clear();

    public OperationContext CreateContext() => new(
        Frame,
        _members.ToArray(),
        IsOrdered,
        ParentFamily?.CreateContext(),
        ParentFocusIndex);

    public Family CreateOrderedCopy()
    {
        var orderedFamily = CreateDerivedFamily(Frame, isOrdered: true);

        foreach (var member in _members)
        {
            orderedFamily.AddMember(member);
        }

        return orderedFamily;
    }

    public Family CreateUnorderedCopy()
    {
        var unorderedFamily = CreateDerivedFamily(Frame, isOrdered: false);

        foreach (var member in _members)
        {
            unorderedFamily.AddMember(member);
        }

        return unorderedFamily;
    }

    public Family CreateShuffledCopy(int? seed = null)
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

    public bool TryFocusMember(int index, out Family? focusedFamily)
    {
        if (index < 0 || index >= _members.Count)
        {
            focusedFamily = null;
            return false;
        }

        var focusedMember = _members[index];

        var focusedOutcome = ReadMember(focusedMember);

        if (!focusedOutcome.IsExact)
        {
            focusedFamily = null;
            return false;
        }

        focusedFamily = CreateDerivedFamily(
            focusedOutcome.Result,
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

    public bool TryFocusMember(GradedElement member, out Family? focusedFamily)
        => TryFocusMember(_members.IndexOf(member), out focusedFamily);

    public bool TrySortByFrameSlot(
        int slotIndex,
        bool descending,
        out Family? sortedFamily)
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

            var readOutcome = ReadMember(member);

            if (!readOutcome.IsExact ||
                !TryGetAtomicSlot(readOutcome.Result, slotIndex, out var slot) ||
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

    public bool TryCollapseToParentFrame(out Family? collapsedFamily)
    {
        if (ParentFamily is null ||
            ParentFocusIndex is null)
        {
            collapsedFamily = null;
            return false;
        }

        var collapsedOutcome = Frame.ViewInFrame(ParentFamily.Frame);

        if (!collapsedOutcome.IsExact)
        {
            collapsedFamily = null;
            return false;
        }

        collapsedFamily = ParentFamily.ParentFamily is null
            ? new Family(ParentFamily.Frame, ParentFamily.IsOrdered)
            : ParentFamily.ParentFamily.CreateDerivedFamily(
                ParentFamily.Frame,
                ParentFamily.IsOrdered,
                ParentFamily.ParentFocusIndex);

        for (var memberIndex = 0; memberIndex <= _members.Count; memberIndex++)
        {
            if (memberIndex == ParentFocusIndex.Value)
            {
                collapsedFamily.AddMember(collapsedOutcome.Result);
            }

            if (memberIndex < _members.Count)
            {
                collapsedFamily.AddMember(_members[memberIndex]);
            }
        }

        return true;
    }

    public EngineElementOutcome ReadMember(GradedElement member) =>
        member.ViewInFrame(Frame);

    public PieceArcResult ReadAllResult()
    {
        var resolvedReads = new List<GradedElement>(_members.Count);
        GradedElement? tension = null;
        string? note = null;

        foreach (var member in _members)
        {
            var outcome = member.CommitToCalibration(Frame);
            resolvedReads.Add(outcome.Result);
            tension = EngineTension.CombineTension(tension, outcome.Tension);
            note = EngineTension.CombineNotes(note, outcome.Note);
        }

        return PieceArcResult.FromResults("Read", CreateContext(), resolvedReads, tension, note);
    }

    public CompositeElement GetMemberBoundaryAxis(GradedElement member)
        => ReadMember(member) is var outcome && outcome.IsExact
            ? EngineBoundary.GetAxis(Frame, outcome.Result)
            : EngineBoundary.CreateUnknownAxis(Frame);

    public OperationResult? AddAllResult() =>
        AccumulateAll(
            "Add",
            static (left, right) => left.Add(right),
            static family => family.Frame);

    public OperationResult? MultiplyAllResult() =>
        AccumulateAll(
            "Multiply",
            static (left, right) => left.Multiply(right),
            static family =>
                family.TryDeriveMultiplyResultFrame(out var resultFrame) &&
                resultFrame is not null
                    ? resultFrame
                    : family.Frame);

    public PieceArcResult? BooleanResult(BooleanOperation operation)
    {
        if (!TryReadBinaryCompositeFamily(
                out var frame,
                out var primary,
                out var secondary,
                out var tension,
                out var note))
        {
            return null;
        }

        return BooleanProjection.TryResolveResult(
            frame,
            primary,
            secondary,
            operation,
            tension,
            note,
            out var result)
            ? result
            : null;
    }

    public PieceArcResult? OccupancyBooleanResult(OccupancyOperation operation)
    {
        if (!TryReadCompositeFamily(
                out var frame,
                out var members,
                out var tension,
                out var note))
        {
            return null;
        }

        return BooleanProjection.TryResolveFamilyResult(
            frame,
            members,
            IsOrdered,
            operation,
            tension,
            note,
            out var result)
            ? result
            : null;
    }

    public IReadOnlyList<PieceArcResult>? BooleanAdjacentPairResults(BooleanOperation operation)
    {
        if (!IsOrdered || _members.Count < 2 || Frame is not CompositeElement frame)
        {
            return null;
        }

        var pairwiseResults = new List<PieceArcResult>(_members.Count - 1);

        for (var index = 0; index < _members.Count - 1; index++)
        {
            if (!TryReadAdjacentCompositePair(
                    index,
                    out _,
                    out var left,
                    out var right,
                    out var tension,
                    out var note) ||
                !BooleanProjection.TryResolveResult(
                    frame,
                    left,
                    right,
                    operation,
                    tension,
                    note,
                    out var pairResult) ||
                pairResult is null)
            {
                return null;
            }

            pairwiseResults.Add(pairResult);
        }

        return pairwiseResults;
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
            var nextOutcome = current.Multiply(Frame);

            if (!nextOutcome.IsExact)
            {
                resultFrame = null;
                return false;
            }

            current = nextOutcome.Result;
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
        var readResult = ReadAllResult();

        if (Frame is not CompositeElement compositeFrame ||
            !TryAsCompositeReads(readResult.Results, out var compositeMembers))
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

        var leftOutcome = _members[leftIndex].CommitToCalibration(Frame);
        var rightOutcome = _members[leftIndex + 1].CommitToCalibration(Frame);

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

    private OperationResult? AccumulateAll(
        string operationName,
        Func<GradedElement, GradedElement, EngineElementOutcome> localLaw,
        Func<Family, GradedElement> resultFrameSelector)
    {
        var readResult = ReadAllResult();

        if (readResult.Results.Count == 0)
        {
            return null;
        }

        var current = readResult.Results[0];
        var tension = readResult.Tension;
        var note = readResult.Note;

        for (var index = 1; index < readResult.Results.Count; index++)
        {
            var stepOutcome = localLaw(current, readResult.Results[index]);
            current = stepOutcome.Result;
            tension = EngineTension.CombineTension(tension, stepOutcome.Tension);
            note = EngineTension.CombineNotes(note, stepOutcome.Note);
        }

        // TODO: Some operations may have several lawful outputs or preserved
        // substructures in addition to the normal expected result. When Core3
        // has a more native way to carry those explicitly, they should be
        // preserved there and later inspected through views instead of being
        // guessed here from operation names.

        return new OperationResult(
            operationName,
            CreateContext(),
            current,
            resultFrame: resultFrameSelector(this),
            tension: tension,
            note: note);
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

    private Family CreateDerivedFamily(
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
                var alignment = sortableMembers[leftIndex].Slot.Align(
                    sortableMembers[rightIndex].Slot);

                if (!alignment.IsExact ||
                    !alignment.TryGetPair(out _, out _))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static int CompareAtomicSlots(AtomicElement left, AtomicElement right)
    {
        var alignment = left.Align(right);

        if (!alignment.IsExact ||
            !alignment.TryGetPair(out var leftElement, out var rightElement) ||
            leftElement is not AtomicElement leftAligned ||
            rightElement is not AtomicElement rightAligned)
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










