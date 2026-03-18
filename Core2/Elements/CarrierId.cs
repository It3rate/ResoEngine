namespace Core2.Elements;

public readonly record struct CarrierId(Guid Value)
{
    public static CarrierId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("N");
}
