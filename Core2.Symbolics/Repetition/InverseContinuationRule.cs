namespace Core2.Symbolics.Repetition;

public enum InverseContinuationRule
{
    Principal,
    PreferPositiveDominant,
    NearestToReference,
}

public enum AreaInverseContinuationMode
{
    FoldFirst,
    StructurePreserving,
}

public enum InverseContinuationTensionKind
{
    InvalidDegree,
    NoCandidates,
    UnsupportedBasis,
    StructurePreservingUnavailable,
}
