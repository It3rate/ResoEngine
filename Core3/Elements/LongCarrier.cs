namespace Core3.Elements;

/// <summary>
/// The current scalar bootstrap for carriers.
/// The raw stored value stays in one orientation; side decides whether that
/// value is read directly or mirrored.
/// </summary>
public readonly record struct LongCarrier(long RawValue, CarrierSide Side = CarrierSide.Outbound) : ICarrier
{
    public static LongCarrier Zero => new(0, CarrierSide.Outbound);

    public long Value => Side == CarrierSide.Inbound ? -RawValue : RawValue;

    public bool IsZero => Value == 0;
    public bool IsPositive => Value > 0;
    public bool IsNegative => Value < 0;

    public bool IsCompatibleWith(ICarrier other) => other is LongCarrier;

    public ICarrier Subtract(ICarrier other) => new LongCarrier(
        checked(RawValue - RequireCompatible(other).RawValue),
        Side);

    public ICarrier PositionAt(ICarrier end, Proportion proportion)
    {
        var compatibleEnd = RequireCompatible(end);
        var resolved = checked((long)Math.Round(RawValue + ((compatibleEnd.RawValue - RawValue) * proportion.ToDecimal())));
        return new LongCarrier(resolved, CarrierSide.Outbound);
    }

    public ICarrier Negate() => new LongCarrier(-RawValue, Side);

    public ICarrier Mirror() => Side == CarrierSide.Inbound
        ? new LongCarrier(RawValue, CarrierSide.Outbound)
        : new LongCarrier(RawValue, CarrierSide.Inbound);

    public ICarrier AsInbound() => Side == CarrierSide.Inbound
        ? this
        : new LongCarrier(RawValue, CarrierSide.Inbound);

    public ICarrier AsOutbound() => Side == CarrierSide.Outbound
        ? this
        : new LongCarrier(RawValue, CarrierSide.Outbound);

    public override string ToString() => Value.ToString();

    private LongCarrier RequireCompatible(ICarrier other)
    {
        if (other is LongCarrier longCarrier)
        {
            return longCarrier;
        }

        throw new InvalidOperationException($"Carrier type {other.GetType().Name} is not compatible with {nameof(LongCarrier)}.");
    }
}
