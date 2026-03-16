namespace Core2.Elements;

public interface IPinnedElement : IElement
{
    PinRelation Relation { get; }
    IElement RecessiveElement { get; }
    IElement DominantElement { get; }
}

public interface IPinnedElement<out TRecessive, out TDominant> : IPinnedElement
    where TRecessive : IElement
    where TDominant : IElement
{
    new TRecessive RecessiveElement { get; }
    new TDominant DominantElement { get; }
}
