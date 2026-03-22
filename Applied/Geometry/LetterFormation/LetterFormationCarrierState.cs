namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationCarrierState(
    string Id,
    string StartSiteId,
    string EndSiteId,
    IReadOnlyList<LetterFormationDesire> Desires);
