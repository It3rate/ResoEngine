namespace Core2.Geometry;

public sealed record StripOrnamentResult(
    StripOrnamentPattern Pattern,
    int RepeatCount,
    IReadOnlyList<StripPathEdge> Segments,
    StripPoint Cursor,
    int MinX,
    int MaxX,
    int MinY,
    int MaxY);
