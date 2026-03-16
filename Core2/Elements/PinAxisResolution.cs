namespace Core2.Elements;

public sealed record PinAxisResolution(
    PinResolvedSide RecessiveSide,
    PinResolvedSide DominantSide)
{
    public bool HasUnresolvedCarrier => RecessiveSide.IsUnresolved || DominantSide.IsUnresolved;
    public int? SharedCarrierRank =>
        !HasUnresolvedCarrier && RecessiveSide.CarrierRank == DominantSide.CarrierRank
            ? RecessiveSide.CarrierRank
            : null;

    public PinBehaviorKind Behavior =>
        HasUnresolvedCarrier
            ? PinBehaviorKind.Unresolved
            : SharedCarrierRank.HasValue
                ? ResolveCollinearBehavior()
                : PinBehaviorKind.OrthogonalStructure;

    public PinRelation Relation =>
        HasUnresolvedCarrier
            ? PinRelation.Ordered
            : SharedCarrierRank.HasValue
                ? ResolveCollinearRelation()
                : ResolveOrthogonalRelation();

    public bool IsCollinear => SharedCarrierRank.HasValue;
    public bool IsOrthogonal => Behavior == PinBehaviorKind.OrthogonalStructure;
    public bool IsDirectedSegment => Behavior == PinBehaviorKind.DirectedSegment;
    public bool IsSequentialReinforcement => Behavior == PinBehaviorKind.SequentialReinforcement;

    private PinBehaviorKind ResolveCollinearBehavior()
    {
        if (RecessiveSide.DirectionSign == 0 || DominantSide.DirectionSign == 0)
        {
            return PinBehaviorKind.Unresolved;
        }

        return RecessiveSide.DirectionSign == DominantSide.DirectionSign
            ? PinBehaviorKind.SequentialReinforcement
            : PinBehaviorKind.DirectedSegment;
    }

    private PinRelation ResolveCollinearRelation()
    {
        if (RecessiveSide.DirectionSign == 0 || DominantSide.DirectionSign == 0)
        {
            return PinRelation.Ordered;
        }

        return RecessiveSide.DirectionSign == DominantSide.DirectionSign
            ? PinRelation.CollinearSame
            : PinRelation.CollinearOpposed;
    }

    private PinRelation ResolveOrthogonalRelation() =>
        RecessiveSide.DirectionSign * DominantSide.DirectionSign >= 0
            ? PinRelation.OrthogonalDirect
            : PinRelation.OrthogonalMirrored;
}
