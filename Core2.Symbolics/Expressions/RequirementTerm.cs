namespace Core2.Symbolics.Expressions;

public sealed record RequirementTerm : ConstraintTerm
{
    public RequirementTerm(RelationTerm relation)
    {
        ArgumentNullException.ThrowIfNull(relation);

        Relation = relation;
    }

    public RelationTerm Relation { get; }
}
