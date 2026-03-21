namespace Core2.Symbolics.Expressions;

public sealed record SiteFlagTerm : RelationTerm
{
    public SiteFlagTerm(SiteReferenceTerm site, SymbolicSiteFlagKind kind)
    {
        ArgumentNullException.ThrowIfNull(site);

        Site = site;
        Kind = kind;
    }

    public SiteReferenceTerm Site { get; }
    public SymbolicSiteFlagKind Kind { get; }
}
