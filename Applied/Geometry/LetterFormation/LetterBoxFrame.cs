using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public enum AxisSectionKind
{
    Start,
    Early,
    Middle,
    Late,
    End,
}

[Flags]
public enum AxisSectionMask
{
    None = 0,
    Start = 1 << 0,
    Early = 1 << 1,
    Middle = 1 << 2,
    Late = 1 << 3,
    End = 1 << 4,
    Tips = Start | End,
    Inside = Early | Middle | Late,
    All = Start | Early | Middle | Late | End,
}

public readonly record struct AxisSectionWindow(
    Proportion Start,
    Proportion End)
{
    public Proportion Representative => (Start + End) / new Proportion(2);
    public bool IsPoint => Start == End;

    public Axis ToOriginAxis(Proportion origin) =>
        Axis.FromCoordinates(Start - origin, End - origin);
}

public static class AxisSectionKindExtensions
{
    public static AxisSectionWindow ResolveWindow(
        this AxisSectionKind section,
        long startValue,
        long middleValue,
        long endValue)
    {
        Validate(startValue, middleValue, endValue);

        return section switch
        {
            AxisSectionKind.Start => Point(startValue, endValue),
            AxisSectionKind.Early => InteriorBand(startValue, middleValue, endValue),
            AxisSectionKind.Middle => Point(middleValue, endValue),
            AxisSectionKind.Late => InteriorBand(middleValue, endValue, endValue),
            AxisSectionKind.End => Point(endValue, endValue),
            _ => Point(middleValue, endValue),
        };
    }

    public static Proportion ResolveRepresentative(
        this AxisSectionKind section,
        long startValue,
        long middleValue,
        long endValue) =>
        section.ResolveWindow(startValue, middleValue, endValue).Representative;

    public static Axis ResolveOriginAxis(
        this AxisSectionKind section,
        long startValue,
        long middleValue,
        long endValue) =>
        section.ResolveWindow(startValue, middleValue, endValue)
            .ToOriginAxis(new Proportion(middleValue, endValue));

    private static AxisSectionWindow Point(long value, long denominator) =>
        new(new Proportion(value, denominator), new Proportion(value, denominator));

    private static AxisSectionWindow InteriorBand(long bandStart, long bandEnd, long denominator)
    {
        long start = bandStart + 1;
        long end = bandEnd - 1;
        if (start > end)
        {
            long fallback = (bandStart + bandEnd) / 2;
            return Point(fallback, denominator);
        }

        return new AxisSectionWindow(
            new Proportion(start, denominator),
            new Proportion(end, denominator));
    }

    private static void Validate(long startValue, long middleValue, long endValue)
    {
        if (endValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(endValue), "End value must be positive.");
        }

        if (startValue < 0 || startValue > middleValue || middleValue > endValue)
        {
            throw new ArgumentException("Section anchors must satisfy start <= middle <= end with nonnegative values.");
        }
    }
}

public sealed record AxisSectionCalibration(
    long StartValue,
    long MiddleValue,
    long EndValue)
{
    public long Resolution => EndValue;

    public AxisSectionWindow Resolve(AxisSectionKind section) =>
        section.ResolveWindow(StartValue, MiddleValue, EndValue);

    public Proportion Representative(AxisSectionKind section) =>
        section.ResolveRepresentative(StartValue, MiddleValue, EndValue);

    public Axis OriginAxis(AxisSectionKind section) =>
        section.ResolveOriginAxis(StartValue, MiddleValue, EndValue);

    public Axis FullAxis() =>
        Axis.FromCoordinates(
            new Proportion(StartValue - MiddleValue, EndValue),
            new Proportion(EndValue - MiddleValue, EndValue));

    public Axis Between(AxisSectionKind from, AxisSectionKind to)
    {
        Proportion origin = new Proportion(MiddleValue, EndValue);
        return Axis.FromCoordinates(
            Representative(from) - origin,
            Representative(to) - origin);
    }
}

public static class AxisSectionCalibrationCatalog
{
    public static AxisSectionCalibration RomanUppercase { get; } = new(0, 4, 8);

    public static AxisSectionCalibration HighMidlineUppercase { get; } = new(0, 5, 8);
}

