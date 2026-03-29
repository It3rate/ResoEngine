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
    string? Variant = null)
{
    public string Name { get; } = !string.IsNullOrWhiteSpace(Name)
        ? Name
        : throw new ArgumentException("An operation law name cannot be empty.", nameof(Name));
}

/// <summary>
/// One named input to an attached operation, supplied by contextual selection.
/// </summary>
public sealed record OperationInputBinding(
    string Name,
    BindingSelector Selector,
    BindingMaterialization Materialization = BindingMaterialization.OnRead)
{
    public string Name { get; } = !string.IsNullOrWhiteSpace(Name)
        ? Name
        : throw new ArgumentException("An input binding name cannot be empty.", nameof(Name));

    public BindingSelector Selector { get; } =
        Selector ?? throw new ArgumentNullException(nameof(Selector));
}

/// <summary>
/// One named output from an attached operation. The output may be transformed
/// before storage so routing and later reads can stay explicit.
/// </summary>
public sealed record OperationOutputBinding(
    string Name,
    BindingStorageTarget Target,
    BindingTransform Transform)
{
    public string Name { get; } = !string.IsNullOrWhiteSpace(Name)
        ? Name
        : throw new ArgumentException("An output binding name cannot be empty.", nameof(Name));

    public BindingStorageTarget Target { get; } =
        Target ?? throw new ArgumentNullException(nameof(Target));

    public BindingTransform Transform { get; } =
        Transform ?? throw new ArgumentNullException(nameof(Transform));
}

/// <summary>
/// Declarative attachment of one operation law to one structural site together
/// with its contextual inputs and storage outputs.
/// </summary>
public sealed record OperationAttachment
{
    public OperationAttachment(
        OperationSite site,
        OperationLawReference law,
        IReadOnlyList<OperationInputBinding> inputs,
        IReadOnlyList<OperationOutputBinding> outputs)
    {
        Site = site ?? throw new ArgumentNullException(nameof(site));
        Law = law ?? throw new ArgumentNullException(nameof(law));
        Inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
        Outputs = outputs ?? throw new ArgumentNullException(nameof(outputs));
    }

    public OperationSite Site { get; }
    public OperationLawReference Law { get; }
    public IReadOnlyList<OperationInputBinding> Inputs { get; }
    public IReadOnlyList<OperationOutputBinding> Outputs { get; }
}
