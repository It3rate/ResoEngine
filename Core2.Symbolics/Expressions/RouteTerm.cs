namespace Core2.Symbolics.Expressions;

public sealed record RouteTerm : RelationTerm
{
    public RouteTerm(ReferenceTerm site, ReferenceTerm from, ReferenceTerm to)
    {
        ArgumentNullException.ThrowIfNull(site);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        Site = site;
        From = from;
        To = to;
    }

    public ReferenceTerm Site { get; }
    public ReferenceTerm From { get; }
    public ReferenceTerm To { get; }
}
