namespace Core3.Binding;

/// <summary>
/// Explicit coupling between one target path and a selected source with a small
/// transform law. This is broader than reference alone and does not itself
/// imply structural pinning.
/// </summary>
public sealed record BindingConstraint(
    string TargetPath,
    BindingSelector Source,
    BindingTransform Transform,
    BindingMaterialization Materialization = BindingMaterialization.OnBind)
{
    public string TargetPath { get; } = !string.IsNullOrWhiteSpace(TargetPath)
        ? TargetPath
        : throw new ArgumentException("A binding target path cannot be empty.", nameof(TargetPath));
}

/// <summary>
/// Base type for partially specified runtime literals that are later bound into
/// fully materialized engine values. These are binding-time templates, not
/// serialized engine ontology.
/// </summary>
public abstract record BoundTemplate
{
    public BindingMaterialization Materialization { get; init; } = BindingMaterialization.OnBind;
    public IReadOnlyList<BindingConstraint> Constraints { get; init; } = [];
}

/// <summary>
/// Grade-0 bound literal. Null means "not specified here" and may be supplied
/// by context binding; it does not mean the same thing as an explicit zero unit
/// inside engine ontology.
/// </summary>
public sealed record BoundScalarTemplate : BoundTemplate
{
    public long? Value { get; init; }
    public long? Unit { get; init; }
    public BindingSelector? ValueBinding { get; init; }
    public BindingSelector? UnitBinding { get; init; }
    public BindingTransform ValueTransform { get; init; } = BindingTransform.Identity;
    public BindingTransform UnitTransform { get; init; } = BindingTransform.Identity;
}

/// <summary>
/// Higher-grade bound literal built from two child templates. Binding and later
/// materialization can therefore proceed recursively without requiring the
/// template itself to be a first-class engine element.
/// </summary>
public sealed record BoundCompositeTemplate : BoundTemplate
{
    public BoundCompositeTemplate(BoundTemplate recessive, BoundTemplate dominant)
    {
        Recessive = recessive ?? throw new ArgumentNullException(nameof(recessive));
        Dominant = dominant ?? throw new ArgumentNullException(nameof(dominant));
    }

    public BoundTemplate Recessive { get; }
    public BoundTemplate Dominant { get; }
}
