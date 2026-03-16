namespace Applied.Geometry.Frieze;

public sealed record StripOrnamentResult(
    StripOrnamentPattern Pattern,
    int RepeatCount,
    IReadOnlyList<StripPathEdge> Segments,
    StripPoint Cursor,
    int MinX,
    int MaxX,
    int MinY,
    int MaxY)
{
    public int HorizontalSpan => MaxX - MinX;
    public int VerticalSpan => MaxY - MinY;
    public int NetX => Cursor.X;
    public int NetY => Cursor.Y;
    public bool FlowsLeft => NetX < 0;
}
