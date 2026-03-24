using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public sealed record VoiceTensionProfile(
    Proportion Importance,
    Proportion WeakUntil,
    Proportion StrongUntil,
    Proportion SaturatesAt)
{
    private static readonly Proportion Quarter = new(1, 4);
    private static readonly Proportion Half = new(1, 2);
    private static readonly Proportion ThreeQuarters = new(3, 4);

    public Proportion ResolveMagnitude(Proportion distance)
    {
        if (distance <= Proportion.Zero || Importance <= Proportion.Zero)
        {
            return Proportion.Zero;
        }

        Validate();

        Proportion normalized = distance <= WeakUntil
            ? Quarter * SafeRatio(distance, WeakUntil)
            : distance <= StrongUntil
                ? Quarter + (Half * SafeRatio(distance - WeakUntil, StrongUntil - WeakUntil))
                : distance <= SaturatesAt
                    ? ThreeQuarters + (Quarter * SafeRatio(distance - StrongUntil, SaturatesAt - StrongUntil))
                    : Proportion.One;

        return Importance * normalized;
    }

    private static Proportion SafeRatio(Proportion value, Proportion span) =>
        span <= Proportion.Zero ? Proportion.One : value / span;

    private void Validate()
    {
        if (WeakUntil < Proportion.Zero || StrongUntil < WeakUntil || SaturatesAt < StrongUntil)
        {
            throw new ArgumentException("Voice profile radii must satisfy 0 <= weak <= strong <= saturates.");
        }
    }
}

public sealed record LandmarkAttachmentVoice(
    Axis Descriptor,
    VoiceTensionProfile Strength);

public sealed record PinAxisLocusVoice(
    PinAxisGoal Goal,
    Axis HostDescriptor,
    VoiceTensionProfile HostStrength,
    Axis SectionDescriptor,
    VoiceTensionProfile SectionStrength);

public sealed record StrokeTopologyVoice(
    Axis Descriptor,
    VoiceTensionProfile Strength);

public sealed record HostRelationVoice(
    Axis Descriptor,
    VoiceTensionProfile Strength);

public sealed record StrokeOrderingVoice(
    Axis Descriptor,
    VoiceTensionProfile Strength);

public sealed record LetterGoalStrokeVoice(
    string Id,
    LetterGoalPrototype Goal,
    string StartPinId,
    string OriginId,
    string EndPinId,
    LandmarkAttachmentVoice StartAttachment,
    PinAxisLocusVoice OriginLocus,
    LandmarkAttachmentVoice EndAttachment,
    StrokeTopologyVoice Topology,
    HostRelationVoice HostRelation,
    StrokeOrderingVoice Ordering);

public sealed record LetterGoalStrokeState(
    string Id,
    PlanarPoint Start,
    PlanarPoint Origin,
    PlanarPoint End);

public sealed record LetterGoalVoiceEvaluation(
    IReadOnlyList<LetterFormationTension> Tensions)
{
    public Proportion TotalMagnitude =>
        LetterFormationGeometry.FromDouble(
            Tensions.Sum(static tension => (double)tension.Magnitude.Fold()));
}

public static class LetterGoalVoiceCatalog
{
    public static LetterGoalStrokeVoice CapitalALeftStem { get; } = CreateCapitalALeftStem();

