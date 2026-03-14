namespace Core2.Branching;

public sealed record BranchMember<T>(
    BranchId Id,
    T Value,
    IReadOnlyList<BranchId> Parents,
    IReadOnlyList<IBranchAnnotation> Annotations)
{
    public BranchMember(T value)
        : this(BranchId.New(), value, [], [])
    {
    }

    public bool TryGetAnnotation<TAnnotation>(out TAnnotation annotation)
        where TAnnotation : IBranchAnnotation
    {
        foreach (var candidate in Annotations)
        {
            if (candidate is TAnnotation typed)
            {
                annotation = typed;
                return true;
            }
        }

        annotation = default!;
        return false;
    }
}
