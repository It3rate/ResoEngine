namespace Core2.Symbolics.Expressions;

public sealed record CarrierSpanTerm : ValueTerm
{
    public CarrierSpanTerm(CarrierReferenceTerm carrier)
    {
        ArgumentNullException.ThrowIfNull(carrier);

        Carrier = carrier;
    }

    public CarrierReferenceTerm Carrier { get; }
}