    private static LetterGoalStrokeVoice CreateCapitalALeftStem()
    {
        LetterGoalPrototype goal = LetterGoalPrototypeCatalog.CapitalA;

        return new LetterGoalStrokeVoice(
            "LeftStem",
            goal,
            "P2",
            "O1",
            "P1",
            new LandmarkAttachmentVoice(
                Axis.Zero,
                new VoiceTensionProfile(
                    new Proportion(5, 4),
                    new Proportion(1, 32),
                    new Proportion(1, 8),
                    new Proportion(1, 2))),
            new PinAxisLocusVoice(
                new PinAxisGoal("O1", "Midline", AxisSectionKind.Early),
                new Axis(0, 1, 0, 1),
                new VoiceTensionProfile(
                    Proportion.One,
                    new Proportion(1, 32),
                    new Proportion(1, 8),
                    new Proportion(1, 2)),
                goal.Frame.ResolveOriginAxis("Midline", AxisSectionKind.Early),
                new VoiceTensionProfile(
                    new Proportion(3, 4),
                    new Proportion(1, 16),
                    new Proportion(1, 4),
                    Proportion.One)),
            new LandmarkAttachmentVoice(
                Axis.Zero,
                new VoiceTensionProfile(
                    new Proportion(5, 4),
                    new Proportion(1, 32),
                    new Proportion(1, 8),
                    new Proportion(1, 2))),
            new StrokeTopologyVoice(
                new Axis(0, 1, 0, 1),
                new VoiceTensionProfile(
                    Proportion.One,
                    new Proportion(1, 64),
                    new Proportion(1, 16),
                    new Proportion(1, 4))),
            new HostRelationVoice(
                new Axis(0, -1, 0, -1),
                new VoiceTensionProfile(
                    new Proportion(1, 3),
                    new Proportion(1, 16),
                    new Proportion(1, 4),
                    Proportion.One)),
            new StrokeOrderingVoice(
                LetterFormationDirections.Vertical,
                new VoiceTensionProfile(
                    Proportion.One,
                    new Proportion(1, 32),
                    new Proportion(1, 8),
                    new Proportion(1, 2))));
    }
}

public static class LetterGoalVoiceEvaluator
{
    public static LetterGoalVoiceEvaluation Evaluate(
        LetterGoalStrokeVoice voice,
        LetterGoalStrokeState state)
    {
        ArgumentNullException.ThrowIfNull(voice);
        ArgumentNullException.ThrowIfNull(state);

        List<LetterFormationTension> tensions = [];

        AddIfPresent(tensions, EvaluateAttachment(voice.Id, "start-pin", voice.Goal.ResolvePinPoint(voice.StartPinId), state.Start, voice.StartAttachment.Strength));
        AddIfPresent(tensions, EvaluateAttachment(voice.Id, "end-pin", voice.Goal.ResolvePinPoint(voice.EndPinId), state.End, voice.EndAttachment.Strength));
        EvaluateOriginLocus(tensions, voice, state);
        AddIfPresent(tensions, EvaluateStraightness(voice, state));
        AddIfPresent(tensions, EvaluateHostRelation(voice, state));
        AddIfPresent(tensions, EvaluateOrdering(voice, state));

        return new LetterGoalVoiceEvaluation(tensions);
    }

    private static void EvaluateOriginLocus(
        ICollection<LetterFormationTension> tensions,
        LetterGoalStrokeVoice voice,
        LetterGoalStrokeState state)
    {
        AxisPointProjection projection = voice.Goal.Frame.ProjectPoint(voice.OriginLocus.Goal.AxisId, state.Origin);
        AxisSectionWindow targetWindow = voice.Goal.Frame.ResolveWindow(
            voice.OriginLocus.Goal.AxisId,
            voice.OriginLocus.Goal.PositionOnAxis);

        Proportion hostDistance = projection.Cross.Abs();
        Proportion sectionDistance = DistanceToWindow(projection.Along, targetWindow);

        AddIfPresent(
            tensions,
            CreateTension(
                voice.Id,
                "origin-host",
                voice.OriginLocus.HostStrength.ResolveMagnitude(hostDistance),
                $"{voice.OriginId} is off {voice.OriginLocus.Goal.AxisId}."));

        AddIfPresent(
            tensions,
            CreateTension(
                voice.Id,
                "origin-section",
                voice.OriginLocus.SectionStrength.ResolveMagnitude(sectionDistance),
                $"{voice.OriginId} drifts away from {voice.OriginLocus.Goal.PositionOnAxis} on {voice.OriginLocus.Goal.AxisId}."));
    }

    private static LetterFormationTension? EvaluateAttachment(
        string componentId,
        string source,
        PlanarPoint target,
        PlanarPoint actual,
        VoiceTensionProfile profile)
    {
        Proportion distance = LetterFormationGeometry.FromDouble(LetterFormationGeometry.Distance(target, actual));
        return CreateTension(
            componentId,
            source,
            profile.ResolveMagnitude(distance),
            $"{source} remains away from its landmark.");
    }

