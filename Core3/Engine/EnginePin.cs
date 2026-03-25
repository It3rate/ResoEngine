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

    public NormalizedPin Normalize() => new(
        Recessive.Mirror(),
        Dominant,
        Position);

    public override string ToString() => $"pin({Recessive}, {Dominant})";
}
