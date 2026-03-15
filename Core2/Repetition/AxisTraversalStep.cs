using Core2.Elements;

namespace Core2.Repetition;

public readonly record struct AxisTraversalStep(
    Scalar Start,
    Scalar End,
    Scalar Delta,
    IReadOnlyList<RepetitionTension> Tensions);