    private static LetterFormationTension? EvaluateStraightness(
        LetterGoalStrokeVoice voice,
        LetterGoalStrokeState state)
    {
        if (!PrefersStraightCarrier(voice.Topology.Descriptor))
        {
            return null;
        }

        double deviation = DistanceToLine(state.Origin, state.Start, state.End);
        Proportion magnitude = voice.Topology.Strength.ResolveMagnitude(LetterFormationGeometry.FromDouble(deviation));
        return CreateTension(voice.Id, "straightness", magnitude, $"{voice.Id} remains bent.");
    }

    private static LetterFormationTension? EvaluateHostRelation(
        LetterGoalStrokeVoice voice,
        LetterGoalStrokeState state)
    {
        if (!PrefersOrthogonalToHost(voice.HostRelation.Descriptor))
        {
            return null;
        }

        LetterBoxFrameAxis hostAxis = voice.Goal.Frame.GetAxis(voice.OriginLocus.Goal.AxisId);
        double dx = LetterFormationGeometry.ToDouble(state.End.Horizontal - state.Start.Horizontal);
        double dy = LetterFormationGeometry.ToDouble(state.End.Vertical - state.Start.Vertical);
        (double normX, double normY) = LetterFormationGeometry.Normalize(dx, dy);

        double deviation = hostAxis.IsHorizontal
            ? Math.Abs(normX)
            : Math.Abs(normY);

        Proportion magnitude = voice.HostRelation.Strength.ResolveMagnitude(LetterFormationGeometry.FromDouble(deviation));
        return CreateTension(voice.Id, "host-relation", magnitude, $"{voice.Id} drifts away from its host relation.");
    }

    private static LetterFormationTension? EvaluateOrdering(
        LetterGoalStrokeVoice voice,
        LetterGoalStrokeState state)
    {
        int verticalSign = Math.Sign(voice.Ordering.Descriptor.Recessive.Numerator);
        if (verticalSign == 0)
        {
            return null;
        }

        Proportion excess = verticalSign < 0
            ? Proportion.Max(Proportion.Zero, state.End.Vertical - state.Start.Vertical)
            : Proportion.Max(Proportion.Zero, state.Start.Vertical - state.End.Vertical);

        Proportion magnitude = voice.Ordering.Strength.ResolveMagnitude(excess);
        return CreateTension(voice.Id, "ordering", magnitude, $"{voice.Id} loses its preferred stroke order.");
    }

    private static Proportion DistanceToWindow(Proportion coordinate, AxisSectionWindow window)
    {
        if (coordinate < window.Start)
        {
            return window.Start - coordinate;
        }

        if (coordinate > window.End)
        {
            return coordinate - window.End;
        }

        return Proportion.Zero;
    }

    private static double DistanceToLine(PlanarPoint point, PlanarPoint start, PlanarPoint end)
    {
        double x0 = LetterFormationGeometry.ToDouble(point.Horizontal);
        double y0 = LetterFormationGeometry.ToDouble(point.Vertical);
        double x1 = LetterFormationGeometry.ToDouble(start.Horizontal);
        double y1 = LetterFormationGeometry.ToDouble(start.Vertical);
        double x2 = LetterFormationGeometry.ToDouble(end.Horizontal);
        double y2 = LetterFormationGeometry.ToDouble(end.Vertical);

        double dx = x2 - x1;
        double dy = y2 - y1;
        double span = Math.Sqrt((dx * dx) + (dy * dy));
        if (span < 0.0001d)
        {
            return Math.Sqrt(((x0 - x1) * (x0 - x1)) + ((y0 - y1) * (y0 - y1)));
        }

        double numerator = Math.Abs((dy * x0) - (dx * y0) + (x2 * y1) - (y2 * x1));
        return numerator / span;
    }

    private static bool PrefersOrthogonalToHost(Axis descriptor) =>
        descriptor.Recessive.Recessive < 0 && descriptor.Dominant.Recessive < 0;

    private static bool PrefersStraightCarrier(Axis descriptor) =>
        descriptor.Recessive.Recessive > 0 && descriptor.Dominant.Recessive > 0;

    private static LetterFormationTension? CreateTension(
        string componentId,
        string source,
        Proportion magnitude,
        string description) =>
        magnitude <= Proportion.Zero
            ? null
            : new LetterFormationTension(componentId, source, magnitude, description);

    private static void AddIfPresent(
        ICollection<LetterFormationTension> tensions,
        LetterFormationTension? tension)
    {
        if (tension is not null)
        {
            tensions.Add(tension);
        }
    }
}
