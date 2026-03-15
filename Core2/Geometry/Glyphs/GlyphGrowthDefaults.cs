namespace Core2.Geometry.Glyphs;

public static class GlyphGrowthDefaults
{
    public const decimal Step = 12m;
    public const decimal BranchCaptureRadius = 14m;
    public const decimal StopCaptureRadius = 12m;
    public const decimal JoinCaptureRadius = 18m;
    public const decimal BranchAmbiguityThreshold = 0.12m;
    public const decimal PacketDecay = 0.65m;
    public const decimal MinimumPacketMagnitude = 0.05m;
    public const decimal AmbientDecay = 0.84m;
    public const decimal AmbientSpread = 1.12m;
    public const decimal MinimumAmbientMagnitude = 0.05m;
    public const decimal RelaxationStep = 2.1m;
    public const decimal RelaxationThreshold = 0.2m;
    public const decimal ResidualTensionThreshold = 0.28m;
    public const int DefaultMaxSteps = 24;
}
