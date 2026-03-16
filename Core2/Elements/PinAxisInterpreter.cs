namespace Core2.Elements;

public static class PinAxisInterpreter
{
    public static PinAxisResolution Resolve(Axis descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var recessive = ResolveSide(PinSideRole.Recessive, descriptor.Recessive);
        var dominant = ResolveSide(PinSideRole.Dominant, descriptor.Dominant);

        return new PinAxisResolution(recessive, dominant);
    }

    public static PinRelation ResolveAxisPairRelation(Axis recessive, Axis dominant)
    {
        ArgumentNullException.ThrowIfNull(recessive);
        ArgumentNullException.ThrowIfNull(dominant);

        int? recessiveCarrier = recessive.PinResolution.SharedCarrierRank;
        int? dominantCarrier = dominant.PinResolution.SharedCarrierRank;

        if (!recessiveCarrier.HasValue || !dominantCarrier.HasValue)
        {
            return PinRelation.Ordered;
        }

        if (recessiveCarrier == dominantCarrier)
        {
            return PinRelation.CollinearOpposed;
        }

        return PinRelation.OrthogonalDirect;
    }

    private static PinResolvedSide ResolveSide(PinSideRole role, Proportion value)
    {
        int unitSign = Math.Sign(value.Recessive);
        int valueSign = Math.Sign(value.Dominant);

        if (unitSign == 0)
        {
            return new PinResolvedSide(
                role,
                value.Recessive,
                value.Dominant,
                null,
                valueSign,
                PinLiftKind.ZeroHold);
        }

        int carrierRank = unitSign > 0 ? 0 : 1;
        int nativePositiveDirection = role == PinSideRole.Recessive ? -1 : 1;
        int directionSign = valueSign == 0 ? 0 : nativePositiveDirection * valueSign;
        PinLiftKind liftKind = unitSign < 0 ? PinLiftKind.OrthogonalBreakout : PinLiftKind.None;

        return new PinResolvedSide(
            role,
            value.Recessive,
            value.Dominant,
            carrierRank,
            directionSign,
            liftKind);
    }

}
