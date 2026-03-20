using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record PreferenceTerm : ConstraintTerm
{
    public PreferenceTerm(RelationTerm relation, Proportion weight)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentNullException.ThrowIfNull(weight);

        Relation = relation;
        Weight = weight;
    }

    public RelationTerm Relation { get; }
    public Proportion Weight { get; }
}
