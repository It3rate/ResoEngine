namespace Core2.Symbolics.Branching;

public readonly record struct BranchEventId(Guid Value)
{
    public static BranchEventId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("N");
}
