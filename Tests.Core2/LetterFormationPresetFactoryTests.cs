using Applied.Geometry.LetterFormation;
using Core2.Elements;

namespace Tests.Core2;

public class LetterFormationPresetFactoryTests
{
    [Theory]
    [InlineData(LetterFormationPresetKind.CapitalD)]
    [InlineData(LetterFormationPresetKind.BridgeH)]
    [InlineData(LetterFormationPresetKind.LetterT)]
    [InlineData(LetterFormationPresetKind.LetterA)]
    [InlineData(LetterFormationPresetKind.LetterY)]
    [InlineData(LetterFormationPresetKind.LetterL)]
    [InlineData(LetterFormationPresetKind.LetterM)]
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
