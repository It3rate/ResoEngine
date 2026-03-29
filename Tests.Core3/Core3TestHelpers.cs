using Core3.Engine;

namespace Tests.Core3;

internal static class Core3TestHelpers
{
    internal static CompositeElement CreateAxisLikeNumber(
        long recessiveValue,
        long recessiveResolution,
        long dominantValue,
        long dominantResolution) =>
        new(
            CreateExactScalar(recessiveValue, recessiveResolution),
            CreateExactScalar(dominantValue, dominantResolution));

    internal static CompositeElement CreateExactScalar(long value, long resolution) =>
        new(
            new AtomicElement(resolution, 1),
            new AtomicElement(value, 1));

    internal static CompositeElement CreateSegmentFrame(long start, long end, long resolution) =>
        new(
            new AtomicElement(start, resolution),
            new AtomicElement(end, resolution));

    internal static double ToDouble(AtomicElement atomic) =>
        (double)atomic.Value / atomic.Unit;
}
