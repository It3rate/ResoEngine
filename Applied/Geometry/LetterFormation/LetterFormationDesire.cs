using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public abstract record LetterFormationDesire(
    string Label,
    Proportion Weight);

public sealed record FrameCoordinateDesire(
    LetterFormationCoordinateAxis Axis,
    Proportion RelativeTarget,
    Proportion Tolerance,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);

public sealed record SiteCoordinateDesire(
    LetterFormationCoordinateAxis Axis,
    string OtherSiteId,
    Proportion Offset,
    Proportion Tolerance,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);

public sealed record JoinSiteDesire(
    string OtherSiteId,
    Proportion CaptureDistance,
    Proportion Escalation,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);

public sealed record CarrierOrientationDesire(
    LetterFormationOrientationKind Orientation,
    Proportion Tolerance,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);

public sealed record CarrierSpanDesire(
    Proportion Minimum,
    Proportion Maximum,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);
