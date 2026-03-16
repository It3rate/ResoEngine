namespace Core2.Elements;

public readonly record struct PinResolvedSide(
    PinSideRole Role,
    long UnitEncoding,
    long ValueEncoding,
    int? CarrierRank,
    int DirectionSign,
    PinLiftKind LiftKind)
{
    public int UnitSign => Math.Sign(UnitEncoding);
    public int ValueSign => Math.Sign(ValueEncoding);
    public bool HasCarrier => CarrierRank.HasValue;
    public bool IsUnresolved => !HasCarrier;
    public bool IsLifted => LiftKind == PinLiftKind.OrthogonalBreakout;
}
