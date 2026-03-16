using Core2.Elements;

namespace ResoEngine.Visualizer.Adapt;

/// <summary>
/// Visual projection helpers derived from a pinned Area.
/// This keeps page-level area math tied to the Core2 Area/Axes model rather than
/// duplicating the same relationships ad hoc in the visualizer.
/// </summary>
public sealed class AreaDisplayGeometry
{
    public AreaDisplayGeometry(Area area)
    {
        Area = area;
    }

    public Area Area { get; }
    public int? RecessiveCarrierRank => Area.Recessive.PreferredCarrierRank;
    public int? DominantCarrierRank => Area.Dominant.PreferredCarrierRank;
    public bool UsesResolvedOrthogonalProjection =>
        RecessiveCarrierRank.HasValue &&
        DominantCarrierRank.HasValue &&
        RecessiveCarrierRank != DominantCarrierRank;

    public Axis Horizontal =>
        UsesResolvedOrthogonalProjection
            ? (RecessiveCarrierRank == 0 ? Area.Recessive : Area.Dominant)
            : Area.Recessive;

    public Axis Vertical =>
        UsesResolvedOrthogonalProjection
            ? (RecessiveCarrierRank == 1 ? Area.Recessive : Area.Dominant)
            : Area.Dominant;
    public AreaQuadrants Quadrants => Area.Quadrants;

    public float HorizontalImaginary => ToFloat(Horizontal.Start);
    public float HorizontalReal => ToFloat(Horizontal.End);
    public float VerticalImaginary => ToFloat(Vertical.Start);
    public float VerticalReal => ToFloat(Vertical.End);

    public float WorldMinX => MathF.Min(0f, MathF.Min(HorizontalImaginary, HorizontalReal));
    public float WorldMaxX => MathF.Max(0f, MathF.Max(HorizontalImaginary, HorizontalReal));
    public float WorldMinY => MathF.Min(0f, MathF.Min(VerticalImaginary, VerticalReal));
    public float WorldMaxY => MathF.Max(0f, MathF.Max(VerticalImaginary, VerticalReal));

    public float FrameMin => MathF.Min(0f, MathF.Min(MathF.Min(HorizontalImaginary, HorizontalReal), MathF.Min(VerticalImaginary, VerticalReal)));
    public float FrameMax => MathF.Max(0f, MathF.Max(MathF.Max(HorizontalImaginary, HorizontalReal), MathF.Max(VerticalImaginary, VerticalReal)));

    public IReadOnlyList<float> OrthogonalXBoundaries => DistinctSorted([0f, HorizontalImaginary, HorizontalReal]);
    public IReadOnlyList<float> OrthogonalYBoundaries => DistinctSorted([0f, VerticalImaginary, VerticalReal]);
    public IReadOnlyList<float> ParallelBoundaries => DistinctSorted([FrameMin, HorizontalImaginary, HorizontalReal, 0f, VerticalImaginary, VerticalReal, FrameMax]);

    public bool InHorizontalImag(float value) => IsWithin(value, 0f, HorizontalImaginary);
    public bool InHorizontalReal(float value) => IsWithin(value, 0f, HorizontalReal);
    public bool InVerticalImag(float value) => IsWithin(value, 0f, VerticalImaginary);
    public bool InVerticalReal(float value) => IsWithin(value, 0f, VerticalReal);

    public bool InHorizontal(float value)
    {
        float min = MathF.Min(HorizontalImaginary, HorizontalReal);
        float max = MathF.Max(HorizontalImaginary, HorizontalReal);
        return value > min && value < max;
    }

    public bool InVertical(float value)
    {
        float min = MathF.Min(VerticalImaginary, VerticalReal);
        float max = MathF.Max(VerticalImaginary, VerticalReal);
        return value > min && value < max;
    }

    public AxisRange HorizontalImagRange => CreateRange(0f, HorizontalImaginary);
    public AxisRange HorizontalRealRange => CreateRange(0f, HorizontalReal);
    public AxisRange VerticalImagRange => CreateRange(0f, VerticalImaginary);
    public AxisRange VerticalRealRange => CreateRange(0f, VerticalReal);

    public static AxisRange CreateRange(float start, float end) => new(MathF.Min(start, end), MathF.Max(start, end));

    private static float ToFloat(Scalar value) => (float)(decimal)value;

    private static IReadOnlyList<float> DistinctSorted(IEnumerable<float> values)
    {
        const float epsilon = 0.0001f;

        return values
            .OrderBy(value => value)
            .Aggregate(
                new List<float>(),
                (list, value) =>
                {
                    if (list.Count == 0 || MathF.Abs(list[^1] - value) > epsilon)
                    {
                        list.Add(value);
                    }

                    return list;
                });
    }

    private static bool IsWithin(float value, float first, float second)
    {
        float min = MathF.Min(first, second);
        float max = MathF.Max(first, second);
        return value > min && value < max;
    }

    public readonly record struct AxisRange(float Min, float Max)
    {
        public bool HasSpan => Max > Min;
    }
}
