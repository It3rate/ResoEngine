namespace Core2.Symbolics.Expressions;

public sealed record CarrierFlagTerm : RelationTerm
{
    public CarrierFlagTerm(CarrierReferenceTerm carrier, SymbolicCarrierFlagKind kind)
    {
        ArgumentNullException.ThrowIfNull(carrier);

        Carrier = carrier;
        Kind = kind;
    }

    public CarrierReferenceTerm Carrier { get; }
    public SymbolicCarrierFlagKind Kind { get; }
}
