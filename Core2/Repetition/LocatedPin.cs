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
        string pinName = Name ?? Location.ToString();
        List<RepetitionTension> tensions = [];

        if (response.BlocksApproachIntoEncounter)
        {
            tensions.Add(new RepetitionTension(
                RepetitionTensionKind.PinFlowBlocked,
                $"Pin '{pinName}' opposes approach into the encounter on the current carrier."));
        }

        if (response.IsTransparent)
        {
            if (response.BlocksContinuationPastEncounter)
            {
                tensions.Add(new RepetitionTension(
                    RepetitionTensionKind.PinFlowBlocked,
                    $"Pin '{pinName}' blocks continuation past the encounter on the current carrier."));
                return LocatedPinTraversalResolution.Absorb(tensions.ToArray());
            }

            return LocatedPinTraversalResolution.Transparent(tensions.ToArray());
        }

        if (response.IsRedirect)
        {
            return LocatedPinTraversalResolution.Redirect(
                new PinEgress(Location, response.EncounterNextDirection, name: $"Implicit({pinName})"),
                tensions.ToArray());
        }

        if (response.HasNoTravelOnCurrentCarrier)
        {
            tensions.Add(new RepetitionTension(
                RepetitionTensionKind.PinBehaviorDeferred,
                $"Pin '{pinName}' has no realized travel on the current carrier in the encountered direction."));
            return LocatedPinTraversalResolution.Absorb(tensions.ToArray());
        }

        if (response.ResolvesOffCurrentCarrier)
        {
            tensions.Add(new RepetitionTension(
                RepetitionTensionKind.PinBehaviorDeferred,
                $"Pin '{pinName}' resolves off the current carrier on its {response.EncounteredSide.Role} side and cannot yet be executed by 1D traversal."));
            return LocatedPinTraversalResolution.Absorb(tensions.ToArray());
        }

        tensions.Add(new RepetitionTension(
            RepetitionTensionKind.PinBehaviorDeferred,
            $"Pin '{pinName}' remains unresolved on the current carrier and cannot yet be executed by 1D traversal."));
        return LocatedPinTraversalResolution.Absorb(tensions.ToArray());
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
