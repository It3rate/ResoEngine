using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record PreferenceTerm : ConstraintTerm
{
    public PreferenceTerm(RelationTerm relation, Proportion weight, string? participantName = null)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentNullException.ThrowIfNull(weight);

        Relation = relation;
        Weight = weight;
        ParticipantName = string.IsNullOrWhiteSpace(participantName) ? null : participantName;
    }

    public RelationTerm Relation { get; }
    public Proportion Weight { get; }
    public string? ParticipantName { get; }
}
