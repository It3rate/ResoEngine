using Core2.Elements;

namespace Core2.Resolution;

/// <summary>
/// Runtime bridge for committing an exact proportion onto a later support policy.
/// This keeps exact arithmetic separate from the later question of which support
/// should carry the committed result.
/// </summary>
public static class PrimitiveResultSupportRuntime
{
    public static ResultSupportCommitment ScaleHostedValue(
        Proportion host,
        Proportion scale,
        ResultSupportPolicy policy = ResultSupportPolicy.PreserveHost)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(scale);

        var exact = host * scale;
        return CommitExactValue(exact, policy, host, host, scale);
    }

    public static ResultSupportCommitment AddInCommonFrame(
        Proportion left,
        Proportion right,
        ResultSupportPolicy policy = ResultSupportPolicy.PreserveExactAlignment)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var exact = PrimitiveProportionResolution.CommonFrameAdd(left, right);
        return CommitExactValue(exact, policy, null, left, right);
    }

    public static ResultSupportCommitment AggregateEvidence(
        Proportion left,
        Proportion right,
        ResultSupportPolicy policy = ResultSupportPolicy.PreserveExactStructure)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var exact = PrimitiveProportionResolution.Aggregate(left, right);
        return CommitExactValue(exact, policy, null, left, right);
    }

    public static ResultSupportCommitment CommitExactValue(
        Proportion exactValue,
        ResultSupportPolicy policy,
        Proportion? host = null,
        params Proportion[] context)
    {
        ArgumentNullException.ThrowIfNull(exactValue);

        long requestedSupport = SelectRequestedSupport(exactValue, policy, host, context);
        var committed = PrimitiveProportionResolution.RefineToSupport(exactValue, requestedSupport);
        return new ResultSupportCommitment(policy, exactValue, requestedSupport, committed);
    }

    private static long SelectRequestedSupport(
        Proportion exactValue,
        ResultSupportPolicy policy,
        Proportion? host,
        Proportion[] context)
    {
        long fallback = exactValue.Denominator == 0 ? 1L : exactValue.Denominator;

        return policy switch
        {
            ResultSupportPolicy.PreserveCoarser => SignedSupport(
                GetMagnitude(context, fallback, Math.Min),
                fallback),
            ResultSupportPolicy.PreserveFiner => SignedSupport(
                GetMagnitude(context, fallback, Math.Max),
                fallback),
            ResultSupportPolicy.PreserveHost => host?.Denominator ?? fallback,
            ResultSupportPolicy.PreserveExactAlignment => exactValue.Denominator,
            ResultSupportPolicy.PreserveExactStructure => exactValue.Denominator,
            ResultSupportPolicy.NegotiateFromUncertainty => SignedSupport(
                EstimateNegotiatedSupport(context, fallback),
                fallback),
            _ => exactValue.Denominator,
        };
    }

    private static long GetMagnitude(
        Proportion[] context,
        long fallback,
        Func<long, long, long> chooser)
    {
        long? current = null;
        foreach (var value in context)
        {
            long magnitude = Math.Abs(value.Denominator);
            if (magnitude == 0)
            {
                continue;
            }

            current = current is null ? magnitude : chooser(current.Value, magnitude);
        }

        return current ?? Math.Max(1L, Math.Abs(fallback));
    }

    private static long EstimateNegotiatedSupport(Proportion[] context, long fallback)
    {
        decimal halfWidth = 0m;
        foreach (var value in context)
        {
            long support = Math.Abs(value.Denominator);
            if (support == 0)
            {
                continue;
            }

            halfWidth += 1m / (2m * support);
        }

        if (halfWidth <= 0m)
        {
            return Math.Max(1L, Math.Abs(fallback));
        }

        decimal estimate = Math.Round(1m / (2m * halfWidth), 0, MidpointRounding.AwayFromZero);
        estimate = decimal.Clamp(estimate, 1m, long.MaxValue);
        return (long)estimate;
    }

    private static long SignedSupport(long magnitude, long signSource)
    {
        int sign = Math.Sign(signSource);
        if (sign == 0)
        {
            sign = 1;
        }

        return sign * Math.Max(1L, magnitude);
    }
}
