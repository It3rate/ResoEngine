namespace Core2.Elements;

/// <summary>
/// A local host-relative pinning event before any later distributed fold.
/// The recessive element is the host, the dominant element is the applied element,
/// and Position is read on the host.
/// </summary>
public sealed record PointPinning<TRecessive, TDominant>(
    TRecessive Host,
    TDominant Applied,
    Proportion Position)
    where TRecessive : IElement
    where TDominant : IElement
{
    public TRecessive RecessiveElement => Host;
    public TDominant DominantElement => Applied;

    /// <summary>
    /// The default local anchor on the applied element.
    /// Later work may allow non-zero applied anchors, but the base convention is local zero.
    /// </summary>
    public Proportion AppliedAnchor => Proportion.Zero;
}
