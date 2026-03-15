using Core2.Geometry.Glyphs;
using Core2.Propagation;

namespace Tests.Core2;

public class GlyphFoundationTests
{
    [Fact]
    public void LetterCatalog_ProvidesCoreDemoLetters()
    {
        Assert.Contains(GlyphLetterCatalog.Specs, spec => spec.Key == "Y");
        Assert.Contains(GlyphLetterCatalog.Specs, spec => spec.Key == "V");
        Assert.Contains(GlyphLetterCatalog.Specs, spec => spec.Key == "T");
        Assert.Contains(GlyphLetterCatalog.Specs, spec => spec.Key == "O");
    }

    [Fact]
    public void GlyphEnvironment_SamplesMidlineFieldMoreStronglyNearMidline()
    {
        var spec = GlyphLetterCatalog.Get("Y");
        var near = spec.Environment.SampleInfluencesAt(new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.MidY));
        var far = spec.Environment.SampleInfluencesAt(new GlyphVector(spec.Environment.Box.MidX, spec.Environment.Box.Top));

        decimal nearMidline = near
            .Where(influence => influence.EmitterKey == "midline")
            .Sum(influence => influence.Weight);
        decimal farMidline = far
            .Where(influence => influence.EmitterKey == "midline")
            .Sum(influence => influence.Weight);

        Assert.True(nearMidline > farMidline);
    }

    [Fact]
    public void GlyphGrowthState_FromSpec_SeedsTipsJunctionsAndPackets()
    {
        var state = GlyphLetterCatalog.CreateSeedState("Y");

        Assert.Single(state.ActiveTips);
        Assert.Single(state.Junctions);
        Assert.Equal(2, state.Packets.Count);
        Assert.Equal("Y", state.LetterKey);
        Assert.True(state.HasActiveTips);
    }

    [Fact]
    public void PointFieldEmitter_UsesDistanceFalloff()
    {
        var emitter = new PointGlyphFieldEmitter(
            "join-point",
            new GlyphVector(50m, 50m),
            10m,
            [new CouplingRule(CouplingKind.Join, 1m)]);

        decimal near = emitter.EmitAt(new GlyphVector(50m, 54m)).Single().Weight;
        decimal far = emitter.EmitAt(new GlyphVector(50m, 59m)).Single().Weight;

        Assert.True(near > far);
    }
}
