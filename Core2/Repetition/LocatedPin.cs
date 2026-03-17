using Core2.Elements;

namespace Core2.Repetition;

public sealed class LocatedPin
{
    public LocatedPin(
        Proportion location,
        Axis descriptor,
        IReadOnlyList<PinEgress>? outputs = null,
        bool absorbs = false,
        string? name = null)
    {
        Location = location;
        Descriptor = descriptor;
        Outputs = outputs ?? [];
        Absorbs = absorbs;
        Name = name;
    }

    public Proportion Location { get; }
    public Axis Descriptor { get; }
    public IReadOnlyList<PinEgress> Outputs { get; }
    public bool Absorbs { get; }
    public string? Name { get; }

    public int OutputCount => Outputs.Count;
    public PinEgress? PrimaryOutput => Outputs.Count > 0 ? Outputs[0] : null;
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
