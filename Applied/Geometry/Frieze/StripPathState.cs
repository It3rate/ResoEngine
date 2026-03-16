namespace Applied.Geometry.Frieze;

public sealed record StripPathState(
    StripPoint Cursor,
    IReadOnlyList<StripPathEdge> Segments,
    int MacroStep)
{
    public static StripPathState Origin { get; } = new(new StripPoint(0, 0), [], 0);
}
