using Core2.Branching;
using Core2.Dynamic;
using Core2.Geometry.Glyphs;

namespace Tests.Core2;

public class GlyphGrowthResolverTests
{
    [Fact]
    public void TipGrowthStrand_ProposesSplitNearBranchLandmark()
    {
        var spec = GlyphLetterCatalog.Get("Y");
        var state = new GlyphGrowthState(
            "Y",
            [
                new GlyphTip("Y-trunk", new GlyphVector(spec.Environment.Box.MidX, 52m), new GlyphVector(0m, 1m))
            ],
            [],
            [],
            [],
            0);

        var strand = new GlyphTipGrowthStrand();
        var proposals = strand.Propose(new DynamicStrandContext<GlyphGrowthState, GlyphEnvironment>(
            0,
            new DynamicFrontierContext<GlyphGrowthState, GlyphEnvironment>(
                BranchId.New(),
                new DynamicContext<GlyphGrowthState, GlyphEnvironment>(state, spec.Environment)),
            []));

        Assert.Contains(proposals, proposal => proposal.Effect.Kind == GlyphGrowthEffectKind.Split);
    }

    [Fact]
    public void TipGrowthStrand_ProposesStopNearBoundary()
    {
        var spec = GlyphLetterCatalog.Get("T");
        var state = new GlyphGrowthState(
            "T",
            [
                new GlyphTip("T-stem", new GlyphVector(spec.Environment.Box.MidX, 6m), new GlyphVector(0m, -1m))
            ],
            [],
            [],
            [],
            0);

        var strand = new GlyphTipGrowthStrand();
        var proposals = strand.Propose(new DynamicStrandContext<GlyphGrowthState, GlyphEnvironment>(
            0,
            new DynamicFrontierContext<GlyphGrowthState, GlyphEnvironment>(
                BranchId.New(),
                new DynamicContext<GlyphGrowthState, GlyphEnvironment>(state, spec.Environment)),
            []));

        Assert.Contains(proposals, proposal => proposal.Effect.Kind == GlyphGrowthEffectKind.Stop);
    }

    [Fact]
    public void GrowthRuntime_Y_SplitsIntoTwoActiveTips()
    {
        var machine = GlyphGrowthRuntime.CreateMachine("Y", maxSteps: 6);

        while (machine.StepCount < 5 && machine.Step())
        {
        }

        var state = machine.Snapshot().SelectedContext!.State;

        Assert.Equal(5, state.MacroStep);
        Assert.Contains(state.Junctions, junction => junction.Kind == GlyphJunctionKind.Split);
        Assert.Equal(2, state.ActiveTips.Count(tip => tip.IsActive));
    }

    [Fact]
    public void SeedState_ContainsFrameAndLandmarkAmbientSignals()
    {
        var state = GlyphLetterCatalog.CreateSeedState("Y");

        Assert.NotNull(state.AmbientSignals);
        Assert.NotEmpty(state.AmbientSignals!);
        Assert.Contains(state.AmbientSignals!, signal => signal.Key.Contains("frame:left", StringComparison.Ordinal));
        Assert.True(state.ResidualTension > 0m);
    }

    [Fact]
    public void GrowthRuntime_V_ResolvesIntoSharedJoin()
    {
        var machine = GlyphGrowthRuntime.CreateMachine("V", maxSteps: GlyphGrowthDefaults.DefaultMaxSteps);
        machine.RunToCompletion();

        var state = machine.Snapshot().SelectedContext!.State;

        Assert.Contains(state.Junctions, junction => junction.Kind == GlyphJunctionKind.Join);
        Assert.DoesNotContain(state.ActiveTips, tip => tip.IsActive);
        Assert.True(state.ResidualTension <= GlyphGrowthDefaults.ResidualTensionThreshold);
    }

    [Fact]
    public void Resolver_RelaxesStatesEvenWhenNoActiveTipsRemain()
    {
        var resolver = new GlyphGrowthResolver();
        var state = new GlyphGrowthState(
            "Relax",
            [new GlyphTip("tip", new GlyphVector(50m, 50m), new GlyphVector(0m, 1m), IsActive: false)],
            [new GlyphCarrier("carrier", new GlyphVector(50m, 50m), new GlyphVector(52m, 62m), IsCommitted: true, Tension: 12m)],
            [],
            [],
            3,
            [new GlyphAmbientSignal("ambient", new GlyphVector(58m, 60m), global::Core2.Propagation.CouplingKind.Attract, 0.4m, 18m)],
            0.6m,
            0.4m);
        var environment = GlyphLetterCatalog.Get("Y").Environment;
        var nodeId = BranchId.New();

        var input = new DynamicResolutionInput<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>(
            0,
            [
                new DynamicFrontierContext<GlyphGrowthState, GlyphEnvironment>(
                    nodeId,
                    new DynamicContext<GlyphGrowthState, GlyphEnvironment>(state, environment))
            ],
            []);

        var resolution = resolver.Resolve(input);

        Assert.Equal(DynamicResolutionKind.Commit, resolution.Kind);
        Assert.Single(resolution.Outcomes.Members);
        Assert.True(resolution.Outcomes.SelectedValue!.State.LastAdjustment > 0m);
    }

    [Fact]
    public void Resolver_BranchesWhenTwoProposalKindsRemainClose()
    {
        var resolver = new GlyphGrowthResolver();
        var state = new GlyphGrowthState(
            "Test",
            [new GlyphTip("tip", new GlyphVector(50m, 50m), new GlyphVector(0m, 1m))],
            [],
            [],
            [],
            0);
        var environment = new GlyphEnvironment(new GlyphBox(0m, 0m, 100m, 100m), [], [], []);
        var nodeId = BranchId.New();

        var input = new DynamicResolutionInput<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>(
            0,
            [
                new DynamicFrontierContext<GlyphGrowthState, GlyphEnvironment>(
                    nodeId,
                    new DynamicContext<GlyphGrowthState, GlyphEnvironment>(state, environment))
            ],
            [
                new DynamicProposal<GlyphGrowthEffect>(
                    "test",
                    nodeId,
                    new GlyphGrowthEffect(
                        GlyphGrowthEffectKind.Grow,
                        "tip",
                        new GlyphVector(50m, 62m),
                        new GlyphVector(0m, 1m),
                        1m),
                    weight: 1m),
                new DynamicProposal<GlyphGrowthEffect>(
                    "test",
                    nodeId,
                    new GlyphGrowthEffect(
                        GlyphGrowthEffectKind.Stop,
                        "tip",
                        new GlyphVector(50m, 60m),
                        new GlyphVector(0m, 1m),
                        0.93m),
                    weight: 0.93m),
            ]);

        var resolution = resolver.Resolve(input);

        Assert.Equal(DynamicResolutionKind.Branch, resolution.Kind);
        Assert.Equal(2, resolution.Outcomes.Members.Count);
        Assert.Equal(BranchSemantics.Alternative, resolution.Outcomes.Semantics);
    }
}
