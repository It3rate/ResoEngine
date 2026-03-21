using Core2.Elements;

namespace Core2.Resolution;

/// <summary>
/// Pass 2 runtime bridge for primitive transform application.
/// This applies the small set of support-preserving transforms that already
/// have native Core 2 meaning, before falling back to generic multiplication.
/// </summary>
public static class PrimitiveTransformRuntime
{
    public static bool TryApplyPreservingSupport(IElement state, IElement transform, out IElement result)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(transform);

        switch (state)
        {
            case Scalar scalarState when transform is Scalar scalarTransform:
            {
                if (scalarTransform == Scalar.One)
                {
                    result = scalarState;
                    return true;
                }

                if (scalarTransform == -Scalar.One)
                {
                    result = -scalarState;
                    return true;
                }

                break;
            }

            case Proportion proportionState when transform is Proportion proportionTransform:
            {
                if (proportionTransform == Proportion.One)
                {
                    result = proportionState;
                    return true;
                }

                if (proportionTransform == -Proportion.One)
                {
                    result = -proportionState;
                    return true;
                }

                break;
            }

            case Axis axisState when transform is Axis axisTransform:
            {
                if (axisTransform == Axis.One)
                {
                    result = axisState;
                    return true;
                }

                if (axisTransform == Axis.I)
                {
                    result = axisState.ApplyOpposition();
                    return true;
                }

                if (axisTransform == Axis.NegativeOne)
                {
                    result = -axisState;
                    return true;
                }

                if (axisTransform == Axis.NegativeI)
                {
                    result = -axisState.ApplyOpposition();
                    return true;
                }

                break;
            }
        }

        result = null!;
        return false;
    }

    public static bool TryDividePreservingSupport(IElement state, IElement transform, out IElement result)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(transform);

        switch (state)
        {
            case Scalar scalarState when transform is Scalar scalarTransform:
            {
                if (scalarTransform == Scalar.One)
                {
                    result = scalarState;
                    return true;
                }

                if (scalarTransform == -Scalar.One)
                {
                    result = -scalarState;
                    return true;
                }

                break;
            }

            case Proportion proportionState when transform is Proportion proportionTransform:
            {
                if (proportionTransform == Proportion.One)
                {
                    result = proportionState;
                    return true;
                }

                if (proportionTransform == -Proportion.One)
                {
                    result = -proportionState;
                    return true;
                }

                break;
            }

            case Axis axisState when transform is Axis axisTransform:
            {
                if (axisTransform == Axis.One)
                {
                    result = axisState;
                    return true;
                }

                if (axisTransform == Axis.I)
                {
                    result = -axisState.ApplyOpposition();
                    return true;
                }

                if (axisTransform == Axis.NegativeOne)
                {
                    result = -axisState;
                    return true;
                }

                if (axisTransform == Axis.NegativeI)
                {
                    result = axisState.ApplyOpposition();
                    return true;
                }

                break;
            }
        }

        result = null!;
        return false;
    }
}
