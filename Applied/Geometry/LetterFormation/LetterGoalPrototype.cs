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
}

public sealed record AxisSectionCalibration(
    AxisSectionWindow Start,
    AxisSectionWindow Early,
    AxisSectionWindow Middle,
    AxisSectionWindow Late,
    AxisSectionWindow End)
{
    public static AxisSectionCalibration Default { get; } = new(
        Point(0, 8),
        Range(1, 8, 3, 8),
        Point(4, 8),
        Range(5, 8, 7, 8),
        Point(8, 8));

    public AxisSectionWindow Resolve(AxisSectionKind section) =>
        section switch
        {
            AxisSectionKind.Start => Start,
            AxisSectionKind.Early => Early,
            AxisSectionKind.Middle => Middle,
            AxisSectionKind.Late => Late,
            AxisSectionKind.End => End,
            _ => Middle,
        };

    private static AxisSectionWindow Point(long numerator, long denominator) =>
        new(new Proportion(numerator, denominator), new Proportion(numerator, denominator));

    private static AxisSectionWindow Range(long startNumerator, long denominator, long endNumerator, long endDenominator) =>
        new(new Proportion(startNumerator, denominator), new Proportion(endNumerator, endDenominator));
}

public sealed record PinAxisGoal(
    string PinId,
    string TargetId,
    AxisSectionKind PositionOnTarget);

public sealed record AxisPairGoal(
    string FromId,
    AxisSectionKind FromPosition,
    string ToId,
    AxisSectionKind ToPosition);

public sealed record LetterGoalPin(
    string Id,
    AxisSectionKind Horizontal,
    AxisSectionKind Vertical,
    Axis Descriptor);

public sealed record LetterGoalPrototype(
    string Id,
    IReadOnlyList<LetterGoalPin> Pins,
    AxisSectionCalibration? Calibration = null)
{
    public AxisSectionCalibration ActiveCalibration => Calibration ?? AxisSectionCalibration.Default;
}

public static class LetterGoalPrototypeCatalog
{
    public static LetterGoalPrototype CapitalA { get; } = new(
        "LetterA",
        [
            Pin("P1", AxisSectionKind.Early, AxisSectionKind.End),
            Pin("P2", AxisSectionKind.Middle, AxisSectionKind.Start),
            Pin("P3", AxisSectionKind.Late, AxisSectionKind.End),
        ]);

    private static LetterGoalPin Pin(
        string id,
        AxisSectionKind horizontal,
        AxisSectionKind vertical) =>
        new(id, horizontal, vertical, Axis.Zero);
}
