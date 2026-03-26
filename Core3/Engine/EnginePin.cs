namespace Core3.Engine;

/// <summary>
/// Generic pin relation between two equal-grade children.
/// The pin is a relation first; folding is decided later.
/// </summary>
public sealed record EnginePin
{
    public EnginePin(GradedElement recessive, GradedElement dominant, GradedElement? position = null)
    {
        ArgumentNullException.ThrowIfNull(recessive);
        ArgumentNullException.ThrowIfNull(dominant);

        if (recessive.Grade != dominant.Grade)
        {
            throw new InvalidOperationException("Pins require children of the same grade.");
        }

        if (position is not null && position.Grade > recessive.Grade)
        {
            throw new InvalidOperationException("Pin position cannot have a higher grade than the pinned children.");
        }

        Recessive = recessive;
        Dominant = dominant;
        Position = position;
    }

    public GradedElement Recessive { get; }
    public GradedElement Dominant { get; }
    public GradedElement? Position { get; }
    public int Grade => Recessive.Grade + 1;

    /// <summary>
    /// The generic contrastive inbound read used for fold classification.
    /// This differs from the host-local inbound side derived from a positioned
    /// host.
    /// </summary>
    public GradedElement Inbound => Recessive.InvertPerspective();
    public GradedElement Outbound => Dominant;
    public bool HasResolvedUnits => Inbound.HasResolvedUnits && Outbound.HasResolvedUnits;
    public bool SharesUnitSpace => Inbound.SharesUnitSpace(Outbound);
    public bool HasContrastSpace => HasResolvedUnits && !SharesUnitSpace;
    public GradedElement? ResolvedPosition => TryResolvePosition(out var resolvedPosition) ? resolvedPosition : null;
    public GradedElement? DeclaredSpan => TryGetDeclaredSpan(out var declaredSpan) ? declaredSpan : null;
    public GradedElement? InboundSide => TryGetInboundSide(out var inboundSide) ? inboundSide : null;
    public GradedElement? OutboundSide => TryGetOutboundSide(out var outboundSide) ? outboundSide : null;
    public GradedElement? InboundTension => OutboundSide;
    public GradedElement? OutboundTension => TryGetOutboundTension(out var tension) ? tension : null;

    public GradedElement? Add() =>
        Inbound.TryAdd(Outbound, out var sum)
            ? sum
            : null;

    public CompositeElement Contrast() => new(Inbound, Outbound);

    public bool MultiplyRequiresLift() => HasResolvedUnits && SharesUnitSpace;

    public EnginePin SlideTo(GradedElement position) => new(Recessive, Dominant, position);

    public bool TrySlideBy(GradedElement offset, out EnginePin? shifted)
    {
        if (Position is not null &&
            Position.TryAdd(offset, out var shiftedPosition) &&
            shiftedPosition is not null)
        {
            shifted = new EnginePin(Recessive, Dominant, shiftedPosition);
            return true;
        }

        shifted = null;
        return false;
    }

    public override string ToString() => $"pin(in {Inbound}, out {Outbound})";

    private bool TryResolvePosition(out GradedElement? resolvedPosition)
    {
        if (TryGetHostFrame(out _, out var position))
        {
            resolvedPosition = position;
            return true;
        }

        resolvedPosition = null;
        return false;
    }

    private bool TryGetDeclaredSpan(out GradedElement? declaredSpan)
    {
        if (TryGetHostFrame(out var host, out _) &&
            host is not null &&
            host.Dominant.TrySubtract(host.Recessive, out declaredSpan) &&
            declaredSpan is not null)
        {
            return true;
        }

        declaredSpan = null;
        return false;
    }

    private bool TryGetInboundSide(out GradedElement? inboundSide)
    {
        if (TryGetHostFrame(out var host, out var position) &&
            host is not null &&
            position is not null &&
            position.TrySubtract(host.Recessive, out inboundSide) &&
            inboundSide is not null)
        {
            return true;
        }

        inboundSide = null;
        return false;
    }

    private bool TryGetOutboundSide(out GradedElement? outboundSide)
    {
        if (TryGetHostFrame(out var host, out var position) &&
            host is not null &&
            position is not null &&
            host.Dominant.TrySubtract(position, out outboundSide) &&
            outboundSide is not null)
        {
            return true;
        }

        outboundSide = null;
        return false;
    }

    private bool TryGetOutboundTension(out GradedElement? tension)
    {
        if (Dominant is CompositeElement applied &&
            TryGetDeclaredSpan(out var declaredSpan) &&
            declaredSpan is not null &&
            declaredSpan.SharesUnitSpace(applied.Dominant))
        {
            tension = declaredSpan;
            return true;
        }

        tension = null;
        return false;
    }

    private bool TryGetHostFrame(out CompositeElement? host, out GradedElement? position)
    {
        host = Recessive as CompositeElement;
        position = Position;

        return host is not null &&
            position is not null &&
            position.Grade == host.Recessive.Grade &&
            position.CanSubtract(host.Recessive) &&
            host.Dominant.CanSubtract(position);
    }
}
