namespace Core2.Branching;

public sealed record BranchNode<T>(
    BranchId Id,
    T Value,
    int Depth,
    int EventIndex,
    IReadOnlyList<IBranchAnnotation> Annotations)
{
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
