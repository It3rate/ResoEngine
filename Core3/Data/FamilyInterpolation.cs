using Core3.Engine;

namespace Core3.Data;

/// <summary>
/// GENERATIVE — derives new elements between existing members.
///
/// This belongs to the unresolved/evaluative layer. Interpolation produces
/// structure that didn't exist in either source element — it lives in the
/// space between the data.
///
/// For now this is a standalone helper. As the generative layer matures,
/// this should integrate with whatever mechanism resolves unresolved (zero-unit)
/// frames. The idea is that a zero-unit frame leaf would invoke something like
/// this to generate the missing structure on demand.
///
/// Tension is preserved when members can't be smoothly interpolated
/// (different unit spaces, incompatible grades). This is genuinely useful —
/// it tells you "these two things resist blending" rather than silently
/// producing garbage values.
/// </summary>
public static class FamilyInterpolation
{
    /// <summary>
    /// Interpolate between two elements using a rational weight (value/unit).
    /// Weight 0/n = pure left, n/n = pure right, k/n = weighted blend.
    ///
    /// The approach: scale left by (unit - value) and right by value,
    /// then add. This keeps everything in exact integer arithmetic.
    /// </summary>
    public static EngineElementOutcome Interpolate(
        GradedElement left,
        GradedElement right,
        AtomicElement weight)
    {
        if (weight.Value == 0)
            return EngineElementOutcome.Exact(left);

        if (weight.Value == weight.Unit)
            return EngineElementOutcome.Exact(right);

        // left * (unit - value) + right * value, all over unit
        var complementWeight = new AtomicElement(weight.Unit - weight.Value, weight.Unit);

        var scaledLeft = left.Scale(complementWeight);
        var scaledRight = right.Scale(weight);
        var sum = scaledLeft.Result.Add(scaledRight.Result);

        if (scaledLeft.IsExact && scaledRight.IsExact && sum.IsExact)
            return sum;

        return EngineElementOutcome.WithTension(
            sum.Result,
            new CompositeElement(left, right),
            "Interpolation preserved tension from incompatible member structure.");
    }

    /// <summary>
    /// Nearest-neighbor: purely structural selection, no generation.
    /// This is actually a VALUE or STRUCTURAL operation — it just picks one side.
    /// No new structure is created, so no generative layer is invoked.
    /// </summary>
    public static GradedElement NearestNeighbor(
        GradedElement left,
        GradedElement right,
        AtomicElement weight)
    {
        return (weight.Value * 2 < weight.Unit) ? left : right;
    }
}
