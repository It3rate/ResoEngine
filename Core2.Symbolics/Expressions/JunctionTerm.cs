namespace Core2.Symbolics.Expressions;

public sealed record JunctionTerm : RelationTerm
{
    public JunctionTerm(SiteReferenceTerm site, SymbolicJunctionKind kind)
    {
        ArgumentNullException.ThrowIfNull(site);

        Site = site;
        Kind = kind;
    }

    public SiteReferenceTerm Site { get; }
    public SymbolicJunctionKind Kind { get; }
}
