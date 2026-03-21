using Core2.Elements;

namespace Core2.Resolution;

/// <summary>
/// Records the difference between an exact resolved proportion and the later
/// support that was requested for commitment, display, or continuation.
/// The committed value remains exact; if the requested support cannot carry it
/// exactly, the actual support rises to the smallest exact support that can.
/// </summary>
public sealed record ResultSupportCommitment(
    ResultSupportPolicy Policy,
    Proportion ExactValue,
    long RequestedSupport,
    Proportion CommittedValue)
{
    public long ActualSupport => CommittedValue.Denominator;

    public bool UsesRequestedSupportExactly => ActualSupport == RequestedSupport;
}
