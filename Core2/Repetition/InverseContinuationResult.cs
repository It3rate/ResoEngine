namespace Core2.Repetition;

public readonly record struct InverseContinuationTension(
    InverseContinuationTensionKind Kind,
    string Message);

public sealed record InverseContinuationResult<T>(
    IReadOnlyList<T> Candidates,
    T? PrincipalCandidate,
    IReadOnlyList<InverseContinuationTension> Tensions)
{
    public bool Succeeded => Candidates.Count > 0;
}
