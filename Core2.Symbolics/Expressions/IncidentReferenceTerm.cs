namespace Core2.Symbolics.Expressions;

public sealed record IncidentReferenceTerm : RelationTerm
{
    public IncidentReferenceTerm(RouteIncidentKind kind)
    {
        Kind = kind;
    }

    public RouteIncidentKind Kind { get; }
}
