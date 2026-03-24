using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

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

    public LetterGoalPin GetPin(string id) =>
        Pins.FirstOrDefault(pin => string.Equals(pin.Id, id, StringComparison.Ordinal))
        ?? throw new KeyNotFoundException($"Unknown goal pin '{id}'.");

    public PlanarPoint ResolvePinPoint(string id)
    {
        LetterGoalPin pin = GetPin(id);
        return Frame.ResolvePoint(pin.Placement.AxisId, pin.Placement.PositionOnAxis);
    }
}

public static class LetterGoalPrototypeCatalog
{
    public static LetterGoalPrototype CapitalA { get; } = new(
        "LetterA",
        LetterBoxFrameCatalog.RomanUppercase,
        [
            Pin("P1", "Baseline", AxisSectionKind.Start),
            Pin("P2", "Topline", AxisSectionKind.Middle),
            Pin("P3", "Baseline", AxisSectionKind.End),
        ]);

    private static LetterGoalPin Pin(
        string id,
        string axisId,
        AxisSectionKind positionOnAxis) =>
        new(id, new PinAxisGoal(id, axisId, positionOnAxis), Axis.Zero);
}
