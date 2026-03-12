namespace ResoEngine.Core2;

public enum ContainmentTensionKind
{
    OutsideExpectedRange,
    ResolutionMismatch,
    UnsupportedInterpretation,
}

public readonly record struct ContainmentTension(
    ContainmentTensionKind Kind,
    string Path,
    string Message);
