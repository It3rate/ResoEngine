using Applied.Geometry.LetterFormation;
using Core2.Elements;

namespace Tests.Core2;

public class LetterFormationPresetFactoryTests
{
    public static IEnumerable<object[]> AllPresets =>
        Enum.GetValues<LetterFormationPresetKind>()
            .Cast<LetterFormationPresetKind>()
            .Select(preset => new object[] { preset });

    [Theory]
    [MemberData(nameof(AllPresets))]
    public void CreateSeed_CanEvaluateAndStepEveryPreset(LetterFormationPresetKind preset)
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateSeed(preset, new Random(1234), environment);

        var evaluated = LetterFormationTensionEvaluator.Evaluate(state);
        var stepped = LetterFormationStepper.Step(evaluated, new Random(4321));

        Assert.NotEmpty(evaluated.Sites);
        Assert.NotEmpty(evaluated.Carriers);
        Assert.Equal(state.StepIndex + 1, stepped.StepIndex);
        Assert.All(
            stepped.Sites,
            site =>
            {
                Assert.True(site.Position.Horizontal >= environment.Left);
                Assert.True(site.Position.Horizontal <= environment.Right);
                Assert.True(site.Position.Vertical >= environment.Top);
                Assert.True(site.Position.Vertical <= environment.Bottom);
            });
    }
}
