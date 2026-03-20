using Core2.Elements;

namespace Core2.Interpretation.Placement;

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

    public PositionedAxisCarrierResponse ResolveCarrierResponse(
        int currentDirection,
        int currentCarrierRank = 0,
        Axis? host = null,
        bool boundaryEncounter = false)
    {
        int normalizedDirection = NormalizeDirection(currentDirection);
        PositionedAxisSide encountered = ResolveEncounteredSide(normalizedDirection, host, boundaryEncounter);
        PositionedAxisSide opposite = encountered.Role == PinSideRole.Recessive
            ? DominantSide
            : RecessiveSide;

        return new PositionedAxisCarrierResponse(
            RecessiveSide,
            DominantSide,
            encountered,
            opposite,
            normalizedDirection,
            currentCarrierRank,
            boundaryEncounter);
    }

    public int? ResolveAmbientCarrierRank(PinSideRole role, int hostCarrierRank = 0)
    {
        PositionedAxisSide side = role == PinSideRole.Recessive ? RecessiveSide : DominantSide;
        return ResolveAmbientCarrierRank(side, hostCarrierRank);
    }

    public static int? ResolveAmbientCarrierRank(int? localCarrierRank, int hostCarrierRank)
    {
        if (!localCarrierRank.HasValue)
        {
            return null;
        }

        return localCarrierRank.Value switch
        {
            0 => hostCarrierRank,
            1 => OrthogonalCarrierRank(hostCarrierRank),
            _ => localCarrierRank,
        };
    }

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

    private PositionedAxisSide ResolveEncounteredSide(int normalizedDirection, Axis? host, bool boundaryEncounter)
    {
        if (boundaryEncounter && host is not null)
        {
            if (normalizedDirection < 0 && EmbeddedOrigin == host.LeftCoordinate)
            {
                return DominantSide;
            }

            if (normalizedDirection > 0 && EmbeddedOrigin == host.RightCoordinate)
            {
                return RecessiveSide;
            }
        }

        return normalizedDirection < 0
            ? RecessiveSide
            : DominantSide;
    }

    internal static int? ResolveAmbientCarrierRank(PositionedAxisSide side, int hostCarrierRank)
        => ResolveAmbientCarrierRank(side.CarrierRank, hostCarrierRank);

    public static int OrthogonalCarrierRank(int carrierRank) => carrierRank == 0 ? 1 : 0;

    private static int NormalizeDirection(int direction) => direction < 0 ? -1 : 1;
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
    public int DisplayDirectionSign => Math.Sign(SignedExtent.Sign);
    public int TransportDirectionSign =>
        DisplayDirectionSign == 0
            ? 0
            : Role == PinSideRole.Dominant
                ? DisplayDirectionSign
                : -DisplayDirectionSign;
    public bool HasCarrier => CarrierRank.HasValue;
    public bool IsUnresolved => !HasCarrier;
    public bool IsLifted => LiftKind == PinLiftKind.OrthogonalBreakout;
    public Proportion Magnitude => SignedExtent.Sign == 0 ? Proportion.Zero : SignedExtent.Abs();
}

public sealed record PositionedAxisCarrierResponse(
    PositionedAxisSide RecessiveSide,
    PositionedAxisSide DominantSide,
    PositionedAxisSide EncounteredSide,
    PositionedAxisSide OppositeSide,
    int CurrentDirection,
    int CurrentCarrierRank,
    bool BoundaryEncounter)
{
    public int? RecessiveAmbientCarrierRank => ResolveAmbientCarrierRank(RecessiveSide);
    public int? DominantAmbientCarrierRank => ResolveAmbientCarrierRank(DominantSide);
    public int? EncounterAmbientCarrierRank => ResolveAmbientCarrierRank(EncounteredSide);
    public int? OppositeAmbientCarrierRank => ResolveAmbientCarrierRank(OppositeSide);
    public bool EncounterSideOnCurrentCarrier => EncounterAmbientCarrierRank == CurrentCarrierRank;
    public bool EncounterSideHasTravel => EncounteredSide.SignedExtent.Sign != 0;
    public int EncounterNextDirection => EncounterSideHasTravel ? Math.Sign(EncounteredSide.SignedExtent.Sign) : 0;
    public bool IsTransparent => EncounterSideOnCurrentCarrier && EncounterNextDirection == CurrentDirection;
    public bool IsRedirect => EncounterSideOnCurrentCarrier && EncounterNextDirection != 0 && EncounterNextDirection != CurrentDirection;
    public bool HasNoTravelOnCurrentCarrier => EncounterSideOnCurrentCarrier && EncounterNextDirection == 0;
    public bool ResolvesOffCurrentCarrier => EncounteredSide.HasCarrier && !EncounterSideOnCurrentCarrier;
    public bool IsUnresolvedOnCurrentCarrier => !EncounteredSide.HasCarrier;
    public bool HasOrthogonalOutlet => HasNonCurrentCarrierTravel(RecessiveSide) || HasNonCurrentCarrierTravel(DominantSide);
    public bool EncounterHasOrthogonalOutlet => HasNonCurrentCarrierTravel(EncounteredSide);
    public bool BlocksHostNegativeSide => OpposesPositiveCarrier(DominantSide);
    public bool BlocksHostPositiveSide => OpposesPositiveCarrier(RecessiveSide);
    public bool BlocksContinuationPastEncounter => CurrentDirection < 0 ? BlocksHostNegativeSide : BlocksHostPositiveSide;
    public PositionedAxisSide ApproachSide => BoundaryEncounter ? EncounteredSide : OppositeSide;
    public bool ApproachSideOnCurrentCarrier => ResolveAmbientCarrierRank(ApproachSide) == CurrentCarrierRank;
    public bool ApproachSideHasTravel => ApproachSide.TransportDirectionSign != 0;
    public bool SupportsApproachIntoEncounter =>
        ApproachSideOnCurrentCarrier &&
        ApproachSideHasTravel &&
        ApproachSide.TransportDirectionSign == CurrentDirection;
    public bool BlocksApproachIntoEncounter =>
        ApproachSideOnCurrentCarrier &&
        ApproachSideHasTravel &&
        ApproachSide.TransportDirectionSign == -CurrentDirection;

    private bool OpposesPositiveCarrier(PositionedAxisSide side) =>
        ResolveAmbientCarrierRank(side) == CurrentCarrierRank && side.TransportDirectionSign < 0;

    private bool HasNonCurrentCarrierTravel(PositionedAxisSide side) =>
        side.HasCarrier &&
        ResolveAmbientCarrierRank(side) != CurrentCarrierRank &&
        side.SignedExtent.Sign != 0;

    private int? ResolveAmbientCarrierRank(PositionedAxisSide side) =>
        PositionedAxis.ResolveAmbientCarrierRank(side, CurrentCarrierRank);
}
