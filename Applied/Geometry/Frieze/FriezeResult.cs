using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.Frieze;

public sealed record FriezeResult(
    FriezePattern Pattern,
    Proportion RepeatCountValue,
    IReadOnlyList<PlanarPathEdge> Segments,
    PlanarPoint Cursor,
    Area Bounds)
{
    public FriezeResult(
        FriezePattern pattern,
        int repeatCount,
        IReadOnlyList<PlanarPathEdge> segments,
        PlanarPoint cursor,
        int minX,
        int maxX,
        int minY,
        int maxY)
        : this(
            pattern,
            new Proportion(repeatCount),
            segments,
            cursor,
            new Area(Axis.FromCoordinates(minX, maxX), Axis.FromCoordinates(minY, maxY)))
    {
    }

    public Axis HorizontalBounds => Bounds.Recessive;
    public Axis VerticalBounds => Bounds.Dominant;
    public int RepeatCount => PlanarValueConverter.ToInt(RepeatCountValue);
    public int MinX => PlanarValueConverter.ToInt(HorizontalBounds.Left);
    public int MaxX => PlanarValueConverter.ToInt(HorizontalBounds.Right);
    public int MinY => PlanarValueConverter.ToInt(VerticalBounds.Left);
    public int MaxY => PlanarValueConverter.ToInt(VerticalBounds.Right);
    public int HorizontalSpan => PlanarValueConverter.ToInt(HorizontalBounds.Span);
    public int VerticalSpan => PlanarValueConverter.ToInt(VerticalBounds.Span);
    public int NetX => Cursor.X;
    public int NetY => Cursor.Y;
    public bool FlowsLeft => NetX < 0;
}
