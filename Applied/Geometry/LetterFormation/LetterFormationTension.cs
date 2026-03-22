using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationTension(
    string ComponentId,
    string Source,
    Proportion Magnitude,
    string Description);
