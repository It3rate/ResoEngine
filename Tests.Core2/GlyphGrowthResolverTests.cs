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
    public void GrowthRuntime_V_ResolvesIntoSharedJoin()
    {
        var machine = GlyphGrowthRuntime.CreateMachine("V", maxSteps: 10);
        machine.RunToCompletion();

        var state = machine.Snapshot().SelectedContext!.State;

        Assert.Contains(state.Junctions, junction => junction.Kind == GlyphJunctionKind.Join);
        Assert.DoesNotContain(state.ActiveTips, tip => tip.IsActive);
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
