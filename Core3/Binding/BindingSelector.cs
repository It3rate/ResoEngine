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
    History,
    Result,
    Constant
}

/// <summary>
/// The local extraction mode once a domain and address have selected a source.
/// </summary>
public enum BindingExtraction
{
    Whole,
    ValueSlot,
    UnitSlot,
    Recessive,
    Dominant,
    Inbound,
    Outbound,
    BoundaryAxis,
    FoldedRead
}

/// <summary>
/// The fallback policy when a selector cannot resolve a value cleanly in the
/// current context.
/// </summary>
public enum BindingMissPolicy
{
    Fail,
    UseDefault,
    PreserveUnknown,
    SelectNearest,
    BranchAlternatives
}

/// <summary>
/// Where a selected or computed value should be retained for later use.
/// </summary>
public enum BindingStorageLocation
{
    TokenSlot,
    ContextSlot,
    ResultSlot,
    History,
    Transient
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
    BindingStorageLocation Location,
    string? Name = null,
    int? Index = null);

/// <summary>
/// The full contextual selector used by bindings and operation attachments.
/// Selection is intentionally decomposed into domain, address, extraction, and
/// miss policy so later execution can stay explicit about why a value was
/// chosen.
/// </summary>
public sealed record BindingSelector(
    BindingDomain Domain,
    BindingAddress Address,
    BindingExtraction Extraction = BindingExtraction.Whole,
    BindingMissPolicy MissPolicy = BindingMissPolicy.Fail,
    BindingStorageTarget? StoreTarget = null)
{
    public static BindingSelector Current(
        BindingDomain domain,
        BindingExtraction extraction = BindingExtraction.Whole,
        BindingMissPolicy missPolicy = BindingMissPolicy.Fail,
        BindingStorageTarget? storeTarget = null) =>
        new(domain, new BindingAddress.Current(), extraction, missPolicy, storeTarget);
}
