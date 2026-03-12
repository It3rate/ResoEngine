namespace Core2.Support;

public enum ContainmentTensionKind
{
    OutsideExpectedRange,
    ResolutionMismatch,
    PlacementUnderspecified,
    UnsupportedInterpretation,
}

public readonly record struct ContainmentTension(
    ContainmentTensionKind Kind,
    string Path,
    string Message);
