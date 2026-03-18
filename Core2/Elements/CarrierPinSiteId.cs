namespace Core2.Elements;

public readonly record struct CarrierPinSiteId(Guid Value)
{
    public static CarrierPinSiteId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("N");
}
