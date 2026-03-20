namespace Core2.Symbolics.Expressions;

public sealed record RequirementTerm : ConstraintTerm
{
    public RequirementTerm(RelationTerm relation, string? participantName = null)
    {
        ArgumentNullException.ThrowIfNull(relation);

        Relation = relation;
        ParticipantName = string.IsNullOrWhiteSpace(participantName) ? null : participantName;
    }

    public RelationTerm Relation { get; }
    public string? ParticipantName { get; }
}
