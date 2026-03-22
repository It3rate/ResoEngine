using Applied.Geometry.LetterFormation;
using Applied.Geometry.Utils;
using Core2.Elements;

namespace Tests.Core2;

public class LetterFormationStepperTests
{
    [Fact]
    public void GenerateProposals_ProducesLocalMotionForCapitalASeed()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateCapitalAAssemblySeed(new Random(1234), environment);

        var proposals = LetterFormationStepper.GenerateProposals(state);

        Assert.NotEmpty(proposals);
        Assert.Contains(
            proposals,
            proposal => proposal.ComponentId is "LeftUpper" or "RightUpper" or "Crossbar" or "LeftApex" or "RightApex");
    }

    [Fact]
    public void Step_AdvancesStateAndKeepsSitesInsideLetterBox()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var state = LetterFormationPresetFactory.CreateCapitalAAssemblySeed(new Random(1234), environment);

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
        var state = LetterFormationPresetFactory.CreateCapitalAAssemblySeed(new Random(1234), environment);
        var initial = LetterFormationTensionEvaluator.Evaluate(state);

        var current = initial;
        var random = new Random(2222);
        for (int index = 0; index < 18; index++)
        {
            current = LetterFormationStepper.Step(current, random);
        }

        Assert.True(Sum(current.Tensions) < Sum(initial.Tensions));
    }

    [Fact]
    public void Step_SnapsJoinSitesTogetherWhenTheyReachCaptureDistance()
    {
        var state = new LetterFormationState(
            "join",
            0,
            LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero),
            [
                new LetterFormationSiteState(
                    "Left",
                    new PlanarPoint(new Proportion(2), new Proportion(2)),
                    Axis.Zero,
                    PlanarOffset.Zero,
                    [
                        new JoinSiteDesire("Right", new Proportion(1), Proportion.One, Proportion.One, "join right"),
                    ]),
                new LetterFormationSiteState(
                    "Right",
                    new PlanarPoint(new Proportion(5, 2), new Proportion(2)),
                    Axis.Zero,
                    PlanarOffset.Zero,
                    [
                        new JoinSiteDesire("Left", new Proportion(1), Proportion.One, Proportion.One, "join left"),
                    ]),
            ],
            [],
            []);

        var stepped = LetterFormationStepper.Step(state, new Random(1));

        Assert.Equal(stepped.GetSite("Left").Position, stepped.GetSite("Right").Position);
    }

    private static double Sum(IEnumerable<LetterFormationTension> tensions) =>
        tensions.Sum(tension => (double)tension.Magnitude.Numerator / tension.Magnitude.Denominator);
}
