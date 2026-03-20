using Core2.Elements;

namespace Core2.Interpretation.Construction;

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
