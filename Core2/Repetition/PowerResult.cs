namespace Core2.Repetition;

public enum PowerTensionKind
{
    InvalidExponent,
    UnsupportedNegativeExponent,
    InverseContinuationFailed,
    ShapeChangingPower,
}

public readonly record struct PowerTension(
    PowerTensionKind Kind,
    string Message);

public sealed record PowerResult<T>(
    IReadOnlyList<T> Candidates,
    T? PrincipalCandidate,
    IReadOnlyList<PowerTension> Tensions)
{
    public bool Succeeded => Candidates.Count > 0;
}
