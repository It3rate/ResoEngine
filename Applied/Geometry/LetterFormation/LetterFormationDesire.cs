using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public abstract record LetterFormationDesire(
    string Label,
    Proportion Weight);

public sealed record FrameProjectionDesire(
    Axis Projection,
    Proportion RelativeTarget,
    Proportion Tolerance,
    Proportion Weight,
    string Label)
    : LetterFormationDesire(Label, Weight);

public sealed record SiteProjectionDesire(
    Axis Projection,
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

public sealed record CarrierDirectionDesire(
    Axis PreferredDirection,
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
