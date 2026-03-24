using Applied.Geometry.LetterFormation;
using Applied.Geometry.Utils;
using Core2.Elements;

namespace Tests.Core2;

public class LetterGoalVoiceTests
{
    [Fact]
    public void Origin_OnSameHostAxisButWrongSection_ReducesMoreTensionThanOffAxis()
    {
        LetterGoalStrokeVoice voice = LetterGoalVoiceCatalog.CapitalALeftStem;
        LetterGoalStrokeState onHostLate = CreateState(
            voice,
            voice.Goal.Frame.ResolvePoint("Midline", AxisSectionKind.Late));
        LetterGoalStrokeState offHostLate = CreateState(
            voice,
            voice.Goal.Frame.ResolvePoint("RightSide", AxisSectionKind.Late));

        LetterGoalVoiceEvaluation onHostEvaluation = LetterGoalVoiceEvaluator.Evaluate(voice, onHostLate);
        LetterGoalVoiceEvaluation offHostEvaluation = LetterGoalVoiceEvaluator.Evaluate(voice, offHostLate);

        Assert.True(onHostEvaluation.TotalMagnitude < offHostEvaluation.TotalMagnitude);
        Assert.Contains(onHostEvaluation.Tensions, tension => tension.Source == "origin-section");
        Assert.DoesNotContain(onHostEvaluation.Tensions, tension => tension.Source == "origin-host");
    }

    [Fact]
    public void Origin_InTargetWindow_ProducesNoOriginLocusTension()
    {
        LetterGoalStrokeVoice voice = LetterGoalVoiceCatalog.CapitalALeftStem;
        LetterGoalStrokeState state = CreateState(
            voice,
            voice.Goal.Frame.ResolvePoint("Midline", AxisSectionKind.Early));

        LetterGoalVoiceEvaluation evaluation = LetterGoalVoiceEvaluator.Evaluate(voice, state);

        Assert.DoesNotContain(evaluation.Tensions, tension => tension.Source == "origin-host");
        Assert.DoesNotContain(evaluation.Tensions, tension => tension.Source == "origin-section");
    }

    [Fact]
    public void BentStroke_ProducesStraightnessTension()
    {
        LetterGoalStrokeVoice voice = LetterGoalVoiceCatalog.CapitalALeftStem;
        LetterGoalStrokeState state = CreateState(
            voice,
            new PlanarPoint(new Proportion(1, 2), new Proportion(3, 8)));

        LetterGoalVoiceEvaluation evaluation = LetterGoalVoiceEvaluator.Evaluate(voice, state);

        Assert.Contains(evaluation.Tensions, tension => tension.Source == "straightness");
    }

    private static LetterGoalStrokeState CreateState(
        LetterGoalStrokeVoice voice,
        PlanarPoint origin) =>
        new(
            voice.Id,
            voice.Goal.ResolvePinPoint(voice.StartPinId),
            origin,
            voice.Goal.ResolvePinPoint(voice.EndPinId));
}
