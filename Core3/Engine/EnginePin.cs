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

    public GradedElement Inbound => Recessive.InvertPerspective();
    public GradedElement Outbound => Dominant;
    public bool HasResolvedUnits => Inbound.HasResolvedUnits && Outbound.HasResolvedUnits;
    public bool SharesUnitSpace => Inbound.SharesUnitSpace(Outbound);
    public bool HasContrastSpace => HasResolvedUnits && !SharesUnitSpace;

    public GradedElement? Add() =>
        Inbound.TryAdd(Outbound, out var sum)
            ? sum
            : null;

    public CompositeElement Contrast() => new(Inbound, Outbound);

    public bool MultiplyRequiresLift() => HasResolvedUnits && SharesUnitSpace;

    public override string ToString() => $"pin(in {Inbound}, out {Outbound})";
}