public sealed record LetterBoxFrameAxis(
    string Id,
    bool IsHorizontal,
    Axis Span,
    AxisSectionKind FixedSection,
    AxisSectionKind FromPosition,
    AxisSectionKind ToPosition);

public sealed record LetterBoxFrame(
    string Id,
    AxisSectionCalibration HorizontalCalibration,
    AxisSectionCalibration VerticalCalibration,
    IReadOnlyList<LetterBoxFrameAxis> Axes)
{
    public LetterBoxFrameAxis GetAxis(string id) =>
        Axes.FirstOrDefault(axis => string.Equals(axis.Id, id, StringComparison.Ordinal))
        ?? throw new KeyNotFoundException($"Unknown letter box axis '{id}'.");

    public PlanarPoint ResolvePoint(string axisId, AxisSectionKind positionOnAxis)
    {
        LetterBoxFrameAxis axis = GetAxis(axisId);
        return axis.IsHorizontal
            ? new PlanarPoint(
                HorizontalCalibration.Representative(positionOnAxis),
                VerticalCalibration.Representative(axis.FixedSection))
            : new PlanarPoint(
                HorizontalCalibration.Representative(axis.FixedSection),
                VerticalCalibration.Representative(positionOnAxis));
    }

    public (PlanarPoint From, PlanarPoint To) ResolveEndpoints(LetterBoxFrameAxis axis) =>
        axis.IsHorizontal
            ? (
                new PlanarPoint(
                    HorizontalCalibration.Representative(axis.FromPosition),
                    VerticalCalibration.Representative(axis.FixedSection)),
                new PlanarPoint(
                    HorizontalCalibration.Representative(axis.ToPosition),
                    VerticalCalibration.Representative(axis.FixedSection)))
            : (
                new PlanarPoint(
                    HorizontalCalibration.Representative(axis.FixedSection),
                    VerticalCalibration.Representative(axis.FromPosition)),
                new PlanarPoint(
                    HorizontalCalibration.Representative(axis.FixedSection),
                    VerticalCalibration.Representative(axis.ToPosition)));
}

public static class LetterBoxFrameCatalog
{
    public static LetterBoxFrame RomanUppercase { get; } = Create(
        "RomanUppercase",
        AxisSectionCalibrationCatalog.RomanUppercase,
        AxisSectionCalibrationCatalog.RomanUppercase);

    public static LetterBoxFrame HighMidlineUppercase { get; } = Create(
        "HighMidlineUppercase",
        AxisSectionCalibrationCatalog.RomanUppercase,
        AxisSectionCalibrationCatalog.HighMidlineUppercase);

    private static LetterBoxFrame Create(
        string id,
        AxisSectionCalibration horizontal,
        AxisSectionCalibration vertical) =>
        new(
            id,
            horizontal,
            vertical,
            [
                HorizontalAxis("Topline", horizontal, vertical, AxisSectionKind.End),
                HorizontalAxis("Baseline", horizontal, vertical, AxisSectionKind.Start),
                HorizontalAxis("Midline", horizontal, vertical, AxisSectionKind.Middle),
                VerticalAxis("LeftSide", horizontal, vertical, AxisSectionKind.Start),
                VerticalAxis("RightSide", horizontal, vertical, AxisSectionKind.End),
                VerticalAxis("Centerline", horizontal, vertical, AxisSectionKind.Middle),
            ]);

    private static LetterBoxFrameAxis HorizontalAxis(
        string id,
        AxisSectionCalibration horizontal,
        AxisSectionCalibration vertical,
        AxisSectionKind fixedVertical) =>
        new(
            id,
            true,
            horizontal.Between(AxisSectionKind.Start, AxisSectionKind.End),
            fixedVertical,
            AxisSectionKind.Start,
            AxisSectionKind.End);

    private static LetterBoxFrameAxis VerticalAxis(
        string id,
        AxisSectionCalibration horizontal,
        AxisSectionCalibration vertical,
        AxisSectionKind fixedHorizontal) =>
        new(
            id,
            false,
            vertical.Between(AxisSectionKind.Start, AxisSectionKind.End),
            fixedHorizontal,
            AxisSectionKind.Start,
            AxisSectionKind.End);
}
