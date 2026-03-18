namespace Core2.Elements;

/// <summary>
/// An Axis descriptor anchored on a line-like host carrier at a host-relative position.
/// This preserves the intrinsic descriptor while exposing how its sides sit relative to the host.
/// </summary>
public sealed record PositionedAxis(Axis Intrinsic, Proportion Position)
{
    public PinAxisResolution Resolution => Intrinsic.PinResolution;
    public Proportion EmbeddedOrigin => Position;
    public bool HasCollinearExtent => Resolution.IsCollinear;
    public Proportion? EmbeddedCollinearStart => HasCollinearExtent ? Position + Intrinsic.StartCoordinate : null;
    public Proportion? EmbeddedCollinearEnd => HasCollinearExtent ? Position + Intrinsic.EndCoordinate : null;
    public PositionedAxisSide RecessiveSide => CreateSide(PinSideRole.Recessive, Intrinsic.Recessive, Resolution.RecessiveSide);
    public PositionedAxisSide DominantSide => CreateSide(PinSideRole.Dominant, Intrinsic.Dominant, Resolution.DominantSide);

    private PositionedAxisSide CreateSide(PinSideRole role, Proportion intrinsicValue, PinResolvedSide resolvedSide)
    {
        Proportion magnitude = MagnitudeOf(intrinsicValue);
        Proportion signedExtent = resolvedSide.DirectionSign == 0
            ? Proportion.Zero
            : new Proportion(resolvedSide.DirectionSign) * magnitude;

        return new PositionedAxisSide(
            role,
            Position,
            intrinsicValue,
            resolvedSide.UnitEncoding,
            resolvedSide.CarrierRank,
            signedExtent,
            resolvedSide.LiftKind);
    }

    private static Proportion MagnitudeOf(Proportion value)
    {
        long numerator = value.Dominant == long.MinValue
            ? throw new OverflowException("Cannot compute magnitude of proportion with long.MinValue numerator.")
            : Math.Abs(value.Dominant);
        long denominator = value.Recessive == long.MinValue
            ? throw new OverflowException("Cannot compute magnitude of proportion with long.MinValue denominator.")
            : Math.Abs(value.Recessive);
        return new Proportion(numerator, denominator);
    }
}

public sealed record PositionedAxisSide(
    PinSideRole Role,
    Proportion Origin,
    Proportion IntrinsicValue,
    long UnitEncoding,
    int? CarrierRank,
    Proportion SignedExtent,
    PinLiftKind LiftKind)
{
    public int UnitSign => Math.Sign(UnitEncoding);
    public bool HasCarrier => CarrierRank.HasValue;
    public bool IsUnresolved => !HasCarrier;
    public bool IsLifted => LiftKind == PinLiftKind.OrthogonalBreakout;
    public Proportion Magnitude => SignedExtent.Sign == 0 ? Proportion.Zero : SignedExtent.Abs();
}
