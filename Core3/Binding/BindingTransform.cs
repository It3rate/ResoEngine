using Core3.Engine;

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
/// A compact Core3-shaped numeric signal for binding-time transforms and
/// projections. The value is stored as an axis-like grade-2 element so nearby
/// variants remain explorable numerically rather than being trapped inside an
/// opaque enum space.
/// </summary>
public sealed record BindingSignal
{
    public BindingSignal(GradedElement value, string? note = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = Normalize(value);
        Note = note;
    }

    public CompositeElement Value { get; }
    public string? Note { get; }

    public static BindingSignal Identity { get; } = Axis(1, 0, "identity");
    public static BindingSignal Negate { get; } = Axis(-1, 0, "negate");
    public static BindingSignal Orthogonal { get; } = Axis(0, 1, "orthogonal");
    public static BindingSignal OppositeOrthogonal { get; } = Axis(0, -1, "opposite-orthogonal");

    public static BindingSignal Axis(
        long recessiveValue,
        long dominantValue,
        string? note = null,
        long resolution = 1)
    {
        if (resolution <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resolution));
        }

        return new BindingSignal(
            CreateAxisLikeNumber(recessiveValue, dominantValue, resolution),
            note);
    }

    public bool TryCompose(BindingSignal other, out BindingSignal? composed)
    {
        if (other is null)
        {
            composed = null;
            return false;
        }

        if (Value.TryMultiply(other.Value, out var product) &&
            product is not null)
        {
            try
            {
                composed = new BindingSignal(product);
                return true;
            }
            catch (InvalidOperationException)
            {
            }
        }

        composed = null;
        return false;
    }

    private static CompositeElement Normalize(GradedElement value) =>
        value switch
        {
            CompositeElement composite when composite.Grade == 2 => composite,
            CompositeElement composite
                when composite.Grade == 1 &&
                     composite.Recessive is AtomicElement recessive &&
                     composite.Dominant is AtomicElement dominant =>
                new CompositeElement(
                    CreateExactScalar(recessive),
                    CreateExactScalar(dominant)),
            _ => throw new InvalidOperationException("Binding signals currently use axis-like grade-2 values or grade-1 pairs that can be lifted back into that form.")
        };

    private static CompositeElement CreateAxisLikeNumber(
        long recessiveValue,
        long dominantValue,
        long resolution) =>
        new(
            CreateExactScalar(recessiveValue, resolution),
            CreateExactScalar(dominantValue, resolution));

    private static CompositeElement CreateExactScalar(long value, long resolution) =>
        new(
            new AtomicElement(resolution, 1),
            new AtomicElement(value, 1));

    private static CompositeElement CreateExactScalar(AtomicElement atomic) =>
        CreateExactScalar(atomic.Value, atomic.Resolution);
}

/// <summary>
/// A lightweight binding-side transform descriptor backed by a Core3 numeric
/// signal rather than an enum token.
/// </summary>
public sealed record BindingTransform(
    BindingSignal Signal,
    string? Note = null)
{
    public static BindingTransform Identity { get; } =
        new(BindingSignal.Identity, "identity");

    public static BindingTransform NegateValue { get; } =
        new(BindingSignal.Negate, "negate-value");

    public static BindingTransform OppositeOrientation { get; } =
        new(BindingSignal.Negate, "opposite-orientation");

    public static BindingTransform Orthogonalize { get; } =
        new(BindingSignal.Orthogonal, "orthogonalize");
}

/// <summary>
/// Binding-time projection metadata backed by the same numeric signal idea as
/// transforms. Common named projections are only convenience aliases over that
/// signal space.
/// </summary>
public sealed record BindingProjection(
    BindingSignal Signal,
    string? Note = null)
{
    public static BindingProjection Whole { get; } =
        new(BindingSignal.Identity, "whole");

    public static BindingProjection Value { get; } =
        new(BindingSignal.Identity, "value");

    public static BindingProjection Unit { get; } =
        new(BindingSignal.Negate, "unit");

    public static BindingProjection Recessive { get; } =
        new(BindingSignal.OppositeOrthogonal, "recessive");

    public static BindingProjection Dominant { get; } =
        new(BindingSignal.Orthogonal, "dominant");

    public static BindingProjection Inbound { get; } =
        new(BindingSignal.OppositeOrthogonal, "inbound");

    public static BindingProjection Outbound { get; } =
        new(BindingSignal.Orthogonal, "outbound");
}
