using Core3.Engine;
using Core3.Operations;

namespace Core3.Runtime;

/// <summary>
/// Generic runtime context for framed operations. This is runtime/API
/// assistance structure, not serialized engine ontology.
/// </summary>
public sealed record EngineOperationContext(
    GradedElement Frame,
    IReadOnlyList<GradedElement> Members,
    bool IsOrdered,
    EngineOperationContext? ParentContext = null,
    int? ParentFocusIndex = null)
{
    public static EngineOperationContext Create(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        bool isOrdered = true) =>
        new(frame, members.ToArray(), isOrdered);

    public int Count => Members.Count;

    public EngineFamily ToFamily() => new(this);

    public EngineOperationContext AsOrdered() =>
        IsOrdered
            ? this
            : new EngineOperationContext(Frame, Members, true, ParentContext, ParentFocusIndex);

    public EngineOperationContext AsUnordered() =>
        !IsOrdered
            ? this
            : new EngineOperationContext(Frame, Members, false, ParentContext, ParentFocusIndex);

    public EngineOperationContext CreateShuffledCopy(int? seed = null) =>
        ToFamily().CreateShuffledCopy(seed).CreateContext();

    public bool TryFocusMember(int index, out EngineOperationContext? focusedContext)
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

    public bool TryFocusMember(GradedElement member, out EngineOperationContext? focusedContext)
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
        out EngineOperationContext? sortedContext)
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

    public bool TryCollapseToParentFrame(out EngineOperationContext? collapsedContext)
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
