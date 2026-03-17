using Core2.Elements;
using SkiaSharp;

namespace ResoEngine.Visualizer.Adapt;

public sealed class PinAxisDisplayGeometry
{
    public PinAxisDisplayGeometry(Axis descriptor)
    {
        Descriptor = descriptor;
        Resolution = descriptor.PinResolution;
        RecessiveRay = CreateRay("Recessive", descriptor.Recessive, Resolution.RecessiveSide);
        DominantRay = CreateRay("Dominant", descriptor.Dominant, Resolution.DominantSide);
    }

    public Axis Descriptor { get; }
    public PinAxisResolution Resolution { get; }
    public PinRelation Relation => Descriptor.Relation;
    public PinDisplayRay RecessiveRay { get; }
    public PinDisplayRay DominantRay { get; }
    public bool IsDirectedSegment => Descriptor.IsSegmentLike;
    public bool IsSequentialReinforcement => Descriptor.IsSequentialReinforcement;
    public bool IsOrthogonalStructure => Descriptor.IsOrthogonalStructure;
    public bool HasUnresolvedCarrier => Resolution.HasUnresolvedCarrier;
    public bool HasDerivedCell =>
        RecessiveRay.HasEndpoint &&
        DominantRay.HasEndpoint &&
        RecessiveRay.CarrierRank.HasValue &&
        DominantRay.CarrierRank.HasValue &&
        RecessiveRay.CarrierRank != DominantRay.CarrierRank;

    public SKPoint SegmentStart => RecessiveRay.Endpoint;
    public SKPoint SegmentEnd => DominantRay.Endpoint;
    public SKPoint SequentialMidpoint => RecessiveRay.Endpoint;
    public SKPoint SequentialEnd => Add(RecessiveRay.Endpoint, DominantRay.Endpoint);
    public SKPoint RecessiveBasis => ResolveBasis(RecessiveRay);
    public SKPoint DominantBasis => ResolveBasis(DominantRay);

    public IReadOnlyList<SKPoint> DerivedCellCorners =>
        HasDerivedCell
            ? [SKPoint.Empty, RecessiveRay.Endpoint, SequentialEnd, DominantRay.Endpoint]
            : [];

    public float SuggestedExtent
    {
        get
        {
            float extent = 1f;

            foreach (var point in EnumeratePoints())
            {
                extent = MathF.Max(extent, MathF.Abs(point.X));
                extent = MathF.Max(extent, MathF.Abs(point.Y));
            }

            return extent;
        }
    }

    private IEnumerable<SKPoint> EnumeratePoints()
    {
        yield return RecessiveRay.Endpoint;
        yield return DominantRay.Endpoint;

        if (IsSequentialReinforcement)
        {
            yield return SequentialEnd;
        }

        if (HasDerivedCell)
        {
            foreach (var point in DerivedCellCorners)
            {
                yield return point;
            }
        }
    }

    private static PinDisplayRay CreateRay(string name, Proportion proportion, PinResolvedSide side)
    {
        float magnitude = side.IsUnresolved
            ? Math.Abs(side.ValueEncoding)
            : SafeMagnitude(proportion);

        if (side.DirectionSign == 0 || magnitude <= 0.0001f)
        {
            return new PinDisplayRay(name, side.CarrierRank, side.DirectionSign, magnitude, SKPoint.Empty, side.IsUnresolved, side.IsLifted);
        }

        float signedMagnitude = side.DirectionSign * magnitude;
        var endpoint = side.CarrierRank switch
        {
            0 => new SKPoint(signedMagnitude, 0f),
            1 => new SKPoint(0f, signedMagnitude),
            _ => SKPoint.Empty,
        };

        return new PinDisplayRay(name, side.CarrierRank, side.DirectionSign, magnitude, endpoint, side.IsUnresolved, side.IsLifted);
    }

    private static float SafeMagnitude(Proportion value)
    {
        decimal folded = value.Fold();
        if (folded == decimal.MaxValue || folded == decimal.MinValue)
        {
            return Math.Abs(value.Dominant);
        }

        return Math.Abs((float)folded);
    }

    private static SKPoint Add(SKPoint left, SKPoint right) =>
        new(left.X + right.X, left.Y + right.Y);

    private static SKPoint ResolveBasis(PinDisplayRay ray)
    {
        if (ray.HasEndpoint && ray.Magnitude > 0.0001f)
        {
            return new(ray.Endpoint.X / ray.Magnitude, ray.Endpoint.Y / ray.Magnitude);
        }

        return ray.CarrierRank switch
        {
            0 => new(ray.DirectionSign == 0 ? 1f : ray.DirectionSign, 0f),
            1 => new(0f, ray.DirectionSign == 0 ? 1f : ray.DirectionSign),
            _ => SKPoint.Empty,
        };
    }
}

public readonly record struct PinDisplayRay(
    string Name,
    int? CarrierRank,
    int DirectionSign,
    float Magnitude,
    SKPoint Endpoint,
    bool IsUnresolved,
    bool IsLifted)
{
    public bool HasEndpoint => !IsUnresolved && CarrierRank.HasValue && DirectionSign != 0 && Magnitude > 0.0001f;
}
