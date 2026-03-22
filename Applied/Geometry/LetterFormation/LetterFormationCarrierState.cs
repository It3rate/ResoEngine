namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationCarrierState(
    string Id,
    string StartSiteId,
    string EndSiteId,
    IReadOnlyList<LetterFormationDesire> Desires,
    string? StrokeId = null,
    int StrokeOrder = 0,
    int StrokeSegmentOrder = 0,
    bool ReverseForStroke = false);
