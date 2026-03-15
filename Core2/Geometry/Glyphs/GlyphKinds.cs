namespace Core2.Geometry.Glyphs;

public enum GlyphSeedKind
{
    Tip,
    Junction,
}

public enum GlyphCarrierKind
{
    Segment,
    Curve,
}

public enum GlyphJunctionKind
{
    Seed,
    Join,
    Split,
    Terminal,
    Crossing,
}

public enum GlyphLandmarkKind
{
    Baseline,
    Midline,
    Capline,
    Centerline,
    BranchPoint,
    StopPoint,
}

public enum GlyphFieldFalloff
{
    Constant,
    Linear,
}
