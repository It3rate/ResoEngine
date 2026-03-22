using Applied.Geometry.LetterFormation;
using Core2.Elements;

namespace Tests.Core2;

public class LetterFormationStepperTests
{
    [Fact]
    public void GenerateProposals_ProducesLocalMotionForCapitalASeed()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateCapitalASeed(new Random(1234), environment);

        var proposals = LetterFormationStepper.GenerateProposals(state);

        Assert.NotEmpty(proposals);
        Assert.Contains(proposals, proposal => proposal.ComponentId == "Apex" || proposal.ComponentId == "LeftLeg" || proposal.ComponentId == "RightLeg");
    }

    [Fact]
    public void Step_AdvancesStateAndKeepsSitesInsideLetterBox()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateCapitalASeed(new Random(1234), environment);

        var stepped = LetterFormationStepper.Step(state, new Random(4321));

        Assert.Equal(state.StepIndex + 1, stepped.StepIndex);
        Assert.Contains(
            stepped.Sites.Zip(state.Sites, (next, previous) => (next, previous)),
            pair => pair.next.Position != pair.previous.Position);
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

    [Fact]
    public void RepeatedSteps_ReduceTotalTensionForSeed()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateCapitalASeed(new Random(1234), environment);
        var initial = LetterFormationTensionEvaluator.Evaluate(state);

        var current = initial;
        var random = new Random(2222);
        for (int index = 0; index < 12; index++)
        {
            current = LetterFormationStepper.Step(current, random);
        }

        Assert.True(Sum(current.Tensions) < Sum(initial.Tensions));
    }

    private static double Sum(IEnumerable<LetterFormationTension> tensions) =>
        tensions.Sum(tension => (double)tension.Magnitude.Numerator / tension.Magnitude.Denominator);
}
