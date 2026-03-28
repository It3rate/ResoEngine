using Core3.Engine;

namespace Core3.Runtime;

/// <summary>
/// Generic runtime context for framed operations. This is runtime/API
/// assistance structure, not serialized engine ontology.
/// </summary>
public sealed record EngineOperationContext
{
    public static EngineOperationContext Create(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        bool isOrdered = true)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        return new EngineOperationContext(
            frame,
            members.ToArray(),
            isOrdered);
    }

    public EngineOperationContext(
        GradedElement frame,
        IReadOnlyList<GradedElement> members,
        bool isOrdered,
        EngineOperationContext? parentContext = null,
        int? parentFocusIndex = null)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        Frame = frame;
        Members = members;
        IsOrdered = isOrdered;
        ParentContext = parentContext;
        ParentFocusIndex = parentFocusIndex;
    }

    public GradedElement Frame { get; }
    public IReadOnlyList<GradedElement> Members { get; }
    public bool IsOrdered { get; }
    public EngineOperationContext? ParentContext { get; }
    public int? ParentFocusIndex { get; }

    public int Count => Members.Count;

    public EngineOperationContext AsOrdered() =>
        IsOrdered
            ? this
            : new EngineOperationContext(Frame, Members, true, ParentContext, ParentFocusIndex);

    public EngineOperationContext AsUnordered() =>
        !IsOrdered
            ? this
            : new EngineOperationContext(Frame, Members, false, ParentContext, ParentFocusIndex);
}
