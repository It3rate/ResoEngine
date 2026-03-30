using Core3.Engine;

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
/// A real mover or trolley is expected to supply the active encounter, so the
/// address is now either a mover-relative numeric position or a named alias.
/// </summary>
public abstract record BindingAddress
{
    public sealed record Position(GradedElement Parameter) : BindingAddress;

    public sealed record Name(string Value) : BindingAddress;

    public static Position At(long value, long resolution = 1) =>
        new(new AtomicElement(value, resolution));
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
    /// <summary>
    /// Creates a selector whose address is a Core3 numeric parameter. A value of
    /// 0/1 typically means "where the mover is now"; signed values may look
    /// backward or forward, and fractional values may query along a path-like
    /// domain.
    /// </summary>
    public static BindingSelector At(
        BindingDomain domain,
        long value,
        long resolution = 1,
        BindingProjection? projection = null,
        BindingStorageTarget? storeTarget = null) =>
        new(domain, BindingAddress.At(value, resolution), projection ?? BindingProjection.Whole, storeTarget);

    /// <summary>
    /// Creates a selector by named alias, which is useful for authored token or
    /// context slots even when the deeper selector model prefers numeric
    /// positions.
    /// </summary>
    public static BindingSelector Named(
        BindingDomain domain,
        string name,
        BindingProjection? projection = null,
        BindingStorageTarget? storeTarget = null) =>
        new(domain, new BindingAddress.Name(name), projection ?? BindingProjection.Whole, storeTarget);
}
