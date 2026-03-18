using Core2.Elements;

namespace Core2.Repetition;

public sealed class LocatedPin
{
    public LocatedPin(
        Proportion location,
        Axis applied,
        IReadOnlyList<PinEgress>? outputs = null,
        bool absorbs = false,
        string? name = null)
    {
        Location = location;
        Applied = applied;
        Outputs = outputs ?? [];
        Absorbs = absorbs;
        Name = name;
    }

    public Proportion Location { get; }
    public Axis Applied { get; }
    public Axis Descriptor => Applied;
    public IReadOnlyList<PinEgress> Outputs { get; }
    public bool Absorbs { get; }
    public string? Name { get; }

    public int OutputCount => Outputs.Count;
    public PinEgress? PrimaryOutput => Outputs.Count > 0 ? Outputs[0] : null;

    public PointPinning<Axis, Axis> AttachTo(Axis host) => host.PinAt(Applied, Location);

    public PositionedAxis PlaceApplied() => Applied.PlaceAt(Location);

    public bool IsHostRelativeTo(Axis host) => Location >= host.LeftCoordinate && Location <= host.RightCoordinate;

    public LocatedPinTraversalResolution ResolveImplicitTraversal(
        Axis? host,
        int currentDirection,
        bool boundaryEncounter = false)
    {
        if (host is null || !IsHostRelativeTo(host))
        {
            return LocatedPinTraversalResolution.Unhandled;
        }

        var placed = PlaceApplied();
        var response = placed.ResolveCarrierResponse(currentDirection, host: host, boundaryEncounter: boundaryEncounter);

        if (response.IsTransparent)
        {
            return LocatedPinTraversalResolution.Transparent();
        }

        if (response.IsRedirect)
        {
            return LocatedPinTraversalResolution.Redirect(
                new PinEgress(Location, response.EncounterNextDirection, name: $"Implicit({Name ?? Location.ToString()})"));
        }

        if (response.HasNoTravelOnCurrentCarrier)
        {
            return LocatedPinTraversalResolution.Absorb(
                new RepetitionTension(
                    RepetitionTensionKind.PinBehaviorDeferred,
                    $"Pin '{Name ?? Location.ToString()}' has no realized travel on the current carrier in the encountered direction."));
        }

        if (response.ResolvesOffCurrentCarrier)
        {
            return LocatedPinTraversalResolution.Absorb(
                new RepetitionTension(
                    RepetitionTensionKind.PinBehaviorDeferred,
                    $"Pin '{Name ?? Location.ToString()}' resolves off the current carrier on its {response.EncounteredSide.Role} side and cannot yet be executed by 1D traversal."));
        }

        return LocatedPinTraversalResolution.Absorb(
            new RepetitionTension(
                RepetitionTensionKind.PinBehaviorDeferred,
                $"Pin '{Name ?? Location.ToString()}' remains unresolved on the current carrier and cannot yet be executed by 1D traversal."));
    }
}

public readonly record struct LocatedPinTraversalResolution(
    bool Handled,
    bool TransparentContinue,
    bool Absorbs,
    PinEgress? PrimaryOutput,
    IReadOnlyList<RepetitionTension> Tensions)
{
    public static LocatedPinTraversalResolution Unhandled =>
        new(false, false, false, null, []);

    public static LocatedPinTraversalResolution Transparent(params RepetitionTension[] tensions) =>
        new(true, true, false, null, tensions);

    public static LocatedPinTraversalResolution Redirect(PinEgress egress, params RepetitionTension[] tensions) =>
        new(true, false, false, egress, tensions);

    public static LocatedPinTraversalResolution Absorb(params RepetitionTension[] tensions) =>
        new(true, false, true, null, tensions);
}

public sealed class PinEgress
{
    public PinEgress(
        Proportion start,
        int directionSign,
        BoundaryPinPair? context = null,
        string? name = null,
        bool preservesCurrentContext = true)
    {
        Start = start;
        DirectionSign = Math.Sign(directionSign);
        Context = context;
        Name = name;
        PreservesCurrentContext = preservesCurrentContext;
    }

    public Proportion Start { get; }
    public int DirectionSign { get; }
    public BoundaryPinPair? Context { get; }
    public string? Name { get; }
    public bool PreservesCurrentContext { get; }

    public Axis? Frame => Context?.Frame;
    public bool IsUnbounded => Context is null && !PreservesCurrentContext;
}
