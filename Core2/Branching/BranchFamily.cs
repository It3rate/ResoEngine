namespace Core2.Branching;

public sealed record BranchFamily<T>
{
    public BranchFamily(
        BranchOrigin origin,
        BranchSemantics semantics,
        BranchDirection direction,
        IReadOnlyList<BranchMember<T>> members,
        BranchSelection selection,
        IReadOnlyList<BranchTension> tensions,
        IReadOnlyList<IBranchAnnotation> annotations)
    {
        Origin = origin;
        Semantics = semantics;
        Direction = direction;
        Members = members.ToArray();
        Selection = selection;
        Tensions = tensions.ToArray();
        Annotations = annotations.ToArray();
        Values = Members.Select(member => member.Value).ToArray();
    }

    public BranchOrigin Origin { get; }
    public BranchSemantics Semantics { get; }
    public BranchDirection Direction { get; }
    public IReadOnlyList<BranchMember<T>> Members { get; }
    public BranchSelection Selection { get; }
    public IReadOnlyList<BranchTension> Tensions { get; }
    public IReadOnlyList<IBranchAnnotation> Annotations { get; }
    public IReadOnlyList<T> Values { get; }
    public bool HasMembers => Members.Count > 0;

    public BranchMember<T>? SelectedMember =>
        TryGetSelectedMember(out var selected) ? selected : null;

    public T? SelectedValue =>
        SelectedMember is null ? default : SelectedMember.Value;

    public bool TryGetSelectedMember(out BranchMember<T>? selected)
    {
        selected = null;
        if (!Selection.SelectedId.HasValue)
        {
            return false;
        }

        selected = Members.FirstOrDefault(member => member.Id == Selection.SelectedId.Value);
        return selected is not null;
    }

    public BranchFamily<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var mappedMembers = Members
            .Select(member => new BranchMember<TResult>(member.Id, selector(member.Value), member.Parents, member.Annotations))
            .ToArray();

        return new BranchFamily<TResult>(
            Origin,
            Semantics,
            Direction,
            mappedMembers,
            Selection,
            Tensions,
            Annotations);
    }

    public BranchFamily<T> WithSelection(BranchSelection selection) =>
        new(Origin, Semantics, Direction, Members, selection, Tensions, Annotations);

    public static BranchFamily<T> Empty(
        BranchOrigin origin,
        BranchSemantics semantics,
        BranchDirection direction,
        IReadOnlyList<BranchTension>? tensions = null,
        IReadOnlyList<IBranchAnnotation>? annotations = null) =>
        new(
            origin,
            semantics,
            direction,
            [],
            BranchSelection.None,
            tensions ?? [],
            annotations ?? []);

    public static BranchFamily<T> FromMembers(
        BranchOrigin origin,
        BranchSemantics semantics,
        BranchDirection direction,
        IReadOnlyList<BranchMember<T>> members,
        BranchSelection? selection = null,
        IReadOnlyList<BranchTension>? tensions = null,
        IReadOnlyList<IBranchAnnotation>? annotations = null) =>
        new(
            origin,
            semantics,
            direction,
            members,
            selection ?? BranchSelection.None,
            tensions ?? [],
            annotations ?? []);

    public static BranchFamily<T> FromValues(
        BranchOrigin origin,
        BranchSemantics semantics,
        BranchDirection direction,
        IReadOnlyList<T> values,
        int? selectedIndex = null,
        BranchSelectionMode selectionMode = BranchSelectionMode.None,
        IReadOnlyList<BranchTension>? tensions = null,
        IReadOnlyList<IBranchAnnotation>? annotations = null)
    {
        var members = values.Select(value => new BranchMember<T>(value)).ToArray();
        BranchSelection selection = ResolveSelection(members, selectedIndex, selectionMode);
        return new BranchFamily<T>(
            origin,
            semantics,
            direction,
            members,
            selection,
            tensions ?? [],
            annotations ?? []);
    }

    private static BranchSelection ResolveSelection(
        IReadOnlyList<BranchMember<T>> members,
        int? selectedIndex,
        BranchSelectionMode selectionMode)
    {
        if (!selectedIndex.HasValue ||
            selectionMode == BranchSelectionMode.None ||
            selectedIndex.Value < 0 ||
            selectedIndex.Value >= members.Count)
        {
            return BranchSelection.None;
        }

        return selectionMode switch
        {
            BranchSelectionMode.Principal => BranchSelection.Principal(members[selectedIndex.Value].Id),
            BranchSelectionMode.Explicit => BranchSelection.Explicit(members[selectedIndex.Value].Id),
            _ => BranchSelection.None,
        };
    }
}
