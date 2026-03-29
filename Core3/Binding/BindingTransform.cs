namespace Core3.Binding;

/// <summary>
/// When a partially specified binding becomes a fully materialized value.
/// </summary>
public enum BindingMaterialization
{
    OnBind,
    OnRead,
    OnStep,
    Deferred
}

/// <summary>
/// Small first-pass transform kinds for contextual binding and slot coupling.
/// These are deliberately light; the operation layer may later host richer
/// executable transform laws.
/// </summary>
public enum BindingTransformKind
{
    Identity,
    NegateValue,
    NegateUnit,
    OrthogonalizeUnit,
    PreserveMagnitude,
    SameOrientation,
    OppositeOrientation,
    Mirror
}

/// <summary>
/// A lightweight binding-side transform descriptor.
/// </summary>
public sealed record BindingTransform(
    BindingTransformKind Kind,
    long? Scalar = null,
    string? Note = null)
{
    public static BindingTransform Identity { get; } =
        new(BindingTransformKind.Identity);
}
