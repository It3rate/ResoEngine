namespace Core2.Repetition;

public enum RepetitionKind
{
    Additive,
    Multiplicative,
    Transform,
    Recursive,
}

public enum BoundaryContinuationLaw
{
    TensionPreserving,
    PeriodicWrap,
    ReflectiveBounce,
}

public enum RepetitionTensionKind
{
    BoundaryExceeded,
}
