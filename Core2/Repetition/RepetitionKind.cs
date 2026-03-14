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
    Clamp,
}

public enum RepetitionTensionKind
{
    BoundaryExceeded,
}
