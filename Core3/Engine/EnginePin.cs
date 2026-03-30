namespace Core3.Engine;

/// <summary>
/// A generic pin relation.
/// It may either split one hosted carrier/object at a position, or explicitly
/// join an inbound side to an outbound side.
/// </summary>
public sealed record EnginePin
{
    public EnginePin(CompositeElement host, GradedElement pinPosition)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(pinPosition);

        if (!TryResolveHostedPin(host, pinPosition, out var resolvedPosition, out var inbound, out var outbound))
        {
            throw new InvalidOperationException("Hosted pins require either a compatible host-local position or an exact ratio-bearing position that can scale onto the host.");
        }

        Host = host;
        PinPosition = pinPosition;
        ResolvedPosition = resolvedPosition;
        Inbound = inbound;
        Outbound = outbound;
    }

    public EnginePin(GradedElement inbound, GradedElement outbound)
    {
        ArgumentNullException.ThrowIfNull(inbound);
        ArgumentNullException.ThrowIfNull(outbound);

        RequireSameGrade(inbound, outbound);

        Inbound = inbound;
        Outbound = outbound;
    }

    public CompositeElement? Host { get; }
    public GradedElement? PinPosition { get; }
    public GradedElement? ResolvedPosition { get; }
    public GradedElement Inbound { get; }
    public GradedElement Outbound { get; }
    public int Grade => Inbound.Grade + 1;

    public bool HasResolvedUnits => Inbound.HasResolvedUnits && Outbound.HasResolvedUnits;
    public bool SharesUnitSpace => Inbound.SharesUnitSpace(Outbound);
    public bool HasContrastSpace => HasResolvedUnits && !SharesUnitSpace;
    public GradedElement? DeclaredSpan => TryGetDeclaredSpan(out var declaredSpan) ? declaredSpan : null;
    public GradedElement? InboundTension => Host is not null ? Outbound : null;
    public GradedElement? OutboundTension => TryGetOutboundTension(out var tension) ? tension : null;

    public GradedElement? Add() =>
        Inbound.TryAdd(Outbound, out var sum)
            ? sum
            : null;

    public GradedElement? Multiply() =>
        HasContrastSpace
            ? Contrast()
            : null;

    public CompositeElement Contrast() => new(Inbound, Outbound);

    public bool MultiplyRequiresLift() => HasResolvedUnits && SharesUnitSpace;

    public EnginePin SlideTo(GradedElement pinPosition)
    {
        if (Host is null)
        {
            throw new InvalidOperationException("Only hosted pins can be slid to a new position.");
        }

        return new EnginePin(Host, pinPosition);
    }

    public bool TrySlideBy(GradedElement offset, out EnginePin? shifted)
    {
        if (Host is not null &&
            ResolvedPosition is not null &&
            ResolvedPosition.TryAdd(offset, out var shiftedPosition) &&
            shiftedPosition is not null)
        {
            shifted = new EnginePin(Host, shiftedPosition);
            return true;
        }

        shifted = null;
        return false;
    }

    public override string ToString() => $"pin(in {Inbound}, out {Outbound})";

    public static EngineHostedPinResult ResolveHostedWithTension(
        CompositeElement host,
        GradedElement pinPosition)
    {
        if (TryResolveHostedPin(host, pinPosition, out var resolvedPosition, out var inbound, out var outbound) &&
            resolvedPosition is not null)
        {
            return new EngineHostedPinResult(
                host,
                pinPosition,
                resolvedPosition,
                inbound,
                outbound);
        }

        var positionOutcome = ResolvePositionWithTension(host, pinPosition);
        var inboundOutcome = positionOutcome.Result.SubtractWithTension(host.Recessive);
        var outboundOutcome = host.Dominant.SubtractWithTension(positionOutcome.Result);
        var note = EngineTension.CombineNotes(positionOutcome.Note, inboundOutcome.Note, outboundOutcome.Note);

        return new EngineHostedPinResult(
            host,
            pinPosition,
            positionOutcome.Result,
            inboundOutcome.Result,
            outboundOutcome.Result,
            positionOutcome.Tension ?? inboundOutcome.Tension ?? outboundOutcome.Tension ?? pinPosition,
            note ?? "Hosted pin preserved unresolved placement.");
    }

    private bool TryGetDeclaredSpan(out GradedElement? declaredSpan)
    {
        if (Host is not null &&
            Host.Dominant.TrySubtract(Host.Recessive, out declaredSpan) &&
            declaredSpan is not null)
        {
            return true;
        }

        declaredSpan = null;
        return false;
    }

    private bool TryGetOutboundTension(out GradedElement? tension)
    {
        if (TryGetDeclaredSpan(out var declaredSpan) &&
            declaredSpan is not null &&
            declaredSpan.SharesUnitSpace(Outbound))
        {
            tension = declaredSpan;
            return true;
        }

        tension = null;
        return false;
    }

    private static bool TryResolveHostedPin(
        CompositeElement host,
        GradedElement pinPosition,
        out GradedElement? resolvedPosition,
        out GradedElement inbound,
        out GradedElement outbound)
    {
        if (TryResolveHostedSides(host, pinPosition, out inbound, out outbound))
        {
            resolvedPosition = pinPosition;
            return true;
        }

        if (TryResolvePositionFromRatio(host, pinPosition, out resolvedPosition) &&
            resolvedPosition is not null &&
            TryResolveHostedSides(host, resolvedPosition, out inbound, out outbound))
        {
            return true;
        }

        resolvedPosition = null;
        inbound = null!;
        outbound = null!;
        return false;
    }

    private static bool TryResolveHostedSides(
        CompositeElement host,
        GradedElement position,
        out GradedElement inbound,
        out GradedElement outbound)
    {
        if (position.Grade == host.Recessive.Grade &&
            position.TrySubtract(host.Recessive, out var inboundSide) &&
            host.Dominant.TrySubtract(position, out var outboundSide) &&
            inboundSide is not null &&
            outboundSide is not null)
        {
            inbound = inboundSide;
            outbound = outboundSide;
            return true;
        }

        inbound = null!;
        outbound = null!;
        return false;
    }

    private static EngineElementOutcome ResolvePositionWithTension(
        CompositeElement host,
        GradedElement pinPosition)
    {
        if (pinPosition.Grade == host.Recessive.Grade)
        {
            return EngineElementOutcome.Exact(pinPosition);
        }

        var folded = pinPosition.Fold();

        if (folded.Result is not AtomicElement ratio)
        {
            return EngineElementOutcome.WithTension(
                pinPosition,
                pinPosition,
                "Hosted pin preserved the requested position because it could not be folded to an atomic ratio.");
        }

        if (!ratio.IsAlignedUnit)
        {
            return EngineElementOutcome.WithTension(
                ratio,
                folded.Tension ?? pinPosition,
                "Hosted pin preserved a contrastive or unresolved ratio position.");
        }

        var declaredSpan = host.Dominant.SubtractWithTension(host.Recessive);
        var offset = declaredSpan.Result.ScaleWithTension(ratio);
        var positioned = host.Recessive.AddWithTension(offset.Result);
        var note = EngineTension.CombineNotes(folded.Note, declaredSpan.Note, offset.Note, positioned.Note);

        return positioned.IsExact && folded.IsExact && declaredSpan.IsExact && offset.IsExact
            ? EngineElementOutcome.Exact(positioned.Result)
            : EngineElementOutcome.WithTension(
                positioned.Result,
                folded.Tension ?? declaredSpan.Tension ?? offset.Tension ?? positioned.Tension ?? pinPosition,
                note ?? "Hosted pin preserved unresolved position from ratio scaling.");
    }

    private static bool TryResolvePositionFromRatio(
        CompositeElement host,
        GradedElement pinPosition,
        out GradedElement? resolvedPosition)
    {
        if (!EngineEvaluation.TryFoldToAtomic(pinPosition, out var ratio) ||
            ratio is null ||
            !ratio.IsAlignedUnit)
        {
            resolvedPosition = null;
            return false;
        }

        if (host.Dominant.TrySubtract(host.Recessive, out var declaredSpan) &&
            declaredSpan is not null &&
            declaredSpan.TryScale(ratio, out var offset) &&
            offset is not null &&
            host.Recessive.TryAdd(offset, out resolvedPosition) &&
            resolvedPosition is not null)
        {
            return true;
        }

        resolvedPosition = null;
        return false;
    }

    private static void RequireSameGrade(GradedElement left, GradedElement right)
    {
        if (left.Grade != right.Grade)
        {
            throw new InvalidOperationException("Pins require children of the same grade.");
        }
    }
}
