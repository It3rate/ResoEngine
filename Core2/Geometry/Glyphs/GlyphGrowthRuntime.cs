using Core2.Dynamic;

namespace Core2.Geometry.Glyphs;

public static class GlyphGrowthRuntime
{
    public static IReadOnlyList<IDynamicStrand<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>> CreateStrands() =>
        [new GlyphTipGrowthStrand()];

    public static DynamicMachine<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> CreateMachine(
        string letterKey,
        int maxSteps = 12)
    {
        var spec = GlyphLetterCatalog.Get(letterKey);
        return CreateMachine(spec, maxSteps);
    }

    public static DynamicMachine<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> CreateMachine(
        GlyphLetterSpec spec,
        int maxSteps = 12) =>
        new(
            new DynamicContext<GlyphGrowthState, GlyphEnvironment>(
                GlyphGrowthState.FromSpec(spec),
                spec.Environment),
            CreateStrands(),
            new GlyphGrowthResolver(),
            new GlyphGrowthConvergencePolicy(maxSteps));
}
