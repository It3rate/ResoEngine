using Core2.Elements;

namespace Core2.Resolution;

/// <summary>
/// Pass 1 classification defaults for primitive support behavior.
/// These methods intentionally express the intended law choice without
/// changing the current arithmetic implementation yet.
/// </summary>
public static class PrimitiveResolutionDefaults
{
    public static PrimitiveSupportLaw ClassifyTransformApplication(IElement state, IElement transform)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(transform);

        return PrimitiveSupportLaw.Inherit;
    }

    public static PrimitiveSupportLaw ClassifyHostedScale(IElement host, IElement scale)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(scale);

        return PrimitiveSupportLaw.Inherit;
    }

    public static PrimitiveSupportLaw ClassifyAggregation() => PrimitiveSupportLaw.Aggregate;

    public static PrimitiveSupportLaw ClassifyIndependentComposition(IElement left, IElement right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return PrimitiveSupportLaw.Compose;
    }

    public static PrimitiveSupportLaw ClassifyRefinement(IElement value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return PrimitiveSupportLaw.Refine;
    }

    public static PrimitiveSupportLaw ClassifyCommonFrameCombination(IElement left, IElement right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return PrimitiveSupportLaw.CommonFrame;
    }

    public static PrimitiveSupportLaw ClassifyPowerProjection(IElement value, Proportion exponent)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(exponent);

        return PrimitiveSupportLaw.Compose;
    }

    public static PrimitiveSupportLaw ClassifyInverseContinuationProjection(IElement value, Proportion degree)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(degree);

        return PrimitiveSupportLaw.Compose;
    }
}
