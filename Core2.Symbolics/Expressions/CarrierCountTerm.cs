namespace Core2.Symbolics.Expressions;

public sealed record CarrierCountTerm : ValueTerm
{
    public CarrierCountTerm(CarrierReferenceTerm carrier, SymbolicCarrierCountKind kind)
    {
        ArgumentNullException.ThrowIfNull(carrier);

        Carrier = carrier;
        Kind = kind;
    }

    public CarrierReferenceTerm Carrier { get; }
    public SymbolicCarrierCountKind Kind { get; }
}
