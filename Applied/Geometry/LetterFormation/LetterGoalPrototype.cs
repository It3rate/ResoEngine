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

public sealed record PinAxisGoal(
    string PinId,
    string AxisId,
    AxisSectionKind PositionOnAxis);

public sealed record AxisPairGoal(
    string FromAxisId,
    AxisSectionKind FromPosition,
    string ToAxisId,
    AxisSectionKind ToPosition);

public sealed record LetterGoalPin(
    string Id,
    PinAxisGoal Placement,
    Axis Descriptor);

public sealed record LetterGoalPrototype(
    string Id,
    LetterBoxFrame Frame,
    IReadOnlyList<LetterGoalPin> Pins)
{
    public AxisSectionCalibration HorizontalCalibration => Frame.HorizontalCalibration;
    public AxisSectionCalibration VerticalCalibration => Frame.VerticalCalibration;
}

public static class LetterGoalPrototypeCatalog
{
    public static LetterGoalPrototype CapitalA { get; } = new(
        "LetterA",
        LetterBoxFrameCatalog.HighMidlineUppercase,
        [
            Pin("P1", "Baseline", AxisSectionKind.Early),
            Pin("P2", "Topline", AxisSectionKind.Middle),
            Pin("P3", "Baseline", AxisSectionKind.Late),
        ]);

    private static LetterGoalPin Pin(
        string id,
        string axisId,
        AxisSectionKind positionOnAxis) =>
        new(id, new PinAxisGoal(id, axisId, positionOnAxis), Axis.Zero);
}
