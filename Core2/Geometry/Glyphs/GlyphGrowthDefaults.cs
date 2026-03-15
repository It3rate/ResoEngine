namespace Core2.Geometry.Glyphs;

public static class GlyphGrowthDefaults
{
    public const decimal Step = 0.35m;
    public const decimal BranchCaptureRadius = 2.4m;
    public const decimal StopCaptureRadius = 2.6m;
    public const decimal JoinCaptureRadius = 3.2m;
    public const decimal BranchAmbiguityThreshold = 0.12m;
    public const decimal PacketDecay = 0.72m;
    public const decimal MinimumPacketMagnitude = 0.05m;
    public const decimal AmbientDecay = 0.975m;
    public const decimal AmbientSpread = 1.005m;
    public const decimal MinimumAmbientMagnitude = 0.004m;
    public const decimal RelaxationStep = 0.12m;
    public const decimal RelaxationThreshold = 0.018m;
    public const decimal ResidualTensionThreshold = 0.12m;
    public const decimal SignalDriftStep = 0.22m;
    public const decimal SignalSpawnInset = 11m;
    public const int MaximumAmbientAge = 180;
    public const int MinimumSettleSteps = 180;
    public const int DefaultMaxSteps = 300;
}
