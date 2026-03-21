namespace Core2.Symbolics.Expressions;

public sealed record AnchorPositionTerm : ValueTerm
{
    public AnchorPositionTerm(AnchorReferenceTerm anchor)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        Anchor = anchor;
    }

    public AnchorReferenceTerm Anchor { get; }
}
