using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationProposal(
    string SiteId,
    string ComponentId,
    string Source,
    PlanarOffset Offset,
    Proportion Strength,
    string Description);
