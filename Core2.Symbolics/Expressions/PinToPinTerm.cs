namespace Core2.Symbolics.Expressions;

public sealed record PinToPinTerm : ValueTerm
{
    public PinToPinTerm(AnchorReferenceTerm hostAnchor, AnchorReferenceTerm appliedAnchor)
    {
        ArgumentNullException.ThrowIfNull(hostAnchor);
        ArgumentNullException.ThrowIfNull(appliedAnchor);

        HostAnchor = hostAnchor;
        AppliedAnchor = appliedAnchor;
    }

    public AnchorReferenceTerm HostAnchor { get; }
    public AnchorReferenceTerm AppliedAnchor { get; }
}
