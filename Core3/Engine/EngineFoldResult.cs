namespace Core3.Engine;

public sealed record EngineFoldResult(
    EngineOperation RequestedOperation,
    NormalizedPin Normalized,
    PinSpaceKind Space,
    bool SupportsBoolean,
    EngineFoldDisposition Disposition,
    EngineLiftKind RequiredLift,
    GradedElement? Folded,
    string Note);
