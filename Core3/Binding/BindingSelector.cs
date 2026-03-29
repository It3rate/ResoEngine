namespace Core3.Binding;

/// <summary>
/// The structural pool a binding reads from before later addressing and
/// extraction rules refine the selection.
/// </summary>
public enum BindingDomain
{
    Frame,
    Family,
    Token,
    Site,
    Context,
    History,
    Result
}

/// <summary>
/// Local addressing inside one binding domain.
/// </summary>
public abstract record BindingAddress
{
    public sealed record Current : BindingAddress;

    public sealed record Slot(int Index) : BindingAddress
    {
        public int Index { get; } = Index >= 0
            ? Index
            : throw new ArgumentOutOfRangeException(nameof(Index));
    }

    public sealed record Name(string Value) : BindingAddress
    {
        public string Value { get; } = !string.IsNullOrWhiteSpace(Value)
            ? Value
            : throw new ArgumentException("A binding name cannot be empty.", nameof(Value));
    }

    public sealed record Offset(int Value) : BindingAddress;

    public sealed record Normalized(decimal Position) : BindingAddress
    {
        public decimal Position { get; } = Position is >= 0m and <= 1m
            ? Position
            : throw new ArgumentOutOfRangeException(nameof(Position));
    }
}

/// <summary>
/// Describes where a selected or computed value should be written if the
/// binding wants to retain the read for later steps.
/// </summary>
public sealed record BindingStorageTarget(
    BindingDomain Domain,
    string? Name = null,
    int? Index = null);

/// <summary>
/// The full contextual selector used by bindings and operation attachments.
/// Selection is intentionally decomposed into domain, address, and projection.
/// The current pass assumes nearest-or-fail resolution behavior and leaves
/// richer attention-style fallback policy for a later layer.
/// </summary>
public sealed record BindingSelector(
    BindingDomain Domain,
    BindingAddress Address,
    BindingProjection Projection,
    BindingStorageTarget? StoreTarget = null)
{
    public static BindingSelector Current(
        BindingDomain domain,
        BindingProjection? projection = null,
        BindingStorageTarget? storeTarget = null) =>
        new(domain, new BindingAddress.Current(), projection ?? BindingProjection.Whole, storeTarget);
}
