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

    [Fact]
    public void RepeatedSteps_StrongJoinAttractionPullsSitesIntoSnap()
    {
        var state = new LetterFormationState(
            "approach",
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
                    new PlanarPoint(new Proportion(4), new Proportion(2)),
                    Axis.Zero,
                    PlanarOffset.Zero,
                    [
                        new JoinSiteDesire("Left", new Proportion(1), Proportion.One, Proportion.One, "join left"),
                    ]),
            ],
            [],
            []);

        var current = state;
        for (int index = 0; index < 4; index++)
        {
            current = LetterFormationStepper.Step(current, new Random(index + 1));
        }

        Assert.Equal(current.GetSite("Left").Position, current.GetSite("Right").Position);
    }

    [Fact]
    public void Step_JoinAttractionPullsHarderNearCapture()
    {
        var environment = LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.Zero);
        var near = new LetterFormationState(
            "near",
            0,
            environment,
            [
                new LetterFormationSiteState(
                    "Left",
                    new PlanarPoint(new Proportion(2), new Proportion(2)),
                    Axis.Zero,
                    PlanarOffset.Zero,
                    [
                        new JoinSiteDesire("Right", new Proportion(1), Proportion.One, new Proportion(4), "join right"),
                    ]),
                new LetterFormationSiteState(
                    "Right",
                    new PlanarPoint(new Proportion(31, 10), new Proportion(2)),
                    Axis.Zero,
                    PlanarOffset.Zero,
                    [
                        new JoinSiteDesire("Left", new Proportion(1), Proportion.One, new Proportion(4), "join left"),
                    ]),
            ],
            [],
            []);
        var far = near with
        {
            Key = "far",
            Sites =
            [
                near.GetSite("Left"),
                near.GetSite("Right") with { Position = new PlanarPoint(new Proportion(6), new Proportion(2)) },
            ],
        };

        var nearBefore = Distance(near);
        var farBefore = Distance(far);
        var nearAfter = Distance(LetterFormationStepper.Step(near, new Random(1)));
        var farAfter = Distance(LetterFormationStepper.Step(far, new Random(1)));

        Assert.True((nearBefore - nearAfter) > (farBefore - farAfter));
    }

    [Fact]
    public void Step_DoesNotJiggleWhenThereAreNoActiveProposals()
    {
        var state = new LetterFormationState(
            "quiet",
            0,
            LetterFormationEnvironment.CreateLetterBox(randomMotionWeight: Proportion.One),
            [
                new LetterFormationSiteState(
                    "Quiet",
                    new PlanarPoint(new Proportion(3), new Proportion(4)),
                    Axis.Zero,
                    new PlanarOffset(new Proportion(1, 5), new Proportion(1, 5)),
                    []),
            ],
            [],
            []);

        var stepped = LetterFormationStepper.Step(state, new Random(7));
        var original = state.GetSite("Quiet");
        var quiet = stepped.GetSite("Quiet");

        Assert.True(original.Position.X == quiet.Position.X && original.Position.Y == quiet.Position.Y);
        Assert.True(quiet.Momentum.Dx == 0 && quiet.Momentum.Dy == 0);
    }

    private static double Sum(IEnumerable<LetterFormationTension> tensions) =>
        tensions.Sum(tension => (double)tension.Magnitude.Numerator / tension.Magnitude.Denominator);

    private static double Distance(LetterFormationState state)
    {
        var left = state.GetSite("Left").Position;
        var right = state.GetSite("Right").Position;
        var dx = (double)(right.Horizontal - left.Horizontal).Numerator / (right.Horizontal - left.Horizontal).Denominator;
        var dy = (double)(right.Vertical - left.Vertical).Numerator / (right.Vertical - left.Vertical).Denominator;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }
}
