namespace Core3.Binding;

/// <summary>
/// The structural site kind at which an operation law is attached.
/// </summary>
public enum OperationSiteKind
{
    Pin,
    Carrier,
    Boundary,
    Token
}

/// <summary>
/// A locatable operation site. The address is optional so the site can refer to
/// either the current encounter or a more explicit landmark.
/// </summary>
public sealed record OperationSite(
    OperationSiteKind Kind,
    string? Name = null,
    BindingAddress? Address = null);

/// <summary>
/// A lightweight reference to an operation law. The binding namespace keeps
/// this as metadata so operations remain separate from structure and data.
/// </summary>
public sealed record OperationLawReference(
    string Name,
    string? Variant = null);

/// <summary>
/// One named input to an attached operation, supplied by contextual selection.
/// </summary>
public sealed record OperationInputBinding(
    string Name,
    BindingSelector Selector,
    BindingMaterialization Materialization = BindingMaterialization.OnRead);

/// <summary>
/// One named output from an attached operation. The output may be transformed
/// before storage so routing and later reads can stay explicit.
/// </summary>
public sealed record OperationOutputBinding(
    string Name,
    BindingStorageTarget Target,
    BindingTransform Transform);

/// <summary>
/// Declarative attachment of one operation law to one structural site together
/// with its contextual inputs and storage outputs.
/// </summary>
public sealed record OperationAttachment(
    OperationSite Site,
    OperationLawReference Law,
    IReadOnlyList<OperationInputBinding> Inputs,
    IReadOnlyList<OperationOutputBinding> Outputs);
