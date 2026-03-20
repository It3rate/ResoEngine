namespace Core2.Symbolics.Expressions;

public sealed record RouteTerm : RelationTerm
{
    public RouteTerm(SiteReferenceTerm site, IncidentReferenceTerm from, IncidentReferenceTerm to)
    {
        ArgumentNullException.ThrowIfNull(site);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        Site = site;
        From = from;
        To = to;
    }

    public SiteReferenceTerm Site { get; }
    public IncidentReferenceTerm From { get; }
    public IncidentReferenceTerm To { get; }
}
