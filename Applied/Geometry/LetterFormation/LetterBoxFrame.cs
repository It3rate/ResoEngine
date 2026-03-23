using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

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
                HorizontalAxis("Topline", horizontal, vertical, AxisSectionKind.Start),
                HorizontalAxis("Baseline", horizontal, vertical, AxisSectionKind.End),
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
            vertical.Between(AxisSectionKind.End, AxisSectionKind.Start),
            fixedHorizontal,
            AxisSectionKind.End,
            AxisSectionKind.Start);
}
