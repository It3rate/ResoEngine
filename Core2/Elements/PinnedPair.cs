namespace Core2.Elements;

public sealed record PinnedPair<TRecessive, TDominant>(
    TRecessive RecessiveElement,
    TDominant DominantElement,
    PinRelation Relation) : IPinnedElement<TRecessive, TDominant>
    where TRecessive : IElement
    where TDominant : IElement
{
    public int Degree => Math.Max(RecessiveElement.Degree, DominantElement.Degree) + 1;

    IElement IPinnedElement.RecessiveElement => RecessiveElement;
    IElement IPinnedElement.DominantElement => DominantElement;

    public PinnedPair<TDominant, TRecessive> Mirror() =>
        new(DominantElement, RecessiveElement, Relation);
}
