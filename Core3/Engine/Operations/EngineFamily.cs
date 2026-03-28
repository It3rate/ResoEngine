using Core3.Engine;

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

        focusedFamily = new EngineFamily(
            focusedFrame,
            IsOrdered,
            this,
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

        collapsedFamily = new EngineFamily(
            ParentFamily.Frame,
            ParentFamily.IsOrdered,
            ParentFamily.ParentFamily,
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
            result = new EngineOperationResult("Add", Frame, _members.ToArray(), sum, Frame);
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
                Frame,
                _members.ToArray(),
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
}
