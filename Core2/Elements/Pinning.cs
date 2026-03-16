namespace Core2.Elements;

public static class Pinning
{
    public static PinnedPair<TRecessive, TDominant> Pin<TRecessive, TDominant>(
        TRecessive recessive,
        TDominant dominant,
        PinRelation relation)
        where TRecessive : IElement
        where TDominant : IElement =>
        new(recessive, dominant, relation);
}
