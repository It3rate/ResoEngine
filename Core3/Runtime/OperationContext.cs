using Core3.Engine;
using Core3.Operations;

namespace Core3.Runtime;

/// <summary>
/// Generic inbound context for framed operations. This is runtime/API
/// assistance structure, not serialized engine ontology. In the broader
/// operation-arc reading, this record is the inbound side: active frame,
/// participating members, ordering, and temporary parent/focus provenance.
/// </summary>
public sealed record OperationContext(
    GradedElement Frame,
    IReadOnlyList<GradedElement> Members,
    bool IsOrdered,
    OperationContext? ParentContext = null,
    int? ParentFocusIndex = null)
{
    public static OperationContext Create(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        bool isOrdered = true) =>
        new(frame, members.ToArray(), isOrdered);

    public int Count => Members.Count;

    public Family ToFamily() => new(this);

    public OperationContext AsOrdered() =>
        IsOrdered
            ? this
            : new OperationContext(Frame, Members, true, ParentContext, ParentFocusIndex);

    public OperationContext AsUnordered() =>
        !IsOrdered
            ? this
            : new OperationContext(Frame, Members, false, ParentContext, ParentFocusIndex);

    public OperationContext CreateShuffledCopy(int? seed = null) =>
        ToFamily().CreateShuffledCopy(seed).CreateContext();

    public bool TryFocusMember(int index, out OperationContext? focusedContext)
    {
        if (ToFamily().TryFocusMember(index, out var focusedFamily) &&
            focusedFamily is not null)
        {
            focusedContext = focusedFamily.CreateContext();
            return true;
        }

        focusedContext = null;
        return false;
    }

    public bool TryFocusMember(GradedElement member, out OperationContext? focusedContext)
    {
        if (ToFamily().TryFocusMember(member, out var focusedFamily) &&
            focusedFamily is not null)
        {
            focusedContext = focusedFamily.CreateContext();
            return true;
        }

        focusedContext = null;
        return false;
    }

    public bool TrySortByFrameSlot(
        int slotIndex,
        bool descending,
        out OperationContext? sortedContext)
    {
        if (ToFamily().TrySortByFrameSlot(slotIndex, descending, out var sortedFamily) &&
            sortedFamily is not null)
        {
            sortedContext = sortedFamily.CreateContext();
            return true;
        }

        sortedContext = null;
        return false;
    }

    public bool TryCollapseToParentFrame(out OperationContext? collapsedContext)
    {
        if (ToFamily().TryCollapseToParentFrame(out var collapsedFamily) &&
            collapsedFamily is not null)
        {
            collapsedContext = collapsedFamily.CreateContext();
            return true;
        }

        collapsedContext = null;
        return false;
    }
}


