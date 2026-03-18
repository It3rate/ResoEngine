namespace Core2.Elements;

public sealed record CarrierIdentity(CarrierId Id, string? Name = null)
{
    public static CarrierIdentity Create(string? name = null) => new(CarrierId.New(), name);

    public override string ToString() => Name is null ? Id.ToString() : $"{Name}<{Id}>";
}
