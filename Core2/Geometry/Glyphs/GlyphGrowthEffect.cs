namespace Core2.Geometry.Glyphs;

public sealed record GlyphGrowthEffect
{
    public GlyphGrowthEffect(
        GlyphGrowthEffectKind kind,
        string sourceTipKey,
        GlyphVector targetPosition,
        GlyphVector direction,
        decimal score,
        string? partnerTipKey = null,
        string? junctionKey = null,
        IReadOnlyList<GlyphGrowthBranch>? branches = null,
        string? note = null)
    {
        Kind = kind;
        SourceTipKey = sourceTipKey;
        TargetPosition = targetPosition;
        Direction = direction;
        Score = score;
        PartnerTipKey = partnerTipKey;
        JunctionKey = junctionKey;
        Branches = branches?.ToArray() ?? [];
        Note = note;
    }

    public GlyphGrowthEffectKind Kind { get; }
    public string SourceTipKey { get; }
    public GlyphVector TargetPosition { get; }
    public GlyphVector Direction { get; }
    public decimal Score { get; }
    public string? PartnerTipKey { get; }
    public string? JunctionKey { get; }
    public IReadOnlyList<GlyphGrowthBranch> Branches { get; }
    public string? Note { get; }
}
